//-----------------------------------------------------------------------
// <copyright file="BroadcastRequestMessage.cs" company="Lukas Handler">
//     Lukas Handler
// </copyright>
// <summary>
// This file represents the broadcast request message. Used for server to server communication to get all the clients from the other server for sending broadcast to all clients in the system.
// </summary>
//-----------------------------------------------------------------------
namespace Shared.Data.Messages
{
    using System;

    /// <summary>
    /// This class represents the broadcast request message.
    /// </summary>
    /// <seealso cref="Shared.Data.Messages.Message" />
    [Serializable]
    public class BroadcastRequestMessage : Message
    {
        /// <summary>
        /// Gets or sets the message to broadcast.
        /// </summary>
        /// <value>
        /// The message to broadcast.
        /// </value>
        public BroadcastMessage MessageToBroadcast { get; set; }

        /// <summary>
        /// Processes the message.
        /// </summary>
        /// <param name="processor">The processor.</param>
        public override void ProcessMessage(IMessageVisitor processor)
        {
            processor.ProcessMessage(this);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return "Broadcast-Request-Message";
        }
    }
}
