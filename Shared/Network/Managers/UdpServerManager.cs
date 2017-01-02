//-----------------------------------------------------------------------
// <copyright file="UdpServerManager.cs" company="Lukas Handler">
//     Lukas Handler
// </copyright>
// <summary>
// This file represents the UDP server manager.
// </summary>
//-----------------------------------------------------------------------
namespace Shared.Data.Managers
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using EventArguments;
    using Messages;

    /// <summary>
    /// This class represents the UDP server manager.
    /// </summary>
    /// <seealso cref="Shared.Data.IDataManager" />
    public class UdpServerManager : IDataManager
    {
        /// <summary>
        /// The UDP receive client.
        /// </summary>
        private UdpClient receiveClient;

        /// <summary>
        /// The UDP send client.
        /// </summary>
        private UdpClient sendClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="UdpServerManager"/> class.
        /// </summary>
        /// <param name="port">The port.</param>
        public UdpServerManager(int port)
        {
            var localEndPoint = new IPEndPoint(IPAddress.Any, port);

            // From: <see href="http://stackoverflow.com/questions/8314168/concurrent-send-and-receive-data-in-one-port-with-udpclient"/>
            this.receiveClient = new UdpClient();
            this.receiveClient.ExclusiveAddressUse = false;
            this.receiveClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            this.receiveClient.Client.Bind(localEndPoint);
            this.receiveClient.BeginReceive(this.ReceivedData, null);

            this.sendClient = new UdpClient();
            this.sendClient.ExclusiveAddressUse = false;
            this.sendClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            this.sendClient.Client.Bind(localEndPoint);
        }

        /// <summary>
        /// Occurs when the manager received data.
        /// </summary>
        public event EventHandler<MessageEventArgs> OnDataReceived;

        /// <summary>
        /// Writes the data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="target">The target.</param>
        public void WriteData(Message data, object target)
        {
            var targetEndpoint = (IPEndPoint)target;
            byte[] bytes = MessageByteConverter.ConvertToBytes(data);
            this.sendClient.Connect(targetEndpoint);
            this.sendClient.Send(bytes, bytes.Length);
        }

        /// <summary>
        /// Registers to the specified target.
        /// </summary>
        /// <param name="target">The target.</param>
        public void Register(object target)
        {
            // Not needed for UDP.
        }

        /// <summary>
        /// Unregisters from the specified target.
        /// </summary>
        /// <param name="target">The target.</param>
        public void Unregister(object target)
        {
            // Not needed for UDP.
        }

        /// <summary>
        /// Receives the data.
        /// </summary>
        /// <param name="data">The data.</param>
        private void ReceivedData(IAsyncResult data)
        {
            IPEndPoint senderIp = new IPEndPoint(IPAddress.Any, 0);
            byte[] received = this.receiveClient.EndReceive(data, ref senderIp);
            this.receiveClient.BeginReceive(this.ReceivedData, null);

            Message receivedMessage = MessageByteConverter.ConvertToMessage(received);

            if (this.OnDataReceived != null)
            {
                this.OnDataReceived(senderIp, new MessageEventArgs(receivedMessage));
            }
        }
    }
}
