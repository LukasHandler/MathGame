//-----------------------------------------------------------------------
// <copyright file="ScoresEventArgs.cs" company="Lukas Handler">
//     Lukas Handler
// </copyright>
// <summary>
// This file represents the scores event arguments.
// </summary>
//-----------------------------------------------------------------------
namespace Client.Application.EventArguments
{
    using System;
    using System.Collections.Generic;
    using Shared.Data;

    /// <summary>
    /// This class represents the score event arguments.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class ScoresEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScoresEventArgs"/> class. Needed when the client wants to know the scores.
        /// </summary>
        /// <param name="scores">The scores.</param>
        public ScoresEventArgs(List<ScoreEntry> scores)
        {
            this.Scores = scores;
        }

        /// <summary>
        /// Gets or sets the scores.
        /// </summary>
        /// <value>
        /// The scores.
        /// </value>
        public List<ScoreEntry> Scores { get; set; }
    }
}
