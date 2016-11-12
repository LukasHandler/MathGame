using Monitor.Application;
using Shared.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Monitor.Presentation
{
    class Program
    {
        static void Main(string[] args)
        {
            //IPAddress serverIp;

            //do
            //{
            //    Console.Write("Server-IP: " + input);
            //    input = Console.ReadLine();

            //} while (!IPAddress.TryParse(input, out serverIp));

            //int serverPort = 0;

            //do
            //{
            //    Console.Write("Server-Port: " + serverPort);
            //    input = Console.ReadLine();

            //} while (!Int32.TryParse(input, out serverPort));

            //serverIp = IPAddress.Parse("127.0.0.1");
            //serverPort = 4801;

            //IPEndPoint serverEndPoint = new IPEndPoint(serverIp, serverPort);

            var serverEndPoint = ConsoleInput.GetIPEndPoint("Server");
            NetworkService.Register(serverEndPoint);
            NetworkService.OnLoggingDataReceived += PrintLogging;

            Console.ReadKey();
            NetworkService.Unregister();
        }

        private static void PrintLogging(object sender, LoggingEventArgs e)
        {
            Console.WriteLine(e.LoggingText);
        }
    }
}
