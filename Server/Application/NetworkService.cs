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

namespace Server.Application
{
    public class NetworkService
    {
        private IDataManager clientManager;

        private Dictionary<Guid, Client> clients;

        private IDataManager monitorManager;

        private List<Guid> monitors;

        private MessageProcessor messageProcessor;

        private IPEndPoint localEndPointClients = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4713);

        private IPEndPoint localEndPointMonitors = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4699);

        private string serverName = "server1";

        private object locker;

        public NetworkService()
        {
            this.locker = new object();

            this.messageProcessor = new MessageProcessor();
            this.messageProcessor.OnConnectionRequestClient += ConnectionRequestedClient;
            this.messageProcessor.OnDisconnect += Disconnect;
            this.messageProcessor.OnAnswer += SubmitAnswer;

            this.messageProcessor.OnConnectionRequestMonitor += ConnectionRequestedMonitor;

            clientManager = new UdpServerManager(localEndPointClients);
            clientManager.OnDataReceived += messageProcessor.DataReceived;

            this.clients = new Dictionary<Guid, Client>();

            this.monitorManager = new TcpServerManager(localEndPointMonitors);
            this.monitorManager.OnDataReceived += messageProcessor.DataReceived;

            this.monitors = new List<Guid>();
        }

        private void SubmitAnswer(object sender, MessageEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ConnectionRequestedMonitor(object sender, MessageEventArgs e)
        {
            ConnectionRequestMonitorMessage request = (ConnectionRequestMonitorMessage)e.MessageContent;
            LogText(string.Format("Connection request from Monitor ({0}) to \"{1}\"", request.SenderId, this.serverName));

            lock (locker)
            {
                if (this.monitors.Contains(request.SenderId))
                {
                    ConnectionDeniedMessage deniedMessage = new ConnectionDeniedMessage();
                    monitorManager.WriteData(deniedMessage, request.SenderId);
                    LogText(string.Format("Connection denied from \"{0}\" to Monitor ({1})", this.serverName, request.SenderId));
                }
                else
                {
                    this.monitors.Add(request.SenderId);
                    ConnectionAcceptMessage acceptMessage = new ConnectionAcceptMessage();
                    monitorManager.WriteData(acceptMessage, request.SenderId);
                    LogText(string.Format("Connection accepted from \"{0}\" to Monitor ({1})", this.serverName, request.SenderId));
                }
            }
        }

        private void Disconnect(object sender, MessageEventArgs e)
        {
            DisconnectMessage disconnectMessage = (DisconnectMessage)e.MessageContent;

            if (this.clients.ContainsKey(disconnectMessage.SenderId))
            {
                LogText(string.Format("\"{0}\" disconnected from \"{1}\"", this.clients[disconnectMessage.SenderId].PlayerName, this.serverName));
                this.clients.Remove(disconnectMessage.SenderId);
            }
            else if (this.monitors.Contains(disconnectMessage.SenderId))
            {
                LogText(string.Format("Monitor ({0}) disconnected from \"{1}\"", disconnectMessage.SenderId, this.serverName));
                this.monitors.Remove(disconnectMessage.SenderId);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private void ConnectionRequestedClient(object sender, MessageEventArgs e)
        {
            ConnectionRequestClientMessage request = (ConnectionRequestClientMessage)e.MessageContent;
            LogText(string.Format("Connection request from \"{0}\" to \"{1}\"", request.PlayerName, this.serverName));

            lock (locker)
            {
                if (this.clients.ContainsKey(request.SenderId) || this.clients.Any(p => p.Value.PlayerName == request.PlayerName))
                {
                    ConnectionDeniedMessage deniedMessage = new ConnectionDeniedMessage();
                    clientManager.WriteData(deniedMessage, request.SenderId);
                    LogText(string.Format("Connection denied from \"{0}\" to \"{1}\"", this.serverName, request.PlayerName));
                }
                else
                {
                    this.clients.Add(request.SenderId, new Client(request.PlayerName));
                    ConnectionAcceptMessage acceptedMessage = new ConnectionAcceptMessage();
                    clientManager.WriteData(acceptedMessage, request.SenderId);
                    LogText(string.Format("Connection accepted from \"{0}\" to \"{1}\"", this.serverName, request.PlayerName));
                    //Send Questions!
                }
            }

        }

        private void LogText(string loggingText)
        {
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

            LoggingMessage loggingMessage = new LoggingMessage()
            {
                Text = loggingText
            };

            monitors.ForEach(p => monitorManager.WriteData(loggingMessage, p));
        }
    }
}
