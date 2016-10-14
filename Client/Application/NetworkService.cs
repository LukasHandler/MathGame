using Client.Data;
using Shared.Data.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Shared.Data;

namespace Client.Application
{
    public static class NetworkService
    {
        private static NetworkManager networkManager;

        private static IPAddress senderIp = IPAddress.Parse("127.0.0.1");
        private static int senderPort = 4712;

        private static IPAddress hostIp;
        private static int hostPort;

        static NetworkService()
        {
            networkManager = new NetworkManager();
            networkManager.OnDataReceived += DataReceived;
        }

        private static void DataReceived(object sender, MessageEventArgs e)
        {
            
        }

        public static void ConnectToServer(IPAddress ip, int port)
        {
            hostIp = ip;
            hostPort = port;

            ConnectionRequest request = new ConnectionRequest()
            {
                SenderIp = senderIp,
                SenderPort = senderPort
            };

            Send(request);
        }

        public static void SubmitAnswer(int answer)
        {
            Answer answerMessage = new Answer()
            {
                SenderIp = senderIp,
                SenderPort = senderPort,
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
            IPEndPoint target = new IPEndPoint(hostIp, hostPort);
            networkManager.WriteData(request, target, new object[] { senderPort });
        }
    }
}
