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
    public class TcpServerManager : IDataManager
    { 
        private Dictionary<Guid, NetworkStream> streams;

        public TcpServerManager(int port)
        {
            this.streams = new Dictionary<Guid, NetworkStream>();

            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, port);
            TcpListener listener = new TcpListener(localEndPoint);
            listener.Start();

            AcceptClients(listener);
        }

        private async void AcceptClients(TcpListener listener)
        {
            while (true)
            {
                var client = await listener.AcceptTcpClientAsync();
                NetworkStream stream = client.GetStream();

                IPEndPoint clientEndPoint = ((IPEndPoint)client.Client.RemoteEndPoint);

                AsyncCallback callback = null;
                byte[] buffer = new byte[10000];

                callback = delegate (IAsyncResult result)
                {
                    try
                    {
                        int bytesRead = stream.EndRead(result);
                    }
                    catch (System.IO.IOException)
                    {
                        return;
                    }

                    Message receivedMessage = MessageByteConverter.ConvertToMessage(buffer);

                    if (!this.streams.ContainsKey(receivedMessage.SenderId))
                    {
                        this.streams.Add(receivedMessage.SenderId, stream);
                    }

                    if (OnDataReceived != null)
                    {
                        OnDataReceived(this, new MessageEventArgs(receivedMessage));
                    }

                    buffer = new byte[10000];
                    stream.BeginRead(buffer, 0, buffer.Length, callback, null);
                };

                stream.BeginRead(buffer, 0, buffer.Length, callback, null);
            }
        }

        public event EventHandler<MessageEventArgs> OnDataReceived;

        public void WriteData(Message data, object target)
        {
            byte[] bytes = MessageByteConverter.ConvertToBytes(data);
            this.streams[(Guid)target].Write(bytes, 0, bytes.Length);
        }
    }
}
