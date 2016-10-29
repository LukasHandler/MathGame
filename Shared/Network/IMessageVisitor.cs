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
        void ProcessMessage(BroadcastRequestMessage message);

        void ProcessMessage(BroadcastResponseMessage message);

        void ProcessMessage(BroadcastMessage message);

        void ProcessMessage(ServerScoreRequestMessage message);

        void ProcessMessage(ServerScoreResponseMessage message);

        void ProcessMessage(AnswerMessage message);

        void ProcessMessage(DisconnectServerMessage message);

        void ProcessMessage(ConnectionAcceptMessage message);
        void ProcessMessage(ConnectionAcceptServerMessage connectionAcceptServerMessage);
        void ProcessMessage(ConnectionDeniedMessage message);

        void ProcessMessage(ConnectionRequestClientMessage message);

        void ProcessMessage(ConnectionRequestServerMessage message);

        void ProcessMessage(ConnectionRequestMonitorMessage message);

        void ProcessMessage(ScoresRequestMessage message);

        void ProcessMessage(ScoresResponseMessage message);

        void ProcessMessage(QuestionMessage message);

        void ProcessMessage(DisconnectMessage message);

        void ProcessMessage(LoggingMessage message);

        void ProcessMessage(GameLostMessage message);

        void ProcessMessage(GameWonMessage message);

        void ProcessMessage(ForwardingMessage message);
    }
}
