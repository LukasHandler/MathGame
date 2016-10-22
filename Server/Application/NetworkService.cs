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
        private IDataManager clientDataManager;

        private Dictionary<Guid, Client> clients;

        private Dictionary<Guid, Timer> clientTimers;

        private Dictionary<Guid, MathQuestion> questions;

        private IDataManager monitorDataManager;
        
        private IDataManager serverDataManager;

        private List<Guid> monitors;

        //private MessageProcessor messageProcessorClients;

        //private MessageProcessor messageProcessorMonitors;

        //private MessageProcessor messageProcessorServers;

        private ServerConfiguration Configuration;

        public EventHandler<LoggingEventArgs> OnLoggingMessage;

        public NetworkService(ServerConfiguration configuration)
        {
            this.questions = new Dictionary<Guid, MathQuestion>();
            this.Configuration = configuration;

            MessageProcessor messageProcessorClients = new MessageProcessor();
            messageProcessorClients.OnConnectionRequest += ConnectionRequestedClient;
            messageProcessorClients.OnDisconnect += Disconnect;
            messageProcessorClients.OnAnswer += SubmitAnswer;
            messageProcessorClients.OnScoreRequest += SendScores;

            clientDataManager = new UdpServerManager(this.Configuration.ClientPort);
            clientDataManager.OnDataReceived += messageProcessorClients.DataReceived;

            MessageProcessor messageProcessorMonitors = new MessageProcessor();
            messageProcessorMonitors.OnConnectionRequest += ConnectionRequestedMonitor;
            monitorDataManager = new TcpServerManager(this.Configuration.MonitorPort);
            monitorDataManager.OnDataReceived += messageProcessorMonitors.DataReceived;
            monitors = new List<Guid>();

            clientTimers = new Dictionary<Guid, Timer>();
            this.clients = new Dictionary<Guid, Client>();

            MessageProcessor messageProcessorServers = new MessageProcessor();
            this.serverDataManager = new TcpServerManager(configuration.ServerPort);
            this.serverDataManager.OnDataReceived += messageProcessorServers.DataReceived;
            messageProcessorServers.OnConnectionRequest += ConnectionRequestServer;
        }

        private void ConnectionRequestServer(object sender, MessageEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void SendScores(object sender, MessageEventArgs e)
        {
            ScoresRequestMessage requestMessage = e.MessageContent as ScoresRequestMessage;

            List<ScoreEntry> scores = this.clients.Select(p => new ScoreEntry(p.Value.PlayerName, p.Value.Score)).OrderByDescending(p => p.Score).ToList();
            ScoresResponseMessage responseMessage = new ScoresResponseMessage()
            {
                Scores = scores
            };

            clientDataManager.WriteData(responseMessage, requestMessage.SenderId);
        }

        private void SubmitAnswer(object sender, MessageEventArgs e)
        {
            AnswerMessage answerMessage = e.MessageContent as AnswerMessage;
            Guid clientGuid = answerMessage.SenderId;

            if (this.clients[clientGuid].Score < this.Configuration.MaxScore &&
                this.clients[clientGuid].Score > this.Configuration.MinScore)
            {
                string logMessage = string.Format("Answer {0} from {1} to {2}. ", answerMessage.Solution, this.clients[clientGuid].PlayerName, this.Configuration.ServerName);

                if (this.clientTimers.ContainsKey(clientGuid))
                {
                    this.clientTimers[clientGuid].Dispose();
                }
                else
                {
                    throw new NotImplementedException();
                }

                if (answerMessage.Solution == this.questions[clientGuid].Answer)
                {
                    logMessage += "Right. Score: " + (this.clients[clientGuid].Score + 1);
                    this.LogText(logMessage);
                    this.clients[clientGuid].Score++;
                }
                else
                {
                    logMessage += "Wrong. Score: " + (this.clients[clientGuid].Score - 1);
                    this.LogText(logMessage);
                    this.clients[clientGuid].Score--;
                }

                this.CreateQuestion(clientGuid);
            }
        }

        private void ConnectionRequestedMonitor(object sender, MessageEventArgs e)
        {
            ConnectionRequestMessage request = (ConnectionRequestMessage)e.MessageContent;
            LogText(string.Format("Connection request from Monitor ({0}) to {1}", request.SenderId, this.Configuration.ServerName));

            if (this.monitors.Contains(request.SenderId))
            {
                ConnectionDeniedMessage deniedMessage = new ConnectionDeniedMessage();
                monitorDataManager.WriteData(deniedMessage, request.SenderId);
                LogText(string.Format("Connection denied from {0} to Monitor ({1})", this.Configuration.ServerName, request.SenderId));
            }
            else
            {
                this.monitors.Add(request.SenderId);
                ConnectionAcceptMessage acceptMessage = new ConnectionAcceptMessage();
                monitorDataManager.WriteData(acceptMessage, request.SenderId);
                LogText(string.Format("Connection accepted from {0} to Monitor ({1})", this.Configuration.ServerName, request.SenderId));
            }
        }

        private void Disconnect(object sender, MessageEventArgs e)
        {
            DisconnectMessage disconnectMessage = (DisconnectMessage)e.MessageContent;

            if (this.clients.ContainsKey(disconnectMessage.SenderId))
            {
                if (this.clientTimers.ContainsKey(disconnectMessage.SenderId))
                {
                    this.clientTimers[disconnectMessage.SenderId].Dispose();
                    this.clientTimers.Remove(disconnectMessage.SenderId);
                }

                LogText(string.Format("{0} disconnected from {1}", this.clients[disconnectMessage.SenderId].PlayerName, this.Configuration.ServerName));
                this.clients.Remove(disconnectMessage.SenderId);
            }
            else if (this.monitors.Contains(disconnectMessage.SenderId))
            {
                LogText(string.Format("Monitor ({0}) disconnected from {1}", disconnectMessage.SenderId, this.Configuration.ServerName));
                this.monitors.Remove(disconnectMessage.SenderId);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private void ConnectionRequestedClient(object sender, MessageEventArgs e)
        {
            ConnectionRequestMessage request = (ConnectionRequestMessage)e.MessageContent;
            LogText(string.Format("Connection request from {0} to {1}", request.PlayerName, this.Configuration.ServerName));

            if (this.clients.ContainsKey(request.SenderId) || this.clients.Any(p => p.Value.PlayerName == request.PlayerName))
            {
                ConnectionDeniedMessage deniedMessage = new ConnectionDeniedMessage();
                clientDataManager.WriteData(deniedMessage, request.SenderId);
                LogText(string.Format("Connection denied from {0} to {1}", this.Configuration.ServerName, request.PlayerName));
            }
            else
            {
                var client = new Client(request.PlayerName, this.Configuration.MinScore, this.Configuration.MaxScore);
                client.MinScoreReached += ClientLost;
                client.MaxScoreReached += ClientWon;
                this.clients.Add(request.SenderId, client);
                ConnectionAcceptMessage acceptedMessage = new ConnectionAcceptMessage();
                clientDataManager.WriteData(acceptedMessage, request.SenderId);
                this.LogText(string.Format("Connection accepted from {0} to {1}", this.Configuration.ServerName, request.PlayerName));

                this.CreateQuestion(request.SenderId);
            }

        }

        private void ClientWon(object sender, EventArgs e)
        {
            var client = sender as Client;
            var clientGuid = this.clients.First(p => p.Value == client).Key;
            GameWonMessage wonMessage = new GameWonMessage()
            {
                Score = client.Score
            };

            this.clientDataManager.WriteData(wonMessage, clientGuid);
            this.LogText(string.Format("{0} won.", this.clients[clientGuid].PlayerName));
        }

        private void ClientLost(object sender, EventArgs e)
        {
            var client = sender as Client;
            var clientGuid = this.clients.First(p => p.Value == client).Key;
            GameLostMessage lostMessage = new GameLostMessage()
            {
                Score = client.Score
            };

            this.clientDataManager.WriteData(lostMessage, clientGuid);
            this.LogText(string.Format("{0} lost.", this.clients[clientGuid].PlayerName));
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

            monitors.ForEach(p => monitorDataManager.WriteData(loggingMessage, p));

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

        private void CreateQuestion(Guid clientGuid)
        {
            if (this.clients[clientGuid].Score < this.Configuration.MaxScore &&
                this.clients[clientGuid].Score > this.Configuration.MinScore)
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
                    Score = this.clients[clientGuid].Score
                };

                //Take a break so other logging messages can be sent
                //Thread.Sleep(1);

                this.LogText(string.Format("Question {0} sent from {1} to {2}", questionMessage.QuestionText, this.Configuration.ServerName, this.clients[clientGuid].PlayerName));
                clientDataManager.WriteData(questionMessage, clientGuid);

                this.questions[clientGuid] = question;
                this.clientTimers[clientGuid] = new Timer(this.QuestionTimeExpired, clientGuid, question.Time * 1000, Timeout.Infinite);
            }
        }

        private void QuestionTimeExpired(object state)
        {
            Guid clientGuid = (Guid)state;
            this.LogText(string.Format("{0} expired the question answer time. Score: {1}", this.clients[clientGuid].PlayerName, this.clients[clientGuid].Score - 1));
            this.clients[clientGuid].Score--;
            this.CreateQuestion(clientGuid);
        }
    }
}