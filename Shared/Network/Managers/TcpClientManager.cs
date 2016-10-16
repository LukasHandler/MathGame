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
    public class TcpClientManager : IDataManager
    {
        //private IPEndPoint localEndPoint;

        private NetworkStream stream;

        public TcpClientManager()
        {
            //this.localEndPoint = localEndPoint;
        }

        public event EventHandler<MessageEventArgs> OnDataReceived;

        public void WriteData(Message data, object target)
        {
            if (stream == null)
            {
                //TcpClient client = new TcpClient(localEndPoint);
                TcpClient client = new TcpClient();
                client.Connect((IPEndPoint)target);

                stream = client.GetStream();

                AsyncCallback callback = null;
                byte[] buffer = new byte[10000];

                callback = delegate (IAsyncResult result)
                {
                    int bytesRead = stream.EndRead(result);
                    Message receivedMessage = MessageByteConverter.ConvertToMessage(buffer);

                    if (OnDataReceived != null)
                    {
                        OnDataReceived(this, new MessageEventArgs(receivedMessage));
                    }

                    buffer = new byte[10000];
                    stream.BeginRead(buffer, 0, buffer.Length, callback, null);
                };

                stream.BeginRead(buffer, 0, buffer.Length, callback, null);
            }

            byte[] bytes = MessageByteConverter.ConvertToBytes(data);
            stream.Write(bytes, 0, bytes.Length);
        }
    }
}
