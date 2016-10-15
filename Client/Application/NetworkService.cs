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
    public static class NetworkService
    {
        public static EventHandler OnConnectionAccepted;

        public static EventHandler OnConnectionDenied;

        private static UdpClientManager networkManager;

        private static MessageProcessor messageProcessor;

        private static IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4712);

        private static IPEndPoint serverEndPoint;

        static NetworkService()
        {
            messageProcessor = new MessageProcessor();

            networkManager = new UdpClientManager(localEndPoint);
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

        public static void Connect(IPEndPoint server, string playerName)
        {
            serverEndPoint = server;

            ConnectionRequestClient request = new ConnectionRequestClient()
            {
                SenderEndPoint = localEndPoint,
                PlayerName = playerName
            };

            Send(request);
        }

        public static void SubmitAnswer(int answer)
        {
            Answer answerMessage = new Answer()
            {
                SenderEndPoint = serverEndPoint,
                Solution = answer
            };

            Send(answerMessage);
        }

        public static List<Tuple<string, int>> GetScores()
        {
            return null;
        }

        private static void Send(Message request)
        {
            networkManager.WriteData(request, serverEndPoint);
        }

        public static void Disconnect()
        {
            Disconnect disconnectMessage = new Disconnect()
            {
                SenderEndPoint = localEndPoint
            };

            Send(disconnectMessage);
        }
    }
}
