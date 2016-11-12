using Shared.Data.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Data.EventArguments
{
    public class LoggingMessageEventArgs : EventArgs
    {
        public LoggingMessage Message { get; set; }

        public LoggingMessageEventArgs(LoggingMessage message)
        {
            this.Message = message;
        }
    }
}
