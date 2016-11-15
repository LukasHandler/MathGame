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
using Server.Application.EventArguments;

namespace Server.Application
{
    public class DataService
    {
        private bool isActive;

        private DateTime startTime;

        private IDataManager clientDataManager;

        private List<Client> clients;

        private List<Monitor> monitors;

        private IDataManager monitorDataManager;

        private IDataManager serverDataManager;

        private ServerConfiguration Configuration;

        public EventHandler<LoggingEventArgs> OnLoggingMessage;

        public EventHandler<ServerConnectionCountChangedEventArgs> OnServerConnectionCountChanged;

        private Server otherServer;

        public int ServerConnectionCount
        {
            get
            {
                if (this.otherServer == null)
                {
                    return 0;
                }
                else
                {
                    return this.otherServer.TargetInformation.Count();
                }
            }
        }

        public DataService(ServerConfiguration configuration)
        {
            this.startTime = DateTime.Now;
            this.isActive = true;
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
            monitors = new List<Monitor>();

            this.clients = new List<Client>();

            MessageProcessor serverMessageProcessor = new MessageProcessor();
            this.serverDataManager = new TcpServerManager(configuration.ServerPort);
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

        private void ClientsResponseMessage(object sender, BroadcastResponseMessageEventArgs e)
        {
            BroadcastResponseMessage clientsResponseMessage = e.Message;
            var broadcastMessage = clientsResponseMessage.MessageToBroadcast;

            foreach (var client in this.clients)
            {
                this.clientDataManager.WriteData(broadcastMessage, client.TargetInformation);
            }

            foreach (var item in clientsResponseMessage.ClientsInformation)
            {
                this.clientDataManager.WriteData(broadcastMessage, item);
            }
        }

        private void ClientsRequestMessage(object sender, BroadcastRequestMessageEventArgs e)
        {
            BroadcastRequestMessage broadcastRequestMessage = e.Message;
            BroadcastResponseMessage clientsResponseMessage = new BroadcastResponseMessage()
            {
                ClientsInformation = this.clients.Select(p => p.TargetInformation).ToList(),
                MessageToBroadcast = broadcastRequestMessage.MessageToBroadcast
            };

            this.serverDataManager.WriteData(clientsResponseMessage, this.otherServer.TargetInformation.First());
        }

        private void ServerScoreRepsonse(object sender, ServerScoreResponseMessageEventArgs e)
        {
            ServerScoreResponseMessage serverScoreResponse = e.Message;

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

        private void ServerScoreRequest(object sender, ServerScoreRequestMessageEventArgs e)
        {
            var serverScoreRequest = e.Message;
            ServerScoreResponseMessage serverScoreResponse = new ServerScoreResponseMessage()
            {
                Scores = this.GetScores(),
                RequestSender = serverScoreRequest.RequestSender
            };

            this.serverDataManager.WriteData(serverScoreResponse, sender);
        }

        private void ForwardMessage(object sender, ForwardingMessageEventArgs e)
        {
            ForwardingMessage forwardingMessage = e.Message;
            this.LogText(string.Format("{0} received message from {1} and sent it to {2}", this.Configuration.ServerName, this.otherServer, forwardingMessage.TargetName));
            this.clientDataManager.WriteData(forwardingMessage.InnerMessage, forwardingMessage.Target);
        }

        private void DisconnectServer(object sender, DisconnectServerMessageEventArgs e)
        {
            this.serverDataManager.Unregister(sender);
            this.otherServer.TargetInformation.Remove(this.otherServer.TargetInformation.First(p => p.Equals(sender)));
            this.OnServerConnectionCountChanged?.Invoke(this, new ServerConnectionCountChangedEventArgs(this.ServerConnectionCount));

            if (this.otherServer.TargetInformation.Count == 0)
            {
                this.isActive = true;
                this.otherServer = null;
            }
        }

        private void ConnectionAcceptedServer(object sender, ConnectionAcceptedServerMessageEventArgs e)
        {
            ConnectionAcceptServerMessage serverReponse = e.Message;
            this.isActive = serverReponse.IsTargetActive;

            if (this.otherServer == null)
            {
                this.otherServer = new Server(serverReponse.SenderName, sender);
            }
            else
            {
                this.otherServer.TargetInformation.Add(sender);
            }

            this.OnServerConnectionCountChanged?.Invoke(this, new ServerConnectionCountChangedEventArgs(this.ServerConnectionCount));
        }

        public void RegisterToServer(object server)
        {
            this.serverDataManager.Register(server);

            ConnectionRequestServerMessage connectionRequest = new ConnectionRequestServerMessage()
            {
                SenderStartTime = this.startTime,
                SenderName = this.Configuration.ServerName
            };

            this.serverDataManager.WriteData(connectionRequest, server);
        }

        public void UnregisterFromServer(object server)
        {
            DisconnectServerMessage disconnect = new DisconnectServerMessage()
            {
                LastConnection = this.otherServer.TargetInformation.Count == 1 ? true : false
            };

            this.serverDataManager.WriteData(disconnect, server);
            this.serverDataManager.Unregister(server);
            this.otherServer.TargetInformation.Remove(this.otherServer.TargetInformation.First(p => p.Equals(server)));
            this.OnServerConnectionCountChanged?.Invoke(this, new ServerConnectionCountChangedEventArgs(this.ServerConnectionCount));
        }

        private void ConnectionRequestServer(object sender, ConnectionRequestServerMessageEventArgs e)
        {
            ConnectionRequestServerMessage request = e.Message;

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

            if (this.otherServer == null)
            {
                this.otherServer = new Server(request.SenderName, sender);
            }
            else
            {
                this.otherServer.TargetInformation.Add(sender);
            }

            this.OnServerConnectionCountChanged?.Invoke(this, new ServerConnectionCountChangedEventArgs(this.ServerConnectionCount));
            this.serverDataManager.WriteData(response, this.otherServer.TargetInformation.First());
        }

        private void SendScores(object sender, MessageEventArgs e)
        {
            ScoresRequestMessage requestMessage = e.MessageContent as ScoresRequestMessage;
            List<ScoreEntry> scores = GetScores();

            if (this.otherServer == null)
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
                    ServerScoreRequestMessage serverRequestMessage = new ServerScoreRequestMessage() { RequestSender = sender };
                    this.serverDataManager.WriteData(serverRequestMessage, this.otherServer.TargetInformation.First());
                }
                else
                {
                    ServerScoreResponseMessage serverResponseMessage = new ServerScoreResponseMessage()
                    {
                        RequestSender = sender,
                        Scores = scores
                    };
                    this.serverDataManager.WriteData(serverResponseMessage, this.otherServer.TargetInformation.First());
                }
            }

        }

        private List<ScoreEntry> GetScores()
        {
            return this.clients.Select(p => new ScoreEntry(p.Name, p.Score)).OrderByDescending(p => p.Score).ToList();
        }

        private void SubmitAnswer(object sender, AnswerMessageEventArgs e)
        {
            AnswerMessage answerMessage = e.Message;
            var client = this.clients.First(p => p.TargetInformation.Equals(sender));

            if (client.Score < this.Configuration.MaxScore &&
                client.Score > this.Configuration.MinScore)
            {
                string logMessage = string.Format("Answer {0} from {1} to {2}. ", answerMessage.Solution, client.Name, this.Configuration.ServerName);

                if (client.QuestionTimer != null)
                {
                    client.QuestionTimer.Dispose();
                }
                else
                {
                    throw new NotImplementedException();
                }

                if (answerMessage.Solution == client.Question.Answer)
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

            if (this.monitors.Contains(sender))
            {
                ConnectionDeniedMessage deniedMessage = new ConnectionDeniedMessage();
                monitorDataManager.WriteData(deniedMessage, sender);
                LogText(string.Format("Connection denied from {0} to Monitor ({1})", this.Configuration.ServerName, sender));
            }
            else
            {
                this.monitors.Add(new Monitor(sender));
                ConnectionAcceptMessage acceptMessage = new ConnectionAcceptMessage();
                monitorDataManager.WriteData(acceptMessage, sender);
                LogText(string.Format("Connection accepted from {0} to Monitor ({1})", this.Configuration.ServerName, sender));
            }
        }

        private void Disconnect(object sender, MessageEventArgs e)
        {
            DisconnectMessage disconnectMessage = (DisconnectMessage)e.MessageContent;
            Client client = this.GetClientFromSenderInformation(sender);
            Monitor monitor = this.monitors.FirstOrDefault(p => p.TargetInformation.Equals(sender));

            if (this.clients.Contains(client))
            {
                if (client.QuestionTimer != null)
                {
                    client.QuestionTimer.Dispose();
                    client.QuestionTimer = null;
                }

                LogText(string.Format("{0} disconnected from {1}", client.Name, this.Configuration.ServerName));
                this.clients.Remove(client);
            }
            else if (monitor != null)
            {
                LogText(string.Format("Monitor ({0}) disconnected from {1}", sender, this.Configuration.ServerName));
                this.monitors.Remove(monitor);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private void ConnectionRequestedClient(object sender, ConnectionRequestClientMessageEventArgs e)
        {
            ConnectionRequestClientMessage message = e.Message;
            var senderName = message.SenderName;

            LogText(string.Format("Connection request from {0} to {1}", senderName, this.Configuration.ServerName));
            var client = this.GetClientFromSenderInformation(sender);

            if (client != null || this.clients.Any(p => p.Name == senderName))
            {
                ConnectionDeniedMessage deniedMessage = new ConnectionDeniedMessage();
                this.SendClientMessage(deniedMessage, sender);
                //clientDataManager.WriteData(deniedMessage, sender);
                LogText(string.Format("Connection denied from {0} to {1}", this.Configuration.ServerName, senderName));
            }
            else
            {
                client = new Client(senderName, this.Configuration.MinScore, this.Configuration.MaxScore, sender);
                client.MinScoreReached += ClientLost;
                client.MaxScoreReached += ClientWon;
                this.clients.Add(client);
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

            this.SendClientMessage(wonMessage, client.TargetInformation);
            var message = string.Format("{0} won.", client.Name);
            this.LogText(message);
            this.SendBroadcastText(message);
        }

        private void SendBroadcastText(string text)
        {
            BroadcastMessage broadcastMessage = new BroadcastMessage() { Text = text };

            if (this.otherServer == null)
            {
                foreach (var client in this.clients)
                {
                    this.clientDataManager.WriteData(broadcastMessage, client.TargetInformation);
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

                    this.serverDataManager.WriteData(broadcastRequestMessage, this.otherServer.TargetInformation.First());
                }
                else
                {
                    BroadcastResponseMessage broadcastResponseMessage = new BroadcastResponseMessage()
                    {
                        ClientsInformation = this.clients.Select(p => p.TargetInformation).ToList(),
                        MessageToBroadcast = broadcastMessage
                    };

                    this.serverDataManager.WriteData(broadcastResponseMessage, this.otherServer.TargetInformation.First());
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

            this.SendClientMessage(lostMessage, client.TargetInformation);
            var message = string.Format("{0} lost.", client.Name);
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

            monitors.ForEach(p => monitorDataManager.WriteData(loggingMessage, p.TargetInformation));

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

                this.LogText(string.Format("Question {0} sent from {1} to {2}", questionMessage.QuestionText, this.Configuration.ServerName, client.Name));
                //clientDataManager.WriteData(questionMessage, this.clientsTargetInformation[client]);
                this.SendClientMessage(questionMessage, client.TargetInformation);

                client.Question = question;
                client.QuestionTimer = new Timer(this.QuestionTimeExpired, client, question.Time * 1000, Timeout.Infinite);
            }
        }

        private void QuestionTimeExpired(object state)
        {
            Client client = (Client)state;
            this.LogText(string.Format("{0} expired the question answer time. Score: {1}", client.Name, client.Score - 1));
            client.Score--;
            this.CreateQuestion(client);
        }

        private Client GetClientFromSenderInformation(object senderInformation)
        {
            return this.clients.FirstOrDefault(p => p.TargetInformation.Equals(senderInformation));
        }

        private void SendClientMessage(Message message, object target)
        {
            if (this.isActive)
            {
                this.clientDataManager.WriteData(message, target);
            }
            else
            {
                this.LogText(string.Format("{0} sent message to {1}", this.Configuration.ServerName, this.otherServer));
                ForwardingMessage forwardingMessage = new ForwardingMessage() { InnerMessage = message, Target = target, TargetName = this.GetClientFromSenderInformation(target).Name };
                this.serverDataManager.WriteData(forwardingMessage, this.otherServer.TargetInformation.First());
            }
        }
    }
}