using Shared.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server.Application
{
    public class Client : GameMember
    {
        private int score;

        private int maxScore;

        private int minScore;

        public string Name { get; set; }

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

        public Timer QuestionTimer { get; set; }

        public MathQuestion Question { get; set; }

        public Client(string name, int minScore, int maxScore, object targetInformation) : base(targetInformation)
        {
            this.TargetInformation = targetInformation;
            this.minScore = minScore;
            this.maxScore = maxScore;
            this.Name = name;
            this.Score = 0;
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
