﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Data.Messages
{
    [Serializable]
    public class BroadcastResponseMessage : Message
    {
        public BroadcastMessage MessageToBroadcast { get; set; }

        public List<object> ClientsInformation { get; set; }

        public override void ProcessMessage(IMessageVisitor processor)
        {
            processor.ProcessMessage(this);
        }

        public override string ToString()
        {
            return "Broadcast-Response-Message";
        }
    }
}
