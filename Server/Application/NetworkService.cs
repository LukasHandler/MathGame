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

        private Dictionary<string, IPEndPoint> clients;

        private IDataManager monitorManager;

        private List<IPEndPoint> monitors;

        private MessageProcessor messageProcessor;

        private IPEndPoint localEndPointClients = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4713);

        private IPEndPoint localEndPointMonitors = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4699);

        private string serverName = "Server1";

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

            this.clients = new Dictionary<string, IPEndPoint>();

            this.monitorManager = new TcpServerManager(localEndPointMonitors);
            this.monitorManager.OnDataReceived += messageProcessor.DataReceived;

            this.monitors = new List<IPEndPoint>();
        }

        private void SubmitAnswer(object sender, MessageEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ConnectionRequestedMonitor(object sender, MessageEventArgs e)
        {
            ConnectionRequestMonitorMessage request = (ConnectionRequestMonitorMessage)e.MessageContent;

            lock(locker)
            {
                if (this.monitors.Any(p => IPEndPoint.Equals(p, request.SenderEndPoint)))
                {
                    ConnectionDeniedMessage deniedMessage = new ConnectionDeniedMessage();
                    monitorManager.WriteData(deniedMessage, request.SenderEndPoint);
                }
                else
                {
                    this.monitors.Add(request.SenderEndPoint);
                    ConnectionAcceptMessage acceptMessage = new ConnectionAcceptMessage();
                    monitorManager.WriteData(acceptMessage, request.SenderEndPoint);
                }
            }
        }

        private void Disconnect(object sender, MessageEventArgs e)
        {
            DisconnectMessage disconnectMessage = (DisconnectMessage)e.MessageContent;

            if (this.clients.Any(p => IPEndPoint.Equals(p.Value, disconnectMessage.SenderEndPoint)))
            {
                var playerName = this.clients.First(p => IPEndPoint.Equals(p.Value, disconnectMessage.SenderEndPoint)).Key;
                this.clients.Remove(playerName);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private void ConnectionRequestedClient(object sender, MessageEventArgs e)
        {
            //LogText()

            ConnectionRequestClientMessage request = (ConnectionRequestClientMessage)e.MessageContent;

            lock (locker)
            {
                if (this.clients.ContainsKey(request.PlayerName) || this.clients.ContainsValue(request.SenderEndPoint))
                {
                    ConnectionDeniedMessage deniedMessage = new ConnectionDeniedMessage();
                    clientManager.WriteData(deniedMessage, request.SenderEndPoint);
                }
                else
                {
                    this.clients.Add(request.PlayerName, request.SenderEndPoint);
                    ConnectionAcceptMessage acceptedMessage = new ConnectionAcceptMessage();
                    clientManager.WriteData(acceptedMessage, request.SenderEndPoint);

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
