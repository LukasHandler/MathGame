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
using Client.Application.EventArguments;

namespace Client.Application
{
    public class DataService
    {
        public EventHandler OnConnectionAccepted;

        public EventHandler OnConnectionDenied;

        public EventHandler<QuestionEventArgs> OnQuestionReceived;

        public EventHandler<GameFinishedEventArgs> OnGameWon;

        public EventHandler<GameFinishedEventArgs> OnGameLost;

        public EventHandler<ScoresEventArgs> OnScoresReceived;

        public EventHandler<BroadcastEventArgs> OnBroadcastTextReceived;

        private IDataManager networkManager;

        private MessageProcessor messageProcessor;

        private IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4712);

        private object serverEndPoint;

        private string clientName;

        public DataService()
        {
            messageProcessor = new MessageProcessor();

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

            messageProcessor.OnQuestion += delegate (object sender, QuestionMessageEventArgs args)
            {
                if (OnQuestionReceived != null)
                {
                    QuestionMessage questionMessage = args.Message;
                    this.OnQuestionReceived(this, new QuestionEventArgs(questionMessage.QuestionText, questionMessage.Time, questionMessage.Score));
                }
            };

            messageProcessor.OnGameWonMessage += delegate (object sender, GameWonMessageEventArgs args)
            {
                if (this.OnGameWon != null)
                {
                    this.OnGameWon(this, new GameFinishedEventArgs(args.Message.Score));
                }
            };

            messageProcessor.OnGameLostMessage += delegate (object sender, GameLostMessageEventArgs args)
            {
                if (this.OnGameLost != null)
                {
                    this.OnGameLost(this, new GameFinishedEventArgs(args.Message.Score));
                }
            };

            messageProcessor.OnScoreResponse += delegate (object sender, ScoresResponseMessageEventArgs args)
            {
                if (this.OnScoresReceived != null)
                {
                    this.OnScoresReceived(this, new ScoresEventArgs(args.Message.Scores));
                }
            };

            messageProcessor.OnBroadcastMessage += delegate (object sender, BroadcastMessageEventArgs args)
            {
                if (this.OnBroadcastTextReceived != null)
                {
                    this.OnBroadcastTextReceived(this, new BroadcastEventArgs((args.Message).Text));
                }
            };
        }

        public void Connect(object server, string playerName, bool isNamedPipes)
        {
            if (isNamedPipes)
            {
                networkManager = new NamedPipeManager();
            }
            else
            {
                networkManager = new UdpClientManager();
            }

            networkManager.OnDataReceived += messageProcessor.DataReceived;

            this.clientName = playerName;
            serverEndPoint = server;

            ConnectionRequestClientMessage request = new ConnectionRequestClientMessage() { SenderName = this.clientName };

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
            networkManager.WriteData(request, serverEndPoint);
        }

        public void Disconnect()
        {
            DisconnectClientMessage disconnectMessage = new DisconnectClientMessage()
            {
            };

            Send(disconnectMessage);
        }
    }
}
