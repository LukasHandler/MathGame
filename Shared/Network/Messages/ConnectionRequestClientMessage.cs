﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Data.Messages
{
    [Serializable]
    public class ConnectionRequestClientMessage : Message
    {
        public string PlayerName { get; set; }

        public override void ProcessMessage(IMessageVisitor processor)
        {
            processor.ProcessMessage(this);
        }
    }
}