using Shared.Data;
using Shared.Data.EventArguments;
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
    public static class DataService
    {
        private static MessageProcessor messageProcessor;

        private static IDataManager clientManager;

        private static IPEndPoint serverEndPoint;

        public static EventHandler<LoggingEventArgs> OnLoggingDataReceived;

        private static bool isConnected = false;


        static DataService()
        {
            messageProcessor = new MessageProcessor();
            messageProcessor.OnConnectionAccepted += ConnectionAccepted;
            messageProcessor.OnLoggingMessage += ReceivedLoggingMessage;

            clientManager = new TcpClientManager();
            clientManager.OnDataReceived += messageProcessor.DataReceived;
        }

        private static void ReceivedLoggingMessage(object sender, LoggingMessageEventArgs e)
        {
            if (OnLoggingDataReceived != null)
            {
                string loggingMessage = e.Message.Text;
                OnLoggingDataReceived(sender, new LoggingEventArgs(loggingMessage));
            }
        }

        private static void ConnectionAccepted(object sender, EventArgs e)
        {
            isConnected = true;
        }

        public static void Register(IPEndPoint serverPoint)
        {
            serverEndPoint = serverPoint;

            clientManager.Register(serverEndPoint);
            ConnectionRequestMonitorMessage requestMessage = new ConnectionRequestMonitorMessage();

            clientManager.WriteData(requestMessage, serverEndPoint);
        }

        public static void Unregister()
        {
            if (isConnected)
            {
                DisconnectMessage disconnectMessage = new DisconnectMessage();

                clientManager.WriteData(disconnectMessage, serverEndPoint);
                clientManager.Unregister(serverEndPoint);
            }
        }
    }
}
