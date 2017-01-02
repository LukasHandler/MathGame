//-----------------------------------------------------------------------
// <copyright file="GameFinishedEventArgs.cs" company="Lukas Handler">
//     Lukas Handler
// </copyright>
// <summary>
// This file represents the game finished event arguments.
// </summary>
//-----------------------------------------------------------------------
namespace Client.Application.EventArguments
{
    using System;

    /// <summary>
    /// This class represents the game finished event arguments.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class GameFinishedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GameFinishedEventArgs"/> class.
        /// </summary>
        /// <param name="score">The score.</param>
        public GameFinishedEventArgs(int score)
        {
            this.Score = score;
        }

        /// <summary>
        /// Gets or sets the score.
        /// </summary>
        /// <value>
        /// The score.
        /// </value>
        public int Score { get; set; }
    }
}
