//-----------------------------------------------------------------------
// <copyright file="QuestionMessage.cs" company="Lukas Handler">
//     Lukas Handler
// </copyright>
// <summary>
// This file represents the question message.
// </summary>
//-----------------------------------------------------------------------
namespace Shared.Data.Messages
{
    using System;

    /// <summary>
    /// This class represents the question message.
    /// </summary>
    /// <seealso cref="Shared.Data.Messages.Message" />
    [Serializable]
    public class QuestionMessage : Message
    {
        /// <summary>
        /// Gets or sets the question identifier.
        /// </summary>
        /// <value>
        /// The question identifier.
        /// </value>
        public int QuestionID { get; set; }

        /// <summary>
        /// Gets or sets the question text.
        /// </summary>
        /// <value>
        /// The question text.
        /// </value>
        public string QuestionText { get; set; }

        /// <summary>
        /// Gets or sets the current player score.
        /// </summary>
        /// <value>
        /// The score.
        /// </value>
        public int Score { get; set; }

        /// <summary>
        /// Gets or sets the time the client has for answering the question.
        /// </summary>
        /// <value>
        /// The time the client has for answering the question.
        /// </value>
        public int Time { get; set; }

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
            return string.Format("Question ({0})", this.QuestionText);
        }
    }
}
