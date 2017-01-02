//-----------------------------------------------------------------------
// <copyright file="UdpClientManager.cs" company="Lukas Handler">
//     Lukas Handler
// </copyright>
// <summary>
// This file represents the UDP client manager.
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
    /// This class represents the UDP client manager.
    /// </summary>
    /// <seealso cref="Shared.Data.IDataManager" />
    public class UdpClientManager : IDataManager
    {
        /// <summary>
        /// The UDP stream to send and receive messages.
        /// </summary>
        private UdpClient udpStream;

        /// <summary>
        /// Indicates if receiving already started.
        /// </summary>
        private bool startedReceiving;

        /// <summary>
        /// Initializes a new instance of the <see cref="UdpClientManager"/> class.
        /// </summary>
        public UdpClientManager()
        {
            this.udpStream = new UdpClient();
            this.startedReceiving = false;
        }

        /// <summary>
        /// Occurs when the manager received data.
        /// </summary>
        public event EventHandler<MessageEventArgs> OnDataReceived;

        /// <summary>
        /// Writes the data.
        /// </summary>
        /// <param name="data">The data to send.</param>
        /// <param name="target">The target.</param>
        public void WriteData(Message data, object target)
        {
            byte[] bytes = MessageByteConverter.ConvertToBytes(data);
            this.udpStream.Send(bytes, bytes.Length, (IPEndPoint)target);
            if (!this.startedReceiving)
            {
                this.startedReceiving = true;
                this.udpStream.BeginReceive(this.ReceivedData, null);
            }
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
            // Not needed for UPD.   
        }

        /// <summary>
        /// Receives the data.
        /// </summary>
        /// <param name="asyncResult">The async result.</param>
        private void ReceivedData(IAsyncResult asyncResult)
        {
            IPEndPoint senderIp = new IPEndPoint(IPAddress.Any, 0);
            byte[] received = this.udpStream.EndReceive(asyncResult, ref senderIp);
            this.udpStream.BeginReceive(this.ReceivedData, null);

            Message receivedMessage = MessageByteConverter.ConvertToMessage(received);

            if (this.OnDataReceived != null)
            {
                this.OnDataReceived(senderIp, new MessageEventArgs(receivedMessage));
            }
        }
    }
}
