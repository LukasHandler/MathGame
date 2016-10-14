using Shared.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Data.Messages;

namespace Server.Application
{
    public class MessageProcessor : IMessageVisitor
    {
        public void ProcessMessage(AnswerDenied message)
        {
            throw new NotImplementedException();
        }

        public void ProcessMessage(ConnectionDenied message)
        {
            throw new NotImplementedException();
        }

        public void ProcessMessage(HighScoreRequest message)
        {
            throw new NotImplementedException();
        }

        public void ProcessMessage(Question message)
        {
            throw new NotImplementedException();
        }

        public void ProcessMessage(HighScoreResponse message)
        {
            throw new NotImplementedException();
        }

        public void ProcessMessage(ConnectionRequest message)
        {
            throw new NotImplementedException();
        }

        public void ProcessMessage(ConnectionAccepted message)
        {
            throw new NotImplementedException();
        }

        public void ProcessMessage(Answer message)
        {
            throw new NotImplementedException();
        }

        public void ProcessMessage(Message message)
        {
            throw new NotImplementedException();
        }
    }
}
