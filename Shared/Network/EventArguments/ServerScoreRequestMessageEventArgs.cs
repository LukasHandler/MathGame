using Shared.Data.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Data.EventArguments
{
    public class ServerScoreRequestMessageEventArgs : EventArgs
    {
        public ServerScoreRequestMessage Message { get; set; }

        public ServerScoreRequestMessageEventArgs(ServerScoreRequestMessage message)
        {
            this.Message = message;
        }
    }
}
