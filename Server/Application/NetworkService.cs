using Server.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Application
{
    public class NetworkService
    {
        private NetworkManager clientManager;

        private int port = 4713;

        public NetworkService()
        {
            clientManager = new NetworkManager(port);
        }
    }
}
