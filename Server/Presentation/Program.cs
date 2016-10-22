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

namespace Server.Presentation
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            OpenFileDialog selectConfigFile = new OpenFileDialog();
            selectConfigFile.Title = "Load Configuration";
            selectConfigFile.Filter = "JSON files|*.json";
            selectConfigFile.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;

            ServerConfiguration configuration;
            //string jsonText = string.Empty;

            //using (StreamReader fileReader = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "server1.json"))
            //{
            //    jsonText = fileReader.ReadToEnd();
            //}

            //configuration = JsonConvert.DeserializeObject<ServerConfiguration>(jsonText);

            if (selectConfigFile.ShowDialog() == DialogResult.OK)
            {
                string jsonText = string.Empty;

                using (StreamReader fileReader = new StreamReader(selectConfigFile.FileName))
                {
                    jsonText = fileReader.ReadToEnd();
                }

                configuration = JsonConvert.DeserializeObject<ServerConfiguration>(jsonText);
            }
            else
            {
                Random randomGenerator = new Random();

                configuration = new ServerConfiguration()
                {
                    ServerName = "server" + randomGenerator.Next(1, 10),
                    ClientPort = 4700 + randomGenerator.Next(1, 100),
                    MonitorPort = 4800 + randomGenerator.Next(1, 100),
                    ServerPort = 4900 + randomGenerator.Next(1, 100)
                };
            }

            NetworkService serverService = new NetworkService(configuration);
            serverService.OnLoggingMessage += PrintLoggingMessage;

            Console.WriteLine("Started server");
            Console.WriteLine("Name: " + configuration.ServerName);
            Console.WriteLine("Client Port: " + configuration.ClientPort);
            Console.WriteLine("Monitor Port: " + configuration.MonitorPort);
            Console.WriteLine("Server Port: " + configuration.ServerPort);
            Console.WriteLine("___________________________________________");

            while (true)
            {
                var serverEndPoint = ConsoleInput.GetIPEndPoint("Server");
                serverService.ConnectToServer(serverEndPoint);

                string input = string.Empty;

                do
                {
                    Console.Write("> ");
                    input = Console.ReadLine();
                } while (input != "disconnect");

                serverService.DisconnectFromServer();
            }
        }

        private static void PrintLoggingMessage(object sender, LoggingEventArgs e)
        {
            Console.WriteLine(e.LoggingText);
        }
    }
}
