﻿using Server.Application;
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
            NetworkService serverService = new NetworkService();
            Console.ReadLine();
        }
    }
}
