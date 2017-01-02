//-----------------------------------------------------------------------
// <copyright file="ConnectionRequestClientMessage.cs" company="Lukas Handler">
//     Lukas Handler
// </copyright>
// <summary>
// This file represents the connection request client message which contains the client name which is needed for verifying if the client is allowed to connect.
// </summary>
//-----------------------------------------------------------------------
namespace Shared.Data.Messages
{
    using System;

    /// <summary>
    /// This class represents the connection request client message.
    /// </summary>
    /// <seealso cref="Shared.Data.Messages.Message" />
    [Serializable]
    public class ConnectionRequestClientMessage : Message
    {
        /// <summary>
        /// Gets or sets the name of the sender.
        /// </summary>
        /// <value>
        /// The name of the sender.
        /// </value>
        public string SenderName { get; set; }

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
            return "Connection-Request-Client-Message";
        }
    }
}
