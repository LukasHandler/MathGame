using Shared.Data.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Data.EventArguments
{
    public class MessageEventArgs : EventArgs
    {
        public Message MessageContent { get; set; }

        public MessageEventArgs(Message messageContent)
        {
            this.MessageContent = messageContent;
        }
    }
}
