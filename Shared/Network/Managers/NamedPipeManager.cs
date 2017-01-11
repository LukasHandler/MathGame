//-----------------------------------------------------------------------
// <copyright file="NamedPipeManager.cs" company="Lukas Handler">
//     Lukas Handler
// </copyright>
// <summary>
// This file represents the named pipe manager. Each client also starts a named pipe server to receive messages.
// </summary>
//-----------------------------------------------------------------------
namespace Shared.Data.Managers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Pipes;
    using System.Linq;
    using EventArguments;
    using Messages;

    /// <summary>
    /// This class represents the named pipe manager.
    /// </summary>
    /// <seealso cref="Shared.Data.IDataManager" />
    public class NamedPipeManager : IDataManager
    {
        /// <summary>
        /// The server name for receiving messages. The server uses his real name, clients use a guid.
        /// </summary>
        private string serverName;

        /// <summary>
        /// The locker to synchronize send operations.
        /// </summary>
        private object locker;

        /// <summary>
        /// The clients of the named pipes (necessary for the server).
        /// </summary>
        private Dictionary<string, NamedPipeClientStream> clients;

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedPipeManager"/> class.
        /// </summary>
        /// <param name="serverPipeName">Name of the server pipe.</param>
        public NamedPipeManager(string serverPipeName)
        {
            this.locker = new object();
            this.serverName = serverPipeName;
            this.clients = new Dictionary<string, NamedPipeClientStream>();
            var server = new NamedPipeServerStream(serverPipeName, PipeDirection.In, 254, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
            server.BeginWaitForConnection(this.ConnectionFound, server);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedPipeManager"/> class.
        /// </summary>
        public NamedPipeManager() : this(Guid.NewGuid().ToString())
        {
        }

        /// <summary>
        /// Occurs when data was received.
        /// </summary>
        public event EventHandler<MessageEventArgs> OnDataReceived;

        /// <summary>
        /// Registers to the specified target.
        /// </summary>
        /// <param name="target">The target.</param>
        public void Register(object target)
        {
            var targetPipe = (string)target;

            lock (this.locker)
            {
                if (!this.clients.ContainsKey(targetPipe))
                {
                    var newClient = new NamedPipeClientStream(".", targetPipe, PipeDirection.Out, PipeOptions.Asynchronous);
                    this.clients.Add(targetPipe, newClient);
                    newClient.Connect(3000);
                    newClient.ReadMode = PipeTransmissionMode.Message;
                }
            }
        }

        /// <summary>
        /// Unregisters from the specified target.
        /// </summary>
        /// <param name="target">The target.</param>
        public void Unregister(object target)
        {
            var targetPipe = (string)target;

            if (this.clients.ContainsKey(targetPipe))
            {
                var client = this.clients[targetPipe];
                client.Dispose();
                this.clients.Remove(targetPipe);
            }
        }

        /// <summary>
        /// Writes the data.
        /// </summary>
        /// <param name="data">The data to send.</param>
        /// <param name="target">The target.</param>
        public void WriteData(Message data, object target)
        {
            var targetPipe = (string)target;

            if (!this.clients.ContainsKey(targetPipe))
            {
                this.Register(target);
            }

            if (this.clients.ContainsKey(targetPipe))
            {
                byte[] bytes = MessageByteConverter.ConvertToBytes(data);
                byte[] senderPipeName = System.Text.ASCIIEncoding.ASCII.GetBytes(this.serverName);
                byte[] senderPipeNameSize = BitConverter.GetBytes(senderPipeName.Count());

                var innerSize = senderPipeNameSize.Count() + senderPipeName.Count() + bytes.Count();
                byte[] payloadSize = BitConverter.GetBytes(innerSize);

                byte[] payload = new byte[payloadSize.Count() + innerSize];

                var s = new MemoryStream();
                s.Write(payloadSize, 0, payloadSize.Length);
                s.Write(senderPipeNameSize, 0, senderPipeNameSize.Length);
                s.Write(senderPipeName, 0, senderPipeName.Length);
                s.Write(bytes, 0, bytes.Length);
                payload = s.ToArray();
                s.Dispose();

                var client = this.clients[targetPipe];

                lock (this.locker)
                {
                    client.Write(payload, 0, payload.Count());
                }
            }
        }

        /// <summary>
        /// Happens when a connection was found. Create a new named pipe server stream with the same name to receive more clients.
        /// </summary>
        /// <param name="asyncResult">The async result.</param>
        private void ConnectionFound(IAsyncResult asyncResult)
        {
            NamedPipeServerStream server = (NamedPipeServerStream)asyncResult.AsyncState;
            server.EndWaitForConnection(asyncResult);

            // Create new server stream.
            var server2 = new NamedPipeServerStream(this.serverName, PipeDirection.In, 254, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
            server2.BeginWaitForConnection(this.ConnectionFound, server2);
            byte[] myBuffer = new byte[1000];
            byte[] payloadSize = new byte[4];
            byte[] payload = null;
            AsyncCallback readSizeCallback = null;

            // Read the full payload, happens after reading the payload size.
            AsyncCallback readPayloadCallback = delegate(IAsyncResult asyncResultPayload)
            {
                int bytesRead = server.EndRead(asyncResultPayload);
                byte[] copyPayload = new byte[bytesRead];
                Array.Copy(payload, copyPayload, bytesRead);
                server.BeginRead(payloadSize, 0, payloadSize.Length, readSizeCallback, null);

                byte[] senderPipeNameSizeBytes = copyPayload.Take(4).ToArray();
                int senderPipeNameSize = BitConverter.ToInt32(senderPipeNameSizeBytes, 0);

                byte[] senderPipeNameBytes = copyPayload.Skip(4).Take(senderPipeNameSize).ToArray();
                string sender = System.Text.ASCIIEncoding.ASCII.GetString(senderPipeNameBytes);

                int messageStartIndex = 4 + senderPipeNameBytes.Count();
                byte[] message = copyPayload.Skip(messageStartIndex).Take(copyPayload.Count()).ToArray();

                Message receivedMessage = MessageByteConverter.ConvertToMessage(message);
                this.OnDataReceived?.Invoke(sender, new MessageEventArgs(receivedMessage));
            };

            // Read the payload size.
            readSizeCallback = delegate(IAsyncResult asynResultSize)
            {
                server.EndRead(asynResultSize);
                payload = new byte[BitConverter.ToInt32(payloadSize, 0)];
                server.BeginRead(payload, 0, payload.Length, readPayloadCallback, null);
            };

            server.BeginRead(payloadSize, 0, payloadSize.Length, readSizeCallback, null);
        }
    }
}
