using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Data
{
    [Serializable]
    public class ScoreEntry
    {
        public string PlayerName { get; set; }

        public int Score { get; set; }

        public ScoreEntry(string playerName, int score)
        {
            this.PlayerName = playerName;
            this.Score = score;
        }
    }
}
