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
using System.Xml.Serialization;

namespace Client.Data
{
    public class NetworkManager : IDataManager
    {
        private UdpClient udpStream;

        private int senderPort;

        public event EventHandler<MessageEventArgs> OnDataReceived;

        public NetworkManager(int senderPort)
        {
            this.senderPort = senderPort;
        }

        public void WriteData(object data, object target)
        {
            var targetEndpoint = (IPEndPoint)target;

            if (udpStream == null)
            {
                this.udpStream = new UdpClient(senderPort);
                udpStream.Connect(targetEndpoint);
                udpStream.BeginReceive(ReceivedData, null);
            }

            byte[] bytes = MessageByteConverter.ConvertToBytes((Message)data);
            udpStream.Send(bytes, bytes.Length);
        }

        //http://stackoverflow.com/questions/7266101/receive-messages-continuously-using-udpclient
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
    }
}
