//-----------------------------------------------------------------------
// <copyright file="TcpManager.cs" company="Lukas Handler">
//     Lukas Handler
// </copyright>
// <summary>
// This file represents the TCP manager. TCP client and server inherit from this.
// </summary>
//-----------------------------------------------------------------------
namespace Shared.Data.Managers
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using EventArguments;
    using Messages;

    /// <summary>
    /// This class represents the TCP manager.
    /// </summary>
    /// <seealso cref="Shared.Data.IDataManager" />
    public abstract class TcpManager : IDataManager
    {
        /// <summary>
        /// Occurs when the manager received data.
        /// </summary>
        public event EventHandler<MessageEventArgs> OnDataReceived;

        /// <summary>
        /// Registers to the specified target.
        /// </summary>
        /// <param name="target">The target.</param>
        public void Register(object target)
        {
            this.Connect((IPEndPoint)target);
        }

        /// <summary>
        /// Unregisters from the specified target.
        /// </summary>
        /// <param name="target">The target.</param>
        public void Unregister(object target)
        {
            this.Disconnect((IPEndPoint)target);
        }

        /// <summary>
        /// Writes the data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="target">The target.</param>
        public void WriteData(Message data, object target)
        {
            byte[] bytes = MessageByteConverter.ConvertToBytes(data);
            byte[] size = BitConverter.GetBytes(bytes.Count());

            byte[] fullData = new byte[bytes.Count() + size.Count()];

            var s = new MemoryStream();
            s.Write(size, 0, size.Length);
            s.Write(bytes, 0, bytes.Length);
            fullData = s.ToArray();
            s.Dispose();

            this.SendData(fullData, (IPEndPoint)target);
        }

        /// <summary>
        /// Sends the data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="target">The target.</param>
        protected abstract void SendData(byte[] data, IPEndPoint target);

        /// <summary>
        /// Disconnects the specified target.
        /// </summary>
        /// <param name="target">The target.</param>
        protected abstract void Disconnect(IPEndPoint target);

        /// <summary>
        /// Connects the specified target.
        /// </summary>
        /// <param name="target">The target.</param>
        protected abstract void Connect(IPEndPoint target);

        /// <summary>
        /// Starts the reading for receiving messages.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="target">The target.</param>
        protected void StartReading(NetworkStream stream, IPEndPoint target)
        {
            byte[] byteSize = new byte[4];
            byte[] buffer = null;
            AsyncCallback callback = null;

            AsyncCallback lengthCallback = delegate(IAsyncResult result)
            {
                int bytesRead;

                try
                {
                    bytesRead = stream.EndRead(result);
                }
                catch (Exception)
                {
                    return;
                }

                buffer = new byte[BitConverter.ToInt32(byteSize, 0)];
                
                stream.BeginRead(buffer, 0, buffer.Length, callback, null);
            };

            callback = delegate(IAsyncResult result)
            {
                int bytesRead;

                try
                {
                    bytesRead = stream.EndRead(result);
                }
                catch (Exception)
                {
                    return;
                }

                if (bytesRead == 0)
                {
                    return;
                }

                // Copy result into new buffer so we can read as soon as possible again - otherwise some messages get lost.
                byte[] toConvertBuffer = new byte[bytesRead];
                Array.Copy(buffer, toConvertBuffer, bytesRead);

                byteSize = new byte[4];
                stream.BeginRead(byteSize, 0, byteSize.Length, lengthCallback, null);

                Message receivedMessage = MessageByteConverter.ConvertToMessage(toConvertBuffer);

                if (this.OnDataReceived != null)
                {
                    this.OnDataReceived(target, new MessageEventArgs(receivedMessage));
                }
            };

            stream.BeginRead(byteSize, 0, byteSize.Length, lengthCallback, null);
        }
    }
}
