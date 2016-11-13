using Shared.Data.Messages;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System;

namespace Shared.Data.Managers
{
    public class TcpServerManager : TcpManager
    {
        private Dictionary<IPEndPoint, List<TcpClient>> clients;

        private TcpListener tcpListener;

        private IPEndPoint localEndPoint;

        public TcpServerManager(int port)
        {
            this.localEndPoint = new IPEndPoint(IPAddress.Any, port);
            this.clients = new Dictionary<IPEndPoint, List<TcpClient>>();

            tcpListener = new TcpListener(localEndPoint);

            tcpListener.Start();
            AcceptClients();
        }

        private async void AcceptClients()
        {
            while (true)
            {
                var client = await tcpListener.AcceptTcpClientAsync();

                AddClient(client);

                this.StartReading(client.GetStream(), (IPEndPoint)client.Client.RemoteEndPoint);
            }
        }

        private void AddClient(TcpClient client)
        {
            var clientEndPoint = (IPEndPoint)client.Client.RemoteEndPoint;

            if (this.clients.ContainsKey(clientEndPoint))
            {
                this.clients[clientEndPoint].Add(client);
            }
            else
            {
                this.clients.Add(clientEndPoint, new List<TcpClient>() { client });
            }
        }

        protected override void SendData(byte[] bytes, IPEndPoint target)
        {
            if (this.clients.ContainsKey(target) && this.clients[target].Count > 0)
            {
                this.clients[target].First().GetStream().Write(bytes, 0, bytes.Length);
            }
        }

        protected override void Disconnect(IPEndPoint target)
        {
            if (this.clients.ContainsKey(target))
            {
                if (this.clients[target].Count > 0)
                {
                    this.clients[target].RemoveAt(0);
                }
            }
        }

        protected override void Connect(IPEndPoint target)
        {
            TcpClient client = new TcpClient();

            try
            {
                client.Connect(target);
                this.AddClient(client);
                this.StartReading(client.GetStream(), target);
            }
            catch (Exception)
            {
                return;
            }
        }
    }
}
