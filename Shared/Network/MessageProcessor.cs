using Shared.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Data.Messages;

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

        public void ProcessMessage(HighScoreRequest message)
        {
            RaiseEvent(OnScoreRequest, message);
        }

        public void ProcessMessage(ConnectionRequestClient message)
        {
            RaiseEvent(OnConnectionRequestClient, message);
        }

        public void ProcessMessage(Answer message)
        {
            RaiseEvent(OnAnswer, message);
        }

        public void ProcessMessage(RightAnswer message)
        {
            RaiseEvent(OnRightAnswer, message);
        }

        public void ProcessMessage(WrongAnswer message)
        {
            RaiseEvent(OnWrongAnswer, message);
        }

        public void ProcessMessage(ConnectionDenied message)
        {
            RaiseEvent(OnConnectionDenied, message);
        }

        public void ProcessMessage(Question message)
        {
            RaiseEvent(OnQuestion, message);
        }

        public void ProcessMessage(HighScoreResponse message)
        {
            RaiseEvent(OnScoreRequest, message);
        }

        public void ProcessMessage(ConnectionAcceptMessage message)
        {
            RaiseEvent(OnConnectionAccepted, message);
        }

        public void ProcessMessage(Disconnect message)
        {
            RaiseEvent(this.OnDisconnect, message);
        }

        public void ProcessMessage(ConnectionRequestMonitor message)
        {
            this.RaiseEvent(OnConnectionRequestMonitor, message);
        }

        public void ProcessMessage(ConnectionRequestServer message)
        {
            this.RaiseEvent(OnConnectionRequestServer, message);
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
