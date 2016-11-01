using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Application
{
    public class Monitor : GameMember
    {
        public Monitor(object targetInformation) : base(targetInformation) { }

        public override string ToString()
        {
            return string.Format("Monitor ({0})", TargetInformation.ToString());
        }
    }
}
