//-----------------------------------------------------------------------
// <copyright file="BroadcastEventArgs.cs" company="Lukas Handler">
//     Lukas Handler
// </copyright>
// <summary>
// This file represents the broadcast event arguments.
// </summary>
//-----------------------------------------------------------------------
namespace Client.Application.EventArguments
{
    using System;

    /// <summary>
    /// This class represents the broadcast event arguments.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class BroadcastEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BroadcastEventArgs"/> class.
        /// </summary>
        /// <param name="broadcastText">The broadcast text.</param>
        public BroadcastEventArgs(string broadcastText)
        {
            this.BroadcastText = broadcastText;
        }

        /// <summary>
        /// Gets or sets the broadcast text.
        /// </summary>
        /// <value>
        /// The broadcast text.
        /// </value>
        public string BroadcastText { get; set; }
    }
}
