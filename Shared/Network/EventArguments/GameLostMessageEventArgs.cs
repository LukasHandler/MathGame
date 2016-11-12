using Shared.Data.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Data.EventArguments
{
    public class GameLostMessageEventArgs : EventArgs
    {
        public GameLostMessage Message { get; set; }

        public GameLostMessageEventArgs(GameLostMessage message)
        {
            this.Message = message;
        }
    }
}
