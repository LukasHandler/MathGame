using Shared.Data.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Shared.Data;
using Shared.Data.Managers;

namespace Client.Application
{
    public class NetworkService
    {
        public EventHandler OnConnectionAccepted;

        public EventHandler OnConnectionDenied;

        private UdpClientManager networkManager;

        private MessageProcessor messageProcessor;

        private IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4712);

        private IPEndPoint serverEndPoint;

        private Guid clientGuid = Guid.NewGuid();

        public NetworkService()
        {
            messageProcessor = new MessageProcessor();

            networkManager = new UdpClientManager();
            networkManager.OnDataReceived += messageProcessor.DataReceived;

            messageProcessor.OnConnectionAccepted += delegate (object sender, MessageEventArgs args)
            {
                if (OnConnectionAccepted != null)
                {
                    OnConnectionAccepted(sender, EventArgs.Empty);
                }
            };

            messageProcessor.OnConnectionDenied += delegate (object sender, MessageEventArgs args)
            {
                if (OnConnectionAccepted != null)
                {
                    OnConnectionDenied(sender, EventArgs.Empty);
                }
            };
        }

        public void Connect(IPEndPoint server, string playerName)
        {
            serverEndPoint = server;

            ConnectionRequestClientMessage request = new ConnectionRequestClientMessage()
            {
                SenderId = clientGuid,
                PlayerName = playerName
            };

            Send(request);
        }

        public void SubmitAnswer(int answer)
        {
            AnswerMessage answerMessage = new AnswerMessage()
            {
                SenderId = clientGuid,
                Solution = answer
            };

            Send(answerMessage);
        }

        public List<Tuple<string, int>> GetScores()
        {
            return null;
        }

        private void Send(Message request)
        {
            networkManager.WriteData(request, serverEndPoint);
        }

        public void Disconnect()
        {
            DisconnectMessage disconnectMessage = new DisconnectMessage()
            {
                SenderId = clientGuid
            };

            Send(disconnectMessage);
        }
    }
}
