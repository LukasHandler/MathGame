using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Application
{
    public class ServerConfiguration
    {
        public string ServerName { get; set; }

        public int ClientPort { get; set; }

        public int MonitorPort { get; set; }

        public int ServerPort { get; set; }

        public int MaxScore { get; set; }

        public int MinScore { get; set; }
    }
}
