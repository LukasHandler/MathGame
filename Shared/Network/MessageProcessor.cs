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
        public EventHandler<MessageEventArgs> OnConnectionRequestClient;

        public EventHandler<MessageEventArgs> OnScoreRequest;

        public EventHandler<MessageEventArgs> OnAnswer;

        public EventHandler<MessageEventArgs> OnConnectionDenied;

        public EventHandler<MessageEventArgs> OnConnectionAccepted;

        public EventHandler<MessageEventArgs> OnWrongAnswer;

        public EventHandler<MessageEventArgs> OnRightAnswer;

        public EventHandler<MessageEventArgs> OnQuestion;

        public EventHandler<MessageEventArgs> OnDisconnect;

        public EventHandler<MessageEventArgs> OnConnectionRequestMonitor;

        public EventHandler<MessageEventArgs> OnConnectionRequestServer;

        public EventHandler<MessageEventArgs> OnLoggingMessage;

        public EventHandler<MessageEventArgs> OnGameWonMessage;

        public EventHandler<MessageEventArgs> OnGameLostMessage;

        public void ProcessMessage(ScoresRequestMessage message)
        {
            RaiseEvent(OnScoreRequest, message);
        }

        public void ProcessMessage(ConnectionRequestClientMessage message)
        {
            RaiseEvent(OnConnectionRequestClient, message);
        }

        public void ProcessMessage(AnswerMessage message)
        {
            RaiseEvent(OnAnswer, message);
        }

        public void ProcessMessage(RightAnswerMessage message)
        {
            RaiseEvent(OnRightAnswer, message);
        }

        public void ProcessMessage(WrongAnswerMessage message)
        {
            RaiseEvent(OnWrongAnswer, message);
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
            RaiseEvent(OnScoreRequest, message);
        }

        public void ProcessMessage(ConnectionAcceptMessage message)
        {
            RaiseEvent(OnConnectionAccepted, message);
        }

        public void ProcessMessage(DisconnectMessage message)
        {
            RaiseEvent(this.OnDisconnect, message);
        }

        public void ProcessMessage(ConnectionRequestMonitorMessage message)
        {
            this.RaiseEvent(OnConnectionRequestMonitor, message);
        }

        public void ProcessMessage(ConnectionRequestServerMessage message)
        {
            this.RaiseEvent(OnConnectionRequestServer, message);
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

        public void RaiseEvent(EventHandler<MessageEventArgs> eventHandler, Message message)
        {
            if (eventHandler != null)
            {
                eventHandler(this, new MessageEventArgs(message));
            }
        }

        public void DataReceived(object sender, MessageEventArgs eventArgs)
        {
            eventArgs.MessageContent.ProcessMessage(this);
        }
    }
}
