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

        public event EventHandler<MessageEventArgs> OnDataReceived;

        public void WriteData(Message data, object target)
        {
            var targetEndpoint = (IPEndPoint)target;

            if (udpStream == null || !IPEndPoint.Equals((IPEndPoint)udpStream.Client.RemoteEndPoint, targetEndpoint))
            {
                this.udpStream = new UdpClient();
                udpStream.Connect(targetEndpoint);
                udpStream.BeginReceive(ReceivedData, null);
            }

            byte[] bytes = MessageByteConverter.ConvertToBytes(data);
            udpStream.Send(bytes, bytes.Length);
        }

        //http://stackoverflow.com/questions/7266101/receive-messages-continuously-using-udpclient
        private void ReceivedData(IAsyncResult data)
        {
            IPEndPoint hostIp = new IPEndPoint(IPAddress.Any, 0);
            byte[] received = udpStream.EndReceive(data, ref hostIp);
            byte[] toConvertBuffer = new byte[received.Count()];
            Array.Copy(received, toConvertBuffer, received.Length);
            udpStream.BeginReceive(ReceivedData, null);

            Message receivedMessage = MessageByteConverter.ConvertToMessage(toConvertBuffer);

            if (OnDataReceived != null)
            {
                OnDataReceived(this, new MessageEventArgs(receivedMessage));
            }
        }
    }
}
