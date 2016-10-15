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

namespace Server.Data
{
    public class NetworkManager : IDataManager
    {
        UdpClient udpStream;

        public NetworkManager(int port)
        {
            udpStream = new UdpClient(port);
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

        public void WriteData(object data, object target)
        {
            var targetEndpoint = (IPEndPoint)target;
            byte[] bytes = MessageByteConverter.ConvertToBytes((Message)data);
            udpStream.Connect(targetEndpoint);
            udpStream.Send(bytes, bytes.Length);
        }
    }
}
