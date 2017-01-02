//-----------------------------------------------------------------------
// <copyright file="ConnectionRequestServerMessage.cs" company="Lukas Handler">
//     Lukas Handler
// </copyright>
// <summary>
// This file represents the connection request from a server. This message contains the name of the server and the start time which is needed to decide which server is active.
// </summary>
//-----------------------------------------------------------------------
namespace Shared.Data.Messages
{
    using System;

    /// <summary>
    /// This class represents the connection request from a server.
    /// </summary>
    /// <seealso cref="Shared.Data.Messages.Message" />
    [Serializable]
    public class ConnectionRequestServerMessage : Message
    {
        /// <summary>
        /// Gets or sets the name of the sender.
        /// </summary>
        /// <value>
        /// The name of the sender.
        /// </value>
        public string SenderName { get; set; }

        /// <summary>
        /// Gets or sets the sender start time.
        /// </summary>
        /// <value>
        /// The sender start time.
        /// </value>
        public DateTime SenderStartTime { get; set; }

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
            return "Connection-Request-Server-Message";
        }
    }
}
