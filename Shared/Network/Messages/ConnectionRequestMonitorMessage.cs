//-----------------------------------------------------------------------
// <copyright file="ConnectionRequestMonitorMessage.cs" company="Lukas Handler">
//     Lukas Handler
// </copyright>
// <summary>
// This file represents the connection request message from the monitor.
// </summary>
//-----------------------------------------------------------------------
namespace Shared.Data.Messages
{
    using System;

    /// <summary>
    /// This class represents the connection request message from the monitor.
    /// </summary>
    /// <seealso cref="Shared.Data.Messages.Message" />
    [Serializable]
    public class ConnectionRequestMonitorMessage : Message
    {
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
            return "Connection-Request-Monitor-Message";
        }
    }
}
