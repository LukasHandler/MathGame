using Shared.Data.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Shared.Data;
using Shared.Data.Managers;
using Shared.Data.EventArguments;

namespace Client.Application
{
    public class NetworkService
    {
        public EventHandler OnConnectionAccepted;

        public EventHandler OnConnectionDenied;

        public EventHandler<QuestionEventArgs> OnQuestionReceived;

        public EventHandler<GameFinishedEventArgs> OnGameWon;

        public EventHandler<GameFinishedEventArgs> OnGameLost;

        public EventHandler<ScoresEventArgs> OnScoresReceived;

        private UdpClientManager networkManager;

        private MessageProcessor messageProcessor;

        private IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4712);

        private IPEndPoint serverEndPoint;

        private string clientName;

        public NetworkService()
        {
            messageProcessor = new MessageProcessor();

            networkManager = new UdpClientManager();
            networkManager.OnDataReceived += messageProcessor.DataReceived;

            messageProcessor.OnConnectionAccepted += delegate (object sender, MessageEventArgs args)
            {
                if (OnConnectionAccepted != null)
                {
                    OnConnectionAccepted(this, EventArgs.Empty);
                }
            };

            messageProcessor.OnConnectionDenied += delegate (object sender, MessageEventArgs args)
            {
                if (OnConnectionAccepted != null)
                {
                    OnConnectionDenied(this, EventArgs.Empty);
                }
            };

            messageProcessor.OnQuestion += delegate (object sender, MessageEventArgs args)
            {
                if (OnQuestionReceived != null)
                {
                    QuestionMessage questionMessage = args.MessageContent as QuestionMessage;
                    this.OnQuestionReceived(this, new QuestionEventArgs(questionMessage.QuestionText, questionMessage.Time, questionMessage.Score));
                }
            };

            messageProcessor.OnGameWonMessage += delegate (object sender, MessageEventArgs args)
            {
                if (this.OnGameWon != null)
                {
                    this.OnGameWon(this, new GameFinishedEventArgs((args.MessageContent as GameWonMessage).Score));
                }
            };

            messageProcessor.OnGameLostMessage += delegate (object sender, MessageEventArgs args)
            {
                if (this.OnGameLost != null)
                {
                    this.OnGameLost(this, new GameFinishedEventArgs((args.MessageContent as GameLostMessage).Score));
                }
            };

            messageProcessor.OnScoreResponse += delegate (object sender, MessageEventArgs args)
            {
                if (this.OnScoresReceived != null)
                {
                    this.OnScoresReceived(this, new ScoresEventArgs(((ScoresResponseMessage)args.MessageContent).Scores));
                }
            };
        }

        public void Connect(IPEndPoint server, string playerName)
        {
            this.clientName = playerName;
            serverEndPoint = server;

            ConnectionRequestClientMessage request = new ConnectionRequestClientMessage();

            Send(request);
        }

        public void SubmitAnswer(int answer)
        {
            AnswerMessage answerMessage = new AnswerMessage()
            {
                Solution = answer
            };

            Send(answerMessage);
        }

        public void GetScores()
        {
            ScoresRequestMessage scoresRequest = new ScoresRequestMessage()
            {
            };

            Send(scoresRequest);
        }

        private void Send(Message request)
        {
            request.SenderName = this.clientName;
            networkManager.WriteData(request, serverEndPoint);
        }

        public void Disconnect()
        {
            DisconnectMessage disconnectMessage = new DisconnectMessage()
            {
            };

            Send(disconnectMessage);
        }
    }
}
