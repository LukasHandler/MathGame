//-----------------------------------------------------------------------
// <copyright file="ServerScoreRequestMessage.cs" company="Lukas Handler">
//     Lukas Handler
// </copyright>
// <summary>
// This file represents the server score request message. The server need extra communication to get all the scores of the whole system.
// </summary>
//-----------------------------------------------------------------------
namespace Shared.Data.Messages
{
    using System;

    /// <summary>
    /// This class represents the server score request message.
    /// </summary>
    /// <seealso cref="Shared.Data.Messages.Message" />
    [Serializable]
    public class ServerScoreRequestMessage : Message
    {
        /// <summary>
        /// Gets or sets the request sender. So the server know where to send the scores.
        /// </summary>
        /// <value>
        /// The request sender.
        /// </value>
        public object RequestSender { get; set; }

        /// <summary>
        /// Processes the message. Implementation of the visitor.
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
            return "Server-Score-Request-Message";
        }
    }
}
