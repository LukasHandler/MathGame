//-----------------------------------------------------------------------
// <copyright file="ForwardingMessageEventArgs.cs" company="Lukas Handler">
//     Lukas Handler
// </copyright>
// <summary>
// This file represents the forwarding message event arguments.
// </summary>
//-----------------------------------------------------------------------
namespace Shared.Data.EventArguments
{
    using System;
    using Shared.Data.Messages;

    /// <summary>
    /// This class represents the forwarding message event arguments.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class ForwardingMessageEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ForwardingMessageEventArgs"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ForwardingMessageEventArgs(ForwardingMessage message)
        {
            this.Message = message;
        }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public ForwardingMessage Message { get; set; }
    }
}
