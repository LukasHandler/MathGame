using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Data;
using System.Net;
using Shared.Data.Messages;
using Shared.Data.Managers;
using System.Reflection;
using Shared.Data.EventArguments;
using System.Threading;

namespace Server.Application
{
    public class NetworkService
    {
        private bool isActive;

        private DateTime startTime;

        private IDataManager clientDataManager;

        private Dictionary<Client, object> clientsTargetInformation;

        private Dictionary<Client, Timer> clientTimers;

        private Dictionary<Client, MathQuestion> questions;

        private IDataManager monitorDataManager;

        private IDataManager serverDataManager;

        private List<object> monitorsTargetInformation;

        private ServerConfiguration Configuration;

        public EventHandler<LoggingEventArgs> OnLoggingMessage;

        private MessageProcessor messageProcessor;

        private Tuple<string, object> serverTargetInformation;

        public NetworkService(ServerConfiguration configuration)
        {
            this.startTime = DateTime.Now;
            this.isActive = true;
            this.questions = new Dictionary<Client, MathQuestion>();
            this.Configuration = configuration;

            messageProcessor = new MessageProcessor();
            messageProcessor.OnConnectionRequestClient += ConnectionRequestedClient;
            messageProcessor.OnDisconnect += Disconnect;
            messageProcessor.OnAnswer += SubmitAnswer;
            messageProcessor.OnScoreRequest += SendScores;

            clientDataManager = new UdpServerManager(this.Configuration.ClientPort);
            clientDataManager.OnDataReceived += OnClientDataReceived;

            messageProcessor.OnConnectionRequestMonitor += ConnectionRequestedMonitor;
            monitorDataManager = new TcpServerManager(this.Configuration.MonitorPort);
            monitorDataManager.OnDataReceived += messageProcessor.DataReceived;
            monitorsTargetInformation = new List<object>();

            clientTimers = new Dictionary<Client, Timer>();
            this.clientsTargetInformation = new Dictionary<Client, object>();

            this.serverDataManager = new TcpClientServerManager(configuration.ServerPort);
            this.serverDataManager.OnDataReceived += ServerMessageReceived;
            messageProcessor.OnConnectionRequestServer += ConnectionRequestServer;
            messageProcessor.OnConnectionAcceptedServer += ConnectionAcceptedServer;
            messageProcessor.OnDisconnectServer += DisconnectServer;
        }

        private void OnClientDataReceived(object sender, MessageEventArgs e)
        {
            if (this.isActive)
            {
                messageProcessor.DataReceived(sender, e);
            }
            else
            {
                this.LogText(string.Format("{0} received message from {1} and sent it to {2}", this.Configuration.ServerName, e.MessageContent.SenderName, this.serverTargetInformation.Item1));
                this.serverDataManager.WriteData(e.MessageContent, this.serverTargetInformation.Item2);
            }
        }

        private void DisconnectServer(object sender, MessageEventArgs e)
        {
            this.isActive = true;
            this.serverTargetInformation = null;
        }

        private void ConnectionAcceptedServer(object sender, MessageEventArgs e)
        {
            ConnectionAcceptServerMessage serverReponse = (ConnectionAcceptServerMessage)e.MessageContent;
            this.isActive = serverReponse.IsTargetActive;
            this.serverTargetInformation = new Tuple<string, object>(serverReponse.SenderName, this.serverTargetInformation.Item2);
        }

        private void ServerMessageReceived(object sender, MessageEventArgs e)
        {
            if (this.isActive)
            {
                this.messageProcessor.DataReceived(this, e);
            }
            else
            {
                this.serverDataManager.WriteData(e.MessageContent, serverTargetInformation);
            }
        }

        public void ConnectToServer(object server)
        {
            this.serverTargetInformation = new Tuple<string, object>(null, server);

            ConnectionRequestServerMessage connectionRequest = new ConnectionRequestServerMessage()
            {
                SenderStartTime = this.startTime,
                SenderName = this.Configuration.ServerName
            };

            this.serverDataManager.WriteData(connectionRequest, server);
        }

        public void DisconnectFromServer()
        {
            DisconnectServerMessage disconnect = new DisconnectServerMessage();
            this.serverDataManager.WriteData(disconnect, this.serverTargetInformation);
        }

        private void ConnectionRequestServer(object sender, MessageEventArgs e)
        {
            ConnectionRequestServerMessage request = (ConnectionRequestServerMessage)e.MessageContent;

            if (request.SenderStartTime == this.startTime)
            {
                Random randomServerSelector = new Random();
                int next = randomServerSelector.Next(0, 2);
                this.isActive = next == 0 ? true : false;
            }
            else
            {
                if (request.SenderStartTime > this.startTime)
                {
                    this.isActive = true;
                }
                else
                {
                    this.isActive = false;

                }
            }

            ConnectionAcceptServerMessage response = new ConnectionAcceptServerMessage()
            {
                SenderName = this.Configuration.ServerName,
                IsTargetActive = !this.isActive
            };

            this.serverTargetInformation = new Tuple<string, object>(request.SenderName, request.SenderInformation);
            this.serverDataManager.WriteData(response, this.serverTargetInformation.Item2);
        }

        private void SendScores(object sender, MessageEventArgs e)
        {
            ScoresRequestMessage requestMessage = e.MessageContent as ScoresRequestMessage;

            List<ScoreEntry> scores = this.clientsTargetInformation.Select(p => new ScoreEntry(p.Key.PlayerName, p.Key.Score)).OrderByDescending(p => p.Score).ToList();
            ScoresResponseMessage responseMessage = new ScoresResponseMessage()
            {
                Scores = scores
            };

            clientDataManager.WriteData(responseMessage, requestMessage.SenderInformation);
        }

        private void SubmitAnswer(object sender, MessageEventArgs e)
        {
            AnswerMessage answerMessage = e.MessageContent as AnswerMessage;
            IPEndPoint clientAdress = (IPEndPoint)answerMessage.SenderInformation;
            var client = this.clientsTargetInformation.First(p => p.Value.Equals(clientAdress)).Key;

            if (client.Score < this.Configuration.MaxScore &&
                client.Score > this.Configuration.MinScore)
            {
                string logMessage = string.Format("Answer {0} from {1} to {2}. ", answerMessage.Solution, client.PlayerName, this.Configuration.ServerName);

                if (this.clientTimers.ContainsKey(client))
                {
                    this.clientTimers[client].Dispose();
                }
                else
                {
                    throw new NotImplementedException();
                }

                if (answerMessage.Solution == this.questions[client].Answer)
                {
                    logMessage += "Right. Score: " + (client.Score + 1);
                    this.LogText(logMessage);
                    client.Score++;
                }
                else
                {
                    logMessage += "Wrong. Score: " + (client.Score - 1);
                    this.LogText(logMessage);
                    client.Score--;
                }

                this.CreateQuestion(client);
            }
        }

        private void ConnectionRequestedMonitor(object sender, MessageEventArgs e)
        {
            ConnectionRequestMonitorMessage request = (ConnectionRequestMonitorMessage)e.MessageContent;
            LogText(string.Format("Connection request from Monitor ({0}) to {1}", request.SenderInformation, this.Configuration.ServerName));

            if (this.monitorsTargetInformation.Contains(request.SenderInformation))
            {
                ConnectionDeniedMessage deniedMessage = new ConnectionDeniedMessage();
                monitorDataManager.WriteData(deniedMessage, request.SenderInformation);
                LogText(string.Format("Connection denied from {0} to Monitor ({1})", this.Configuration.ServerName, request.SenderInformation));
            }
            else
            {
                this.monitorsTargetInformation.Add(request.SenderInformation);
                ConnectionAcceptMessage acceptMessage = new ConnectionAcceptMessage();
                monitorDataManager.WriteData(acceptMessage, request.SenderInformation);
                LogText(string.Format("Connection accepted from {0} to Monitor ({1})", this.Configuration.ServerName, request.SenderInformation));
            }
        }

        private void Disconnect(object sender, MessageEventArgs e)
        {
            DisconnectMessage disconnectMessage = (DisconnectMessage)e.MessageContent;
            Client client = this.GetClientFromMessage(disconnectMessage);

            if (this.clientsTargetInformation.ContainsKey(client))
            {
                if (this.clientTimers.ContainsKey(client))
                {
                    this.clientTimers[client].Dispose();
                    this.clientTimers.Remove(client);
                }

                LogText(string.Format("{0} disconnected from {1}", client.PlayerName, this.Configuration.ServerName));
                this.clientsTargetInformation.Remove(client);
            }
            else if (this.monitorsTargetInformation.Contains(disconnectMessage.SenderInformation))
            {
                LogText(string.Format("Monitor ({0}) disconnected from {1}", disconnectMessage.SenderInformation, this.Configuration.ServerName));
                this.monitorsTargetInformation.Remove(disconnectMessage.SenderInformation);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private void ConnectionRequestedClient(object sender, MessageEventArgs e)
        {
            var message = e.MessageContent;
            var senderName = message.SenderName;
            var senderInformation = message.SenderInformation;

            LogText(string.Format("Connection request from {0} to {1}", senderName, this.Configuration.ServerName));
            var client = this.GetClientFromMessage(message);

            if (client != null || this.clientsTargetInformation.Any(p => p.Key.PlayerName == senderName))
            {
                ConnectionDeniedMessage deniedMessage = new ConnectionDeniedMessage();
                clientDataManager.WriteData(deniedMessage, senderInformation);
                LogText(string.Format("Connection denied from {0} to {1}", this.Configuration.ServerName, senderName));
            }
            else
            {
                client = new Client(senderName, this.Configuration.MinScore, this.Configuration.MaxScore);
                client.MinScoreReached += ClientLost;
                client.MaxScoreReached += ClientWon;
                this.clientsTargetInformation.Add(client, senderInformation);
                ConnectionAcceptMessage acceptedMessage = new ConnectionAcceptMessage();
                clientDataManager.WriteData(acceptedMessage, senderInformation);
                this.LogText(string.Format("Connection accepted from {0} to {1}", this.Configuration.ServerName, senderName));

                this.CreateQuestion(client);
            }

        }

        private void ClientWon(object sender, EventArgs e)
        {
            var client = sender as Client;

            GameWonMessage wonMessage = new GameWonMessage()
            {
                Score = client.Score
            };

            this.clientDataManager.WriteData(wonMessage, this.clientsTargetInformation[client]);
            this.LogText(string.Format("{0} won.", client.PlayerName));
        }

        private void ClientLost(object sender, EventArgs e)
        {
            var client = sender as Client;
            GameLostMessage lostMessage = new GameLostMessage()
            {
                Score = client.Score
            };

            this.clientDataManager.WriteData(lostMessage, this.clientsTargetInformation[client]);
            this.LogText(string.Format("{0} lost.", client.PlayerName));
        }

        private void LogText(string loggingText)
        {
            if (this.OnLoggingMessage != null)
            {
                //this.OnLoggingMessage(this, new LoggingEventArgs(loggingText));
            }

            LoggingMessage loggingMessage = new LoggingMessage()
            {
                Text = loggingText
            };

            monitorsTargetInformation.ForEach(p => monitorDataManager.WriteData(loggingMessage, p));

            #region reflectionLogging
            //var message = e.MessageContent;
            //Type messageType = message.GetType();

            //string messageName = messageType.Name.Split(new string[] { "Message" }, StringSplitOptions.RemoveEmptyEntries)[0];
            //string senderIp = message.SenderEndPoint.ToString();

            //string recipientIp = e.Recipient.ToString();
            //recipientIp += string.Format(" ({0}) ", serverName);

            //if (clients.Any(p => IPEndPoint.Equals(p, message.SenderEndPoint)))
            //{
            //    senderIp += string.Format(" ({0}) ", clients.First(p => IPEndPoint.Equals(p, message.SenderEndPoint)));
            //}

            //var properties = messageType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            //string propertyStrings = string.Empty;

            //fullLoggingText = string.Format("{0} from {1} to {2}", messageName, senderIp, recipientIp);

            //if (properties.Count() != 0)
            //{
            //    foreach (var property in properties)
            //    {
            //        if (property.Name != "SenderEndPoint")
            //        {
            //            propertyStrings += string.Format("{0}: {1},", property.Name, property.GetValue(message));
            //        }
            //    }

            //    fullLoggingText += " arguments " + propertyStrings;
            //}
            #endregion
        }

        private void CreateQuestion(Client client)
        {
            if (client.Score < this.Configuration.MaxScore &&
                client.Score > this.Configuration.MinScore)
            {
                var questionCount = QuestionAccessor.MathQuestions.Count;

                Random randomQuestionGenerator = new Random();
                int questionIndex = randomQuestionGenerator.Next(0, questionCount);

                MathQuestion question = QuestionAccessor.MathQuestions[questionIndex];

                QuestionMessage questionMessage = new QuestionMessage()
                {
                    QuestionID = question.ID,
                    QuestionText = question.Question,
                    Time = question.Time,
                    Score = client.Score
                };

                this.LogText(string.Format("Question {0} sent from {1} to {2}", questionMessage.QuestionText, this.Configuration.ServerName, client.PlayerName));
                clientDataManager.WriteData(questionMessage, this.clientsTargetInformation[client]);

                this.questions[client] = question;
                this.clientTimers[client] = new Timer(this.QuestionTimeExpired, client, question.Time * 1000, Timeout.Infinite);
            }
        }

        private void QuestionTimeExpired(object state)
        {
            Client client = (Client)state;
            this.LogText(string.Format("{0} expired the question answer time. Score: {1}", client.PlayerName, client.Score - 1));
            client.Score--;
            this.CreateQuestion(client);
        }

        private Client GetClientFromMessage(Message message)
        {
            return this.clientsTargetInformation.FirstOrDefault(p => p.Value.Equals(message.SenderInformation)).Key;
        }
    }
}