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

        public delegate void ParameterlessDelegate();

        public EventHandler<ConnectionRequestClientMessageEventArgs> OnConnectionRequestClient;

        public EventHandler<ConnectionRequestServerMessageEventArgs> OnConnectionRequestServer;

        public EventHandler<MessageEventArgs> OnConnectionRequestMonitor;

        public EventHandler<ConnectionAcceptedServerMessageEventArgs> OnConnectionAcceptedServer;

        public EventHandler<MessageEventArgs> OnScoreRequest;

        public EventHandler<ScoresResponseMessageEventArgs> OnScoreResponse;

        public EventHandler<AnswerMessageEventArgs> OnAnswer;

        public EventHandler<MessageEventArgs> OnConnectionDenied;

        public EventHandler<MessageEventArgs> OnConnectionAccepted;

        public EventHandler<QuestionMessageEventArgs> OnQuestion;

        public EventHandler<MessageEventArgs> OnDisconnect;

        public EventHandler<DisconnectServerMessageEventArgs> OnDisconnectServer;

        public EventHandler<LoggingMessageEventArgs> OnLoggingMessage;

        public EventHandler<GameWonMessageEventArgs> OnGameWonMessage;

        public EventHandler<GameLostMessageEventArgs> OnGameLostMessage;

        public EventHandler<ForwardingMessageEventArgs> OnForwardingMessage;

        public EventHandler<ServerScoreRequestMessageEventArgs> OnServerScoreRequestMessage;

        public EventHandler<ServerScoreResponseMessageEventArgs> OnServerScoreResponseMessage;

        public EventHandler<BroadcastMessageEventArgs> OnBroadcastMessage;

        public EventHandler<BroadcastRequestMessageEventArgs> OnServerClientsRequestMessage;

        public EventHandler<BroadcastResponseMessageEventArgs> OnServerClientsResponseMessage;

        public void ProcessMessage(BroadcastMessage message)
        {
            this.OnBroadcastMessage?.Invoke(this.senderInformation[message], new BroadcastMessageEventArgs(message));
        }

        public void ProcessMessage(BroadcastRequestMessage message)
        {
            this.OnServerClientsRequestMessage?.Invoke(this.senderInformation[message], new BroadcastRequestMessageEventArgs(message));
        }

        public void ProcessMessage(BroadcastResponseMessage message)
        {
            this.OnServerClientsResponseMessage?.Invoke(this.senderInformation[message], new BroadcastResponseMessageEventArgs(message));
        }

        public void ProcessMessage(ConnectionAcceptServerMessage message)
        {
            this.OnConnectionAcceptedServer?.Invoke(this.senderInformation[message], new ConnectionAcceptedServerMessageEventArgs(message));
        }

        public void ProcessMessage(ScoresRequestMessage message)
        {
            this.OnScoreRequest?.Invoke(this.senderInformation[message], new MessageEventArgs(message));
        }

        public void ProcessMessage(ConnectionRequestClientMessage message)
        {
            this.OnConnectionRequestClient?.Invoke(this.senderInformation[message], new ConnectionRequestClientMessageEventArgs(message));
        }

        public void ProcessMessage(ConnectionRequestServerMessage message)
        {
            this.OnConnectionRequestServer?.Invoke(this.senderInformation[message], new ConnectionRequestServerMessageEventArgs(message));
        }

        public void ProcessMessage(ConnectionRequestMonitorMessage message)
        {
            this.OnConnectionRequestMonitor?.Invoke(this.senderInformation[message], new MessageEventArgs(message));
        }

        public void ProcessMessage(AnswerMessage message)
        {
            this.OnAnswer?.Invoke(this.senderInformation[message], new AnswerMessageEventArgs(message));
        }

        public void ProcessMessage(ConnectionDeniedMessage message)
        {
            this.OnConnectionDenied?.Invoke(this.senderInformation[message], new MessageEventArgs(message));
        }

        public void ProcessMessage(QuestionMessage message)
        {
            this.OnQuestion?.Invoke(this.senderInformation[message], new QuestionMessageEventArgs(message));
        }

        public void ProcessMessage(ScoresResponseMessage message)
        {
            this.OnScoreResponse?.Invoke(this.senderInformation[message], new ScoresResponseMessageEventArgs(message));
        }

        public void ProcessMessage(ConnectionAcceptMessage message)
        {
            this.OnConnectionAccepted?.Invoke(this.senderInformation[message], new MessageEventArgs(message));
        }

        public void ProcessMessage(DisconnectMessage message)
        {
            this.OnDisconnect?.Invoke(this.senderInformation[message], new MessageEventArgs(message));
        }

        public void ProcessMessage(DisconnectServerMessage message)
        {
            this.OnDisconnectServer?.Invoke(this.senderInformation[message], new DisconnectServerMessageEventArgs(message));
        }

        public void ProcessMessage(LoggingMessage message)
        {
            this.OnLoggingMessage?.Invoke(this.senderInformation[message], new LoggingMessageEventArgs(message));
        }

        public void ProcessMessage(GameWonMessage message)
        {
            this.OnGameWonMessage?.Invoke(this.senderInformation[message], new GameWonMessageEventArgs(message));
        }

        public void ProcessMessage(GameLostMessage message)
        {
            this.OnGameLostMessage?.Invoke(this.senderInformation[message], new GameLostMessageEventArgs(message));
        }

        public void ProcessMessage(ForwardingMessage message)
        {
            this.OnForwardingMessage?.Invoke(this.senderInformation[message], new ForwardingMessageEventArgs(message));
        }

        public void ProcessMessage(ServerScoreResponseMessage message)
        {
            this.OnServerScoreResponseMessage?.Invoke(this.senderInformation[message], new ServerScoreResponseMessageEventArgs(message));
        }

        public void ProcessMessage(ServerScoreRequestMessage message)
        {
            this.OnServerScoreRequestMessage?.Invoke(this.senderInformation[message], new ServerScoreRequestMessageEventArgs(message));
        }

        public void DataReceived(object sender, MessageEventArgs eventArgs)
        {
            lock (locker)
            {
                this.senderInformation.Add(eventArgs.MessageContent, sender);
            }

            eventArgs.MessageContent.ProcessMessage(this);

            lock (locker)
            {
                this.senderInformation.Remove(eventArgs.MessageContent);
            }
        }
    }
}
