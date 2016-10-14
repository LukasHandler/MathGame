using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Data.Messages
{
    [Serializable]
    public class Question : Message
    {
        public int QuestionID { get; set; }

        public string QuestionText { get; set; }

        public int Time { get; set; }
    }
}
