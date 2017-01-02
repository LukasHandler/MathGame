//-----------------------------------------------------------------------
// <copyright file="QuestionEventArgs.cs" company="Lukas Handler">
//     Lukas Handler
// </copyright>
// <summary>
// This file represents the question event arguments.
// </summary>
//-----------------------------------------------------------------------
namespace Client.Application.EventArguments
{
    using System;

    /// <summary>
    /// This class represents the question event arguments.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class QuestionEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionEventArgs"/> class.
        /// </summary>
        /// <param name="questionText">The question.</param>
        /// <param name="time">The time, needed for the UI counter.</param>
        /// <param name="score">The current score of the client.</param>
        public QuestionEventArgs(string questionText, int time, int score)
        {
            this.QuestionText = questionText;
            this.Time = time;
            this.Score = score;
        }

        /// <summary>
        /// Gets or sets the question.
        /// </summary>
        /// <value>
        /// The question.
        /// </value>
        public string QuestionText { get; set; }

        /// <summary>
        /// Gets or sets the time the client has to answer the question.
        /// </summary>
        /// <value>
        /// The time the client has to answer the question.
        /// </value>
        public int Time { get; set; }

        /// <summary>
        /// Gets or sets the score.
        /// </summary>
        /// <value>
        /// The score.
        /// </value>
        public int Score { get; set; }
    }
}
