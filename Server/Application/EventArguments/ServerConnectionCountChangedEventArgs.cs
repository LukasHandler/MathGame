//-----------------------------------------------------------------------
// <copyright file="ServerConnectionCountChangedEventArgs.cs" company="Lukas Handler">
//     Lukas Handler
// </copyright>
// <summary>
// This file represents the event arguments, which get used when the server connection count changed.
// </summary>
//-----------------------------------------------------------------------
namespace Server.Application.EventArguments
{
    using System;

    /// <summary>
    /// This file represents the event arguments, which get used when the server connection count changed.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class ServerConnectionCountChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServerConnectionCountChangedEventArgs"/> class.
        /// </summary>
        /// <param name="newConnectionCount">The new connection count.</param>
        public ServerConnectionCountChangedEventArgs(int newConnectionCount)
        {
            this.NewConnectionCount = newConnectionCount;
        }

        /// <summary>
        /// Gets the new connection count.
        /// </summary>
        /// <value>
        /// The new connection count.
        /// </value>
        public int NewConnectionCount { get; private set; }
    }
}
