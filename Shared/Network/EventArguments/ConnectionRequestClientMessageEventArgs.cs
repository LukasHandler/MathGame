using Shared.Data.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Data.EventArguments
{
    public class ConnectionRequestClientMessageEventArgs : EventArgs
    {
        public ConnectionRequestClientMessage Message { get; set; }

        public ConnectionRequestClientMessageEventArgs(ConnectionRequestClientMessage message)
        {
            this.Message = message;
        }
    }
}
