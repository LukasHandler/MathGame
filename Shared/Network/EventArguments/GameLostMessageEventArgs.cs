//-----------------------------------------------------------------------
// <copyright file="GameLostMessageEventArgs.cs" company="Lukas Handler">
//     Lukas Handler
// </copyright>
// <summary>
// This file represents the game lost message event args message event arguments.
// </summary>
//-----------------------------------------------------------------------
namespace Shared.Data.EventArguments
{
    using System;
    using Shared.Data.Messages;

    /// <summary>
    /// This class represents the game lost message event args message event arguments.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class GameLostMessageEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GameLostMessageEventArgs"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public GameLostMessageEventArgs(GameLostMessage message)
        {
            this.Message = message;
        }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public GameLostMessage Message { get; set; }
    }
}
