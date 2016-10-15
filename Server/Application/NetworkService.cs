using Server.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Data;
using System.Net;
using Shared.Data.Messages;

namespace Server.Application
{
    public class NetworkService
    {
        private NetworkManager clientManager;
        private Dictionary<string, IPEndPoint> clients;

        private MessageProcessor messageProcessor;

        private int port = 4713;

        private object locker;

        public NetworkService()
        {
            this.locker = new object();

            this.messageProcessor = new MessageProcessor();
            this.messageProcessor.OnConnectionRequest += ConnectionRequested;

            this.clients = new Dictionary<string, IPEndPoint>();

            clientManager = new NetworkManager(port);
            clientManager.OnDataReceived += DataReceived;
        }

        private void ConnectionRequested(object sender, MessageEventArgs e)
        {
            ConnectionRequest request = (ConnectionRequest)e.MessageContent;

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
                    ConnectionAccepted acceptedMessage = new ConnectionAccepted();
                    clientManager.WriteData(acceptedMessage, request.SenderEndPoint);

                    //Send Questions!
                }
            }

        }

        private void DataReceived(object sender, MessageEventArgs eventArgs)
        {
            eventArgs.MessageContent.ProcessMessage(messageProcessor);
        }
    }
}
