using Newtonsoft.Json;
using Server.Application;
using Server.Application.EventArguments;
using Server.Application.Exceptions;
using Shared.SharedPresentation.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Presentation.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string serverName;

        private int clientConnectionPort;

        private int monitorConnectionPort;

        private int serverConnectionPort;

        private int serverPort;

        private int serverConnections;

        private RelayCommand addConnection;

        private RelayCommand removeConnection;

        private DataService dataService;

        public string ServerName
        {
            get
            {
                return this.serverName;
            }
            set
            {
                this.serverName = value;
                this.OnPropertyChanged();
            }
        }

        public int ClientConnectionPort
        {
            get
            {
                return this.clientConnectionPort;
            }
            set
            {
                this.clientConnectionPort = value;
                this.OnPropertyChanged();
            }
        }

        public int MonitorConnectionPort
        {
            get
            {
                return this.monitorConnectionPort;
            }
            set
            {
                this.monitorConnectionPort = value;
                this.OnPropertyChanged();
            }
        }

        public int ServerConnectionPort
        {
            get
            {
                return this.serverConnectionPort;
            }
            set
            {
                this.serverConnectionPort = value;
                this.OnPropertyChanged();
            }
        }

        public IPAddress ServerAddress { get; set; }

        public int ServerPort
        {
            get
            {
                return this.serverPort;
            }
            set
            {
                this.serverPort = value;
                this.OnPropertyChanged();
            }
        }

        public int ServerConnections
        {
            get
            {
                return this.serverConnections;
            }
            set
            {
                this.serverConnections = value;
                this.OnPropertyChanged();
            }
        }

        public RelayCommand AddConnection
        {
            get
            {
                if (this.addConnection == null)
                {
                    Action<object> execute = delegate (object o)
                    {
                        if (this.dataService != null)
                        {
                            try
                            {
                                this.dataService.ServerRegister(new IPEndPoint(this.ServerAddress, this.ServerPort));
                            }
                            catch (Exception)
                            {
                                MessageBox.Show("Couldn't connect. Wrong IP/Port?");
                            }
                        }
                    };

                    Predicate<object> canExecute = delegate (object o)
                    {
                        return this.ServerConnections < int.MaxValue && this.ServerAddress != default(IPAddress);
                    };

                    this.addConnection = new RelayCommand(execute, canExecute);
                }

                return this.addConnection;
            }
        }

        public RelayCommand RemoveConnection
        {
            get
            {
                if (this.removeConnection == null)
                {
                    Action<object> execute = delegate (object o)
                    {
                        if (this.dataService != null)
                        {
                            this.dataService.ServerUnregister();
                        }
                    };

                    Predicate<object> canExecute = delegate (object o)
                    {
                        return this.ServerConnections > 0;
                    };

                    this.removeConnection = new RelayCommand(execute, canExecute);
                }

                return this.removeConnection;
            }
        }

        private ServerConfiguration configuration;

        public event PropertyChangedEventHandler PropertyChanged;

        public MainViewModel()
        {
            //http://stackoverflow.com/questions/2498521/wpf-mvvm-viewmodel-constructor-designmode
            if (!DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject()))
            {
                OpenFileDialog selectConfigFile = new OpenFileDialog();
                selectConfigFile.Title = "Load Configuration";
                selectConfigFile.Filter = "JSON files|*.json";
                selectConfigFile.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory + @"Resources\";

                if (selectConfigFile.ShowDialog() == DialogResult.OK)
                {
                    string jsonText = string.Empty;

                    try
                    {
                        using (StreamReader fileReader = new StreamReader(selectConfigFile.FileName))
                        {
                            jsonText = fileReader.ReadToEnd();
                        }

                        configuration = JsonConvert.DeserializeObject<ServerConfiguration>(jsonText);
                    }
                    catch (Exception)
                    {
                        CreateDefaultConfiguration();
                    }
                }
                else
                {
                    CreateDefaultConfiguration();
                }

                this.ServerName = configuration.ServerName;
                this.MonitorConnectionPort = configuration.MonitorPort;
                this.ClientConnectionPort = configuration.ClientPort;
                this.ServerConnectionPort = configuration.ServerPort;

                try
                {
                    dataService = new DataService(configuration);
                }
                catch (PortException e)
                {
                    MessageBox.Show(e.Message + Environment.NewLine + "Restart application with new free port", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(1);
                }

                dataService.OnServerConnectionCountChanged += delegate (object sender, ServerConnectionCountChangedEventArgs args)
                {
                    this.ServerConnections = args.NewConnectionCount;
                };
            }
        }

        private void CreateDefaultConfiguration()
        {
            Random randomGenerator = new Random();

            configuration = new ServerConfiguration()
            {
                ServerName = "server" + randomGenerator.Next(1, 10),
                ClientPort = 4700 + randomGenerator.Next(1, 100),
                MonitorPort = 4800 + randomGenerator.Next(1, 100),
                ServerPort = 4900 + randomGenerator.Next(1, 100),
                MaxScore = randomGenerator.Next(10, 100),
                MinScore = randomGenerator.Next(-100, -5)
            };
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
