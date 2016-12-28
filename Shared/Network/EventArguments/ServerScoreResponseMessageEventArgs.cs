using Shared.Data.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Data.EventArguments
{
    public class ServerScoreResponseMessageEventArgs : EventArgs
    {
        public ServerScoreResponseMessage Message { get; set; }

        public ServerScoreResponseMessageEventArgs(ServerScoreResponseMessage message)
        {
            this.Message = message;
        }
    }
}
