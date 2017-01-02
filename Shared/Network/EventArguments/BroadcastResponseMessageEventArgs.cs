//-----------------------------------------------------------------------
// <copyright file="BroadcastResponseMessageEventArgs.cs" company="Lukas Handler">
//     Lukas Handler
// </copyright>
// <summary>
// This file represents the broadcast response message event arguments.
// </summary>
//-----------------------------------------------------------------------
namespace Shared.Data.EventArguments
{
    using System;
    using Shared.Data.Messages;

    /// <summary>
    /// This class represents the broadcast response message event arguments.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class BroadcastResponseMessageEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BroadcastResponseMessageEventArgs"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public BroadcastResponseMessageEventArgs(BroadcastResponseMessage message)
        {
            this.Message = message;
        }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public BroadcastResponseMessage Message { get; set; }
    }
}
