using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Data.Messages
{
    [Serializable]
    public class QuestionMessage : Message
    {
        public int QuestionID { get; set; }

        public string QuestionText { get; set; }

        public int Score { get; set; }

        public int Time { get; set; }

        public override void ProcessMessage(IMessageVisitor processor)
        {
            processor.ProcessMessage(this);
        }
    }
}
