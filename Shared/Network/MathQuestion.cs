using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Data
{
    public class MathQuestion
    {
        public int ID { get; set; }

        public string Question { get; set; }

        public int Answer { get; set; }

        public int Time { get; set; }

        public override bool Equals(object obj)
        {
            MathQuestion toCompare = (MathQuestion)obj;

            if (this.Question == toCompare.Question && this.Time == toCompare.Time)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}