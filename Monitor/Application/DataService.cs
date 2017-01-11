//-----------------------------------------------------------------------
// <copyright file="DataService.cs" company="Lukas Handler">
//     Lukas Handler
// </copyright>
// <summary>
// This file contains the logic for the monitor.
// </summary>
//-----------------------------------------------------------------------
namespace Monitor.Application
{
    using System;
    using EventArguments;
    using Shared.Data;
    using Shared.Data.EventArguments;
    using Shared.Data.Managers;
    using Shared.Data.Messages;

    /// <summary>
    /// This class contains the logic for the monitor.
    /// </summary>
    public class DataService
    {
        /// <summary>
        /// The client data manager, which is responsible for sending and receiving messages.
        /// </summary>
        private IDataManager clientDataManager;

        /// <summary>
        /// The server target information.
        /// </summary>
        private object serverTargetInformation;

        /// <summary>
        /// Indicates if the monitor is connected.
        /// </summary>
        private bool isConnected = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataService"/> class.
        /// </summary>
        public DataService()
        {
            var messageProcessor = new MessageProcessor();
            messageProcessor.OnConnectionAccepted += this.ConnectionAccepted;
            messageProcessor.OnLoggingMessage += this.ReceivedLoggingMessage;
            this.clientDataManager = new TcpClientManager();
            this.clientDataManager.OnDataReceived += messageProcessor.DataReceived;
        }

        /// <summary>
        /// Gets or sets the on logging data received event.
        /// </summary>
        /// <value>
        /// The on logging data received event.
        /// </value>
        public EventHandler<LoggingEventArgs> OnLoggingDataReceived { get; set; }

        /// <summary>
        /// Gets or sets the on connection created event.
        /// </summary>
        /// <value>
        /// The on connection created event.
        /// </value>
        public EventHandler OnConnectionCreated { get; set; }

        /// <summary>
        /// Registers to the specified server target.
        /// </summary>
        /// <param name="serverTargetInformation">The server target information.</param>
        public void Register(object serverTargetInformation)
        {
            this.serverTargetInformation = serverTargetInformation;
            this.clientDataManager.Register(this.serverTargetInformation);
            ConnectionRequestMonitorMessage requestMessage = new ConnectionRequestMonitorMessage();
            this.clientDataManager.WriteData(requestMessage, this.serverTargetInformation);
        }

        /// <summary>
        /// Unregisters from the server.
        /// </summary>
        public void Unregister()
        {
            if (this.isConnected)
            {
                DisconnectClientMessage disconnectMessage = new DisconnectClientMessage();
                this.clientDataManager.WriteData(disconnectMessage, this.serverTargetInformation);
                this.clientDataManager.Unregister(this.serverTargetInformation);
            }
        }

        /// <summary>
        /// Gets executed when a logging message was received.
        /// </summary>
        /// <param name="sender">The sender target information.</param>
        /// <param name="eventArgs">The <see cref="LoggingMessageEventArgs"/> instance containing the event data.</param>
        private void ReceivedLoggingMessage(object sender, LoggingMessageEventArgs eventArgs)
        {
            if (this.OnLoggingDataReceived != null)
            {
                string loggingMessage = eventArgs.Message.Text;
                this.OnLoggingDataReceived(sender, new LoggingEventArgs(loggingMessage));
            }
        }

        /// <summary>
        /// Gets executed when the connection was accepted.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void ConnectionAccepted(object sender, EventArgs eventArgs)
        {
            this.isConnected = true;
            this.OnConnectionCreated?.Invoke(this, EventArgs.Empty);
        }
    }
}
