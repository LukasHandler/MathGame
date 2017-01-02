//-----------------------------------------------------------------------
// <copyright file="ServerScoreResponseMessageEventArgs.cs" company="Lukas Handler">
//     Lukas Handler
// </copyright>
// <summary>
// This file represents the server score response message event arguments.
// </summary>
//-----------------------------------------------------------------------
namespace Shared.Data.EventArguments
{
    using System;
    using Shared.Data.Messages;

    /// <summary>
    /// This class represents the server score response message event arguments.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class ServerScoreResponseMessageEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServerScoreResponseMessageEventArgs"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ServerScoreResponseMessageEventArgs(ServerScoreResponseMessage message)
        {
            this.Message = message;
        }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public ServerScoreResponseMessage Message { get; set; }
    }
}
