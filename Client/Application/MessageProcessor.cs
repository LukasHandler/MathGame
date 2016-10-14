using Shared.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Data.Messages;

namespace Client.Application
{
    public class MessageProcessor : IMessageVisitor
    {
        public void ProcessMessage(AnswerDenied message)
        {
        }

        public void ProcessMessage(ConnectionDenied message)
        {
        }

        public void ProcessMessage(HighScoreRequest message)
        {
        }

        public void ProcessMessage(Question message)
        {
        }

        public void ProcessMessage(HighScoreResponse message)
        {
        }

        public void ProcessMessage(ConnectionRequest message)
        {
        }

        public void ProcessMessage(ConnectionAccepted message)
        {
        }

        public void ProcessMessage(Answer message)
        {
        }

        public void ProcessMessage(Message message)
        {
        }
    }
}
