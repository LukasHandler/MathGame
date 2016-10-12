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
        public event PropertyChangedEventHandler PropertyChanged;

        private RelayCommand connectCommand;

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

        public RelayCommand ConnectCommand
        {
            get
            {
                if (this.connectCommand == null)
                {
                    Action<object> execute = delegate (object argument)
                    {
                        MessageBox.Show("asdf");
                        this.IsConnected = true;
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

                    this.connectCommand = new RelayCommand(execute, canExecute);
                }

                return this.connectCommand;
            }
        }

        public MainViewModel()
        {
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
