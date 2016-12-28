//-----------------------------------------------------------------------
// <copyright file="SystemElement.cs" company="Lukas Handler">
//     Lukas Handler
// </copyright>
// <summary>
// This file represents the system element. Used for allowing methods to use system elements to allow all three subtypes.
// </summary>
//-----------------------------------------------------------------------
namespace Server.Application
{
    /// <summary>
    /// This class represents the system element.
    /// </summary>
    public abstract class SystemElement
    {
        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public abstract override string ToString();
    }
}
