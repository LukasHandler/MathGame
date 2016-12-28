using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Data.Messages
{
    [Serializable]
    public class ForwardingMessage : Message
    {
        public Message InnerMessage { get; set; }

        public object Target { get; set; }

        public string TargetName { get; set; }

        public override void ProcessMessage(IMessageVisitor processor)
        {
            processor.ProcessMessage(this);
        }

        public override string ToString()
        {
            return "Forwarding-Message";
        }
    }
}
