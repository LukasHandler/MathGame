using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Application.Exceptions
{
    public class PortException : Exception
    {
        public PortException(int port) : base(string.Format("There is already a server running on port {0}", port)) { }
    }
}
