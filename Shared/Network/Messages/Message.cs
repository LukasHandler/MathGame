//-----------------------------------------------------------------------
// <copyright file="Message.cs" company="Lukas Handler">
//     Lukas Handler
// </copyright>
// <summary>
// This file represents the message. All other messages inherit from this class.
// </summary>
//-----------------------------------------------------------------------
namespace Shared.Data.Messages
{
    using System;

    /// <summary>
    /// This class represents the message.
    /// </summary>
    [Serializable]
    public abstract class Message
    {
        /// <summary>
        /// Processes the message. Implementation of the visitor.
        /// </summary>
        /// <param name="processor">The processor.</param>
        public abstract void ProcessMessage(IMessageVisitor processor);

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public abstract override string ToString();
    }
}
