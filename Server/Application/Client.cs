using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Application
{
    public class Client
    {
        private int score;

        private int maxScore;

        private int minScore;

        public string PlayerName { get; set; }

        public int Score
        {
            get
            {
                return this.score;
            }
            set
            {
                this.score = value;

                if (this.score == minScore)
                {
                    if (this.MinScoreReached != null)
                    {
                        this.MinScoreReached(this, EventArgs.Empty);
                    }
                }
                else if (this.score == maxScore)
                {
                    if (this.MaxScoreReached != null)
                    {
                        this.MaxScoreReached(this, EventArgs.Empty);
                    }
                }
            }
        }

        public EventHandler MinScoreReached;

        public EventHandler MaxScoreReached;

        public Client(string playerName, int minScore, int maxScore)
        {
            this.minScore = minScore;
            this.maxScore = maxScore;
            this.PlayerName = playerName;
            this.Score = 0;
        }
    }
}
