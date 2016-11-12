using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Application.EventArguments
{
    public class GameFinishedEventArgs : EventArgs
    {
        public int Score { get; set; }

        public GameFinishedEventArgs(int score)
        {
            this.Score = score;
        }
    }
}
