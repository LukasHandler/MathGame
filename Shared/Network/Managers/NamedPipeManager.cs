using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Data.EventArguments;
using Shared.Data.Messages;
using System.IO.Pipes;
using System.IO;

namespace Shared.Data.Managers
{
    public class NamedPipeManager : IDataManager
    {
        private string serverName;

        private Dictionary<string, NamedPipeClientStream> clients;

        public event EventHandler<MessageEventArgs> OnDataReceived;


        public NamedPipeManager(string serverPipeName)
        {
            this.serverName = serverPipeName;
            this.clients = new Dictionary<string, NamedPipeClientStream>();
            var server = new NamedPipeServerStream(serverPipeName, PipeDirection.In, 254, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
            server.BeginWaitForConnection(connectionFound, server);
        }

        public NamedPipeManager() : this(Guid.NewGuid().ToString()) { }

        private void connectionFound(IAsyncResult ar)
        {
            NamedPipeServerStream server = (NamedPipeServerStream)ar.AsyncState;
            server.EndWaitForConnection(ar);

            var server2 = new NamedPipeServerStream(serverName, PipeDirection.In, 254, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
            server2.BeginWaitForConnection(connectionFound, server2);

            byte[] myBuffer = new byte[1000];
            Tuple<NamedPipeServerStream, byte[]> tuple = new Tuple<NamedPipeServerStream, byte[]>(server, myBuffer);

            while (true)
            {
                byte[] payloadSize = new byte[4];
                server.Read(payloadSize, 0, payloadSize.Length);
                byte[] payload = new byte[BitConverter.ToInt32(payloadSize, 0)];

                int bytesRead = server.Read(payload, 0, payload.Length);

                byte[] copyPayload = new byte[bytesRead];
                Array.Copy(payload, copyPayload, bytesRead);

                byte[] senderPipeNameSizeBytes = copyPayload.Take(4).ToArray();
                int senderPipeNameSize = BitConverter.ToInt32(senderPipeNameSizeBytes, 0);

                byte[] senderPipeNameBytes = copyPayload.Skip(4).Take(senderPipeNameSize).ToArray();
                string sender = System.Text.ASCIIEncoding.ASCII.GetString(senderPipeNameBytes);

                int messageStartIndex = 4 + senderPipeNameBytes.Count();
                byte[] message = copyPayload.Skip(messageStartIndex).Take(copyPayload.Count()).ToArray();

                Message receivedMessage = MessageByteConverter.ConvertToMessage(message);
                OnDataReceived?.Invoke(sender, new MessageEventArgs(receivedMessage));
            }
        }

        public void Register(object target)
        {
            var targetPipe = (string)target;

            if (!this.clients.ContainsKey(targetPipe))
            {
                var newClient = new NamedPipeClientStream(".", targetPipe, PipeDirection.Out, PipeOptions.Asynchronous);
                newClient.Connect();
                newClient.ReadMode = PipeTransmissionMode.Message;
                clients.Add(targetPipe, newClient);
            }
        }

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
                client.Write(payload, 0, payload.Count());
            }
        }
    }
}
