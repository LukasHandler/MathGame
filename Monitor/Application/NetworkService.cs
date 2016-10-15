using Shared.Data;
using Shared.Data.Managers;
using Shared.Data.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Monitor.Application
{
    public static class NetworkService
    {
        private static MessageProcessor messageProcessor;

        private static IDataManager clientManager;

        private static IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4714);

        public static EventHandler<LoggingEventArgs> OnLoggingDataReceived;

        static NetworkService()
        {
            messageProcessor = new MessageProcessor();
            messageProcessor.OnConnectionAccepted += ConnectionAccepted;
            messageProcessor.OnLoggingMessage += ReceivedLoggingMessage;

            clientManager = new TcpClientManager(localEndPoint);
            clientManager.OnDataReceived += messageProcessor.DataReceived;
        }

        private static void ReceivedLoggingMessage(object sender, MessageEventArgs e)
        {
            if (OnLoggingDataReceived != null)
            {
                string loggingMessage = ((LoggingMessage)e.MessageContent).Text;

                OnLoggingDataReceived(sender, new LoggingEventArgs(loggingMessage));
            }
        }

        private static void ConnectionAccepted(object sender, MessageEventArgs e)
        {
            
        }

        public static void Connect(IPEndPoint serverEndPoint)
        {
            ConnectionRequestMonitorMessage requestMessage = new ConnectionRequestMonitorMessage()
            {
                SenderEndPoint = localEndPoint
            };

            clientManager.WriteData(requestMessage, serverEndPoint);
        }
    }
}
