using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Data.EventArguments;
using Shared.Data.Messages;
using System.Net.Sockets;
using System.Net;

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
            this.SendData(data, (IPEndPoint)target);
        }

        protected abstract void SendData(Message data, IPEndPoint target);

        protected void StartReading(NetworkStream stream, IPEndPoint target)
        {
            AsyncCallback callback = null;
            byte[] buffer = new byte[10000];

            callback = delegate (IAsyncResult result)
            {
                int bytesRead;

                try
                {
                    bytesRead = stream.EndRead(result);
                }
                catch (System.IO.IOException)
                {
                    return;
                }

                //Copy result into new buffer so we can read as soon as possible again - otherwise some messages get lost
                byte[] toConvertBuffer = new byte[bytesRead];
                Array.Copy(buffer, toConvertBuffer, bytesRead);

                buffer = new byte[10000];
                stream.BeginRead(buffer, 0, buffer.Length, callback, null);

                Message receivedMessage = MessageByteConverter.ConvertToMessage(toConvertBuffer);

                if (OnDataReceived != null)
                {
                    OnDataReceived(target, new MessageEventArgs(receivedMessage));
                }
            };

            stream.BeginRead(buffer, 0, buffer.Length, callback, null);
        }


    }
}
