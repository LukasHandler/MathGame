//-----------------------------------------------------------------------
// <copyright file="MessageEventArgs.cs" company="Lukas Handler">
//     Lukas Handler
// </copyright>
// <summary>
// This file represents the default message event arguments. This is used if the message does not contain any additional parameters. 
// </summary>
//-----------------------------------------------------------------------
namespace Shared.Data.EventArguments
{
    using System;
    using Shared.Data.Messages;

    /// <summary>
    /// This class represents the default message event arguments. 
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class MessageEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageEventArgs"/> class.
        /// </summary>
        /// <param name="messageContent">Content of the message.</param>
        public MessageEventArgs(Message messageContent)
        {
            this.Message = messageContent;
        }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public Message Message { get; set; }
    }
}
