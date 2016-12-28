using Server.Application.EventArguments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Application
{
    public class Server : SystemElement
    {
        public string Name { get; set; }

        public List<object> TargetInformation { get; set; }

        public Server(string name, object targetInformation)
        {
            this.Name = name;
            this.TargetInformation = new List<object>();
            this.TargetInformation.Add(targetInformation);
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
