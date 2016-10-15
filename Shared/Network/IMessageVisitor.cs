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
        void ProcessMessage(AnswerMessage message);

        void ProcessMessage(WrongAnswerMessage message);

        void ProcessMessage(RightAnswerMessage message);

        void ProcessMessage(ConnectionAcceptMessage message);

        void ProcessMessage(ConnectionDeniedMessage message);

        void ProcessMessage(ConnectionRequestClientMessage message);

        void ProcessMessage(ScoresRequestMessage message);

        void ProcessMessage(ScoresResponseMessage message);

        void ProcessMessage(QuestionMessage message);

        void ProcessMessage(DisconnectMessage message);

        void ProcessMessage(ConnectionRequestMonitorMessage message);

        void ProcessMessage(ConnectionRequestServerMessage message);

        void ProcessMessage(LoggingMessage message);
    }
}
