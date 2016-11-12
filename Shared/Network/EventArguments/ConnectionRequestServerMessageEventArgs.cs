using Shared.Data.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Data.EventArguments
{
    public class ConnectionRequestServerMessageEventArgs : EventArgs
    {
        public ConnectionRequestServerMessage Message { get; set; }

        public ConnectionRequestServerMessageEventArgs(ConnectionRequestServerMessage message)
        {
            this.Message = message;
        }
    }
}
