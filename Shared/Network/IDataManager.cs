//-----------------------------------------------------------------------
// <copyright file="IDataManager.cs" company="Lukas Handler">
//     Lukas Handler
// </copyright>
// <summary>
// This file represents the data manger interface.
// </summary>
//-----------------------------------------------------------------------
namespace Shared.Data
{
    using System;
    using EventArguments;
    using Messages;

    /// <summary>
    /// This class represents the data manager interface.
    /// </summary>
    public interface IDataManager
    {
        /// <summary>
        /// Occurs when the manager received data.
        /// </summary>
        event EventHandler<MessageEventArgs> OnDataReceived;

        /// <summary>
        /// Writes the data.
        /// </summary>
        /// <param name="data">The data to send.</param>
        /// <param name="target">The target.</param>
        void WriteData(Message data, object target);

        /// <summary>
        /// Registers to the specified target.
        /// </summary>
        /// <param name="target">The target.</param>
        void Register(object target);

        /// <summary>
        /// Unregisters from the specified target.
        /// </summary>
        /// <param name="target">The target.</param>
        void Unregister(object target);
    }
}
