using Monitor.Application;
using System;
using System.Collections.Generic;
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
            IPAddress serverIp;
            string input;

            do
            {
                Console.Write("Server-IP: ");
                input = Console.ReadLine();

            } while (!IPAddress.TryParse(input, out serverIp));

            int serverPort;

            do
            {
                Console.Write("Server-Port: ");
                input = Console.ReadLine();

            } while (!Int32.TryParse(input, out serverPort));

            IPEndPoint serverEndPoint = new IPEndPoint(serverIp, serverPort);

            NetworkService.Connect(serverEndPoint);
            Console.ReadLine();
        }
    }
}
