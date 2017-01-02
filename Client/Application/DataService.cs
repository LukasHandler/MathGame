//-----------------------------------------------------------------------
// <copyright file="DataService.cs" company="Lukas Handler">
//     Lukas Handler
// </copyright>
// <summary>
// This file contains the logic for the client.
// </summary>
//-----------------------------------------------------------------------
namespace Client.Application
{
    using System;
    using EventArguments;
    using Shared.Data;
    using Shared.Data.EventArguments;
    using Shared.Data.Managers;
    using Shared.Data.Messages;

    /// <summary>
    /// This class contains the logic for the client.
    /// </summary>
    public class DataService
    {
        /// <summary>
        /// The data manager for sending and receiving data.
        /// </summary>
        private IDataManager dataManager;

        /// <summary>
        /// The message processor, which gets connected to the data manager when a connection request gets sent.
        /// </summary>
        private MessageProcessor messageProcessor;

        /// <summary>
        /// The server target information, needed for sending data.
        /// </summary>
        private object serverTargetInformation;

        /// <summary>
        /// The client name.
        /// </summary>
        private string clientName;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataService"/> class.
        /// </summary>
        public DataService()
        {
            this.messageProcessor = new MessageProcessor();

            this.messageProcessor.OnConnectionAccepted += delegate(object sender, MessageEventArgs args)
            {
                if (this.OnConnectionAccepted != null)
                {
                    this.OnConnectionAccepted(this, EventArgs.Empty);
                }
            };

            this.messageProcessor.OnConnectionDenied += delegate(object sender, MessageEventArgs args)
            {
                if (this.OnConnectionAccepted != null)
                {
                    this.OnConnectionDenied(this, EventArgs.Empty);
                }
            };

            this.messageProcessor.OnQuestion += delegate(object sender, QuestionMessageEventArgs args)
            {
                if (this.OnQuestionReceived != null)
                {
                    QuestionMessage questionMessage = args.Message;
                    this.OnQuestionReceived(this, new QuestionEventArgs(questionMessage.QuestionText, questionMessage.Time, questionMessage.Score));
                }
            };

            this.messageProcessor.OnGameWonMessage += delegate(object sender, GameWonMessageEventArgs args)
            {
                if (this.OnGameWon != null)
                {
                    this.OnGameWon(this, new GameFinishedEventArgs(args.Message.Score));
                }
            };

            this.messageProcessor.OnGameLostMessage += delegate(object sender, GameLostMessageEventArgs args)
            {
                if (this.OnGameLost != null)
                {
                    this.OnGameLost(this, new GameFinishedEventArgs(args.Message.Score));
                }
            };

            this.messageProcessor.OnScoreResponse += delegate(object sender, ScoresResponseMessageEventArgs args)
            {
                if (this.OnScoresReceived != null)
                {
                    this.OnScoresReceived(this, new ScoresEventArgs(args.Message.Scores));
                }
            };

            this.messageProcessor.OnBroadcastMessage += delegate(object sender, BroadcastMessageEventArgs args)
            {
                if (this.OnBroadcastTextReceived != null)
                {
                    this.OnBroadcastTextReceived(this, new BroadcastEventArgs(args.Message.Text));
                }
            };
        }

        /// <summary>
        /// Gets or sets the event which gets fired when the server accepted a connection.
        /// </summary>
        /// <value>
        /// The event which gets fired when the server accepted a connection.
        /// </value>
        public EventHandler OnConnectionAccepted { get; set; }

        /// <summary>
        /// Gets or sets the event which gets fired when the server denied a connection.
        /// </summary>
        /// <value>
        /// The event which gets fired when the server denied a connection.
        /// </value>
        public EventHandler OnConnectionDenied { get; set; }

        /// <summary>
        /// Gets or sets the event which gets fired when the client received a question.
        /// </summary>
        /// <value>
        /// The event which gets fired when the client received a question
        /// </value>
        public EventHandler<QuestionEventArgs> OnQuestionReceived { get; set; }

        /// <summary>
        /// Gets or sets the event which gets fired when the client won.
        /// </summary>
        /// <value>
        /// The event which gets fired when the client won.
        /// </value>
        public EventHandler<GameFinishedEventArgs> OnGameWon { get; set; }

        /// <summary>
        /// Gets or sets the event which gets fired when the client lost.
        /// </summary>
        /// <value>
        /// The event which gets fired when the client lost.
        /// </value>
        public EventHandler<GameFinishedEventArgs> OnGameLost { get; set; }

        /// <summary>
        /// Gets or sets the event which gets fired when the client received scores.
        /// </summary>
        /// <value>
        /// The event which gets fired when the client received scores.
        /// </value>
        public EventHandler<ScoresEventArgs> OnScoresReceived { get; set; }

        /// <summary>
        /// Gets or sets the event which gets fired when the client received a broadcast message.
        /// </summary>
        /// <value>
        /// The event which gets fired when the client received a broadcast message.
        /// </value>
        public EventHandler<BroadcastEventArgs> OnBroadcastTextReceived { get; set; }

        /// <summary>
        /// Connects to the server.
        /// </summary>
        /// <param name="serverTargetInformation">The server target information.</param>
        /// <param name="clientName">Name of the client.</param>
        /// <param name="isNamedPipes">Indicates if named pipes or UDP should be used for connection.</param>
        public void Connect(object serverTargetInformation, string clientName, bool isNamedPipes)
        {
            if (isNamedPipes)
            {
                this.dataManager = new NamedPipeManager();
            }
            else
            {
                this.dataManager = new UdpClientManager();
            }

            this.dataManager.OnDataReceived += this.messageProcessor.DataReceived;
            this.clientName = clientName;
            this.serverTargetInformation = serverTargetInformation;
            ConnectionRequestClientMessage request = new ConnectionRequestClientMessage() { SenderName = this.clientName };
            this.Send(request);
        }

        /// <summary>
        /// Submits the answer.
        /// </summary>
        /// <param name="answer">The answer.</param>
        public void SubmitAnswer(int answer)
        {
            AnswerMessage answerMessage = new AnswerMessage()
            {
                Solution = answer
            };
            this.Send(answerMessage);
        }

        /// <summary>
        /// Gets the scores.
        /// </summary>
        public void GetScores()
        {
            ScoresRequestMessage scoresRequest = new ScoresRequestMessage();
            this.Send(scoresRequest);
        }

        /// <summary>
        /// Disconnects from the server.
        /// </summary>
        public void Disconnect()
        {
            DisconnectClientMessage disconnectMessage = new DisconnectClientMessage();
            this.Send(disconnectMessage);
        }

        /// <summary>
        /// Sends the specified message.
        /// </summary>
        /// <param name="message">The message to send.</param>
        private void Send(Message message)
        {
            this.dataManager.WriteData(message, this.serverTargetInformation);
        }
    }
}
