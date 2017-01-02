//-----------------------------------------------------------------------
// <copyright file="ConnectionRequestClientMessageEventArgs.cs" company="Lukas Handler">
//     Lukas Handler
// </copyright>
// <summary>
// This file represents the connection request client message event arguments.
// </summary>
//-----------------------------------------------------------------------
namespace Shared.Data.EventArguments
{
    using System;
    using Shared.Data.Messages;

    /// <summary>
    /// This class represents the connection request client message event arguments.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class ConnectionRequestClientMessageEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionRequestClientMessageEventArgs"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ConnectionRequestClientMessageEventArgs(ConnectionRequestClientMessage message)
        {
            this.Message = message;
        }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public ConnectionRequestClientMessage Message { get; set; }
    }
}
