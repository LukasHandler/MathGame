using System;
using System.Collections.Generic;
using System.Linq;
using Shared.Data;
using Shared.Data.Messages;
using Shared.Data.Managers;
using Shared.Data.EventArguments;
using System.Threading;
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
            clientMessageProcessor.OnConnectionRequestClient += ClientConnectionRequest;
            clientMessageProcessor.OnDisconnectClient += ClientDisconnect;
            clientMessageProcessor.OnAnswer += ClientSubmitAnswer;
            clientMessageProcessor.OnScoreRequest += ClientSendScores;

            //Create actions for monitor to server communication.
            MessageProcessor monitorMessageProcessor = new MessageProcessor();
            monitorMessageProcessor.OnConnectionRequestMonitor += MonitorConnectionRequest;
            monitorMessageProcessor.OnDisconnectClient += MonitorDisconnect;

            //Create actions for server to server communication.
            MessageProcessor serverMessageProcessor = new MessageProcessor();
            serverMessageProcessor.OnConnectionRequestServer += ServerConnectionRequestReceived;
            serverMessageProcessor.OnConnectionAcceptedServer += ServerConnectionAcceptReceived;
            serverMessageProcessor.OnDisconnectServer += ServerDisconnectReceived;
            serverMessageProcessor.OnForwardingMessage += ServerForwardingReceived;
            serverMessageProcessor.OnServerScoreRequestMessage += ServerScoreRequestReceived;
            serverMessageProcessor.OnServerScoreResponseMessage += ServerScoreRepsonseReceived;
            serverMessageProcessor.OnServerClientsRequestMessage += ServerClientsRequestMessage;
            serverMessageProcessor.OnServerClientsResponseMessage += ServerClientsResponseMessage;

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
        private void ServerClientsRequestMessage(object sender, BroadcastRequestMessageEventArgs eventArgs)
        {
            BroadcastRequestMessage broadcastRequestMessage = eventArgs.Message;
            this.LogReceivedText(broadcastRequestMessage, this.otherServer);

            BroadcastResponseMessage clientsResponseMessage = new BroadcastResponseMessage()
            {
                ClientsInformation = this.clients.Select(p => p.TargetInformation).ToList(),
                MessageToBroadcast = broadcastRequestMessage.MessageToBroadcast
            };

            this.LogSentText(clientsResponseMessage, this.otherServer);
            this.serverDataManager.WriteData(clientsResponseMessage, this.otherServer.TargetInformation.First());
        }

        /// <summary>
        /// Server to server communication. The active servers received the client target information from the passive server and sends the broadcast message to all clients.
        /// </summary>
        /// <param name="sender">The passive server.</param>
        /// <param name="eventArgs">Contains the message response with the passive server client target information.</param>
        private void ServerClientsResponseMessage(object sender, BroadcastResponseMessageEventArgs eventArgs)
        {
            BroadcastResponseMessage broadcastResponseMessage = eventArgs.Message;
            this.LogReceivedText(broadcastResponseMessage, this.otherServer);

            var broadcastMessage = broadcastResponseMessage.MessageToBroadcast;

            //Send message to this server's clients.
            foreach (var client in this.clients)
            {
                this.LogSentText(broadcastMessage, client);
                this.clientDataManager.WriteData(broadcastMessage, client.TargetInformation);
            }

            //Send message to passive server's clients.
            foreach (var client in broadcastResponseMessage.ClientsInformation)
            {
                this.LogText("{0} sent {1} to {2}", this.ToString(), broadcastMessage.ToString(), client.ToString());
                this.clientDataManager.WriteData(broadcastMessage, client);
            }
        }

        /// <summary>
        /// Server to server communication. The active server needs to answer a highscore request message and asks the passive server for score information.
        /// </summary>
        /// <param name="sender">The active server.</param>
        /// <param name="eventArgs">Contains the message request with the client who wants to be informed about the high scores. This parameter will be sent back to the active server so it knows where to send the scores to.</param>
        private void ServerScoreRequestReceived(object sender, ServerScoreRequestMessageEventArgs eventArgs)
        {
            var serverScoreRequestMessage = eventArgs.Message;
            this.LogReceivedText(serverScoreRequestMessage, this.otherServer);

            ServerScoreResponseMessage serverScoreResponseMessage = new ServerScoreResponseMessage()
            {
                Scores = this.GetScores(),
                RequestSender = serverScoreRequestMessage.RequestSender
            };

            this.LogSentText(serverScoreResponseMessage, this.otherServer);
            this.serverDataManager.WriteData(serverScoreResponseMessage, sender);
        }

        /// <summary>
        /// Server to server communication. The active server received the score information and sends it to the client who requested the scores.
        /// </summary>
        /// <param name="sender">The passive server.</param>
        /// <param name="eventArgs">Contains the message response with the scores and the client who wants to be informed about the high scores.</param>
        private void ServerScoreRepsonseReceived(object sender, ServerScoreResponseMessageEventArgs eventArgs)
        {
            ServerScoreResponseMessage serverScoreResponseMessage = eventArgs.Message;
            this.LogReceivedText(serverScoreResponseMessage, this.otherServer);

            var scores = this.GetScores();

            foreach (var item in serverScoreResponseMessage.Scores)
            {
                scores.Add(item);
            }

            ScoresResponseMessage scoreResponseMessage = new ScoresResponseMessage()
            {
                Scores = scores.OrderByDescending(p => p.Score).ToList()
            };

            this.LogSentText(scoreResponseMessage, this.otherServer);
            this.clientDataManager.WriteData(scoreResponseMessage, serverScoreResponseMessage.RequestSender);
        }

        /// <summary>
        /// Server to server communication. The passive server doesn't send any messages directly to clients. This method receives a message from the passive server and sends it to the client.
        /// </summary>
        /// <param name="sender">The passive server.</param>
        /// <param name="eventArgs">Contains the forwarding message with the given target.</param>
        private void ServerForwardingReceived(object sender, ForwardingMessageEventArgs eventArgs)
        {
            ForwardingMessage forwardingMessage = eventArgs.Message;
            this.LogReceivedText(forwardingMessage, this.otherServer);
            this.LogSentText(forwardingMessage.InnerMessage, this.otherServer);
            this.clientDataManager.WriteData(forwardingMessage.InnerMessage, forwardingMessage.Target);
        }

        /// <summary>
        /// Server to server communication. When a server wants to connect to another server. Sending a connection request message.
        /// </summary>
        /// <param name="server">The server target information where it connects to.</param>
        public void ServerRegister(object server)
        {
            this.serverDataManager.Register(server);

            ConnectionRequestServerMessage connectionRequestMessage = new ConnectionRequestServerMessage()
            {
                SenderStartTime = this.startTime,
                SenderName = this.Configuration.ServerName
            };

            this.LogText("{0} sent {1} to {2}", this.ToString(), connectionRequestMessage.ToString(), server.ToString());
            this.serverDataManager.WriteData(connectionRequestMessage, server);
        }

        /// <summary>
        /// Server to server communication. When a server sends a connection request to another server. Sends a connection accept message back and decides who is the active or passive server.
        /// </summary>
        /// <param name="sender">The other server who wants to connect.</param>
        /// <param name="eventArgs">Contains the request message.</param>
        private void ServerConnectionRequestReceived(object sender, ConnectionRequestServerMessageEventArgs eventArgs)
        {
            ConnectionRequestServerMessage request = eventArgs.Message;
            LogReceivedText(request, this.otherServer);

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
            this.LogSentText(response, this.otherServer);
            this.serverDataManager.WriteData(response, sender);
        }

        /// <summary>
        /// Server to server communication. Gets informed that the connection was accepted and adds the server as passive or active server.
        /// </summary>
        /// <param name="sender">The sender server.</param>
        /// <param name="eventArgs">Contains the connection accepted message with parameters to decide who's active or passive now.</param>
        private void ServerConnectionAcceptReceived(object sender, ConnectionAcceptedServerMessageEventArgs eventArgs)
        {
            ConnectionAcceptServerMessage serverReponseMessage = eventArgs.Message;
            LogReceivedText(serverReponseMessage, this.otherServer);

            this.isActive = serverReponseMessage.IsTargetActive;

            if (this.otherServer == null)
            {
                this.otherServer = new Server(serverReponseMessage.SenderName, sender);
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
            DisconnectServerMessage disconnectMessage = new DisconnectServerMessage();

            //Delete the last connection.
            var server = this.otherServer.TargetInformation.LastOrDefault();

            if (server != null)
            {
                this.LogSentText(disconnectMessage, this.otherServer);
                this.serverDataManager.WriteData(disconnectMessage, server);
                this.serverDataManager.Unregister(server);
                this.otherServer.TargetInformation.Remove(this.otherServer.TargetInformation.Last(p => p.Equals(server)));
                this.OnServerConnectionCountChanged?.Invoke(this, new ServerConnectionCountChangedEventArgs(this.ServerConnectionCount));

                if (this.ServerConnectionCount == 0)
                {
                    this.LogText("{0} disconnected from {1}", this.ToString(), this.otherServer.ToString());
                    this.isActive = true;
                    this.otherServer = null;
                }
                else
                {
                    this.LogText("{0} closed a connection to {1}", this.ToString(), this.otherServer.ToString());
                }
            }
        }

        /// <summary>
        /// Server to server communication. Happens when one server disconnects from the other. Removes a server connection and calls event that the server connection count has changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">Contains the disconnect message, which doesn't contain any parameters.</param>
        private void ServerDisconnectReceived(object sender, DisconnectServerMessageEventArgs eventArgs)
        {
            this.LogReceivedText(eventArgs.Message, this.otherServer);
            this.serverDataManager.Unregister(sender);
            this.otherServer.TargetInformation.Remove(this.otherServer.TargetInformation.Last(p => p.Equals(sender)));
            this.OnServerConnectionCountChanged?.Invoke(this, new ServerConnectionCountChangedEventArgs(this.ServerConnectionCount));

            //If there are no more connections, both server become active again.
            if (this.ServerConnectionCount == 0)
            {
                this.LogText("{0} disconnected from {1}", this.otherServer.ToString(), this.ToString());
                this.isActive = true;
                this.otherServer = null;
            }
            else
            {
                this.LogText("{0} closed a connection to {1}", this.otherServer.ToString(), this.ToString());
            }
        }

        /// <summary>
        /// Clients to server connection. When the client wants to establish a connection with the server, the server validates if the client is allowed to connect.
        /// </summary>
        /// <param name="sender">The client which want to connect.</param>
        /// <param name="eventArgs">Contains the connection request message with the name of the client.</param>
        private void ClientConnectionRequest(object sender, ConnectionRequestClientMessageEventArgs eventArgs)
        {
            ConnectionRequestClientMessage message = eventArgs.Message;
            var senderName = message.SenderName;
            this.LogText("{0} received {1} from {2}", this.ToString(), message.ToString(), senderName);

            var client = this.GetClientFromSenderInformation(sender);

            //If this client is already connected or a player with the same name is playing, deny the connection.
            if (client != null || this.clients.Any(p => p.Name == senderName))
            {
                ConnectionDeniedMessage deniedMessage = new ConnectionDeniedMessage();
                this.SendClientMessage(deniedMessage, sender);
                LogText("{0} sent {1} to {2}", this.ToString(), deniedMessage.ToString(), senderName);
            }
            else
            {
                //Create new client.
                client = new Client(senderName, this.Configuration.MinScore, this.Configuration.MaxScore, sender);
                client.MinScoreReached += ClientLost;
                client.MaxScoreReached += ClientWon;
                this.clients.Add(client);
                this.LogText("{0} connected to {1}", client.ToString(), this.ToString());

                //Send accept message.
                ConnectionAcceptMessage acceptedMessage = new ConnectionAcceptMessage();
                this.SendClientMessage(acceptedMessage, sender);

                this.CreateQuestion(client);
            }
        }

        /// <summary>
        /// Client to server communication. A client wants to get the scores. The server needs to ask the possible other server for his scores.
        /// </summary>
        /// <param name="sender">The client who asks for the scores.</param>
        /// <param name="eventArgs">Contains the scores request message, which doesn't contain any parameters.</param>
        private void ClientSendScores(object sender, MessageEventArgs eventArgs)
        {
            ScoresRequestMessage requestMessage = eventArgs.MessageContent as ScoresRequestMessage;
            this.LogReceivedClient(requestMessage, sender);
            List<ScoreEntry> scores = GetScores();

            //If there is no other server, send the scores.
            if (this.otherServer == null)
            {
                ScoresResponseMessage responseMessage = new ScoresResponseMessage()
                {
                    Scores = scores
                };

                this.SendClientMessage(responseMessage, sender);
            }
            else
            {
                //If there is another server, and this server is active, ask for the passive server scores.
                if (this.isActive)
                {
                    ServerScoreRequestMessage serverScoreRequestMessage = new ServerScoreRequestMessage() { RequestSender = sender };
                    this.LogSentText(serverScoreRequestMessage, this.otherServer);
                    this.serverDataManager.WriteData(serverScoreRequestMessage, this.otherServer.TargetInformation.First());
                }
                //If there is another server, and this server is passive, send the passive server the scores with information which client requested the scores.
                else
                {
                    ServerScoreResponseMessage serverScoreResponseMessage = new ServerScoreResponseMessage()
                    {
                        RequestSender = sender,
                        Scores = scores
                    };

                    this.LogSentText(serverScoreResponseMessage, this.otherServer);
                    this.serverDataManager.WriteData(serverScoreResponseMessage, this.otherServer.TargetInformation.First());
                }
            }
        }

        /// <summary>
        /// Client to server communication. Validates the answer a client submitted and sends the new question containing the current points.
        /// </summary>
        /// <param name="sender">The client, who submitted the answer.</param>
        /// <param name="eventArgs">Contains the answer message.</param>
        private void ClientSubmitAnswer(object sender, AnswerMessageEventArgs eventArgs)
        {
            AnswerMessage answerMessage = eventArgs.Message;
            this.LogReceivedClient(answerMessage, sender);
            var client = this.clients.FirstOrDefault(p => p.TargetInformation.Equals(sender));

            //Is the client valid?
            if (client != null &&
                client.Score < this.Configuration.MaxScore &&
                client.Score > this.Configuration.MinScore)
            {
                string logMessage = string.Format("Answer ({0}) from {1} is ", answerMessage.Solution, client.Name);

                //Dispose the timer for the question.
                if (client.QuestionTimer != null)
                {
                    client.QuestionTimer.Dispose();
                }

                //If correct:
                if (answerMessage.Solution == client.Question.Answer)
                {
                    logMessage += "right. New score: " + (client.Score + 1);
                    this.LogText(logMessage);
                    client.Score++;
                }
                //If incorrect:
                else
                {
                    logMessage += "wrong. New score: " + (client.Score - 1);
                    this.LogText(logMessage);
                    client.Score--;
                }

                this.CreateQuestion(client);
            }
        }

        /// <summary>
        /// Disconnects the client.
        /// </summary>
        /// <param name="sender">The client who wants to disconnect.</param>
        /// <param name="eventArgs">Contains the client disconnect message.</param>
        private void ClientDisconnect(object sender, MessageEventArgs eventArgs)
        {
            this.LogReceivedClient(eventArgs.MessageContent, sender);
            Client client = this.GetClientFromSenderInformation(sender);

            //Is this client even connected?
            if (this.clients.Contains(client))
            {
                //Stop question timer
                if (client.QuestionTimer != null)
                {
                    client.QuestionTimer.Dispose();
                }
                
                LogText(string.Format("{0} disconnected from {1}", client.Name, this.Configuration.ServerName));
                this.clients.Remove(client);
            }
        }

        /// <summary>
        /// Happens when the client won the game by reaching the max points.
        /// </summary>
        /// <param name="sender">The client (client class, not the target information) who won.</param>
        /// <param name="eventArgs">Contains no important information.</param>
        private void ClientWon(object sender, EventArgs eventArgs)
        {
            var client = sender as Client;
            string message = string.Format("{0} won", client.ToString());
            this.LogText(message);

            //Send won message.
            GameWonMessage wonMessage = new GameWonMessage()
            {
                Score = client.Score
            };

            this.SendClientMessage(wonMessage, client.TargetInformation);

            //Broadcast a win to all other clients.
            this.SendBroadcastText(message);
        }

        /// <summary>
        /// Happens when the client lost the game by reaching the min points.
        /// </summary>
        /// <param name="sender">The client (client class, not the target information) who lost.</param>
        /// <param name="eventArgs">Contains no important information.</param>
        private void ClientLost(object sender, EventArgs eventArgs)
        {
            var client = sender as Client;
            string message = string.Format("{0} lost", client.ToString());
            this.LogText(message);

            //Send lost message.
            GameLostMessage lostMessage = new GameLostMessage()
            {
                Score = client.Score
            };

            this.SendClientMessage(lostMessage, client.TargetInformation);

            //Broadcast a loss to all other clients.
            this.SendBroadcastText(message);
        }

        /// <summary>
        /// Monitor to server communication. When the monitor sends a connection request.
        /// </summary>
        /// <param name="sender">The monitor which wants to connect.</param>
        /// <param name="eventArgs">Contains no useful data.</param>
        private void MonitorConnectionRequest(object sender, MessageEventArgs eventArgs)
        {
            LogText(string.Format("Connection request from Monitor ({0}) to {1}", sender, this.Configuration.ServerName));
            this.LogReceivedMonitor(eventArgs.MessageContent, sender);
            var monitor = this.monitors.FirstOrDefault(p => p.TargetInformation.Equals(sender));

            //Allow connection depending on if the monitor is already connected or not.
            if (monitor != null)
            {
                ConnectionDeniedMessage deniedMessage = new ConnectionDeniedMessage();
                this.LogSentText(deniedMessage, monitor);
                monitorDataManager.WriteData(deniedMessage, sender);
            }
            else
            {
                monitor = new Monitor(sender);
                this.monitors.Add(monitor);
                ConnectionAcceptMessage acceptMessage = new ConnectionAcceptMessage();
                this.LogText("{0} connected to {1}", monitor.ToString(), this.ToString());
                this.LogSentText(acceptMessage, monitor);
                monitorDataManager.WriteData(acceptMessage, sender);
            }
        }

        /// <summary>
        /// Monitor to server communication. When the monitor wants to disconnect.
        /// </summary>
        /// <param name="sender">The monitor which wants to disconnect.</param>
        /// <param name="eventArgs">Contains no useful data.</param>
        private void MonitorDisconnect(object sender, MessageEventArgs eventArgs)
        {
            Monitor monitor = this.monitors.FirstOrDefault(p => p.TargetInformation.Equals(sender));
            this.LogReceivedMonitor(eventArgs.MessageContent, monitor);

            //Is this monitor even connected?
            if (monitor != null)
            {
                LogText("{0} disconnected from {1}", monitor.ToString(), this.ToString());
                this.monitors.Remove(monitor);
            }
        }

        /// <summary>
        /// Sends the broadcast text. Needs to transmit information if there is another server connected to receive all connected clients with the system.
        /// </summary>
        /// <param name="text">The text, which needs to be broadcasted.</param>
        private void SendBroadcastText(string text)
        {
            BroadcastMessage broadcastMessage = new BroadcastMessage() { Text = text };

            //If there is no other server connected send the message to the clients.
            if (this.otherServer == null)
            {
                foreach (var client in this.clients)
                {
                    SendClientMessage(broadcastMessage, client.TargetInformation);
                }
            }
            else
            {
                //If there is another server connected extra communication is necessary.
                if (this.isActive)
                {
                    BroadcastRequestMessage broadcastRequestMessage = new BroadcastRequestMessage()
                    {
                        MessageToBroadcast = broadcastMessage
                    };

                    this.LogSentText(broadcastRequestMessage, this.otherServer);
                    this.serverDataManager.WriteData(broadcastRequestMessage, this.otherServer.TargetInformation.First());
                }
                else
                {
                    BroadcastResponseMessage broadcastResponseMessage = new BroadcastResponseMessage()
                    {
                        ClientsInformation = this.clients.Select(p => p.TargetInformation).ToList(),
                        MessageToBroadcast = broadcastMessage
                    };

                    this.LogSentText(broadcastResponseMessage, this.otherServer);
                    this.serverDataManager.WriteData(broadcastResponseMessage, this.otherServer.TargetInformation.First());
                }
            }
        }

        /// <summary>
        /// Logs the text.
        /// </summary>
        /// <param name="loggingText">The logging text.</param>
        private void LogText(string loggingText, params string[] arguments)
        {
            LoggingMessage loggingMessage = new LoggingMessage()
            {
                Text = string.Format(loggingText, arguments)
            };

            monitors.ForEach(p => monitorDataManager.WriteData(loggingMessage, p.TargetInformation));
        }

        private void LogReceivedText(Message message, SystemElement from)
        {
            this.LogText("{0} received {1} from {2}", this.ToString(), message.ToString(), from.ToString());
        }

        private void LogSentText(Message message, SystemElement to)
        {
            this.LogText("{0} sent {1} to {2}", this.ToString(), message.ToString(), to.ToString());
        }

        private void LogReceivedClient(Message message, object clientTarget)
        {
            var client = GetClientFromSenderInformation(clientTarget);

            if (client != null)
                this.LogReceivedText(message, client);
            else
                this.LogText("{0} received {1} from {2}", this.ToString(), message.ToString(), clientTarget.ToString());
        }

        private void LogReceivedMonitor(Message message, object monitorTarget)
        {
            var monitor = this.monitors.FirstOrDefault(p => p.TargetInformation.Equals(monitorTarget));

            if (monitor != null)
                this.LogReceivedText(message, monitor);
            else
                this.LogText("{0} received {1} from {2}", this.ToString(), message.ToString(), monitorTarget.ToString());
        }

        /// <summary>
        /// Creates the question for a client.
        /// </summary>
        /// <param name="client">The client.</param>
        private void CreateQuestion(Client client)
        {
            if (client.Score < this.Configuration.MaxScore &&
                client.Score > this.Configuration.MinScore)
            {
                //Get a random question.
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

                this.SendClientMessage(questionMessage, client.TargetInformation);

                //Start question timer.
                client.Question = question;
                client.QuestionTimer = new Timer(this.QuestionTimeExpired, client, question.Time * 1000, Timeout.Infinite);
            }
        }

        /// <summary>
        /// When the question time expires, the question is marked as wrong.
        /// </summary>
        /// <param name="clientState">The client who timed out the question answering time.</param>
        private void QuestionTimeExpired(object clientState)
        {
            Client client = (Client)clientState;
            this.LogText(string.Format("{0} expired the question answer time. Score: {1}", client.Name, client.Score - 1));
            client.Score--;
            this.CreateQuestion(client);
        }

        /// <summary>
        /// Gets the client from sender information.
        /// </summary>
        /// <param name="senderInformation">The sender information.</param>
        /// <returns>The client to the sender information. Null if there was no match.</returns>
        private Client GetClientFromSenderInformation(object senderInformation)
        {
            return this.clients.FirstOrDefault(p => p.TargetInformation.Equals(senderInformation));
        }

        /// <summary>
        /// Sends the client message. If there is an active server connected, we need to forward the message to it.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="target">The target.</param>
        private void SendClientMessage(Message message, object target)
        {
            if (this.isActive)
            {
                var client = GetClientFromSenderInformation(target);

                if (client != null)
                    this.LogSentText(message, client);
                else
                    this.LogText("{0} sent {1} to {2}", this.ToString(), message.ToString(), target.ToString());

                this.clientDataManager.WriteData(message, target);
            }
            else
            {
                //Send forwarding message to active server.
                this.LogSentText(message, this.otherServer);
                ForwardingMessage forwardingMessage = new ForwardingMessage() { InnerMessage = message, Target = target, TargetName = this.GetClientFromSenderInformation(target).Name };
                this.serverDataManager.WriteData(forwardingMessage, this.otherServer.TargetInformation.First());
            }
        }

        /// <summary>
        /// Gets the scores.
        /// </summary>
        /// <returns>A list of score entries.</returns>
        private List<ScoreEntry> GetScores()
        {
            return this.clients.Select(p => new ScoreEntry(p.Name, p.Score)).OrderByDescending(p => p.Score).ToList();
        }

        public override string ToString()
        {
            return this.Configuration.ServerName;
        }
    }
}