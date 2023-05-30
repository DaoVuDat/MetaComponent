using MetaComponent.Solvers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MetaComponent.Views
{
    public partial class ConfigurationForm : Form, GrasshopperActions, UIActions
    {
        private readonly MetaComponent _component;
        private bool running = false;

        public ConfigurationForm(MetaComponent component)
        {
            InitializeComponent();

            _component = component;
            
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            // Set up UI Button
            running = !running;
            if(running)
            {
                btnStart.Text = "Stop";
            } else
            {
                btnStart.Text = "Run";
            }

            // Start the Algorithm
            // If running -> stop

            // Get value from UI
            var maxIteration = Convert.ToInt32(numberMaxIter.Value);
            var population = Convert.ToInt32(numberPop.Value);
            var algorithm = GetAlgorithm();

            var solver = new Solver(maxIteration, population, algorithm, _component, this, this);
            solver.Solve();

            running = !running;
            btnStart.Text = "Run";

            btnStart.Update();
        }

        private string GetAlgorithm()
        {
            switch (cbAlgorithms.SelectedIndex)
            {
                case 0:
                    return "AVOA";
                case 1:
                    return "GWO";
                default:
                    return "AVOA";
            }
        }

        bool GrasshopperActions.UpdateGrasshopperSlider(List<decimal> positions)
        {
            _component.grasshopperInput.NewSolution(positions);
            return true;
        }

        public double GetGrasshopperObjectiveValue()
        {
            return _component.grasshopperInput.GetObjectiveValue();
        }

        public void UpdateUI(string positionString, string fitnessString)
        {
            lbIteration.Text = positionString;
            lbFitness.Text = fitnessString;
            lbIteration.Invalidate();
            lbIteration.Update();
            lbFitness.Invalidate();
            lbFitness.Update();
        }
    }
}
