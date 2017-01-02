//-----------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="Lukas Handler">
//     Lukas Handler
// </copyright>
// <summary>
// This file represents the view model for the main window.
// </summary>
//-----------------------------------------------------------------------
namespace Client.Presentation.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Net;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Windows;
    using System.Windows.Data;
    using Client.Application;
    using Client.Application.EventArguments;
    using Shared.Data;
    using Shared.SharedPresentation.Utilities;

    /// <summary>
    /// This class represents the view model for the main window.
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public class MainViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// The questions lock, needed for updating the collection synchronized.
        /// </summary>
        private readonly object questionsLock = new object();

        /// <summary>
        /// The timer thread.
        /// </summary>
        private Thread timerThread;

        /// <summary>
        /// The data service from the application layer.
        /// </summary>
        private DataService dataService;

        /// <summary>
        /// The disconnect command.
        /// </summary>
        private RelayCommand disconnect;

        /// <summary>
        /// The connect command.
        /// </summary>
        private RelayCommand connect;

        /// <summary>
        /// The submit answer command.
        /// </summary>
        private RelayCommand submitAnswer;

        /// <summary>
        /// The get scores command.
        /// </summary>
        private RelayCommand getScores;

        /// <summary>
        /// The history contains previous messages.
        /// </summary>
        private ObservableCollection<string> history;

        /// <summary>
        /// The remaining time for answering the question.
        /// </summary>
        private int remainingTime;

        /// <summary>
        /// The current score.
        /// </summary>
        private int score;

        /// <summary>
        /// Indicates if the game is over.
        /// </summary>
        private bool isOver;

        /// <summary>
        /// The current question.
        /// </summary>
        private string question;

        /// <summary>
        /// Indicates if the client is connected.
        /// </summary>
        private bool isConnected;

        /// <summary>
        /// The scores.
        /// </summary>
        private List<ScoreEntry> scores;

        /// <summary>
        /// The server name.
        /// </summary>
        private string serverName;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        public MainViewModel()
        {
            this.PlayerName = "horst";
            this.ServerAddress = IPAddress.Parse("127.0.0.1");
            this.ServerPort = 4701;
            this.ServerName = "server1";
            this.History = new ObservableCollection<string>();
            this.Scores = new List<ScoreEntry>();

            this.dataService = new DataService();
            this.dataService.OnConnectionAccepted += delegate(object sender, EventArgs args)
            {
                this.IsConnected = true;
            };
            this.dataService.OnConnectionDenied += delegate(object sender, EventArgs args)
            {
                MessageBox.Show("Server denied the connection.");
                this.IsConnected = false;
            };
            this.dataService.OnQuestionReceived += delegate(object sender, QuestionEventArgs args)
            {
                this.History.Add(args.QuestionText);
                this.Question = args.QuestionText;
                this.RemainingTime = args.Time;
                this.Score = args.Score;

                ThreadStart timerThreadStart = new ThreadStart(this.UpdateQuestionTimer);

                if (this.timerThread != null)
                {
                    this.timerThread.Abort();
                }

                this.timerThread = new Thread(timerThreadStart);
                this.timerThread.Start();
            };
            this.dataService.OnGameWon += delegate(object sender, GameFinishedEventArgs args)
            {
                this.Score = args.Score;
                this.timerThread.Abort();
                this.RemainingTime = 0;
                MessageBox.Show("Congratulations, you won!");
                this.IsOver = true;
            };
            this.dataService.OnGameLost += delegate(object sender, GameFinishedEventArgs args)
            {
                this.Score = args.Score;
                this.timerThread.Abort();
                this.RemainingTime = 0;
                MessageBox.Show("You lost!");
                this.IsOver = true;
            };
            this.dataService.OnScoresReceived += delegate(object sender, ScoresEventArgs args)
            {
                this.Scores = args.Scores;
            };
            this.dataService.OnBroadcastTextReceived += delegate(object sender, BroadcastEventArgs args)
            {
                this.History.Add(args.BroadcastText);
            };
        }

        /// <summary>
        /// Occurs when property changed to notify UI.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the history (for previous messages).
        /// </summary>
        /// <value>
        /// The history.
        /// </value>
        public ObservableCollection<string> History
        {
            get
            {
                return this.history;
            }

            set
            {
                if (this.history != value)
                {
                    this.history = value;
                    this.OnPropertyChanged();

                    // Avoid problem when updating the observable collection from a different thread <see href="http://stackoverflow.com/questions/2104614/updating-an-observablecollection-in-a-separate-thread"/>
                    BindingOperations.EnableCollectionSynchronization(this.history, this.questionsLock);
                }
            }
        }

        /// <summary>
        /// Gets or sets the remaining time.
        /// </summary>
        /// <value>
        /// The remaining time.
        /// </value>
        public int RemainingTime
        {
            get
            {
                return this.remainingTime;
            }

            set
            {
                if (this.remainingTime != value)
                {
                    this.remainingTime = value;
                    this.OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the scores.
        /// </summary>
        /// <value>
        /// The scores.
        /// </value>
        public List<ScoreEntry> Scores
        {
            get
            {
                return this.scores;
            }

            set
            {
                if (this.scores != value)
                {
                    this.scores = value;
                    this.OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the game is over.
        /// </summary>
        /// <value>
        ///   <c>true</c> if game is over; otherwise, <c>false</c>.
        /// </value>
        public bool IsOver
        {
            get
            {
                return this.isOver;
            }

            set
            {
                if (this.isOver != value)
                {
                    this.isOver = value;
                    this.OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of the player.
        /// </summary>
        /// <value>
        /// The name of the player.
        /// </value>
        public string PlayerName { get; set; }

        /// <summary>
        /// Gets the question.
        /// </summary>
        /// <value>
        /// The question.
        /// </value>
        public string Question
        {
            get
            {
                return this.question;
            }

            private set
            {
                if (this.question != value)
                {
                    this.question = value;
                    this.OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the server address. For UDP.
        /// </summary>
        /// <value>
        /// The server address.
        /// </value>
        public IPAddress ServerAddress { get; set; }

        /// <summary>
        /// Gets or sets the server port. For UDP.
        /// </summary>
        /// <value>
        /// The server port.
        /// </value>
        public int ServerPort { get; set; }

        /// <summary>
        /// Gets or sets the name of the server. For named pipes.
        /// </summary>
        /// <value>
        /// The name of the server.
        /// </value>
        public string ServerName
        {
            get
            {
                return this.serverName;
            }

            set
            {
                if (this.serverName != value)
                {
                    this.serverName = value;
                    this.OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance should use named pipes connection.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance should use named pipes; otherwise, <c>false</c>.
        /// </value>
        public bool IsNamedPipes { get; set; }

        /// <summary>
        /// Gets or sets the answer.
        /// </summary>
        /// <value>
        /// The answer.
        /// </value>
        public int Answer { get; set; }

        /// <summary>
        /// Gets or sets the score.
        /// </summary>
        /// <value>
        /// The score.
        /// </value>
        public int Score
        {
            get
            {
                return this.score;
            }

            set
            {
                if (this.score != value)
                {
                    this.score = value;
                    this.OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is connected.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is connected; otherwise, <c>false</c>.
        /// </value>
        public bool IsConnected
        {
            get
            {
                return this.isConnected;
            }

            set
            {
                if (this.isConnected != value)
                {
                    this.isConnected = value;
                    this.OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the connect command.
        /// </summary>
        /// <value>
        /// The connect command.
        /// </value>
        public RelayCommand Connect
        {
            get
            {
                if (this.connect == null)
                {
                    Action<object> execute = delegate(object argument)
                    {
                        try
                        {
                            if (this.IsNamedPipes)
                            {
                                this.dataService.Connect(this.serverName, this.PlayerName, this.IsNamedPipes);
                            }
                            else
                            {
                                this.dataService.Connect(new IPEndPoint(this.ServerAddress, this.ServerPort), this.PlayerName, this.IsNamedPipes);
                            }
                        }
                        catch (Exception exc)
                        {
                            MessageBox.Show(exc.Message);
                        }
                    };

                    Predicate<object> canExecute = delegate(object argument)
                    {
                        if (this.IsConnected
                        || this.ServerAddress == default(IPAddress)
                        || this.ServerPort == default(int)
                        || this.PlayerName == default(string)
                        || this.PlayerName == string.Empty)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    };

                    this.connect = new RelayCommand(execute, canExecute);
                }

                return this.connect;
            }
        }

        /// <summary>
        /// Gets the disconnect command.
        /// </summary>
        /// <value>
        /// The disconnect command.
        /// </value>
        public RelayCommand Disconnect
        {
            get
            {
                if (this.disconnect == null)
                {
                    Action<object> execute = delegate(object argument)
                    {
                        try
                        {
                            this.dataService.Disconnect();
                            this.IsConnected = false;
                            this.IsOver = false;
                            if (this.timerThread != null && this.timerThread.IsAlive)
                            {
                                this.timerThread.Abort();
                            }

                            this.RemainingTime = 0;
                            this.History = new ObservableCollection<string>();
                            this.Scores = new List<ScoreEntry>();
                            this.Answer = 0;
                            this.Question = string.Empty;
                            this.OnPropertyChanged("Answer");
                        }
                        catch (Exception exc)
                        {
                            MessageBox.Show(exc.Message);
                        }
                    };

                    Predicate<object> canExecute = delegate(object argument)
                    {
                        return this.IsConnected;
                    };

                    this.disconnect = new RelayCommand(execute, canExecute);
                }

                return this.disconnect;
            }
        }

        /// <summary>
        /// Gets the submit command.
        /// </summary>
        /// <value>
        /// The submit command.
        /// </value>
        public RelayCommand Submit
        {
            get
            {
                if (this.submitAnswer == null)
                {
                    Action<object> execute = delegate(object argument)
                    {
                        this.dataService.SubmitAnswer(this.Answer);
                    };

                    Predicate<object> canExecute = delegate(object argument)
                    {
                        return !this.IsOver;
                    };

                    this.submitAnswer = new RelayCommand(execute, canExecute);
                }

                return this.submitAnswer;
            }
        }

        /// <summary>
        /// Gets the get scores command.
        /// </summary>
        /// <value>
        /// The get scores command.
        /// </value>
        public RelayCommand GetScores
        {
            get
            {
                if (this.getScores == null)
                {
                    Action<object> execute = delegate(object argument)
                    {
                        this.dataService.GetScores();
                    };

                    this.getScores = new RelayCommand(execute);
                }

                return this.getScores;
            }
        }

        /// <summary>
        /// Simple form, found from  <see href="http://www.pmbs.de/inotifypropertychanged-robust-und-ohne-strings-callermembername-mvvm/"/>
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Updates the question timer.
        /// </summary>
        private void UpdateQuestionTimer()
        {
            while (this.RemainingTime != 0)
            {
                Thread.Sleep(1000);
                this.RemainingTime--;
            }
        }
    }
}
