using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaComponent.Models
{
    public struct Variable
    {
        public readonly decimal LowerB;
        public readonly decimal UpperB;
        public readonly bool Integer;

        public Variable(decimal lowerB, decimal upperB, bool integer)
        {
            LowerB = lowerB;
            UpperB = upperB;
            Integer = integer;
        }

        public override string ToString()
        {
            return string.Format("LowerB {0} UpperB {1} Integer {2}", LowerB, UpperB, Integer);
        }
    }
}
