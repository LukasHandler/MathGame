using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Application
{
    public class Monitor
    {
        public object TargetInformation { get; set; }

        public Monitor(object targetInformation)
        {
            this.TargetInformation = targetInformation;
        }

        public override string ToString()
        {
            return string.Format("Monitor ({0})", TargetInformation.ToString());
        }
    }
}
