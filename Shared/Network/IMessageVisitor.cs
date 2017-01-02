//-----------------------------------------------------------------------
// <copyright file="IMessageVisitor.cs" company="Lukas Handler">
//     Lukas Handler
// </copyright>
// <summary>
// This file represents the visitor to fire events for the different message types.
// </summary>
//-----------------------------------------------------------------------
namespace Shared.Data
{
    using Shared.Data.Messages;

    /// <summary>
    /// This class represents the visitor to fire events for the different message types.
    /// </summary>
    public interface IMessageVisitor
    {
        /// <summary>
        /// Processes the message.
        /// </summary>
        /// <param name="message">The message.</param>
        void ProcessMessage(BroadcastRequestMessage message);

        /// <summary>
        /// Processes the message.
        /// </summary>
        /// <param name="message">The message.</param>
        void ProcessMessage(BroadcastResponseMessage message);

        /// <summary>
        /// Processes the message.
        /// </summary>
        /// <param name="message">The message.</param>
        void ProcessMessage(BroadcastMessage message);

        /// <summary>
        /// Processes the message.
        /// </summary>
        /// <param name="message">The message.</param>
        void ProcessMessage(ServerScoreRequestMessage message);

        /// <summary>
        /// Processes the message.
        /// </summary>
        /// <param name="message">The message.</param>
        void ProcessMessage(ServerScoreResponseMessage message);

        /// <summary>
        /// Processes the message.
        /// </summary>
        /// <param name="message">The message.</param>
        void ProcessMessage(AnswerMessage message);

        /// <summary>
        /// Processes the message.
        /// </summary>
        /// <param name="message">The message.</param>
        void ProcessMessage(DisconnectServerMessage message);

        /// <summary>
        /// Processes the message.
        /// </summary>
        /// <param name="message">The message.</param>
        void ProcessMessage(ConnectionAcceptMessage message);

        /// <summary>
        /// Processes the message.
        /// </summary>
        /// <param name="connectionAcceptServerMessage">The connection accept server message.</param>
        void ProcessMessage(ConnectionAcceptServerMessage connectionAcceptServerMessage);

        /// <summary>
        /// Processes the message.
        /// </summary>
        /// <param name="message">The message.</param>
        void ProcessMessage(ConnectionDeniedMessage message);

        /// <summary>
        /// Processes the message.
        /// </summary>
        /// <param name="message">The message.</param>
        void ProcessMessage(ConnectionRequestClientMessage message);

        /// <summary>
        /// Processes the message.
        /// </summary>
        /// <param name="message">The message.</param>
        void ProcessMessage(ConnectionRequestServerMessage message);

        /// <summary>
        /// Processes the message.
        /// </summary>
        /// <param name="message">The message.</param>
        void ProcessMessage(ConnectionRequestMonitorMessage message);

        /// <summary>
        /// Processes the message.
        /// </summary>
        /// <param name="message">The message.</param>
        void ProcessMessage(ScoresRequestMessage message);

        /// <summary>
        /// Processes the message.
        /// </summary>
        /// <param name="message">The message.</param>
        void ProcessMessage(ScoresResponseMessage message);

        /// <summary>
        /// Processes the message.
        /// </summary>
        /// <param name="message">The message.</param>
        void ProcessMessage(QuestionMessage message);

        /// <summary>
        /// Processes the message.
        /// </summary>
        /// <param name="message">The message.</param>
        void ProcessMessage(DisconnectClientMessage message);

        /// <summary>
        /// Processes the message.
        /// </summary>
        /// <param name="message">The message.</param>
        void ProcessMessage(DisconnectMonitorMessage message);

        /// <summary>
        /// Processes the message.
        /// </summary>
        /// <param name="message">The message.</param>
        void ProcessMessage(LoggingMessage message);

        /// <summary>
        /// Processes the message.
        /// </summary>
        /// <param name="message">The message.</param>
        void ProcessMessage(GameLostMessage message);

        /// <summary>
        /// Processes the message.
        /// </summary>
        /// <param name="message">The message.</param>
        void ProcessMessage(GameWonMessage message);

        /// <summary>
        /// Processes the message.
        /// </summary>
        /// <param name="message">The message.</param>
        void ProcessMessage(ForwardingMessage message);
    }
}
