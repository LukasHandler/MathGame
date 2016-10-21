using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Data.Messages
{
    [Serializable]
    public class ScoresResponseMessage : Message
    {
        public List<ScoreEntry> Scores { get; set; }

        public override void ProcessMessage(IMessageVisitor processor)
        {
            processor.ProcessMessage(this);
        }
    }
}
