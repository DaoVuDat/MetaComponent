﻿using MathNet.Numerics;
using MetaComponent.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaComponent.Solvers.Algorithms
{
    public class AVOA
    {
        #region Properties
        public double[] Lb { get; private set; }
        public double[] Ub { get; private set; }
        public int Population { get; private set; }
        public int Itermax { get; private set; }
        public Func<double[], double> objectiveFunction { get; private set; }
        public Action<OptimizationResult> updateUI { get; private set; }
        public double[] Xopt { get; private set; }
        public double Fxopt { get; private set; }

        public double P1 { get; private set; }
        public double P2 { get; private set; }
        public double P3 { get; private set; }
        public double Alpha { get; private set; }
        public double Beta { get; private set; }
        public double Gamma { get; private set; }

        private BackgroundWorker backgroundWorker;
        private DoWorkEventArgs doWork;

        private Dictionary<string, string> topResults;
        private Random rnd;
        #endregion

        #region Constructor
        public AVOA(double[] lb, double[] ub, int population, int itermax, 
            Func<double[], double> evalfnc, 
            Action<OptimizationResult> updateUI,
            BackgroundWorker backgroundWorker,
            DoWorkEventArgs doWork)
        {
            this.Lb = lb;
            this.Ub = ub;
            this.Population = population;
            this.Itermax = itermax;
            this.objectiveFunction = evalfnc;

            // Parameters for AVOA
            this.P1 = 0.6;
            this.P2 = 0.4;
            this.P3 = 0.6;
            this.Alpha = 0.8;
            this.Beta = 0.2;
            this.Gamma = 2.5;

            this.rnd = new Random();

            // For utils
            this.updateUI = updateUI;
            this.backgroundWorker = backgroundWorker;
            this.doWork = doWork;
            this.topResults = new Dictionary<string, string>();
        }
        #endregion

        #region Methods
        public void Solve()
        {
            // Utils
            int dimension = Lb.Length;
            List<double> cumulativeSum = new List<double>();
            cumulativeSum.Add(this.Alpha);
            cumulativeSum.Add(this.Alpha + this.Beta);

            // Store Best values
            StringBuilder storeBestResults = new StringBuilder();
            List<Vurture> vurtures = new List<Vurture>();

            #region Initialization Population
            for (int i = 0; i < this.Population; i++)
            {
                // Create a new wolve
                Vurture vurture = new Vurture(dimension, this.Lb, this.Ub);

                // Calculate fitness
                double fitness = objectiveFunction(vurture.Position);
                vurture.Fitness = fitness;

                vurtures.Add(vurture);
            }
            #endregion

            #region Initialize two best vurtures
            Vurture bestVurture1 = new Vurture(dimension, this.Lb, this.Ub);
            bestVurture1.Fitness = double.MaxValue;

            Vurture bestVurture2 = new Vurture(dimension, this.Lb, this.Ub);
            bestVurture2.Fitness = double.MaxValue;
            #endregion

            #region Optimization Loop
            for (int t = 0; t < Itermax; t++)
            {
                if (backgroundWorker.IsBusy)
                {
                    #region Check for cancelation in background thread from user
                    if (backgroundWorker.CancellationPending)
                    {
                        doWork.Cancel = true;
                        return;
                    }
                    #endregion
                    #region If user does not cancel the calculation
                    else
                    {

                        #region Evaluate Fitness Function and Find Best Vurture 1 and Best Vurture 2
                        foreach (var vurture in vurtures)
                        {

                            for (int dim = 0; dim < dimension; dim++)
                            {
                                double pos = vurture.Position[dim];
                                if (pos > this.Ub[dim])
                                {
                                    vurture.Position[dim] = this.Ub[dim];
                                }

                                if (pos < this.Lb[dim])
                                {
                                    vurture.Position[dim] = this.Lb[dim];
                                }
                            }

                            // Calculate fitness
                            double fitness = objectiveFunction(vurture.Position);
                            vurture.Fitness = fitness;

                            // Add to dictionary
                            if(!topResults.ContainsKey(Math.Round(fitness, 3).ToString()))
                            {
                                topResults.Add(
                                    Math.Round(fitness, 3).ToString(), 
                                    string.Join(";", vurture.Position.Select(x => Math.Round(x, 3)))
                                    );
                            }
                            

                            // Find alpha, beta, delta wolve
                            if (fitness <= bestVurture1.Fitness)
                            {
                                bestVurture1.Position = vurture.CopyPosition();
                                bestVurture1.Fitness = fitness;
                                continue;
                            }

                            if (fitness <= bestVurture2.Fitness)
                            {
                                bestVurture2.Position = vurture.CopyPosition();
                                bestVurture2.Fitness = fitness;
                            }
                        }
                        #endregion

                        #region Calculate Rate of being satiated
                        var randomRangeMinus2To2 = this.rnd.NextDouble() * (2 - (-2)) + (-2);
                        double a = randomRangeMinus2To2 * (
                            Math.Pow(Math.Sin((Math.PI / 2) * (t / (double)this.Itermax)), this.Gamma) +
                            Math.Cos((Math.PI / 2) * (t / this.Itermax)) - 1
                            );

                        double P1 = (2 * this.rnd.NextDouble() + 1) * (1 - (t / (double)this.Itermax)) + a;
                        #endregion

                        #region Update position of each vurture
                        foreach (var vurture in vurtures)
                        {
                            var currentVurturePosition = vurture.Position;
                            var F = P1 * (2 * this.rnd.NextDouble() - 1);

                            #region Select random vurture (bestVurture1 or bestVurture2)
                            var r = this.rnd.NextDouble();
                            var selectIndex = 0;
                            for (var probIndex = 0; probIndex < cumulativeSum.Count; probIndex++)
                            {
                                // Roulette Wheel Selection
                                if (r <= cumulativeSum[probIndex])
                                {
                                    selectIndex = probIndex;
                                }
                            }
                            var randomSelectVurture = selectIndex == 0 ? bestVurture1 : bestVurture2;
                            #endregion

                            #region Exploration
                            if (Math.Abs(F) >= 1)
                            {
                                var rand = this.rnd.NextDouble();
                                #region Update each variable or dimension of vurture (Eq. 6,7)
                                if (rand < this.P1)
                                {
                                    var rand1 = this.rnd.NextDouble();

                                    for (var dim = 0; dim < dimension; dim++)
                                    {
                                        vurture.Position[dim] = randomSelectVurture.Position[dim] - (Math.Abs(2 * rand1 * randomSelectVurture.Position[dim] - vurture.Position[dim])) * F;
                                    }
                                }
                                #endregion
                                #region Update each variable or dimension of vurture (Eq. 8)
                                else
                                {
                                    var rand1 = this.rnd.NextDouble();
                                    var rand2 = this.rnd.NextDouble();
                                    for (var dim = 0; dim < dimension; dim++)
                                    {
                                        vurture.Position[dim] = randomSelectVurture.Position[dim] - F + rand1 * ((this.Ub[dim] - this.Lb[dim]) * rand2 + this.Lb[dim]);
                                    }
                                }
                                #endregion
                            }
                            #endregion
                            #region Exploitation
                            else
                            {
                                #region Phase 1
                                if (Math.Abs(F) >= 0.5)
                                {
                                    var rand = this.rnd.NextDouble();
                                    #region Update each variable or dimension of vurture (Eq. 10)
                                    if (rand < this.P3)
                                    {
                                        var rand1 = this.rnd.NextDouble();
                                        var rand2 = this.rnd.NextDouble();
                                        for (var dim = 0; dim < dimension; dim++)
                                        {
                                            vurture.Position[dim] = Math.Abs(2 * rand1 * randomSelectVurture.Position[dim] - vurture.Position[dim]) * (F + rand2) - (randomSelectVurture.Position[dim] - vurture.Position[dim]);
                                        }
                                    }
                                    #endregion
                                    #region Update each variable or dimension of vurture (Eq. 13)
                                    else
                                    {
                                        var rand1 = this.rnd.NextDouble();
                                        var rand2 = this.rnd.NextDouble();
                                        for (var dim = 0; dim < dimension; dim++)
                                        {
                                            double s1 = randomSelectVurture.Position[dim] * (rand1 * vurture.Position[dim] / (2 * Math.PI)) * Math.Cos(vurture.Position[dim]);
                                            double s2 = randomSelectVurture.Position[dim] * (rand2 * vurture.Position[dim] / (2 * Math.PI)) * Math.Sin(vurture.Position[dim]);
                                            vurture.Position[dim] = randomSelectVurture.Position[dim] - (s1 - s2);
                                        }

                                    }
                                    #endregion
                                }
                                #endregion
                                #region Phase 2
                                else
                                {
                                    var rand = this.rnd.NextDouble();
                                    #region Update each variable or dimension of vurture (Eq. 15, 16)
                                    if (rand < this.P2)
                                    {
                                        for (var dim = 0; dim < dimension; dim++)
                                        {
                                            double A = bestVurture1.Position[dim] - ((bestVurture1.Position[dim] * vurture.Position[dim]) / (bestVurture1.Position[dim] - Math.Pow(vurture.Position[dim], 2))) * F;
                                            double B = bestVurture2.Position[dim] - ((bestVurture2.Position[dim] * vurture.Position[dim]) / (bestVurture2.Position[dim] - Math.Pow(vurture.Position[dim], 2))) * F;
                                            vurture.Position[dim] = (A + B) / 2;
                                        }
                                    }
                                    #endregion
                                    #region Update each variable or dimension of vurture (Eq. 17)
                                    else
                                    {
                                        #region Get values from Levy Flight
                                        var levyFlight = this.LevyFlight(dimension);
                                        #endregion
                                        for (var dim = 0; dim < dimension; dim++)
                                        {
                                            vurture.Position[dim] = randomSelectVurture.Position[dim] - Math.Abs(randomSelectVurture.Position[dim] - vurture.Position[dim]) * F * levyFlight[dim];
                                        }
                                    }
                                    #endregion
                                }
                                #endregion

                            }
                            #endregion

                        }
                        #endregion

                        #region Update UI
                        updateUI(new OptimizationResult(
                            fitness: (double)bestVurture1.Fitness,
                            positions: bestVurture1.Position.Select(pos => Convert.ToDecimal(pos)).ToList()
                            ));
                        #endregion

                        #region Get optimal solution and fitness of that one
                        this.Xopt = bestVurture1.Position;
                        this.Fxopt = (double)bestVurture1.Fitness;
                        #endregion

                        #region Round and Store optimal solution in each iteration
                        var roundedPosition = bestVurture1.Position.Select(x => Math.Round(x, 3));

                        var positionString = string.Join(";", roundedPosition);
                        var fitnessString = Math.Round((double)bestVurture1.Fitness, 3).ToString();

                        storeBestResults.AppendLine(positionString + "," + fitnessString);
                        #endregion

                    }
                    #endregion
                }
            }
            #endregion


            #region Convert Vurture to Optimization Result to save Top design
            // Add The best to list
            StringBuilder topDesigns = new StringBuilder();
            foreach (var design in topResults)
            {
                topDesigns.AppendLine(design.Value + "," + design.Key);
            }
            #endregion

            #region Export result into file
            try
            {
                File.WriteAllText(@".\results_avoa_iteration.csv", storeBestResults.ToString());
                File.WriteAllText(@".\results_avoa_top_design.csv", topDesigns.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Data could not be written to the CSV file.");
                return;
            }
            #endregion
        }
        private double[] LevyFlight(int dimension)
        {
            double beta = 3 / 2;
            var sigma = Math.Pow((SpecialFunctions.Gamma(1 + beta) * Math.Sin(Math.PI * beta / 2) / (SpecialFunctions.Gamma((1 + beta) / 2) * beta * Math.Pow(2, (beta - 1) / 2))), (1 / beta));
            double[] step = new double[dimension];
            for (var dim = 0; dim < dimension; dim++)
            {
                var rand1 = this.rnd.NextDouble();
                var u = rand1 * sigma;
                var rand2 = this.rnd.NextDouble();
                var v = rand2;
                step[dim] = u / Math.Pow(Math.Abs(v), 1 / beta);
            }
            return step;
        }
        public double[] get_Xoptimum()
        {
            return this.Xopt;
        }
        public double get_fxoptimum()
        {
            return this.Fxopt;
        }
        #endregion

        #region Vurture Model
        private class Vurture
        {
            public double[] Position { get; set; }
            public double? Fitness { get; set; } = null;


            private Random rnd;

            public Vurture(int dimension, double[] lb, double[] ub)
            {
                this.rnd = new Random();
                this.Position = new double[dimension];
                this.Initialization(dimension, lb, ub);
            }

            private void Initialization(int dimension, double[] lb, double[] ub)
            {
                for (int i = 0; i < dimension; i++)
                {
                    this.Position[i] = rnd.NextDouble() * (ub[i] - lb[i]) + lb[i];
                }
            }

            public double[] CopyPosition()
            {
                return this.Position.ToArray();
            }
        }
        #endregion
    }

}
