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
            var serverEndPoint = ConsoleInput.GetIPEndPoint("Server");
            DataService.Register(serverEndPoint);
            DataService.OnLoggingDataReceived += PrintLogging;

            Console.ReadLine();
            DataService.Unregister();
        }

        private static void PrintLogging(object sender, LoggingEventArgs e)
        {
            Console.WriteLine(e.LoggingText);
        }
    }
}
