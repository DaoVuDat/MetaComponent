using MetaComponent.Models;
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

            for(var iter  = 0; iter < maxIteration; iter++)
            {
                if(backgroundWorker.IsBusy) { 
                // Check for cancelation in background thread from user
                    if (backgroundWorker.CancellationPending)
                    {
                        doWork.Cancel = true;
                        return;
                    }
                    else {
                        Positions = new List<decimal>();
                        // Generate Random Variables
                        for (var i = 0; i < variables.Count(); i++)
                        {
                            // Get lowerbound and upperbound
                            var lb = Convert.ToDouble(variables[i].LowerB);
                            var ub = Convert.ToDouble(variables[i].UpperB);

                            // Random a new position in a dimension
                            var newPos = random.NextDouble() * (ub - lb) + lb;

                            // Convert to decimal
                            Positions.Add(Convert.ToDecimal(newPos));
                        }

                        // Update Grasshopper Slider
                        actions.UpdateGrasshopperSlider(Positions);

                        // And Get the objective
                        Fitness = actions.GetGrasshopperObjectiveValue();
                        Thread.Sleep(1500);

                        results.Add(new OptimizationResult(
                            positions: Positions,
                            fitness: Fitness
                        ));

                        // Update UI
                        backgroundWorker.ReportProgress(100, results);
                    }
                }
            }
        }
    }
}
