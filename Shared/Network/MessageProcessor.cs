//-----------------------------------------------------------------------
// <copyright file="MessageProcessor.cs" company="Lukas Handler">
//     Lukas Handler
// </copyright>
// <summary>
// This file implements the concrete visitor for message processing.
// </summary>
//-----------------------------------------------------------------------
namespace Shared.Data
{
    using System;
    using System.Collections.Generic;
    using EventArguments;
    using Messages;

    /// <summary>
    /// This class implements the concrete visitor for message processing.
    /// </summary>
    /// <seealso cref="Shared.Data.IMessageVisitor" />
    public class MessageProcessor : IMessageVisitor
    {
        /// <summary>
        /// The sender information, saved here so it doesn't get lost during the visiting process.
        /// </summary>
        private Dictionary<Message, object> senderInformation;

        /// <summary>
        /// The locker needed for synchronizing the access on the dictionary.
        /// </summary>
        private object locker;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageProcessor"/> class.
        /// </summary>
        public MessageProcessor()
        {
            this.senderInformation = new Dictionary<Message, object>();
            this.locker = new object();
        }

        /// <summary>
        /// Gets or sets the on connection request client event.
        /// </summary>
        /// <value>
        /// The on connection request client event.
        /// </value>
        public EventHandler<ConnectionRequestClientMessageEventArgs> OnConnectionRequestClient { get; set; }

        /// <summary>
        /// Gets or sets the on connection request server event.
        /// </summary>
        /// <value>
        /// The on connection request server event.
        /// </value>
        public EventHandler<ConnectionRequestServerMessageEventArgs> OnConnectionRequestServer { get; set; }

        /// <summary>
        /// Gets or sets the on connection request monitor event.
        /// </summary>
        /// <value>
        /// The on connection request monitor event.
        /// </value>
        public EventHandler<MessageEventArgs> OnConnectionRequestMonitor { get; set; }

        /// <summary>
        /// Gets or sets the on connection accepted server event.
        /// </summary>
        /// <value>
        /// The on connection accepted server event.
        /// </value>
        public EventHandler<ConnectionAcceptedServerMessageEventArgs> OnConnectionAcceptedServer { get; set; }

        /// <summary>
        /// Gets or sets the on score request event.
        /// </summary>
        /// <value>
        /// The on score request event.
        /// </value>
        public EventHandler<MessageEventArgs> OnScoreRequest { get; set; }

        /// <summary>
        /// Gets or sets the on score response event.
        /// </summary>
        /// <value>
        /// The on score response event.
        /// </value>
        public EventHandler<ScoresResponseMessageEventArgs> OnScoreResponse { get; set; }

        /// <summary>
        /// Gets or sets the on answer event.
        /// </summary>
        /// <value>
        /// The on answer event.
        /// </value>
        public EventHandler<AnswerMessageEventArgs> OnAnswer { get; set; }

        /// <summary>
        /// Gets or sets the on connection denied event.
        /// </summary>
        /// <value>
        /// The on connection denied event.
        /// </value>
        public EventHandler<MessageEventArgs> OnConnectionDenied { get; set; }

        /// <summary>
        /// Gets or sets the on connection accepted event.
        /// </summary>
        /// <value>
        /// The on connection accepted event.
        /// </value>
        public EventHandler<MessageEventArgs> OnConnectionAccepted { get; set; }

        /// <summary>
        /// Gets or sets the on question event.
        /// </summary>
        /// <value>
        /// The on question event.
        /// </value>
        public EventHandler<QuestionMessageEventArgs> OnQuestion { get; set; }

        /// <summary>
        /// Gets or sets the on disconnect client event.
        /// </summary>
        /// <value>
        /// The on disconnect client event.
        /// </value>
        public EventHandler<MessageEventArgs> OnDisconnectClient { get; set; }

        /// <summary>
        /// Gets or sets the on disconnect monitor event.
        /// </summary>
        /// <value>
        /// The on disconnect monitor event.
        /// </value>
        public EventHandler<MessageEventArgs> OnDisconnectMonitor { get; set; }

        /// <summary>
        /// Gets or sets the on disconnect server event.
        /// </summary>
        /// <value>
        /// The on disconnect server event.
        /// </value>
        public EventHandler<DisconnectServerMessageEventArgs> OnDisconnectServer { get; set; }

        /// <summary>
        /// Gets or sets the on logging message event.
        /// </summary>
        /// <value>
        /// The on logging message event.
        /// </value>
        public EventHandler<LoggingMessageEventArgs> OnLoggingMessage { get; set; }

        /// <summary>
        /// Gets or sets the on game won message event.
        /// </summary>
        /// <value>
        /// The on game won message event.
        /// </value>
        public EventHandler<GameWonMessageEventArgs> OnGameWonMessage { get; set; }

        /// <summary>
        /// Gets or sets the on game lost message event.
        /// </summary>
        /// <value>
        /// The on game lost message event.
        /// </value>
        public EventHandler<GameLostMessageEventArgs> OnGameLostMessage { get; set; }

        /// <summary>
        /// Gets or sets the on forwarding message event.
        /// </summary>
        /// <value>
        /// The on forwarding message event.
        /// </value>
        public EventHandler<ForwardingMessageEventArgs> OnForwardingMessage { get; set; }

        /// <summary>
        /// Gets or sets the on server score request message event.
        /// </summary>
        /// <value>
        /// The on server score request message event.
        /// </value>
        public EventHandler<ServerScoreRequestMessageEventArgs> OnServerScoreRequestMessage { get; set; }

        /// <summary>
        /// Gets or sets the on server score response message event.
        /// </summary>
        /// <value>
        /// The on server score response message event.
        /// </value>
        public EventHandler<ServerScoreResponseMessageEventArgs> OnServerScoreResponseMessage { get; set; }

        /// <summary>
        /// Gets or sets the on broadcast message event.
        /// </summary>
        /// <value>
        /// The on broadcast message event.
        /// </value>
        public EventHandler<BroadcastMessageEventArgs> OnBroadcastMessage { get; set; }

        /// <summary>
        /// Gets or sets the on server clients request message event.
        /// </summary>
        /// <value>
        /// The on server clients request message event.
        /// </value>
        public EventHandler<BroadcastRequestMessageEventArgs> OnServerClientsRequestMessage { get; set; }

        /// <summary>
        /// Gets or sets the on server clients response message event.
        /// </summary>
        /// <value>
        /// The on server clients response message event.
        /// </value>
        public EventHandler<BroadcastResponseMessageEventArgs> OnServerClientsResponseMessage { get; set; }

        /// <summary>
        /// Processes the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void ProcessMessage(BroadcastMessage message)
        {
            this.OnBroadcastMessage?.Invoke(this.senderInformation[message], new BroadcastMessageEventArgs(message));
        }

        /// <summary>
        /// Processes the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void ProcessMessage(BroadcastRequestMessage message)
        {
            this.OnServerClientsRequestMessage?.Invoke(this.senderInformation[message], new BroadcastRequestMessageEventArgs(message));
        }

        /// <summary>
        /// Processes the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void ProcessMessage(BroadcastResponseMessage message)
        {
            this.OnServerClientsResponseMessage?.Invoke(this.senderInformation[message], new BroadcastResponseMessageEventArgs(message));
        }

        /// <summary>
        /// Processes the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void ProcessMessage(ConnectionAcceptServerMessage message)
        {
            this.OnConnectionAcceptedServer?.Invoke(this.senderInformation[message], new ConnectionAcceptedServerMessageEventArgs(message));
        }

        /// <summary>
        /// Processes the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void ProcessMessage(ScoresRequestMessage message)
        {
            this.OnScoreRequest?.Invoke(this.senderInformation[message], new MessageEventArgs(message));
        }

        /// <summary>
        /// Processes the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void ProcessMessage(ConnectionRequestClientMessage message)
        {
            this.OnConnectionRequestClient?.Invoke(this.senderInformation[message], new ConnectionRequestClientMessageEventArgs(message));
        }

        /// <summary>
        /// Processes the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void ProcessMessage(ConnectionRequestServerMessage message)
        {
            this.OnConnectionRequestServer?.Invoke(this.senderInformation[message], new ConnectionRequestServerMessageEventArgs(message));
        }

        /// <summary>
        /// Processes the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void ProcessMessage(ConnectionRequestMonitorMessage message)
        {
            this.OnConnectionRequestMonitor?.Invoke(this.senderInformation[message], new MessageEventArgs(message));
        }

        /// <summary>
        /// Processes the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void ProcessMessage(AnswerMessage message)
        {
            this.OnAnswer?.Invoke(this.senderInformation[message], new AnswerMessageEventArgs(message));
        }

        /// <summary>
        /// Processes the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void ProcessMessage(ConnectionDeniedMessage message)
        {
            this.OnConnectionDenied?.Invoke(this.senderInformation[message], new MessageEventArgs(message));
        }

        /// <summary>
        /// Processes the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void ProcessMessage(QuestionMessage message)
        {
            this.OnQuestion?.Invoke(this.senderInformation[message], new QuestionMessageEventArgs(message));
        }

        /// <summary>
        /// Processes the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void ProcessMessage(ScoresResponseMessage message)
        {
            this.OnScoreResponse?.Invoke(this.senderInformation[message], new ScoresResponseMessageEventArgs(message));
        }

        /// <summary>
        /// Processes the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void ProcessMessage(ConnectionAcceptMessage message)
        {
            this.OnConnectionAccepted?.Invoke(this.senderInformation[message], new MessageEventArgs(message));
        }

        /// <summary>
        /// Processes the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void ProcessMessage(DisconnectClientMessage message)
        {
            this.OnDisconnectClient?.Invoke(this.senderInformation[message], new MessageEventArgs(message));
        }

        /// <summary>
        /// Processes the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void ProcessMessage(DisconnectMonitorMessage message)
        {
            this.OnDisconnectMonitor?.Invoke(this.senderInformation[message], new MessageEventArgs(message));
        }

        /// <summary>
        /// Processes the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void ProcessMessage(DisconnectServerMessage message)
        {
            this.OnDisconnectServer?.Invoke(this.senderInformation[message], new DisconnectServerMessageEventArgs(message));
        }

        /// <summary>
        /// Processes the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void ProcessMessage(LoggingMessage message)
        {
            this.OnLoggingMessage?.Invoke(this.senderInformation[message], new LoggingMessageEventArgs(message));
        }

        /// <summary>
        /// Processes the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void ProcessMessage(GameWonMessage message)
        {
            this.OnGameWonMessage?.Invoke(this.senderInformation[message], new GameWonMessageEventArgs(message));
        }

        /// <summary>
        /// Processes the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void ProcessMessage(GameLostMessage message)
        {
            this.OnGameLostMessage?.Invoke(this.senderInformation[message], new GameLostMessageEventArgs(message));
        }

        /// <summary>
        /// Processes the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void ProcessMessage(ForwardingMessage message)
        {
            this.OnForwardingMessage?.Invoke(this.senderInformation[message], new ForwardingMessageEventArgs(message));
        }

        /// <summary>
        /// Processes the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void ProcessMessage(ServerScoreResponseMessage message)
        {
            this.OnServerScoreResponseMessage?.Invoke(this.senderInformation[message], new ServerScoreResponseMessageEventArgs(message));
        }

        /// <summary>
        /// Processes the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void ProcessMessage(ServerScoreRequestMessage message)
        {
            this.OnServerScoreRequestMessage?.Invoke(this.senderInformation[message], new ServerScoreRequestMessageEventArgs(message));
        }

        /// <summary>
        /// This gets called whenever data was received. Uses the visitor.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">The <see cref="MessageEventArgs"/> instance containing the event data.</param>
        public void DataReceived(object sender, MessageEventArgs eventArgs)
        {
            lock (this.locker)
            {
                this.senderInformation.Add(eventArgs.Message, sender);
            }

            eventArgs.Message.ProcessMessage(this);

            lock (this.locker)
            {
                this.senderInformation.Remove(eventArgs.Message);
            }
        }
    }
}
