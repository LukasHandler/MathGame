//-----------------------------------------------------------------------
// <copyright file="PortException.cs" company="Lukas Handler">
//     Lukas Handler
// </copyright>
// <summary>
// This file implements an exception which gets used when a port is already in use.
// </summary>
//-----------------------------------------------------------------------
namespace Server.Application.Exceptions
{
    using System;

    /// <summary>
    /// This class implements an exception which gets used when a port is already in use.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class PortException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PortException"/> class.
        /// </summary>
        /// <param name="port">The port which is already in use.</param>
        public PortException(int port) : base(string.Format("There is already a server running on port {0}", port))
        {
        }
    }
}
