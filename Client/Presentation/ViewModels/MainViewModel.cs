using Client.Presentation.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client.Presentation.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            this.PlayerName = "horst";
            this.ServerAddress = IPAddress.Parse("127.0.0.1");
            this.ServerPort = 4713;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private RelayCommand connect;

        private RelayCommand submitAnswer;

        private RelayCommand getScores;

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

        public bool IsConnected
        {
            get
            {
                return this.isConnected;
            }
            set
            {
                if (this.isConnected = value)
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
                            Application.NetworkService.ConnectToServer(this.ServerAddress, this.ServerPort);
                            this.IsConnected = true;
                        }
                        catch (Exception exc)
                        {
                            MessageBox.Show(exc.Message);
                        }
                    };

                    Predicate<object> canExecute = delegate (object argument)
                    {
                        if (this.ServerAddress == default(IPAddress)
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

        public RelayCommand Submit
        {
            get
            {
                if (this.submitAnswer == null)
                {
                    Action<object> execute = delegate (object argument)
                    {
                        MessageBox.Show("asdf");
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
