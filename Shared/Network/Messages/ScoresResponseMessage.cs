//-----------------------------------------------------------------------
// <copyright file="ScoresResponseMessage.cs" company="Lukas Handler">
//     Lukas Handler
// </copyright>
// <summary>
// This file represents the score response message.
// </summary>
//-----------------------------------------------------------------------
namespace Shared.Data.Messages
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// This class represents the score response message.
    /// </summary>
    /// <seealso cref="Shared.Data.Messages.Message" />
    [Serializable]
    public class ScoresResponseMessage : Message
    {
        /// <summary>
        /// Gets or sets the scores of the clients connected to the system.
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
