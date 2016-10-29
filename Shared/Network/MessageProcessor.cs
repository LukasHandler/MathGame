using Shared.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Data.Messages;
using Shared.Data.EventArguments;

namespace Shared.Data
{
    public class MessageProcessor : IMessageVisitor
    {
        private Dictionary<Message, object> senderInformation;

        private object locker;

        public MessageProcessor()
        {
            this.senderInformation = new Dictionary<Message, object>();
            locker = new object();
        }

        public EventHandler<MessageEventArgs> OnConnectionRequestClient;

        public EventHandler<MessageEventArgs> OnConnectionRequestServer;

        public EventHandler<MessageEventArgs> OnConnectionRequestMonitor;

        public EventHandler<MessageEventArgs> OnConnectionAcceptedServer;

        public EventHandler<MessageEventArgs> OnScoreRequest;

        public EventHandler<MessageEventArgs> OnScoreResponse;

        public EventHandler<MessageEventArgs> OnAnswer;

        public EventHandler<MessageEventArgs> OnConnectionDenied;

        public EventHandler<MessageEventArgs> OnConnectionAccepted;

        public EventHandler<MessageEventArgs> OnQuestion;

        public EventHandler<MessageEventArgs> OnDisconnect;

        public EventHandler<MessageEventArgs> OnDisconnectServer;

        public EventHandler<MessageEventArgs> OnLoggingMessage;

        public EventHandler<MessageEventArgs> OnGameWonMessage;

        public EventHandler<MessageEventArgs> OnGameLostMessage;

        public EventHandler<MessageEventArgs> OnForwardingMessage;

        public EventHandler<MessageEventArgs> OnServerScoreRequestMessage;

        public EventHandler<MessageEventArgs> OnServerScoreResponseMessage;

        public EventHandler<MessageEventArgs> OnBroadcastMessage;

        public EventHandler<MessageEventArgs> OnServerClientsRequestMessage;

        public EventHandler<MessageEventArgs> OnServerClientsResponseMessage;

        public void ProcessMessage(BroadcastMessage message)
        {
            this.RaiseEvent(this.OnBroadcastMessage, message);
        }

        public void ProcessMessage(BroadcastRequestMessage message)
        {
            this.RaiseEvent(this.OnServerClientsRequestMessage, message);
        }

        public void ProcessMessage(BroadcastResponseMessage message)
        {
            this.RaiseEvent(this.OnServerClientsResponseMessage, message);
        }

        public void ProcessMessage(ConnectionAcceptServerMessage message)
        {
            RaiseEvent(this.OnConnectionAcceptedServer, message);
        }

        public void ProcessMessage(ScoresRequestMessage message)
        {
            RaiseEvent(OnScoreRequest, message);
        }

        public void ProcessMessage(ConnectionRequestClientMessage message)
        {
            RaiseEvent(OnConnectionRequestClient, message);
        }

        public void ProcessMessage(ConnectionRequestServerMessage message)
        {
            this.RaiseEvent(OnConnectionRequestServer, message);
        }

        public void ProcessMessage(ConnectionRequestMonitorMessage message)
        {
            this.RaiseEvent(OnConnectionRequestMonitor, message);
        }

        public void ProcessMessage(AnswerMessage message)
        {
            RaiseEvent(OnAnswer, message);
        }

        public void ProcessMessage(ConnectionDeniedMessage message)
        {
            RaiseEvent(OnConnectionDenied, message);
        }

        public void ProcessMessage(QuestionMessage message)
        {
            RaiseEvent(OnQuestion, message);
        }

        public void ProcessMessage(ScoresResponseMessage message)
        {
            RaiseEvent(OnScoreResponse, message);
        }

        public void ProcessMessage(ConnectionAcceptMessage message)
        {
            RaiseEvent(OnConnectionAccepted, message);
        }

        public void ProcessMessage(DisconnectMessage message)
        {
            RaiseEvent(this.OnDisconnect, message);
        }

        public void ProcessMessage(DisconnectServerMessage message)
        {
            this.RaiseEvent(this.OnDisconnectServer, message);
        }

        public void ProcessMessage(LoggingMessage message)
        {
            this.RaiseEvent(OnLoggingMessage, message);
        }

        public void ProcessMessage(GameWonMessage message)
        {
            this.RaiseEvent(OnGameWonMessage, message);
        }

        public void ProcessMessage(GameLostMessage message)
        {
            this.RaiseEvent(OnGameLostMessage, message);
        }

        public void ProcessMessage(ForwardingMessage message)
        {
            this.RaiseEvent(OnForwardingMessage, message);
        }

        public void ProcessMessage(ServerScoreResponseMessage message)
        {
            this.RaiseEvent(this.OnServerScoreResponseMessage, message);
        }

        public void ProcessMessage(ServerScoreRequestMessage message)
        {
            this.RaiseEvent(this.OnServerScoreRequestMessage, message);
        }

        public void RaiseEvent(EventHandler<MessageEventArgs> eventHandler, Message message)
        {
            if (eventHandler != null)
            {
                eventHandler(this.senderInformation[message], new MessageEventArgs(message));
            }

            lock (locker)
            {
                this.senderInformation.Remove(message);
            }
        }

        public void DataReceived(object sender, MessageEventArgs eventArgs)
        {
            lock (locker)
            {
                this.senderInformation.Add(eventArgs.MessageContent, sender);
            }

            eventArgs.MessageContent.ProcessMessage(this);
        }
    }
}
