using Shared.Data;
using Shared.Data.EventArguments;
using Shared.Data.Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Shared.Data.Managers
{
    public class UdpClientManager : IDataManager
    {
        private UdpClient udpStream;
        private bool startedReceiving;

        public UdpClientManager()
        {
            this.udpStream = new UdpClient();
            this.startedReceiving = false;
        }

        public event EventHandler<MessageEventArgs> OnDataReceived;

        public void WriteData(Message data, object target)
        {
            byte[] bytes = MessageByteConverter.ConvertToBytes(data);

            udpStream.Send(bytes, bytes.Length, (IPEndPoint)target);

            if (!this.startedReceiving)
            {
                this.startedReceiving = true;
                this.udpStream.BeginReceive(this.ReceivedData, null);
            }
        }

        private void ReceivedData(IAsyncResult data)
        {
            IPEndPoint senderIp = new IPEndPoint(IPAddress.Any, 0);
            byte[] received = udpStream.EndReceive(data, ref senderIp);
            udpStream.BeginReceive(ReceivedData, null);

            Message receivedMessage = MessageByteConverter.ConvertToMessage(received);

            if (OnDataReceived != null)
            {
                OnDataReceived(senderIp, new MessageEventArgs(receivedMessage));
            }
        }
    }
}
