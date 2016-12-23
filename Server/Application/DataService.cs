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
using System.Net.Sockets;
using Server.Application.Exceptions;

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
            //Initiate relevant properties.
            this.startTime = DateTime.Now;
            this.isActive = true;
            this.Configuration = configuration;
            this.monitors = new List<Monitor>();
            this.clients = new List<Client>();

            //Create the actions for clients to server communication.
            MessageProcessor clientMessageProcessor = new MessageProcessor();
            clientMessageProcessor.OnConnectionRequestClient += ConnectionRequestedClient;
            clientMessageProcessor.OnDisconnect += Disconnect;
            clientMessageProcessor.OnAnswer += SubmitAnswer;
            clientMessageProcessor.OnScoreRequest += SendScores;

            //Create actions for monitor to server communication.
            MessageProcessor monitorMessageProcessor = new MessageProcessor();
            monitorMessageProcessor.OnConnectionRequestMonitor += ConnectionRequestedMonitor;
            monitorMessageProcessor.OnDisconnect += Disconnect;

            //Create actions for server to server communication.
            MessageProcessor serverMessageProcessor = new MessageProcessor();
            serverMessageProcessor.OnConnectionRequestServer += ServerConnectionRequestReceived;
            serverMessageProcessor.OnConnectionAcceptedServer += ServerConnectionAcceptReceived;
            serverMessageProcessor.OnDisconnectServer += ServerDisconnectReceived;
            serverMessageProcessor.OnForwardingMessage += ServerForwardingReceived;
            serverMessageProcessor.OnServerScoreRequestMessage += ServerScoreRequestReceived;
            serverMessageProcessor.OnServerScoreResponseMessage += ServerScoreRepsonseReceived;
            serverMessageProcessor.OnServerClientsRequestMessage += ClientsRequestMessage;
            serverMessageProcessor.OnServerClientsResponseMessage += ClientsResponseMessage;

            //Try to start servers, throw exceptions wich given port (or named pipe name) if not possible.
            int port = this.Configuration.ClientPort;

            try
            {
                if (configuration.UseNamedPipes)
                {
                    clientDataManager = new NamedPipeManager(configuration.ServerName);
                }
                else
                {
                    clientDataManager = new UdpServerManager(port);
                }

                clientDataManager.OnDataReceived += clientMessageProcessor.DataReceived;

                port = this.Configuration.MonitorPort;
                monitorDataManager = new TcpServerManager(port);
                monitorDataManager.OnDataReceived += monitorMessageProcessor.DataReceived;

                port = this.Configuration.ServerPort;
                this.serverDataManager = new TcpServerManager(port);
                this.serverDataManager.OnDataReceived += serverMessageProcessor.DataReceived;
            }
            catch (SocketException)
            {
                throw new PortException(port);
            }
        }

        /// <summary>
        /// Server to server communication. When the active server needs to send a broadcast message he needs to get all client target information from the passive server.
        /// </summary>
        /// <param name="sender">The active server.</param>
        /// <param name="eventArgs">Contains the message request.</param>
        private void ClientsRequestMessage(object sender, BroadcastRequestMessageEventArgs eventArgs)
        {
            BroadcastRequestMessage broadcastRequestMessage = eventArgs.Message;
            BroadcastResponseMessage clientsResponseMessage = new BroadcastResponseMessage()
            {
                ClientsInformation = this.clients.Select(p => p.TargetInformation).ToList(),
                MessageToBroadcast = broadcastRequestMessage.MessageToBroadcast
            };

            this.serverDataManager.WriteData(clientsResponseMessage, this.otherServer.TargetInformation.First());
        }

        /// <summary>
        /// Server to server communication. The active servers received the client target information from the passive server and sends the broadcast message to all clients.
        /// </summary>
        /// <param name="sender">The passive server.</param>
        /// <param name="eventArgs">Contains the message response with the passive server client target information.</param>
        private void ClientsResponseMessage(object sender, BroadcastResponseMessageEventArgs eventArgs)
        {
            BroadcastResponseMessage clientsResponseMessage = eventArgs.Message;
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

        /// <summary>
        /// Server to server communication. The active server needs to answer a highscore request message and asks the passive server for score information.
        /// </summary>
        /// <param name="sender">The active server.</param>
        /// <param name="eventArgs">Contains the message request with the client who wants to be informed about the high scores. This parameter will be sent back to the active server so it knows where to send the scores to.</param>
        private void ServerScoreRequestReceived(object sender, ServerScoreRequestMessageEventArgs eventArgs)
        {
            var serverScoreRequest = eventArgs.Message;
            ServerScoreResponseMessage serverScoreResponse = new ServerScoreResponseMessage()
            {
                Scores = this.GetScores(),
                RequestSender = serverScoreRequest.RequestSender
            };

            this.serverDataManager.WriteData(serverScoreResponse, sender);
        }

        /// <summary>
        /// Server to server communication. The active server received the score information and sends it to the client who requested the scores.
        /// </summary>
        /// <param name="sender">The passive server.</param>
        /// <param name="eventArgs">Contains the message response with the scores and the client who wants to be informed about the high scores.</param>
        private void ServerScoreRepsonseReceived(object sender, ServerScoreResponseMessageEventArgs eventArgs)
        {
            ServerScoreResponseMessage serverScoreResponse = eventArgs.Message;
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

        /// <summary>
        /// Server to server communication. The passive server doesn't send any messages directly to clients. This method receives a message from the passive server and sends it to the client.
        /// </summary>
        /// <param name="sender">The passive server.</param>
        /// <param name="eventArgs">Contains the forwarding message with the given target.</param>
        private void ServerForwardingReceived(object sender, ForwardingMessageEventArgs eventArgs)
        {
            ForwardingMessage forwardingMessage = eventArgs.Message;
            this.LogText(string.Format("{0} received message from {1} and sent it to {2}", this.Configuration.ServerName, this.otherServer, forwardingMessage.TargetName));
            this.clientDataManager.WriteData(forwardingMessage.InnerMessage, forwardingMessage.Target);
        }

        /// <summary>
        /// Server to server communication. When a server wants to connect to another server. Sending a connection request message.
        /// </summary>
        /// <param name="server">The server target information where it connects to.</param>
        public void ServerRegister(object server)
        {
            this.serverDataManager.Register(server);

            ConnectionRequestServerMessage connectionRequest = new ConnectionRequestServerMessage()
            {
                SenderStartTime = this.startTime,
                SenderName = this.Configuration.ServerName
            };

            this.serverDataManager.WriteData(connectionRequest, server);
        }

        /// <summary>
        /// Server to server communication. When a server sends a connection request to another server. Sends a connection accept message back and decides who is the active or passive server.
        /// </summary>
        /// <param name="sender">The other server who wants to connect.</param>
        /// <param name="eventArgs">Contains the request message.</param>
        private void ServerConnectionRequestReceived(object sender, ConnectionRequestServerMessageEventArgs eventArgs)
        {
            ConnectionRequestServerMessage request = eventArgs.Message;

            //Decide if active or passive server.
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

            //Send response
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
            this.serverDataManager.WriteData(response, sender);
        }

        /// <summary>
        /// Server to server communication. Gets informed that the connection was accepted and adds the server as passive or active server.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">Contains the connection accepted message with parameters to decide who's active or passive now.</param>
        private void ServerConnectionAcceptReceived(object sender, ConnectionAcceptedServerMessageEventArgs eventArgs)
        {
            ConnectionAcceptServerMessage serverReponse = eventArgs.Message;
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

        /// <summary>
        /// Unregisters one server connection.
        /// </summary>
        public void ServerUnregister()
        {
            DisconnectServerMessage disconnect = new DisconnectServerMessage();

            //Delete the last connection.
            var server = this.otherServer.TargetInformation.LastOrDefault();

            if (server != null)
            {
                this.serverDataManager.WriteData(disconnect, server);
                this.serverDataManager.Unregister(server);
                this.otherServer.TargetInformation.Remove(this.otherServer.TargetInformation.Last(p => p.Equals(server)));
                this.OnServerConnectionCountChanged?.Invoke(this, new ServerConnectionCountChangedEventArgs(this.ServerConnectionCount));
            }
        }

        /// <summary>
        /// Server to server communication. Happens when one server disconnects from the other. Removes a server connection and calls event that the server connection count has changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">Contains the disconnect message, which doesn't contain any parameters.</param>
        private void ServerDisconnectReceived(object sender, DisconnectServerMessageEventArgs eventArgs)
        {
            this.serverDataManager.Unregister(sender);
            this.otherServer.TargetInformation.Remove(this.otherServer.TargetInformation.Last(p => p.Equals(sender)));
            this.OnServerConnectionCountChanged?.Invoke(this, new ServerConnectionCountChangedEventArgs(this.ServerConnectionCount));

            //If there are no more connections, both server become active again.
            if (this.otherServer.TargetInformation.Count == 0)
            {
                this.isActive = true;
                this.otherServer = null;
            }
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