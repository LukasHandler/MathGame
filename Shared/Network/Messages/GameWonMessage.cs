//-----------------------------------------------------------------------
// <copyright file="GameWonMessage.cs" company="Lukas Handler">
//     Lukas Handler
// </copyright>
// <summary>
// This file represents the game won message.
// </summary>
//-----------------------------------------------------------------------
namespace Shared.Data.Messages
{
    using System;

    /// <summary>
    /// This class represents the game won message.
    /// </summary>
    /// <seealso cref="Shared.Data.Messages.Message" />
    [Serializable]
    public class GameWonMessage : Message
    {
        /// <summary>
        /// Gets or sets the score.
        /// </summary>
        /// <value>
        /// The score.
        /// </value>
        public int Score { get; set; }

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
            return "Game-Won-Message";
        }
    }
}
