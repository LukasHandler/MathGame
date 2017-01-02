//-----------------------------------------------------------------------
// <copyright file="ForwardingMessage.cs" company="Lukas Handler">
//     Lukas Handler
// </copyright>
// <summary>
// This file represents the forwarding message. These messages are needed if a passive server wants to send a message. Only the active server is allowed to send directly to the clients.
// </summary>
//-----------------------------------------------------------------------
namespace Shared.Data.Messages
{
    using System;

    /// <summary>
    /// This class represents the forwarding message.
    /// </summary>
    /// <seealso cref="Shared.Data.Messages.Message" />
    [Serializable]
    public class ForwardingMessage : Message
    {
        /// <summary>
        /// Gets or sets the inner message.
        /// </summary>
        /// <value>
        /// The inner message.
        /// </value>
        public Message InnerMessage { get; set; }

        /// <summary>
        /// Gets or sets the target of the inner message.
        /// </summary>
        /// <value>
        /// The target.
        /// </value>
        public object Target { get; set; }

        /// <summary>
        /// Gets or sets the name of the target. This is only needed for logging when the active server sends the message.
        /// </summary>
        /// <value>
        /// The name of the target.
        /// </value>
        public string TargetName { get; set; }

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
            return "Forwarding-Message";
        }
    }
}
