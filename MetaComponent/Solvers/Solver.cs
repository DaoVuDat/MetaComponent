using MetaComponent.Models;
using MetaComponent.Solvers.Algorithms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MetaComponent.Solvers
{
    public interface GrasshopperActions
    {
        bool UpdateGrasshopperSlider(List<decimal> positions);
        double GetGrasshopperObjectiveValue();
    }


    public class Solver
    {
        private int maxIteration;
        private int population;
        private string algorithm;
        private MetaComponent component;

        private GrasshopperActions actions;

        private BackgroundWorker backgroundWorker;
        private DoWorkEventArgs doWork;

        public List<decimal> Positions { get; set; }
        public double Fitness { get; set; }

        public List<OptimizationResult> results;

        private Random random;

        public Solver(
            int maxIteration, 
            int population, 
            string algorithm, 
            MetaComponent component, 
            GrasshopperActions actions, 
            BackgroundWorker backgroundWorker,
            DoWorkEventArgs doWork)
        {
            this.maxIteration = maxIteration;
            this.population = population;
            this.algorithm = algorithm;
            this.component = component;
            this.random = new Random();
            this.actions = actions;
            this.Positions = new List<decimal>();
            this.backgroundWorker = backgroundWorker;
            this.doWork = doWork;
            this.results = new List<OptimizationResult>();
        }

        // Report progress for updating UI
        public void ReportProgressFromBackground(OptimizationResult result)
        {
            results.Add(result);
            // Update UI
            backgroundWorker.ReportProgress(100, results);
        }

        // Get objective value
        public double GetObjectiveValueFromGrasshopper(double[] positions)
        {
            // Update Slider
            actions.UpdateGrasshopperSlider(positions.Select(pos => Convert.ToDecimal(pos)).ToList());

            return actions.GetGrasshopperObjectiveValue();
        }

        // This heavy calculation must be run in background
        public void Solve()
        {
            //Setup Variables => return value of Variables
            component.GrasshopperInput_Instantiate();
            if (!component.grasshopperInput.SetInputs() || !component.grasshopperInput.SetOutput())
            {
                MessageBox.Show("Getting Variables and/or Objective failed", "Get Values from GrassHopper Error");
                return;
            }
            // Get Positions
            var variables = component.grasshopperInput.Variables;

            // Create Lower bound and Upper bound
            var lb = new List<double>();
            var ub = new List<double>();
            for (var i = 0; i < variables.Count(); i++)
            {
                // Get lowerbound and upperbound
                lb.Add(Convert.ToDouble(variables[i].LowerB));
                ub.Add(Convert.ToDouble(variables[i].UpperB));
            }

            if(algorithm == "AVOA")
            {
                // Create AVOA Algorithm
                var avoa = new AVOA(
                    lb.ToArray(),
                    ub.ToArray(),
                    population,
                    maxIteration,
                    GetObjectiveValueFromGrasshopper,
                    ReportProgressFromBackground,
                    backgroundWorker,
                    doWork);

                avoa.Solve();
            } else if (algorithm == "GWO")
            {

            }
            


            
        }
    }
}
