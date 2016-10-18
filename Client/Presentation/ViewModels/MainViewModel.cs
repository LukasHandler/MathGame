using Client.Application;
using Client.Presentation.Utilities;
using Shared.Data.EventArguments;
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

        private NetworkService networkService;

        private readonly object questionsLock = new object();

        public MainViewModel()
        {
            this.PlayerName = "horst";
            this.ServerAddress = IPAddress.Parse("127.0.0.1");
            this.ServerPort = 4701;
            this.Questions = new ObservableCollection<string>();

            networkService = new NetworkService();

            networkService.OnConnectionAccepted += delegate (object sender, EventArgs args)
            {
                this.IsConnected = true;
            };

            networkService.OnConnectionDenied += delegate (object sender, EventArgs args)
            {
                MessageBox.Show("Verbindung wurde vom Server nicht akzeptiert.");
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
            };

            networkService.OnGameLost += delegate (object sender, GameFinishedEventArgs args)
            {
                this.Score = args.Score;
                this.timerThread.Abort();
                this.Time = 0;
                MessageBox.Show("You lost!");
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
                                networkService.Connect(new IPEndPoint(this.ServerAddress, this.ServerPort), this.PlayerName);
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

                    this.submitAnswer = new RelayCommand(execute);
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
                        MessageBox.Show("asdf");
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
