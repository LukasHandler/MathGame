using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Data.Messages
{
    //public enum StatusCode
    //{
    //    ConnectionRequest,
    //    ConnectionRefused,
    //    ConnectionEstablished,
    //    Question,
    //    Answer,
    //    AskScores,
    //    SendScores
    //}

    [Serializable]
    public class Message
    {
        public IPAddress SenderIp { get; set; }

        public int SenderPort { get; set; }

        public IPAddress RecipientIp { get; set; }

        public int RecipientPort { get; set; }
    }
}
