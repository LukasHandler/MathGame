using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Data;
using System.Net;
using Shared.Data.Messages;
using Shared.Data.Managers;

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

        private object locker;

        public NetworkService()
        {
            this.locker = new object();

            this.messageProcessor = new MessageProcessor();
            this.messageProcessor.OnConnectionRequestClient += ConnectionRequestedClient;
            this.messageProcessor.OnDisconnect += ClientDisconnect;

            this.messageProcessor.OnConnectionRequestMonitor += ConnectionRequestedMonitor;

            clientManager = new UdpServerManager(localEndPointClients);
            clientManager.OnDataReceived += messageProcessor.DataReceived;

            this.clients = new Dictionary<string, IPEndPoint>();

            this.monitorManager = new TcpServerManager(localEndPointMonitors);
            this.monitorManager.OnDataReceived += messageProcessor.DataReceived;

            this.monitors = new List<IPEndPoint>();
        }

        private void ConnectionRequestedMonitor(object sender, MessageEventArgs e)
        {
            ConnectionRequestMonitor request = (ConnectionRequestMonitor)e.MessageContent;

            lock(locker)
            {
                if (this.monitors.Any(p => p.Address.ToString() == request.SenderEndPoint.Address.ToString() 
                && p.Port == request.SenderEndPoint.Port))
                {
                    ConnectionDenied deniedMessage = new ConnectionDenied();
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

        private void ClientDisconnect(object sender, MessageEventArgs e)
        {
            Disconnect disconnectMessage = (Disconnect)e.MessageContent;

            if (this.clients.ContainsValue(disconnectMessage.SenderEndPoint))
            {
                var playerName = this.clients.Where(p => p.Value.Address.ToString() == disconnectMessage.SenderEndPoint.Address.ToString()
                                                    && p.Value.Port == disconnectMessage.SenderEndPoint.Port).First().Key;
                this.clients.Remove(playerName);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private void ConnectionRequestedClient(object sender, MessageEventArgs e)
        {
            ConnectionRequestClient request = (ConnectionRequestClient)e.MessageContent;

            lock (locker)
            {
                if (this.clients.ContainsKey(request.PlayerName) || this.clients.ContainsValue(request.SenderEndPoint))
                {
                    ConnectionDenied deniedMessage = new ConnectionDenied();
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
    }
}
