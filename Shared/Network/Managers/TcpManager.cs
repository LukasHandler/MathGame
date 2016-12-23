using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Data.EventArguments;
using Shared.Data.Messages;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace Shared.Data.Managers
{
    public abstract class TcpManager : IDataManager
    {
        public event EventHandler<MessageEventArgs> OnDataReceived;

        public void Register(object target)
        {
            this.Connect((IPEndPoint)target);
        }

        protected abstract void Disconnect(IPEndPoint target);

        protected abstract void Connect(IPEndPoint target);

        public void Unregister(object target)
        {
            this.Disconnect((IPEndPoint)target);
        }

        public void WriteData(Message data, object target)
        {
            byte[] bytes = MessageByteConverter.ConvertToBytes(data);
            byte[] size = BitConverter.GetBytes(bytes.Count());

            byte[] fullData = new byte[bytes.Count() + size.Count()];

            var s = new MemoryStream();
            s.Write(size, 0, size.Length);
            s.Write(bytes, 0, bytes.Length);
            fullData = s.ToArray();
            s.Dispose();

            this.SendData(fullData, (IPEndPoint)target);
        }

        protected abstract void SendData(byte[] data, IPEndPoint target);

        protected void StartReading(NetworkStream stream, IPEndPoint target)
        {
            byte[] byteSize = new byte[4];
            byte[] buffer = null;
            AsyncCallback callback = null;

            AsyncCallback lengthCallback = delegate (IAsyncResult result)
            {
                int bytesRead;

                try
                {
                    bytesRead = stream.EndRead(result);
                }
                //IOException, ObjectDisposedException
                catch (Exception)
                {
                    return;
                }

                buffer = new byte[BitConverter.ToInt32(byteSize, 0)];
                
                stream.BeginRead(buffer, 0, buffer.Length, callback, null);
            };

            callback = delegate (IAsyncResult result)
            {
                int bytesRead;

                try
                {
                    bytesRead = stream.EndRead(result);
                }
                catch (Exception)
                {
                    return;
                }

                if (bytesRead == 0)
                {
                    return;
                }

                //Copy result into new buffer so we can read as soon as possible again - otherwise some messages get lost
                byte[] toConvertBuffer = new byte[bytesRead];
                Array.Copy(buffer, toConvertBuffer, bytesRead);

                byteSize = new byte[4];
                stream.BeginRead(byteSize, 0, byteSize.Length, lengthCallback, null);

                Message receivedMessage = MessageByteConverter.ConvertToMessage(toConvertBuffer);

                if (OnDataReceived != null)
                {
                    OnDataReceived(target, new MessageEventArgs(receivedMessage));
                }
            };

            stream.BeginRead(byteSize, 0, byteSize.Length, lengthCallback, null);
        }

        private int GetPacketSize(NetworkStream stream)
        {
            byte[] bufferSize = new byte[4];
            stream.Read(bufferSize, 0, 4);
            return Convert.ToInt32(bufferSize);
        }
    }
}
