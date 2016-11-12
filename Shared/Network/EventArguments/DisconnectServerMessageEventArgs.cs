using Shared.Data.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Data.EventArguments
{
    public class DisconnectServerMessageEventArgs : EventArgs
    {
        public DisconnectServerMessage Message { get; set; }

        public DisconnectServerMessageEventArgs(DisconnectServerMessage message)
        {
            this.Message = message;
        }
    }
}
