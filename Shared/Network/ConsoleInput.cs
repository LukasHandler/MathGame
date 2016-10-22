using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Data
{
    public static class ConsoleInput
    {
        public static IPEndPoint GetIPEndPoint(string endPointTargetName)
        {
            IPAddress address = null;
            string input = string.Empty;

            do
            {
                Console.Write("{0}-IP: ", endPointTargetName);
                input = Console.ReadLine();

            } while (!IPAddress.TryParse(input, out address));

            int port = 0;

            do
            {
                Console.Write("{0}-Port: ", endPointTargetName);
                input = Console.ReadLine();

            } while (!Int32.TryParse(input, out port));

            return new IPEndPoint(address, port);
        }
    }
}
