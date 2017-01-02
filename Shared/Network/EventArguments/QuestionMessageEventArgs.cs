//-----------------------------------------------------------------------
// <copyright file="QuestionMessageEventArgs.cs" company="Lukas Handler">
//     Lukas Handler
// </copyright>
// <summary>
// This file represents the question message event arguments.
// </summary>
//-----------------------------------------------------------------------
namespace Shared.Data.EventArguments
{
    using System;
    using Shared.Data.Messages;

    /// <summary>
    /// This class represents the question message event arguments.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class QuestionMessageEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionMessageEventArgs"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public QuestionMessageEventArgs(QuestionMessage message)
        {
            this.Message = message;
        }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public QuestionMessage Message { get; set; }
    }
}
