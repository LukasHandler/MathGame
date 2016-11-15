using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Application.EventArguments
{
    public class ServerConnectionCountChangedEventArgs : EventArgs
    {
        public int NewConnectionCount { get; private set; }

        public ServerConnectionCountChangedEventArgs(int newConnectionCount)
        {
            this.NewConnectionCount = newConnectionCount;
        }
    }
}
