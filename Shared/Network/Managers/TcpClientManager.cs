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
    public class TcpClientManager : IDataManager
    {
        private NetworkStream stream;

        private TcpClient client;

        public event EventHandler<MessageEventArgs> OnDataReceived;

        public void WriteData(Message data, object target)
        {
            if (stream == null)
            {
                //TcpClient client = new TcpClient(localEndPoint);
                client = new TcpClient();
                try
                {
                    client.Connect((IPEndPoint)target);
                }
                catch (Exception)
                {
                    return;
                }

                stream = client.GetStream();

                AsyncCallback callback = null;
                byte[] buffer = new byte[10000];

                callback = delegate (IAsyncResult result)
                {
                    int bytesRead = stream.EndRead(result);

                    //Copy result into new buffer so we can read as soon as possible again - otherwise some messages get lost
                    byte[] toConvertBuffer = new byte[bytesRead];
                    Array.Copy(buffer, toConvertBuffer, bytesRead);

                    buffer = new byte[10000];
                    stream.BeginRead(buffer, 0, buffer.Length, callback, null);

                    Message receivedMessage = MessageByteConverter.ConvertToMessage(toConvertBuffer);

                    if (OnDataReceived != null)
                    {
                        OnDataReceived(this, new MessageEventArgs(receivedMessage));
                    }
                };

                stream.BeginRead(buffer, 0, buffer.Length, callback, null);
            }

            if (data.SenderInformation == null)
            {
                data.SenderInformation = client.Client.LocalEndPoint;
            }

            byte[] bytes = MessageByteConverter.ConvertToBytes(data);
            stream.Write(bytes, 0, bytes.Length);
        }
    }
}
