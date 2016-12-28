//-----------------------------------------------------------------------
// <copyright file="ServerConfiguration.cs" company="Lukas Handler">
//     Lukas Handler
// </copyright>
// <summary>
// This file represents the server configuration.
// </summary>
//-----------------------------------------------------------------------
namespace Server.Application
{
    /// <summary>
    /// This class represents the server configuration.
    /// </summary>
    public class ServerConfiguration
    {
        /// <summary>
        /// Gets or sets the name of the server.
        /// </summary>
        /// <value>
        /// The name of the server.
        /// </value>
        public string ServerName { get; set; }

        /// <summary>
        /// Gets or sets the incoming client port.
        /// </summary>
        /// <value>
        /// The incoming client server port.
        /// </value>
        public int ClientPort { get; set; }

        /// <summary>
        /// Gets or sets the incoming monitor port.
        /// </summary>
        /// <value>
        /// The incoming monitor port.
        /// </value>
        public int MonitorPort { get; set; }

        /// <summary>
        /// Gets or sets the incoming server port.
        /// </summary>
        /// <value>
        /// The incoming server port.
        /// </value>
        public int ServerPort { get; set; }

        /// <summary>
        /// Gets or sets the maximum score.
        /// </summary>
        /// <value>
        /// The maximum score.
        /// </value>
        public int MaxScore { get; set; }

        /// <summary>
        /// Gets or sets the minimum score.
        /// </summary>
        /// <value>
        /// The minimum score.
        /// </value>
        public int MinScore { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use named pipes or UDP for client to server connection.
        /// </summary>
        /// <value>
        ///   <c>true</c> if named pipes are used; otherwise, <c>false</c>.
        /// </value>
        public bool UseNamedPipes { get; set; }
    }
}
