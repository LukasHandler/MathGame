using Shared.Data.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Data.EventArguments
{
    public class QuestionMessageEventArgs : EventArgs
    {
        public QuestionMessage Message { get; set; }

        public QuestionMessageEventArgs(QuestionMessage message)
        {
            this.Message = message;
        }
    }
}
