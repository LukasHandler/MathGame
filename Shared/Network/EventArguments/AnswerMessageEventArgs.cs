using Shared.Data.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Data.EventArguments
{
    public class AnswerMessageEventArgs : EventArgs
    {
        public AnswerMessage Message { get; set; }

        public AnswerMessageEventArgs(AnswerMessage message)
        {
            this.Message = message;
        }
    }
}
