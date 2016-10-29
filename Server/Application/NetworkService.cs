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
using System.Collections.Concurrent;

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

        private Tuple<string, object> serverTargetInformation;

        public NetworkService(ServerConfiguration configuration)
        {
            this.startTime = DateTime.Now;
            this.isActive = true;
            this.questions = new Dictionary<Client, MathQuestion>();
            this.Configuration = configuration;

            MessageProcessor clientMessageProcessor = new MessageProcessor();
            clientMessageProcessor.OnConnectionRequestClient += ConnectionRequestedClient;
            clientMessageProcessor.OnDisconnect += Disconnect;
            clientMessageProcessor.OnAnswer += SubmitAnswer;
            clientMessageProcessor.OnScoreRequest += SendScores;

            clientDataManager = new UdpServerManager(this.Configuration.ClientPort);
            clientDataManager.OnDataReceived += clientMessageProcessor.DataReceived;

            MessageProcessor monitorMessageProcessor = new MessageProcessor();
            monitorMessageProcessor.OnConnectionRequestMonitor += ConnectionRequestedMonitor;
            monitorDataManager = new TcpServerManager(this.Configuration.MonitorPort);
            monitorDataManager.OnDataReceived += monitorMessageProcessor.DataReceived;
            monitorsTargetInformation = new List<object>();

            clientTimers = new Dictionary<Client, Timer>();
            this.clientsTargetInformation = new Dictionary<Client, object>();

            MessageProcessor serverMessageProcessor = new MessageProcessor();
            this.serverDataManager = new TcpClientServerManager(configuration.ServerPort);
            this.serverDataManager.OnDataReceived += serverMessageProcessor.DataReceived;
            serverMessageProcessor.OnConnectionRequestServer += ConnectionRequestServer;
            serverMessageProcessor.OnConnectionAcceptedServer += ConnectionAcceptedServer;
            serverMessageProcessor.OnDisconnectServer += DisconnectServer;
            serverMessageProcessor.OnForwardingMessage += ForwardMessage;
            serverMessageProcessor.OnServerScoreRequestMessage += ServerScoreRequest;
            serverMessageProcessor.OnServerScoreResponseMessage += ServerScoreRepsonse;

            serverMessageProcessor.OnServerClientsRequestMessage += ClientsRequestMessage;
            serverMessageProcessor.OnServerClientsResponseMessage += ClientsResponseMessage;
        }

        private void ClientsResponseMessage(object sender, MessageEventArgs e)
        {
            BroadcastResponseMessage clientsResponseMessage = e.MessageContent as BroadcastResponseMessage;
            var broadcastMessage = clientsResponseMessage.MessageToBroadcast;

            foreach (var item in this.clientsTargetInformation)
            {
                this.clientDataManager.WriteData(broadcastMessage, item.Value);
            }

            foreach (var item in clientsResponseMessage.ClientsInformation)
            {
                this.clientDataManager.WriteData(broadcastMessage, item);
            }
        }

        private void ClientsRequestMessage(object sender, MessageEventArgs e)
        {
            BroadcastRequestMessage broadcastRequestMessage = e.MessageContent as BroadcastRequestMessage;

            BroadcastResponseMessage clientsResponseMessage = new BroadcastResponseMessage()
            {
                ClientsInformation = this.clientsTargetInformation.Select(p => p.Value).ToList(),
                MessageToBroadcast = broadcastRequestMessage.MessageToBroadcast
            };

            this.serverDataManager.WriteData(clientsResponseMessage, this.serverTargetInformation.Item2);
        }

        private void ServerScoreRepsonse(object sender, MessageEventArgs e)
        {
            ServerScoreResponseMessage serverScoreResponse = e.MessageContent as ServerScoreResponseMessage;

            var scores = this.GetScores();

            foreach (var item in serverScoreResponse.Scores)
            {
                scores.Add(item);
            }

            ScoresResponseMessage scoreResponse = new ScoresResponseMessage()
            {
                Scores = scores.OrderByDescending(p => p.Score).ToList()
            };

            this.clientDataManager.WriteData(scoreResponse, serverScoreResponse.RequestSender);
        }

        private void ServerScoreRequest(object sender, MessageEventArgs e)
        {
            var serverScoreRequest = e.MessageContent as ServerScoreRequestMessage;
            ServerScoreResponseMessage serverScoreResponse = new ServerScoreResponseMessage()
            {
                Scores = this.GetScores(),
                RequestSender = serverScoreRequest.RequestSender
            };

            this.serverDataManager.WriteData(serverScoreResponse, sender);
        }

        private void ForwardMessage(object sender, MessageEventArgs e)
        {
            ForwardingMessage forwardingMessage = e.MessageContent as ForwardingMessage;
            this.LogText(string.Format("{0} received message from {1} and sent it to {2}", this.Configuration.ServerName, this.serverTargetInformation.Item1, forwardingMessage.TargetName));
            this.clientDataManager.WriteData(forwardingMessage.InnerMessage, forwardingMessage.Target);
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
            this.serverDataManager.WriteData(disconnect, this.serverTargetInformation.Item2);
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

            this.serverTargetInformation = new Tuple<string, object>(request.SenderName, sender);
            this.serverDataManager.WriteData(response, this.serverTargetInformation.Item2);
        }

        private void SendScores(object sender, MessageEventArgs e)
        {
            ScoresRequestMessage requestMessage = e.MessageContent as ScoresRequestMessage;
            List<ScoreEntry> scores = GetScores();

            if (this.serverTargetInformation == null)
            {
                ScoresResponseMessage responseMessage = new ScoresResponseMessage()
                {
                    Scores = scores
                };

                this.clientDataManager.WriteData(responseMessage, sender);
            }
            else
            {
                if (this.isActive)
                {
                    ServerScoreRequestMessage serverRequestMessage = new ServerScoreRequestMessage() { RequestSender = sender};
                    this.serverDataManager.WriteData(serverRequestMessage, this.serverTargetInformation.Item2);
                }
                else
                {
                    ServerScoreResponseMessage serverResponseMessage = new ServerScoreResponseMessage()
                    {
                        RequestSender = sender,
                        Scores = scores
                    };
                    this.serverDataManager.WriteData(serverResponseMessage, this.serverTargetInformation.Item2);
                }
            }

        }

        private List<ScoreEntry> GetScores()
        {
            return this.clientsTargetInformation.Select(p => new ScoreEntry(p.Key.PlayerName, p.Key.Score)).OrderByDescending(p => p.Score).ToList();
        }

        private void SubmitAnswer(object sender, MessageEventArgs e)
        {
            AnswerMessage answerMessage = e.MessageContent as AnswerMessage;
            //IPEndPoint clientAdress = (IPEndPoint)answerMessage.SenderInformation;
            var client = this.clientsTargetInformation.First(p => p.Value.Equals(sender)).Key;

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
            LogText(string.Format("Connection request from Monitor ({0}) to {1}", sender, this.Configuration.ServerName));

            if (this.monitorsTargetInformation.Contains(sender))
            {
                ConnectionDeniedMessage deniedMessage = new ConnectionDeniedMessage();
                monitorDataManager.WriteData(deniedMessage, sender);
                LogText(string.Format("Connection denied from {0} to Monitor ({1})", this.Configuration.ServerName, sender));
            }
            else
            {
                this.monitorsTargetInformation.Add(sender);
                ConnectionAcceptMessage acceptMessage = new ConnectionAcceptMessage();
                monitorDataManager.WriteData(acceptMessage, sender);
                LogText(string.Format("Connection accepted from {0} to Monitor ({1})", this.Configuration.ServerName, sender));
            }
        }

        private void Disconnect(object sender, MessageEventArgs e)
        {
            DisconnectMessage disconnectMessage = (DisconnectMessage)e.MessageContent;
            Client client = this.GetClientFromSenderInformation(sender);

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
            else if (this.monitorsTargetInformation.Contains(sender))
            {
                LogText(string.Format("Monitor ({0}) disconnected from {1}", sender, this.Configuration.ServerName));
                this.monitorsTargetInformation.Remove(sender);
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

            LogText(string.Format("Connection request from {0} to {1}", senderName, this.Configuration.ServerName));
            var client = this.GetClientFromSenderInformation(sender);

            if (client != null || this.clientsTargetInformation.Any(p => p.Key.PlayerName == senderName))
            {
                ConnectionDeniedMessage deniedMessage = new ConnectionDeniedMessage();
                this.SendClientMessage(deniedMessage, sender);
                //clientDataManager.WriteData(deniedMessage, sender);
                LogText(string.Format("Connection denied from {0} to {1}", this.Configuration.ServerName, senderName));
            }
            else
            {
                client = new Client(senderName, this.Configuration.MinScore, this.Configuration.MaxScore);
                client.MinScoreReached += ClientLost;
                client.MaxScoreReached += ClientWon;
                this.clientsTargetInformation.Add(client, sender);
                ConnectionAcceptMessage acceptedMessage = new ConnectionAcceptMessage();
                //clientDataManager.WriteData(acceptedMessage, sender);
                this.SendClientMessage(acceptedMessage, sender);
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

            this.SendClientMessage(wonMessage, this.clientsTargetInformation[client]);
            var message = string.Format("{0} won.", client.PlayerName);
            this.LogText(message);
            this.SendBroadcastText(message);
        }

        private void SendBroadcastText(string text)
        {
            BroadcastMessage broadcastMessage = new BroadcastMessage() { Text = text };

            if (this.serverTargetInformation == null)
            {
                foreach (var item in this.clientsTargetInformation)
                {
                    this.clientDataManager.WriteData(broadcastMessage, item.Value);
                }
            }
            else
            {
                if (this.isActive)
                {
                    BroadcastRequestMessage broadcastRequestMessage = new BroadcastRequestMessage()
                    {
                        MessageToBroadcast = broadcastMessage
                    };

                    this.serverDataManager.WriteData(broadcastRequestMessage, this.serverTargetInformation.Item2);
                }
                else
                {
                    BroadcastResponseMessage broadcastResponseMessage = new BroadcastResponseMessage()
                    {
                        ClientsInformation = this.clientsTargetInformation.Select(p => p.Value).ToList(),
                        MessageToBroadcast = broadcastMessage
                    };

                    this.serverDataManager.WriteData(broadcastResponseMessage, this.serverTargetInformation.Item2);
                }
            }
        }

        private void ClientLost(object sender, EventArgs e)
        {
            var client = sender as Client;
            GameLostMessage lostMessage = new GameLostMessage()
            {
                Score = client.Score
            };

            this.SendClientMessage(lostMessage, this.clientsTargetInformation[client]);
            var message = string.Format("{0} lost.", client.PlayerName);
            this.LogText(message);
            this.SendBroadcastText(message);
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
                //clientDataManager.WriteData(questionMessage, this.clientsTargetInformation[client]);
                this.SendClientMessage(questionMessage, this.clientsTargetInformation[client]);

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

        private Client GetClientFromSenderInformation(object senderInformation)
        {
            return this.clientsTargetInformation.FirstOrDefault(p => p.Value.Equals(senderInformation)).Key;
        }

        private void SendClientMessage(Message message, object target)
        {
            if (this.isActive)
            {
                this.clientDataManager.WriteData(message, target);
            }
            else
            {
                this.LogText(string.Format("{0} sent message to {1}", this.Configuration.ServerName, this.serverTargetInformation.Item1));
                ForwardingMessage forwardingMessage = new ForwardingMessage() { InnerMessage = message, Target = target, TargetName = this.GetClientFromSenderInformation(target).PlayerName};
                this.serverDataManager.WriteData(forwardingMessage, this.serverTargetInformation.Item2);
            }
        }
    }
}