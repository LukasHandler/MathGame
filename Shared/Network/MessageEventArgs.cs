using Shared.Data.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Data
{
    public class MessageEventArgs : EventArgs
    {
        public Message MessageContent { get; set; }
    }
}
