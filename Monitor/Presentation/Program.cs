//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="Lukas Handler">
//     Lukas Handler
// </copyright>
// <summary>
// This file represents the UI for the monitor.
// </summary>
//-----------------------------------------------------------------------
namespace Monitor.Presentation
{
    using System;
    using System.Net;
    using Application;
    using Application.EventArguments;

    /// <summary>
    /// This class represents the UI for the monitor.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Entry point when the program gets launched.
        /// </summary>
        /// <param name="args">The command line arguments.</param>
        private static void Main(string[] args)
        {
            var serverEndPoint = GetIPEndPoint();
            DataService dataService = new DataService();

            try
            {
                dataService.Register(serverEndPoint);
                dataService.OnLoggingDataReceived += PrintLogging;

                Console.ReadLine();
                dataService.Unregister();
            }
            catch (Exception)
            {
                Console.WriteLine("Couldn't create connection, please restart and try again");
                Console.ReadLine();
            }
        }

        /// <summary>
        /// Gets the IP end point.
        /// </summary>
        /// <returns>The IP end point.</returns>
        private static IPEndPoint GetIPEndPoint()
        {
            IPAddress address = null;
            string input = string.Empty;

            do
            {
                Console.Write("Server-IP: ");
                input = Console.ReadLine();
            }
            while (!IPAddress.TryParse(input, out address));

            int port = 0;

            do
            {
                Console.Write("Server-Port: ");
                input = Console.ReadLine();
            }
            while (!int.TryParse(input, out port));

            return new IPEndPoint(address, port);
        }

        /// <summary>
        /// Prints a logged message.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">The <see cref="LoggingEventArgs"/> instance containing the event data.</param>
        private static void PrintLogging(object sender, LoggingEventArgs eventArgs)
        {
            Console.WriteLine(eventArgs.LoggingText);
        }
    }
}
