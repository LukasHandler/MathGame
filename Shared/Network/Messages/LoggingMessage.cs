//-----------------------------------------------------------------------
// <copyright file="LoggingMessage.cs" company="Lukas Handler">
//     Lukas Handler
// </copyright>
// <summary>
// This file represents the logging message.
// </summary>
//-----------------------------------------------------------------------
namespace Shared.Data.Messages
{
    using System;

    /// <summary>
    /// This class represents the logging message.
    /// </summary>
    /// <seealso cref="Shared.Data.Messages.Message" />
    [Serializable]
    public class LoggingMessage : Message
    {
        /// <summary>
        /// Gets or sets the logged text.
        /// </summary>
        /// <value>
        /// The logged text.
        /// </value>
        public string Text { get; set; }

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
            return "Logging-Message";
        }
    }
}
