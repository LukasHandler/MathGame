//-----------------------------------------------------------------------
// <copyright file="Server.cs" company="Lukas Handler">
//     Lukas Handler
// </copyright>
// <summary>
// This file represents the server.
// </summary>
//-----------------------------------------------------------------------
namespace Server.Application
{
    using System.Collections.Generic;

    /// <summary>
    /// This class represents the server.
    /// </summary>
    /// <seealso cref="Server.Application.SystemElement" />
    public class Server : SystemElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Server"/> class.
        /// </summary>
        /// <param name="name">The name of the server.</param>
        /// <param name="targetInformation">The target information.</param>
        public Server(string name, object targetInformation)
        {
            this.Name = name;
            this.TargetInformation = new List<object>();
            this.TargetInformation.Add(targetInformation);
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name of the server.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the target information.
        /// </summary>
        /// <value>
        /// The target information.
        /// </value>
        public List<object> TargetInformation { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.Name;
        }
    }
}
