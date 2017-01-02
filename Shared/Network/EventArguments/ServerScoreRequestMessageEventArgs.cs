//-----------------------------------------------------------------------
// <copyright file="ServerScoreRequestMessageEventArgs.cs" company="Lukas Handler">
//     Lukas Handler
// </copyright>
// <summary>
// This file represents the server score request message event arguments.
// </summary>
//-----------------------------------------------------------------------
namespace Shared.Data.EventArguments
{
    using System;
    using Shared.Data.Messages;

    /// <summary>
    /// This class represents the server score request message event arguments.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class ServerScoreRequestMessageEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServerScoreRequestMessageEventArgs"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ServerScoreRequestMessageEventArgs(ServerScoreRequestMessage message)
        {
            this.Message = message;
        }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public ServerScoreRequestMessage Message { get; set; }
    }
}
