//-----------------------------------------------------------------------
// <copyright file="ConnectionAcceptServerMessage.cs" company="Lukas Handler">
//     Lukas Handler
// </copyright>
// <summary>
// This file represents the connection accept server message. Containing additional information like the sender name and if the recipient is the active server.
// </summary>
//-----------------------------------------------------------------------
namespace Shared.Data.Messages
{
    using System;

    /// <summary>
    /// This class represents the connection accept server message.
    /// </summary>
    /// <seealso cref="Shared.Data.Messages.Message" />
    [Serializable]
    public class ConnectionAcceptServerMessage : Message
    {
        /// <summary>
        /// Gets or sets the name of the sender, which is another server.
        /// </summary>
        /// <value>
        /// The name of the sender.
        /// </value>
        public string SenderName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the recipient is active.
        /// </summary>
        /// <value>
        /// <c>true</c> if this server is active; otherwise, <c>false</c>.
        /// </value>
        public bool IsTargetActive { get; set; }

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
            return "Connection-Accept-Server-Message";
        }
    }
}
