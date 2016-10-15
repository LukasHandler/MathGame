using Shared.Data.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Data
{
    public interface IMessageVisitor
    {
        void ProcessMessage(Answer message);

        void ProcessMessage(WrongAnswer message);

        void ProcessMessage(RightAnswer message);

        void ProcessMessage(ConnectionAccepted message);

        void ProcessMessage(ConnectionDenied message);

        void ProcessMessage(ConnectionRequest message);

        void ProcessMessage(HighScoreRequest message);

        void ProcessMessage(HighScoreResponse message);

        void ProcessMessage(Question message);

        void ProcessMessage(Disconnect message);
    }
}
