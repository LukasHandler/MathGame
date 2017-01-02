//-----------------------------------------------------------------------
// <copyright file="AnswerMessageEventArgs.cs" company="Lukas Handler">
//     Lukas Handler
// </copyright>
// <summary>
// This file represents the answer message event arguments.
// </summary>
//-----------------------------------------------------------------------
namespace Shared.Data.EventArguments
{
    using System;
    using Shared.Data.Messages;

    /// <summary>
    /// This class represents the answer message event arguments.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class AnswerMessageEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnswerMessageEventArgs"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public AnswerMessageEventArgs(AnswerMessage message)
        {
            this.Message = message;
        }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public AnswerMessage Message { get; set; }
    }
}
