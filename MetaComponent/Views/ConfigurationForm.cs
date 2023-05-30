using MetaComponent.Models;
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
    public partial class ConfigurationForm : Form, GrasshopperActions
    {
        private readonly MetaComponent _component;
        private BackgroundWorker _backgroundWorker;
        private Solver solver;
        private List<OptimizationResult> results;

        public ConfigurationForm(MetaComponent component)
        {
            InitializeComponent();

            _component = component;
            results = new List<OptimizationResult>();

            InitializeBackgroundWorker();
            
        }


        // Background Worker
        private void InitializeBackgroundWorker()
        {
            _backgroundWorker.WorkerReportsProgress = true;
            _backgroundWorker.WorkerSupportsCancellation = true;
            _backgroundWorker.DoWork += new DoWorkEventHandler(_handleOptimization);
            _backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_handleComplete);
            _backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(_handleProgress);
        }

        private void _handleProgress(object sender, ProgressChangedEventArgs e)
        {
            results = e.UserState as List<OptimizationResult>;

            // Get latest results
            UpdateUI(results.Count().ToString(), results.Last().Fitness.ToString());
        }

        private void _handleComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            if(e.Error != null)
            {
                // Handle Error
                MessageBox.Show(e.Error.Message);
            }
            else if (e.Cancelled)
            {
                // Handle Cancelled Work
                MessageBox.Show(results.Last().Fitness.ToString(), "Cancelled");

            }
            else
            {
                // Handle Finished Work
                solver = e.Result as Solver;

                // Get result from Solver
                results = solver.results;

                SetupButtonCompleteState();
                MessageBox.Show(results.Last().Fitness.ToString(), "Completed");

            }
        }

        private void _handleOptimization(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            var dictionary = e.Argument as Dictionary<string, object>;

            solver = new Solver(
                (int)dictionary["MaxIteration"],
                (int)dictionary["Population"],
                (string)dictionary["Algorithm"], 
                _component, 
                this, 
                worker,
                e);
            solver.Solve();

            // Pass the Result to handleComplete after heavy calculation
            e.Result = solver;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            SetupButtonStartState();

            // Get value from UI
            var maxIteration = Convert.ToInt32(numberMaxIter.Value);
            var population = Convert.ToInt32(numberPop.Value);
            var algorithm = GetAlgorithm();

            var dictionary = new Dictionary<string, object>
            {
                { "MaxIteration", maxIteration },
                { "Population", population },
                { "Algorithm", algorithm }
            };

            _backgroundWorker.RunWorkerAsync(dictionary);
   
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            // Cancel the asynchronous operation.
            this._backgroundWorker.CancelAsync();

            SetupButtonCompleteState();
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
            lbIteration.Text = "Iteration: " + positionString;
            lbFitness.Text = "Fitness: " + fitnessString;
            lbIteration.Invalidate();
            lbIteration.Update();
            lbFitness.Invalidate();
            lbFitness.Update();
        }

        public void SetupButtonStartState()
        {
            btnStart.Enabled = false;
            cbAlgorithms.Enabled = false;
            numberMaxIter.Enabled = false;
            numberPop.Enabled = false;
            btnStop.Enabled = true;
        }

        public void SetupButtonCompleteState()
        {
            btnStart.Enabled = true;
            cbAlgorithms.Enabled = true;
            numberMaxIter.Enabled = true;
            numberPop.Enabled = true;
            btnStop.Enabled = false;
        }


    }
}
