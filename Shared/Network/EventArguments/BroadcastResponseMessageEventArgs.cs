using Shared.Data.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Data.EventArguments
{
    public class BroadcastResponseMessageEventArgs : EventArgs
    {
        public BroadcastResponseMessage Message { get; set; }

        public BroadcastResponseMessageEventArgs(BroadcastResponseMessage message)
        {
            this.Message = message;
        }
    }
}
