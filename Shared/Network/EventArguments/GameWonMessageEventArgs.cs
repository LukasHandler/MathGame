//-----------------------------------------------------------------------
// <copyright file="GameWonMessageEventArgs.cs" company="Lukas Handler">
//     Lukas Handler
// </copyright>
// <summary>
// This file represents the game won message event arguments.
// </summary>
//-----------------------------------------------------------------------
namespace Shared.Data.EventArguments
{
    using System;
    using Shared.Data.Messages;

    /// <summary>
    /// This class represents the game won message event arguments.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class GameWonMessageEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GameWonMessageEventArgs"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public GameWonMessageEventArgs(GameWonMessage message)
        {
            this.Message = message;
        }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public GameWonMessage Message { get; set; }
    }
}
