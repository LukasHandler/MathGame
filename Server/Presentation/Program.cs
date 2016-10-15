using Server.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Presentation
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Client Port: 4713");
            Console.WriteLine("Monitor Port: 4699");
            NetworkService serverService = new NetworkService();
            Console.ReadLine();
        }
    }
}
