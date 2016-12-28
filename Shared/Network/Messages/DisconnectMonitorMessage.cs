using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Data.Messages
{
    [Serializable]
    public class DisconnectMonitorMessage : Message
    {
        public override void ProcessMessage(IMessageVisitor processor)
        {
            processor.ProcessMessage(this);
        }

        public override string ToString()
        {
            return "Disconnect-Monitor-Message";
        }
    }
}
