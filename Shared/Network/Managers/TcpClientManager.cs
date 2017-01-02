//-----------------------------------------------------------------------
// <copyright file="TcpClientManager.cs" company="Lukas Handler">
//     Lukas Handler
// </copyright>
// <summary>
// This file represents the TCP client manager.
// </summary>
//-----------------------------------------------------------------------
namespace Shared.Data.Managers
{
    using System.Net;
    using System.Net.Sockets;

    /// <summary>
    /// This class represents the TCP client manager.
    /// </summary>
    /// <seealso cref="Shared.Data.Managers.TcpManager" />
    public class TcpClientManager : TcpManager
    {
        /// <summary>
        /// The stream to the server.
        /// </summary>
        private NetworkStream stream;

        /// <summary>
        /// Sends the data.
        /// </summary>
        /// <param name="data">The data to send.</param>
        /// <param name="target">The target.</param>
        protected override void SendData(byte[] data, IPEndPoint target)
        {
            if (this.stream != null)
            {
                this.stream.Write(data, 0, data.Length);
            }
        }

        /// <summary>
        /// Disconnects from the specified target.
        /// </summary>
        /// <param name="target">The target.</param>
        protected override void Disconnect(IPEndPoint target)
        {
            this.stream.Dispose();
        }

        /// <summary>
        /// Connects to the specified target.
        /// </summary>
        /// <param name="target">The target.</param>
        protected override void Connect(IPEndPoint target)
        {
            if (this.stream == null)
            {
                var client = new TcpClient();
                client.Connect(target);
                this.stream = client.GetStream();
                this.StartReading(this.stream, target);
            }
        }
    }
}
