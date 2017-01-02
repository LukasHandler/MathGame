//-----------------------------------------------------------------------
// <copyright file="BroadcastResponseMessage.cs" company="Lukas Handler">
//     Lukas Handler
// </copyright>
// <summary>
// This file represents the broadcast response message. Contains all the clients from the server, so the active server can send the broadcast message to all clients connected to the system.
// </summary>
//-----------------------------------------------------------------------
namespace Shared.Data.Messages
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// This class represents the broadcast response message.
    /// </summary>
    /// <seealso cref="Shared.Data.Messages.Message" />
    [Serializable]
    public class BroadcastResponseMessage : Message
    {
        /// <summary>
        /// Gets or sets the message to broadcast.
        /// </summary>
        /// <value>
        /// The message to broadcast.
        /// </value>
        public BroadcastMessage MessageToBroadcast { get; set; }

        /// <summary>
        /// Gets or sets the clients information.
        /// </summary>
        /// <value>
        /// The clients information (from the passive server).
        /// </value>
        public List<object> ClientsInformation { get; set; }

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
            return "Broadcast-Response-Message";
        }
    }
}
