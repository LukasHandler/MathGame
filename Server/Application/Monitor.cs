//-----------------------------------------------------------------------
// <copyright file="Monitor.cs" company="Lukas Handler">
//     Lukas Handler
// </copyright>
// <summary>
// This file represents the monitor.
// </summary>
//-----------------------------------------------------------------------
namespace Server.Application
{
    /// <summary>
    /// This class represents the monitor.
    /// </summary>
    /// <seealso cref="Server.Application.SystemElement" />
    public class Monitor : SystemElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Monitor"/> class.
        /// </summary>
        /// <param name="targetInformation">The target information.</param>
        public Monitor(object targetInformation)
        {
            this.TargetInformation = targetInformation;
        }

        /// <summary>
        /// Gets or sets the target information.
        /// </summary>
        /// <value>
        /// The target information.
        /// </value>
        public object TargetInformation { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("Monitor ({0})", this.TargetInformation.ToString());
        }
    }
}
