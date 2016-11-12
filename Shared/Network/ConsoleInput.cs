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


//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Text;
//using System.Threading.Tasks;

//namespace Shared.Data
//{
//    public static class ConsoleInput
//    {
//        public static IPEndPoint GetIPEndPoint(string endPointTargetName)
//        {
//            int maxLength = 20;

//            IPAddress address = null;
//            int port = 0;
//            string input = string.Empty;
//            char inputKey = '\0';

//            do
//            {
//                ClearLine();
//                Console.Write("Ip: ");
//                input = string.Empty;

//                do
//                {
//                    inputKey = Console.ReadKey().KeyChar;
//                    input += inputKey;

//                } while (input.Length < maxLength && inputKey != (char)13);

//            } while (!IPAddress.TryParse(input, out address));

//            do
//            {
//                do
//                {
//                    ClearLine();
//                    Console.Write("Port: ");
//                    inputKey = Console.ReadKey().KeyChar;
//                    input += inputKey;

//                } while (input.Length < maxLength);

//            } while (!Int32.TryParse(input, out port));

//            return new IPEndPoint(address, port);
//        }

//        private static void ClearLine()
//        {
//            int clearLength = 26;

//            int topCursor = Console.CursorTop;
//            int leftCursor = Console.CursorLeft;
//            Console.SetCursorPosition(leftCursor, topCursor);

//            for (int i = 0; i < clearLength; i++)
//            {
//                Console.Write(" ");
//            }

//            Console.SetCursorPosition(leftCursor, topCursor);
//        }
//    }
//}
