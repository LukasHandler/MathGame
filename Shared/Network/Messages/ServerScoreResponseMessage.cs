//-----------------------------------------------------------------------
// <copyright file="ServerScoreResponseMessage.cs" company="Lukas Handler">
//     Lukas Handler
// </copyright>
// <summary>
// This file represents the server score response message. The answer to a server score request message. Contains the scores and the request sender from the score request. 
// </summary>
//-----------------------------------------------------------------------
namespace Shared.Data.Messages
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// This class represents the server score response message.
    /// </summary>
    /// <seealso cref="Shared.Data.Messages.Message" />
    [Serializable]
    public class ServerScoreResponseMessage : Message
    {
        /// <summary>
        /// Gets or sets the request sender. The active server will send the scores to this client.
        /// </summary>
        /// <value>
        /// The request sender.
        /// </value>
        public object RequestSender { get; set; }

        /// <summary>
        /// Gets or sets the scores of the passive server.
        /// </summary>
        /// <value>
        /// The scores.
        /// </value>
        public List<ScoreEntry> Scores { get; set; }

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
            return "Server-Score-Response-Message";
        }
    }
}
