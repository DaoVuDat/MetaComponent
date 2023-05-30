using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MetaComponent.Solvers
{
    public interface GrasshopperActions
    {
        bool UpdateGrasshopperSlider(List<decimal> positions);
        double GetGrasshopperObjectiveValue();
    }

    public interface UIActions
    {
        void UpdateUI(string positionString, string fitnessString);
    }

    public class Solver
    {
        private int maxIteration;
        private int population;
        private string algorithm;
        private MetaComponent component;

        private GrasshopperActions actions;
        private UIActions uIActions;

        public List<decimal> Positions { get; set; }
        public List<double> PositionsDouble { get; set; }
        public string PositionsString { get; set; }
        public double Fitness { get; set; }
        public string FitnessString { get; set; }

        private Random random;

        public Solver(int maxIteration, int population, string algorithm, MetaComponent component, GrasshopperActions actions, UIActions uIActions)
        {
            this.maxIteration = maxIteration;
            this.population = population;
            this.algorithm = algorithm;
            this.component = component;
            this.random = new Random();
            this.actions = actions;
            this.uIActions = uIActions;
            this.Positions = new List<decimal>();
            this.PositionsDouble = new List<double>();
        }

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
            var fitness = component.grasshopperInput.GetObjectiveValue();

            // Generate Random Variables
            for (var i = 0; i < variables.Count(); i++)
            {
                // Get lowerbound and upperbound
                var lb = Convert.ToDouble(variables[i].LowerB);
                var ub = Convert.ToDouble(variables[i].UpperB);

                // Random a new position in a dimension
                var newPos = random.NextDouble() * (ub - lb) + lb;


                PositionsDouble.Add(newPos); // Test

                // Convert to decimal
                Positions.Add(Convert.ToDecimal(newPos));
            }

            // Update Grasshopper Slider
            actions.UpdateGrasshopperSlider(Positions);

            // And Get the objective
            fitness = actions.GetGrasshopperObjectiveValue();

            // Write value
            PositionsString = "New Pos: ";
            foreach (var pos in PositionsDouble)
            {
                PositionsString += Math.Round(pos, 3) + ", ";
            }

            FitnessString = fitness.ToString() + " - I = " + maxIteration + " - P = " + population + " - A = " + algorithm;
            
            uIActions.UpdateUI(PositionsString, FitnessString);
        }
    }
}
