using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Application
{
    public abstract class GameMember
    {
        public object TargetInformation { get; set; }

        public GameMember(object targetInformation)
        {
            this.TargetInformation = targetInformation;
        }
    }
}
