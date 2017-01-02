//-----------------------------------------------------------------------
// <copyright file="DisconnectServerMessageEventArgs.cs" company="Lukas Handler">
//     Lukas Handler
// </copyright>
// <summary>
// This file represents the disconnect server message event arguments.
// </summary>
//-----------------------------------------------------------------------
namespace Shared.Data.EventArguments
{
    using System;
    using Shared.Data.Messages;

    /// <summary>
    /// This class represents the disconnect server message event arguments.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class DisconnectServerMessageEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DisconnectServerMessageEventArgs"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public DisconnectServerMessageEventArgs(DisconnectServerMessage message)
        {
            this.Message = message;
        }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public DisconnectServerMessage Message { get; set; }
    }
}
