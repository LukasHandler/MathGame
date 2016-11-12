using Shared.Data.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Data.EventArguments
{
    public class BroadcastRequestMessageEventArgs : EventArgs
    {
        public BroadcastRequestMessage Message { get; set; }

        public BroadcastRequestMessageEventArgs(BroadcastRequestMessage message)
        {
            this.Message = message;
        }
    }
}
