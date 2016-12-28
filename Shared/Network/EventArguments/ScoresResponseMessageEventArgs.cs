using Shared.Data.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Data.EventArguments
{
    public class ScoresResponseMessageEventArgs :  EventArgs
    {
        public ScoresResponseMessage Message { get; set; }

        public ScoresResponseMessageEventArgs(ScoresResponseMessage message)
        {
            this.Message = message;
        }
    }
}
