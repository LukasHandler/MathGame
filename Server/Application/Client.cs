using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Application
{
    public class Client
    {
        public string PlayerName { get; set; }

        public int Score { get; set; }

        public Client(string playerName)
        {
            this.PlayerName = playerName;
            this.Score = 0;
        }

    }
}
