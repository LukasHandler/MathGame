//-----------------------------------------------------------------------
// <copyright file="TcpServerManager.cs" company="Lukas Handler">
//     Lukas Handler
// </copyright>
// <summary>
// This file represents the TCP server manager.
// </summary>
//-----------------------------------------------------------------------
namespace Shared.Data.Managers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;

    /// <summary>
    /// This class represents the TCP server manager.
    /// </summary>
    /// <seealso cref="Shared.Data.Managers.TcpManager" />
    public class TcpServerManager : TcpManager
    {
        /// <summary>
        /// The clients.
        /// </summary>
        private Dictionary<IPEndPoint, List<TcpClient>> clients;

        /// <summary>
        /// The TCP listener.
        /// </summary>
        private TcpListener tcpListener;

        /// <summary>
        /// The local end point of the server.
        /// </summary>
        private IPEndPoint localEndPoint;

        /// <summary>
        /// The port of the server.
        /// </summary>
        private int port;

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpServerManager"/> class.
        /// </summary>
        /// <param name="port">The server port.</param>
        public TcpServerManager(int port)
        {
            this.port = port;
            this.localEndPoint = new IPEndPoint(IPAddress.Any, port);
            this.clients = new Dictionary<IPEndPoint, List<TcpClient>>();
            this.tcpListener = new TcpListener(this.localEndPoint);
            this.tcpListener.Start();
            this.AcceptClients();
        }

        /// <summary>
        /// Sends the data.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="target">The target.</param>
        protected override void SendData(byte[] bytes, IPEndPoint target)
        {
            if (this.clients.ContainsKey(target) && this.clients[target].Count > 0)
            {
                try
                {
                this.clients[target].Last().GetStream().Write(bytes, 0, bytes.Length);
                }
                catch (Exception)
                {
                    return;
                }
            }
        }

        /// <summary>
        /// Disconnects the specified target.
        /// </summary>
        /// <param name="target">The target.</param>
        protected override void Disconnect(IPEndPoint target)
        {
            if (this.clients.ContainsKey(target))
            {
                if (this.clients[target].Count > 0)
                {
                    this.clients[target].RemoveAt(this.clients[target].Count - 1);
                }
            }
        }

        /// <summary>
        /// Connects the specified target.
        /// </summary>
        /// <param name="target">The target.</param>
        protected override void Connect(IPEndPoint target)
        {
            TcpClient client = new TcpClient();

            try
            {
                client.Connect(target);
                this.AddClient(client, target);
                this.StartReading(client.GetStream(), target);
            }
            catch (Exception)
            {
                return;
            }
        }

        /// <summary>
        /// Accepts clients.
        /// </summary>
        private async void AcceptClients()
        {
            while (true)
            {
                var client = await this.tcpListener.AcceptTcpClientAsync();
                this.AddClient(client, (IPEndPoint)client.Client.RemoteEndPoint);
                this.StartReading(client.GetStream(), (IPEndPoint)client.Client.RemoteEndPoint);
            }
        }

        /// <summary>
        /// Add a client.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="target">The target information.</param>
        private void AddClient(TcpClient client, IPEndPoint target)
        {
            if (this.clients.ContainsKey(target))
            {
                this.clients[target].Add(client);
            }
            else
            {
                this.clients.Add(target, new List<TcpClient>() { client });
            }
        }
    }
}
