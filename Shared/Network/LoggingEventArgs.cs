using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Data
{
    public class LoggingEventArgs : EventArgs
    {
        public string LoggingText { get; set; }

        public LoggingEventArgs(string loggingText)
        {
            this.LoggingText = loggingText;
        }
    }
}
