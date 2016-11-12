using Shared.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Application.EventArguments
{
    public class ScoresEventArgs : EventArgs
    {
        public List<ScoreEntry> Scores { get; set; }

        public ScoresEventArgs(List<ScoreEntry> scores)
        {
            this.Scores = scores;
        }
    }
}
