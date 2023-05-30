using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaComponent.Models
{
    public class OptimizationResult
    {
        public double[] Positions { get; set; }
        public double Fitness { get; set; }

        public OptimizationResult(List<decimal> positions, double fitness)
        {
            Positions = positions.Select(v => Convert.ToDouble(v)).ToArray();
            Fitness = fitness;
        }


    }

}
