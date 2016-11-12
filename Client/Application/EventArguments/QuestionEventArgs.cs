using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Application.EventArguments
{
    public class QuestionEventArgs : EventArgs
    {
        public string QuestionText { get; set; }

        public int Time { get; set; }

        public int Score { get; set; }

        public QuestionEventArgs(string questionText, int time, int score)
        {
            this.QuestionText = questionText;
            this.Time = time;
            this.Score = score;
        }
    }
}
