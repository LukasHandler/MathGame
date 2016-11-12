using Shared.Data.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Data.EventArguments
{
    public class ForwardingMessageEventArgs : EventArgs
    {
        public ForwardingMessage Message { get; set; }

        public ForwardingMessageEventArgs(ForwardingMessage message)
        {
            this.Message = message;
        }
    }
}
