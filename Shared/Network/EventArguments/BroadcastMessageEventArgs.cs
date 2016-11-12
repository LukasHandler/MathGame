using Shared.Data.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Data.EventArguments
{
    public class BroadcastMessageEventArgs : EventArgs
    {
        public BroadcastMessage Message { get; set; }

        public BroadcastMessageEventArgs(BroadcastMessage message)
        {
            this.Message = message;
        }
    }
}
