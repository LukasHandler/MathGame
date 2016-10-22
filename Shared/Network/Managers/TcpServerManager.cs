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
        private Dictionary<IPEndPoint, NetworkStream> streams;

        private TcpListener tcpListener;

        //private TcpClient tcpClient;

        private IPEndPoint localEndPoint;

        private void Init()
        {
            this.streams = new Dictionary<IPEndPoint, NetworkStream>();

            tcpListener = new TcpListener(localEndPoint);

            tcpListener.Start();
            AcceptClients();
        }

        public TcpServerManager(int port)
        {
            this.localEndPoint = new IPEndPoint(IPAddress.Any, port);
            Init();
        }

        private async void AcceptClients()
        {
            while (true)
            {
                var client = await tcpListener.AcceptTcpClientAsync();
                NetworkStream stream = client.GetStream();

                IPEndPoint clientEndPoint = ((IPEndPoint)client.Client.RemoteEndPoint);

                AsyncCallback callback = null;
                byte[] buffer = new byte[10000];

                callback = delegate (IAsyncResult result)
                {
                    try
                    {
                        int bytesRead = stream.EndRead(result);

                        //Copy result into new buffer so we can read as soon as possible again - otherwise some messages get lost
                        byte[] toConvertBuffer = new byte[bytesRead];
                        Array.Copy(buffer, toConvertBuffer, bytesRead);

                        buffer = new byte[10000];
                        stream.BeginRead(buffer, 0, buffer.Length, callback, null);

                        Message receivedMessage = MessageByteConverter.ConvertToMessage(toConvertBuffer);

                        if (!this.streams.Any(p => p.Key.Equals(receivedMessage.SenderInformation)))
                        {
                            this.streams.Add((IPEndPoint)receivedMessage.SenderInformation, stream);
                        }

                        if (OnDataReceived != null)
                        {
                            OnDataReceived(this, new MessageEventArgs(receivedMessage));
                        }
                    }
                    catch (System.IO.IOException)
                    {
                        return;
                    }
                };

                stream.BeginRead(buffer, 0, buffer.Length, callback, null);
            }
        }

        public event EventHandler<MessageEventArgs> OnDataReceived;

        public void WriteData(Message data, object target)
        {
            var targetEndPoint = (IPEndPoint)target;

            if (data.SenderInformation == null)
            {
                //Returns 0.0.0.0 -> so the ip's it listens to.
                data.SenderInformation = tcpListener.LocalEndpoint;
            }

            byte[] bytes = MessageByteConverter.ConvertToBytes(data);

            //if (this.streams.Count == 0)
            //{
            //    if (this.tcpClient == null)
            //    {
            //        tcpListener.Stop();
            //        tcpClient = new TcpClient(this.localEndPoint);
            //        tcpClient.Connect(targetEndPoint);
            //    }

            //    tcpClient.GetStream().Write(bytes, 0, bytes.Length);

            //    if (data is Disco)
            //}
            //else
            //{

            if (this.streams.ContainsKey(targetEndPoint))
            {
                this.streams[targetEndPoint].Write(bytes, 0, bytes.Length);
            }
            //}
        }
    }
}
