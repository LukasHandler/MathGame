using Shared.Data.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Data.EventArguments
{
    public class ConnectionAcceptedServerMessageEventArgs : EventArgs
    {
        public ConnectionAcceptServerMessage Message { get; set; }

        public ConnectionAcceptedServerMessageEventArgs(ConnectionAcceptServerMessage message)
        {
            this.Message = message;
        }
    }
}
