//-----------------------------------------------------------------------
// <copyright file="LoggingMessageEventArgs.cs" company="Lukas Handler">
//     Lukas Handler
// </copyright>
// <summary>
// This file represents the logging message event arguments.
// </summary>
//-----------------------------------------------------------------------
namespace Shared.Data.EventArguments
{
    using System;
    using Shared.Data.Messages;

    /// <summary>
    /// This class represents the logging message event arguments.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class LoggingMessageEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingMessageEventArgs"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public LoggingMessageEventArgs(LoggingMessage message)
        {
            this.Message = message;
        }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public LoggingMessage Message { get; set; }
    }
}
