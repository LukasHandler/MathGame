//-----------------------------------------------------------------------
// <copyright file="ConnectionRequestMonitorMessageEventArgs.cs" company="Lukas Handler">
//     Lukas Handler
// </copyright>
// <summary>
// This file represents the connection request monitor message event arguments.
// </summary>
//-----------------------------------------------------------------------
namespace Shared.Data.EventArguments
{
    using System;
    using Shared.Data.Messages;

    /// <summary>
    /// This class represents the connection request monitor message event arguments.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class ConnectionRequestMonitorMessageEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionRequestMonitorMessageEventArgs"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ConnectionRequestMonitorMessageEventArgs(ConnectionRequestMonitorMessage message)
        {
            this.Message = message;
        }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public ConnectionRequestMonitorMessage Message { get; set; }
    }
}
