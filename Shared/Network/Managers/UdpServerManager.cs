using Shared.Data;
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

namespace Shared.Data.Managers
{
    public class UdpServerManager : IDataManager
    {
        private UdpClient udpStream;
        private IPEndPoint localEndPoint;

        public UdpServerManager(IPEndPoint localEndPoint)
        {
            this.localEndPoint = localEndPoint;
            udpStream = new UdpClient(localEndPoint);
            udpStream.BeginReceive(ReceivedData, null);
        }

        private void ReceivedData(IAsyncResult data)
        {
            IPEndPoint hostIp = new IPEndPoint(IPAddress.Any, 0);
            byte[] received = udpStream.EndReceive(data, ref hostIp);

            Message receivedMessage = MessageByteConverter.ConvertToMessage(received);

            if (OnDataReceived != null)
            {
                OnDataReceived(this, new MessageEventArgs(receivedMessage));
            }

            udpStream.BeginReceive(ReceivedData, null);
        }

        public event EventHandler<MessageEventArgs> OnDataReceived;

        public void WriteData(Message data, object target)
        {
            var targetEndpoint = (IPEndPoint)target;
            byte[] bytes = MessageByteConverter.ConvertToBytes(data);
            udpStream.Connect(targetEndpoint);
            udpStream.Send(bytes, bytes.Length);
        }
    }
}
