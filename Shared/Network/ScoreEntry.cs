//-----------------------------------------------------------------------
// <copyright file="ScoreEntry.cs" company="Lukas Handler">
//     Lukas Handler
// </copyright>
// <summary>
// This file represents a score entry.
// </summary>
//-----------------------------------------------------------------------
namespace Shared.Data
{
    using System;

    /// <summary>
    /// This class represents a score entry.
    /// </summary>
    [Serializable]
    public class ScoreEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScoreEntry"/> class.
        /// </summary>
        /// <param name="playerName">Name of the player.</param>
        /// <param name="score">The score.</param>
        public ScoreEntry(string playerName, int score)
        {
            this.PlayerName = playerName;
            this.Score = score;
        }

        /// <summary>
        /// Gets or sets the name of the player.
        /// </summary>
        /// <value>
        /// The name of the player.
        /// </value>
        public string PlayerName { get; set; }

        /// <summary>
        /// Gets or sets the score.
        /// </summary>
        /// <value>
        /// The score.
        /// </value>
        public int Score { get; set; }
    }
}
