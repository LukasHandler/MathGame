using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Application.EventArguments
{
    public class BroadcastEventArgs : EventArgs
    {
        public string BroadcastText { get; set; }

        public BroadcastEventArgs(string broadcastText)
        {
            this.BroadcastText = broadcastText;
        }
    }
}
