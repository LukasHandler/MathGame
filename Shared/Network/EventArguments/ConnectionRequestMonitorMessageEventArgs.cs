using Shared.Data.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Data.EventArguments
{
    public class ConnectionRequestMonitorMessageEventArgs : EventArgs
    {
        public ConnectionRequestMonitorMessage Message { get; set; }

        public ConnectionRequestMonitorMessageEventArgs(ConnectionRequestMonitorMessage message)
        {
            this.Message = message;
        }
    }
}
