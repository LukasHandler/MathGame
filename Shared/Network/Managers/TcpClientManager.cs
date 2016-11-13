using Shared.Data.EventArguments;
using Shared.Data.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Data.Managers
{
    public class TcpClientManager : TcpManager
    {
        private NetworkStream stream;

        private TcpClient client;

        protected override void SendData(byte[] data, IPEndPoint target)
        {
            if (this.stream != null)
            {
                stream.Write(data, 0, data.Length);
            }
        }

        protected override void Disconnect(IPEndPoint target)
        {
            stream.Dispose();
        }

        protected override void Connect(IPEndPoint target)
        {
            if (stream == null)
            {
                client = new TcpClient();
                try
                {
                    client.Connect(target);
                }
                catch (Exception)
                {
                    return;
                }

                stream = client.GetStream();
                this.StartReading(stream, target);
            }
        }
    }
}
