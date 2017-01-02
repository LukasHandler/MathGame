//-----------------------------------------------------------------------
// <copyright file="ScoresResponseMessageEventArgs.cs" company="Lukas Handler">
//     Lukas Handler
// </copyright>
// <summary>
// This file represents the scores response message event arguments.
// </summary>
//-----------------------------------------------------------------------
namespace Shared.Data.EventArguments
{
    using System;
    using Shared.Data.Messages;

    /// <summary>
    /// This class represents the scores response message event arguments.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class ScoresResponseMessageEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScoresResponseMessageEventArgs"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ScoresResponseMessageEventArgs(ScoresResponseMessage message)
        {
            this.Message = message;
        }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public ScoresResponseMessage Message { get; set; }
    }
}
