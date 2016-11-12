using Shared.Data.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Data.EventArguments
{
    public class GameWonMessageEventArgs : EventArgs
    {
        public GameWonMessage Message { get; set; }

        public GameWonMessageEventArgs(GameWonMessage message)
        {
            this.Message = message;
        }
    }
}
