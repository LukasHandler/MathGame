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

        private BinaryFormatter binarySerializer = new BinaryFormatter();

        public event EventHandler<MessageEventArgs> OnDataReceived;

        public void WriteData(object data, object target, object[] args)
        {
            var targetEndpoint = (IPEndPoint)target;

            if (udpStream == null)
            {
                udpStream = new UdpClient((int)args[0]);
                udpStream.Connect(targetEndpoint);
                udpStream.BeginReceive(ReceivedData, null);
            }
            
            byte[] bytes;

            using (var bufferStream = new MemoryStream())
            {
                binarySerializer.Serialize(bufferStream, data);
                bytes = bufferStream.ToArray();
            }

            udpStream.Send(bytes, bytes.Length);
            
        }

        //http://stackoverflow.com/questions/7266101/receive-messages-continuously-using-udpclient
        private void ReceivedData(IAsyncResult data)
        {
            IPEndPoint hostIp = new IPEndPoint(IPAddress.Any, 0);
            byte[] received = udpStream.EndReceive(data, ref hostIp);
            Message receivedMessage;

            using (var bufferStream = new MemoryStream(received))
            {
                 receivedMessage = (Message)binarySerializer.Deserialize(bufferStream);
            }

            if (OnDataReceived != null)
            {
                OnDataReceived(this, new MessageEventArgs() { MessageContent = receivedMessage });
            }

            udpStream.BeginReceive(ReceivedData, null);
        }
    }
}
