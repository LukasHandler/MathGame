//-----------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="Lukas Handler">
//     Lukas Handler
// </copyright>
// <summary>
// This file represents the view model for the main window.
// </summary>
//-----------------------------------------------------------------------
namespace Presentation.ViewModels
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Net;
    using System.Runtime.CompilerServices;
    using System.Windows.Forms;
    using Newtonsoft.Json;
    using Server.Application;
    using Server.Application.EventArguments;
    using Server.Application.Exceptions;
    using Shared.SharedPresentation.Utilities;

    /// <summary>
    /// This class represents the view model for the main window.
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public class MainViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// The server name.
        /// </summary>
        private string serverName;

        /// <summary>
        /// The client connection port.
        /// </summary>
        private int clientConnectionPort;

        /// <summary>
        /// The monitor connection port.
        /// </summary>
        private int monitorConnectionPort;

        /// <summary>
        /// The server connection port.
        /// </summary>
        private int serverConnectionPort;

        /// <summary>
        /// The server port when connecting to another server.
        /// </summary>
        private int serverPort;

        /// <summary>
        /// The server connections count (from server to server).
        /// </summary>
        private int serverConnections;

        /// <summary>
        /// The add connection command.
        /// </summary>
        private RelayCommand addConnection;

        /// <summary>
        /// The remove connection command.
        /// </summary>
        private RelayCommand removeConnection;

        /// <summary>
        /// The data service, which is on the application layer.
        /// </summary>
        private DataService dataService;

        /// <summary>
        /// The configuration of the server.
        /// </summary>
        private ServerConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        public MainViewModel()
        {
            // <see href=http://stackoverflow.com/questions/2498521/wpf-mvvm-viewmodel-constructor-designmode"/>
            if (!DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject()))
            {
                // Open dialog to select the server configuration file.
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

                        this.configuration = JsonConvert.DeserializeObject<ServerConfiguration>(jsonText);
                    }
                    catch (Exception)
                    {
                        this.CreateDefaultConfiguration();
                    }
                }
                else
                {
                    this.CreateDefaultConfiguration();
                }

                // Sent view values.
                this.ServerName = this.configuration.ServerName;
                this.MonitorConnectionPort = this.configuration.MonitorPort;
                this.ClientConnectionPort = this.configuration.ClientPort;
                this.ServerConnectionPort = this.configuration.ServerPort;

                // Start servers.
                try
                {
                    this.dataService = new DataService(this.configuration);
                }
                catch (PortException e)
                {
                    MessageBox.Show(e.Message + Environment.NewLine + "Restart application with new free port", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(1);
                }

                // Refresh value in UI when the connection count changed.
                this.dataService.OnServerConnectionCountChanged += delegate(object sender, ServerConnectionCountChangedEventArgs args)
                {
                    this.ServerConnections = args.NewConnectionCount;
                };
            }
        }

        /// <summary>
        /// Occurs when specific property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the name of the server.
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
                this.serverName = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the client connection port.
        /// </summary>
        /// <value>
        /// The client connection port.
        /// </value>
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

        /// <summary>
        /// Gets or sets the monitor connection port.
        /// </summary>
        /// <value>
        /// The monitor connection port.
        /// </value>
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

        /// <summary>
        /// Gets or sets the server connection port.
        /// </summary>
        /// <value>
        /// The server connection port.
        /// </value>
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

        /// <summary>
        /// Gets or sets the server address to connect to another server.
        /// </summary>
        /// <value>
        /// The server address.
        /// </value>
        public IPAddress ServerAddress { get; set; }

        /// <summary>
        /// Gets or sets the server port to connect to another server.
        /// </summary>
        /// <value>
        /// The server port.
        /// </value>
        public int ServerPort { get; set; }

        /// <summary>
        /// Gets or sets the server connections count.
        /// </summary>
        /// <value>
        /// The server connections count.
        /// </value>
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

        /// <summary>
        /// Gets the add connection command.
        /// </summary>
        /// <value>
        /// The add connection command.
        /// </value>
        public RelayCommand AddConnection
        {
            get
            {
                if (this.addConnection == null)
                {
                    // Call the application layer to register to server.
                    Action<object> execute = delegate(object o)
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

                    // Can only add connections when the counter is smaller than max value, and server address and port are set.
                    Predicate<object> canExecute = delegate(object o)
                    {
                        return this.ServerConnections < int.MaxValue && this.ServerAddress != default(IPAddress) && this.ServerPort != 0;
                    };

                    this.addConnection = new RelayCommand(execute, canExecute);
                }

                return this.addConnection;
            }
        }

        /// <summary>
        /// Gets the remove connection command.
        /// </summary>
        /// <value>
        /// The remove connection command.
        /// </value>
        public RelayCommand RemoveConnection
        {
            get
            {
                if (this.removeConnection == null)
                {
                    // Call the application layer to remove a connection.
                    Action<object> execute = delegate(object o)
                    {
                        if (this.dataService != null)
                        {
                            this.dataService.ServerUnregister();
                        }
                    };

                    // Can only disconnect if there are any connections.
                    Predicate<object> canExecute = delegate(object o)
                    {
                        return this.ServerConnections > 0;
                    };

                    this.removeConnection = new RelayCommand(execute, canExecute);
                }

                return this.removeConnection;
            }
        }

        /// <summary>
        /// Simple implementation to fire the property changed event <see href="http://www.pmbs.de/inotifypropertychanged-robust-und-ohne-strings-callermembername-mvvm/"/>.
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
        /// Creates the default configuration.
        /// </summary>
        private void CreateDefaultConfiguration()
        {
            Random randomGenerator = new Random();
            this.configuration = new ServerConfiguration()
            {
                ServerName = "server" + randomGenerator.Next(1, 10),
                ClientPort = 4700 + randomGenerator.Next(1, 100),
                MonitorPort = 4800 + randomGenerator.Next(1, 100),
                ServerPort = 4900 + randomGenerator.Next(1, 100),
                MaxScore = randomGenerator.Next(10, 100),
                MinScore = randomGenerator.Next(-100, -5)
            };
        }
    }
}
