//-----------------------------------------------------------------------
// <copyright file="Client.cs" company="Lukas Handler">
//     Lukas Handler
// </copyright>
// <summary>
// This file represents the client.
// </summary>
//-----------------------------------------------------------------------
namespace Server.Application
{
    using System;
    using System.Threading;
    using Shared.Data;

    /// <summary>
    /// This class represents the client.
    /// </summary>
    /// <seealso cref="Server.Application.SystemElement" />
    public class Client : SystemElement
    {
        /// <summary>
        /// The score.
        /// </summary>
        private int score;

        /// <summary>
        /// The maximum score.
        /// </summary>
        private int maxScore;

        /// <summary>
        /// The minimum score.
        /// </summary>
        private int minScore;

        /// <summary>
        /// Initializes a new instance of the <see cref="Client"/> class.
        /// </summary>
        /// <param name="name">The client name.</param>
        /// <param name="minScore">The minimum score limit.</param>
        /// <param name="maxScore">The maximum score limit.</param>
        /// <param name="targetInformation">The target information.</param>
        public Client(string name, int minScore, int maxScore, object targetInformation)
        {
            this.TargetInformation = targetInformation;
            this.minScore = minScore;
            this.maxScore = maxScore;
            this.Name = name;
            this.Score = 0;
        }

        /// <summary>
        /// Gets or sets the target information.
        /// </summary>
        /// <value>
        /// The target information.
        /// </value>
        public object TargetInformation { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The client name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the score.
        /// </summary>
        /// <value>
        /// The score.
        /// </value>
        public int Score
        {
            get
            {
                return this.score;
            }

            set
            {
                this.score = value;

                if (this.score == this.minScore)
                {
                    if (this.MinScoreReached != null)
                    {
                        this.MinScoreReached(this, EventArgs.Empty);
                    }
                }
                else if (this.score == this.maxScore)
                {
                    if (this.MaxScoreReached != null)
                    {
                        this.MaxScoreReached(this, EventArgs.Empty);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the min score reached event. Gets fired when the minimum score was reached.
        /// </summary>
        /// <value>
        /// The minimum score reached event.
        /// </value>
        public EventHandler MinScoreReached { get; set; }

        /// <summary>
        /// Gets or sets the maximum score reached event. Gets fired when the maximum score was reached.
        /// </summary>
        /// <value>
        /// The maximum score reached event.
        /// </value>
        public EventHandler MaxScoreReached { get; set; }

        /// <summary>
        /// Gets or sets the question timer event.
        /// </summary>
        /// <value>
        /// The question timer event.
        /// </value>
        public Timer QuestionTimer { get; set; }

        /// <summary>
        /// Gets or sets the current question for the client.
        /// </summary>
        /// <value>
        /// The current question for the client.
        /// </value>
        public MathQuestion Question { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.Name;
        }
    }
}
