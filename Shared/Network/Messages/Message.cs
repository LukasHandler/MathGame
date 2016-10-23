using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Data.Messages
{
    [Serializable]
    public abstract class Message
    {
        public object SenderInformation { get; set; }

        public string SenderName { get; set; }
        
        public abstract void ProcessMessage(IMessageVisitor processor);
    }
}
