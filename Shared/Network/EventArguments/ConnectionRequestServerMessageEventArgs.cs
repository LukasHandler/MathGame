//-----------------------------------------------------------------------
// <copyright file="ConnectionRequestServerMessageEventArgs.cs" company="Lukas Handler">
//     Lukas Handler
// </copyright>
// <summary>
// This file represents the connection request server message event arguments.
// </summary>
//-----------------------------------------------------------------------
namespace Shared.Data.EventArguments
{
    using System;
    using Shared.Data.Messages;

    /// <summary>
    /// This class represents the connection request server message event arguments.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class ConnectionRequestServerMessageEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionRequestServerMessageEventArgs"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ConnectionRequestServerMessageEventArgs(ConnectionRequestServerMessage message)
        {
            this.Message = message;
        }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public ConnectionRequestServerMessage Message { get; set; }
    }
}
