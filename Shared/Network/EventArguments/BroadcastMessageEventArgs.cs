//-----------------------------------------------------------------------
// <copyright file="BroadcastMessageEventArgs.cs" company="Lukas Handler">
//     Lukas Handler
// </copyright>
// <summary>
// This file represents the broadcast message event arguments.
// </summary>
//-----------------------------------------------------------------------
namespace Shared.Data.EventArguments
{
    using System;
    using Shared.Data.Messages;

    /// <summary>
    /// This class represents the broadcast message event arguments.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class BroadcastMessageEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BroadcastMessageEventArgs"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public BroadcastMessageEventArgs(BroadcastMessage message)
        {
            this.Message = message;
        }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public BroadcastMessage Message { get; set; }
    }
}
