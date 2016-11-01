using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Application
{
    public class Server : GameMember
    {
        public string Name { get; set; }

        public Server(string name, object targetInformation) : base(targetInformation)
        {
            this.Name = name;
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
