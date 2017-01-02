//-----------------------------------------------------------------------
// <copyright file="BroadcastRequestMessageEventArgs.cs" company="Lukas Handler">
//     Lukas Handler
// </copyright>
// <summary>
// This file represents the broadcast request message event arguments.
// </summary>
//-----------------------------------------------------------------------
namespace Shared.Data.EventArguments
{
    using System;
    using Shared.Data.Messages;

    /// <summary>
    /// This class represents the broadcast request message event arguments.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class BroadcastRequestMessageEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BroadcastRequestMessageEventArgs"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public BroadcastRequestMessageEventArgs(BroadcastRequestMessage message)
        {
            this.Message = message;
        }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public BroadcastRequestMessage Message { get; set; }
    }
}
