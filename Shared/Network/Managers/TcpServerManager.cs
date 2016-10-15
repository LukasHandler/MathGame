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
        private IPEndPoint localEndPoint;

        private Dictionary<IPEndPoint, NetworkStream> streams;

        public TcpServerManager(IPEndPoint localEndPoint)
        {
            this.localEndPoint = localEndPoint;
            this.streams = new Dictionary<IPEndPoint, NetworkStream>();

            TcpListener listener = new TcpListener(this.localEndPoint);
            listener.Start();

            AcceptClients(listener);
        }

        private async void AcceptClients(TcpListener listener)
        {
            while (true)
            {
                var client = await listener.AcceptTcpClientAsync();
                NetworkStream stream = client.GetStream();

                IPEndPoint serverEndPoint = ((IPEndPoint)client.Client.RemoteEndPoint);

                if (this.streams.Any(p => p.Key.Address.ToString() == serverEndPoint.Address.ToString() 
                && p.Key.Port == serverEndPoint.Port))
                {
                    throw new NotImplementedException();
                }
                else
                {
                    this.streams.Add(serverEndPoint, stream);
                }


                AsyncCallback callback = null;
                byte[] buffer = new byte[10000];

                callback = delegate(IAsyncResult result)
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
        }

        public event EventHandler<MessageEventArgs> OnDataReceived;

        public void WriteData(Message data, object target)
        {
            byte[] bytes = MessageByteConverter.ConvertToBytes(data);
            this.streams[(IPEndPoint)target].Write(bytes, 0, bytes.Length);
        }
    }
}
