using Client.Application;
using Client.Application.EventArguments;
using Shared.Data;
using Shared.Data.EventArguments;
using Shared.SharedPresentation.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;

namespace Client.Presentation.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private Thread timerThread;

        private DataService networkService;

        private readonly object questionsLock = new object();

        public MainViewModel()
        {
            this.PlayerName = "horst";
            this.ServerAddress = IPAddress.Parse("127.0.0.1");
            this.ServerPort = 4701;
            this.ServerName = "server1";
            this.Questions = new ObservableCollection<string>();
            this.Scores = new List<ScoreEntry>();

            networkService = new DataService();

            networkService.OnConnectionAccepted += delegate (object sender, EventArgs args)
            {
                this.IsConnected = true;
            };

            networkService.OnConnectionDenied += delegate (object sender, EventArgs args)
            {
                MessageBox.Show("Server denied the connection.");
                this.IsConnected = false;
            };

            networkService.OnQuestionReceived += delegate (object sender, QuestionEventArgs args)
            {
                this.Questions.Add(args.QuestionText);
                this.Question = args.QuestionText;
                this.Time = args.Time;
                this.Score = args.Score;

                ThreadStart timerThreadStart = new ThreadStart(this.updateQuestionTimer);

                if (this.timerThread != null)
                {
                    this.timerThread.Abort();
                }

                timerThread = new Thread(timerThreadStart);
                timerThread.Start();
            };

            networkService.OnGameWon += delegate (object sender, GameFinishedEventArgs args)
            {
                this.Score = args.Score;
                this.timerThread.Abort();
                this.Time = 0;
                MessageBox.Show("Congratulations, you won!");
                this.IsOver = true;
            };

            networkService.OnGameLost += delegate (object sender, GameFinishedEventArgs args)
            {
                this.Score = args.Score;
                this.timerThread.Abort();
                this.Time = 0;
                MessageBox.Show("You lost!");
                this.IsOver = true;
            };

            networkService.OnScoresReceived += delegate (object sender, ScoresEventArgs args)
            {
                this.Scores = args.Scores;
            };

            networkService.OnBroadcastTextReceived += delegate (object sender, BroadcastEventArgs args)
            {
                this.Questions.Add(args.BroadcastText);
            };
        }

        private void updateQuestionTimer()
        {
            while (this.Time != 0)
            {
                Thread.Sleep(1000);
                this.Time--;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private RelayCommand disconnect;

        private RelayCommand connect;

        private RelayCommand submitAnswer;

        private RelayCommand getScores;

        private ObservableCollection<string> questions;

        private int time;

        private int score;

        private bool isOver;

        public ObservableCollection<string> Questions
        {
            get
            {
                return this.questions;
            }
            set
            {
                if (this.questions != value)
                {
                    this.questions = value;
                    this.OnPropertyChanged();

                    //Avoid problem when updating the observable collection from a different thread (http://stackoverflow.com/questions/2104614/updating-an-observablecollection-in-a-separate-thread)
                    BindingOperations.EnableCollectionSynchronization(this.questions, questionsLock);
                }
            }
        }

        public int Time
        {
            get
            {
                return this.time;
            }
            set
            {
                if (this.time != value)
                {
                    this.time = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private string question;

        private bool isConnected;

        private List<ScoreEntry> scores;

        private string serverName;

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

        public string PlayerName { get; set; }

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

        public IPAddress ServerAddress { get; set; }

        public int ServerPort { get; set; }

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

        public bool IsNamedPipes { get; set; }

        public int Answer { get; set; }

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

        public RelayCommand Connect
        {
            get
            {
                if (this.connect == null)
                {
                    Action<object> execute = delegate (object argument)
                    {
                        try
                        {
                            //if (!isConnecting)
                            {
                                //isConnecting = true;
                                if (this.IsNamedPipes)
                                {
                                    networkService.Connect(this.serverName, this.PlayerName, this.IsNamedPipes);
                                }
                                else
                                {
                                    networkService.Connect(new IPEndPoint(this.ServerAddress, this.ServerPort), this.PlayerName, this.IsNamedPipes);
                                }
                            }
                        }
                        catch (Exception exc)
                        {
                            MessageBox.Show(exc.Message);
                        }
                    };

                    Predicate<object> canExecute = delegate (object argument)
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

        public RelayCommand Disconnect
        {
            get
            {
                if (this.disconnect == null)
                {
                    Action<object> execute = delegate (object argument)
                    {
                        try
                        {
                            networkService.Disconnect();
                            this.IsConnected = false;
                            this.IsOver = false;
                            if (this.timerThread != null && this.timerThread.IsAlive)
                            {
                                this.timerThread.Abort();
                            }

                            this.Time = 0;
                            this.Questions = new ObservableCollection<string>();
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

                    Predicate<object> canExecute = delegate (object argument)
                    {
                        return this.IsConnected;
                    };

                    this.disconnect = new RelayCommand(execute, canExecute);
                }

                return this.disconnect;
            }
        }

        public RelayCommand Submit
        {
            get
            {
                if (this.submitAnswer == null)
                {
                    Action<object> execute = delegate (object argument)
                    {
                        networkService.SubmitAnswer(this.Answer);
                    };

                    Predicate<object> canExecute = delegate (object argument)
                    {
                        return !this.IsOver;
                    };

                    this.submitAnswer = new RelayCommand(execute, canExecute);
                }

                return this.submitAnswer;
            }
        }

        public RelayCommand GetScores
        {
            get
            {
                if (this.getScores == null)
                {
                    Action<object> execute = delegate (object argument)
                    {
                        networkService.GetScores();
                    };

                    this.getScores = new RelayCommand(execute);
                }

                return this.getScores;
            }
        }

        //http://www.pmbs.de/inotifypropertychanged-robust-und-ohne-strings-callermembername-mvvm/
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
