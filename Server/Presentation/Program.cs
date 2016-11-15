using Newtonsoft.Json;
using Server.Application;
using Shared.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Server.Application.EventArguments;

namespace Server.Presentation
{
    class Program
    {
        private static ServerConfiguration configuration;

        [STAThread]
        static void Main(string[] args)
        {
            Console.Title = "Server-Server-Connections: 0";
            OpenFileDialog selectConfigFile = new OpenFileDialog();
            selectConfigFile.Title = "Load Configuration";
            selectConfigFile.Filter = "JSON files|*.json";
            selectConfigFile.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;

            //string jsonText = string.Empty;

            //using (StreamReader fileReader = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "server1.json"))
            //{
            //    jsonText = fileReader.ReadToEnd();
            //}

            //configuration = JsonConvert.DeserializeObject<ServerConfiguration>(jsonText);

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
                catch(Exception)
                {
                    CreateDefaultConfiguration();
                }

            }
            else
            {
                CreateDefaultConfiguration();
            }

            DataService serverService = new DataService(configuration);
            serverService.OnLoggingMessage += PrintLoggingMessage;
            serverService.OnServerConnectionCountChanged += ChangeServerConnectionCount;

            PrintServerInformation();

            while (true)
            {
                var serverEndPoint = ConsoleInput.GetIPEndPoint("Server");
                string input = string.Empty;

                do
                {
                    Console.WriteLine("connect/disconnect/exit", serverService.ServerConnectionCount);

                    Console.Write("> ");
                    input = Console.ReadLine().ToLower();

                    if (input == "connect")
                    {
                        if (serverService.ServerConnectionCount == int.MaxValue)
                        {
                            Console.WriteLine("Error, can't create more connections");
                        }

                        serverService.RegisterToServer(serverEndPoint);
                    }
                    else if (input == "disconnect")
                    {
                        if (serverService.ServerConnectionCount == 0)
                        {
                            Console.WriteLine("Error, there is no connection to disconnect from");
                        }
                        else
                        {
                            serverService.UnregisterFromServer(serverEndPoint);
                        }
                    }

                } while (input != "exit");

                while(serverService.ServerConnectionCount > 0)
                {
                    serverService.UnregisterFromServer(serverEndPoint);
                }
            }
        }

        private static void ChangeServerConnectionCount(object sender, ServerConnectionCountChangedEventArgs e)
        {
            Console.Title = "Server-Server-Connections: " + Convert.ToString(e.NewConnectionCount);
        }

        private static void CreateDefaultConfiguration()
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

        private static void PrintServerInformation()
        {
            Console.WriteLine("Name: {0}, ClientPort: {1}, MonitorPort: {2}, ServerPort: {3}",
                configuration.ServerName,
                configuration.ClientPort,
                configuration.MonitorPort,
                configuration.ServerPort);
            Console.WriteLine("------------------------------------------------------------------------------");
        }

        private static void PrintLoggingMessage(object sender, LoggingEventArgs e)
        {
            Console.WriteLine(e.LoggingText);
        }
    }
}
