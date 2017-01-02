//-----------------------------------------------------------------------
// <copyright file="ConnectionAcceptedServerMessageEventArgs.cs" company="Lukas Handler">
//     Lukas Handler
// </copyright>
// <summary>
// This file represents the connection accepted server message event arguments.
// </summary>
//-----------------------------------------------------------------------
namespace Shared.Data.EventArguments
{
    using System;
    using Shared.Data.Messages;

    /// <summary>
    /// This class represents the connection accepted server message event arguments.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class ConnectionAcceptedServerMessageEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionAcceptedServerMessageEventArgs"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ConnectionAcceptedServerMessageEventArgs(ConnectionAcceptServerMessage message)
        {
            this.Message = message;
        }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public ConnectionAcceptServerMessage Message { get; set; }
    }
}
