﻿using Shared.Data;
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
        private UdpClient receiveClient;
        private UdpClient sendClient;
        private Dictionary<Guid, IPEndPoint> ipAdresses;

        public UdpServerManager(IPEndPoint localEndPoint)
        {
            receiveClient = new UdpClient();
            receiveClient.ExclusiveAddressUse = false;
            receiveClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            receiveClient.Client.Bind(localEndPoint);

            this.ipAdresses = new Dictionary<Guid, IPEndPoint>();
            receiveClient.BeginReceive(ReceivedData, null);

            sendClient = new UdpClient();
            sendClient.ExclusiveAddressUse = false;
            sendClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            sendClient.Client.Bind(localEndPoint);
        }

        private void ReceivedData(IAsyncResult data)
        {
            IPEndPoint senderIp = new IPEndPoint(IPAddress.Any, 0);
            byte[] received = receiveClient.EndReceive(data, ref senderIp);
            receiveClient.BeginReceive(ReceivedData, null);

            Message receivedMessage = MessageByteConverter.ConvertToMessage(received);
            
            if (!this.ipAdresses.ContainsKey(receivedMessage.SenderId))
            {
                this.ipAdresses.Add(receivedMessage.SenderId, senderIp);
            }

            if (OnDataReceived != null)
            {
                OnDataReceived(this, new MessageEventArgs(receivedMessage));
            }
        }

        public event EventHandler<MessageEventArgs> OnDataReceived;

        public void WriteData(Message data, object target)
        {
            var targetEndpoint = this.ipAdresses[(Guid)target];
            byte[] bytes = MessageByteConverter.ConvertToBytes(data);
            sendClient.Connect(targetEndpoint);
            sendClient.Send(bytes, bytes.Length);
        }
    }
}
