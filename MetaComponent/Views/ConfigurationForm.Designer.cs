namespace MetaComponent.Views
{
    partial class ConfigurationForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.cbAlgorithms = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lbFitness = new System.Windows.Forms.Label();
            this.lbIteration = new System.Windows.Forms.Label();
            this.numberMaxIter = new System.Windows.Forms.NumericUpDown();
            this.numberPop = new System.Windows.Forms.NumericUpDown();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numberMaxIter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numberPop)).BeginInit();
            this.SuspendLayout();
            // 
            // cbAlgorithms
            // 
            this.cbAlgorithms.FormattingEnabled = true;
            this.cbAlgorithms.Items.AddRange(new object[] {
            "AVOA",
            "GWO"});
            this.cbAlgorithms.Location = new System.Drawing.Point(15, 27);
            this.cbAlgorithms.Name = "cbAlgorithms";
            this.cbAlgorithms.Size = new System.Drawing.Size(132, 21);
            this.cbAlgorithms.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Select Algorithm";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(105, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Number of Iterations:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 108);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Population:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lbFitness);
            this.groupBox1.Controls.Add(this.lbIteration);
            this.groupBox1.Location = new System.Drawing.Point(186, 27);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 84);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Result";
            // 
            // lbFitness
            // 
            this.lbFitness.AutoSize = true;
            this.lbFitness.Location = new System.Drawing.Point(6, 51);
            this.lbFitness.Name = "lbFitness";
            this.lbFitness.Size = new System.Drawing.Size(43, 13);
            this.lbFitness.TabIndex = 1;
            this.lbFitness.Text = "Fitness:";
            // 
            // lbIteration
            // 
            this.lbIteration.AutoSize = true;
            this.lbIteration.Location = new System.Drawing.Point(7, 23);
            this.lbIteration.Name = "lbIteration";
            this.lbIteration.Size = new System.Drawing.Size(51, 13);
            this.lbIteration.TabIndex = 0;
            this.lbIteration.Text = "Iteration: ";
            // 
            // numberMaxIter
            // 
            this.numberMaxIter.Location = new System.Drawing.Point(13, 77);
            this.numberMaxIter.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numberMaxIter.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numberMaxIter.Name = "numberMaxIter";
            this.numberMaxIter.Size = new System.Drawing.Size(134, 20);
            this.numberMaxIter.TabIndex = 5;
            this.numberMaxIter.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // numberPop
            // 
            this.numberPop.Location = new System.Drawing.Point(12, 124);
            this.numberPop.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numberPop.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numberPop.Name = "numberPop";
            this.numberPop.Size = new System.Drawing.Size(135, 20);
            this.numberPop.TabIndex = 6;
            this.numberPop.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(311, 121);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 7;
            this.btnStart.Text = "Run";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.Enabled = false;
            this.btnStop.Location = new System.Drawing.Point(186, 121);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 8;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // ConfigurationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(397, 156);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.numberPop);
            this.Controls.Add(this.numberMaxIter);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbAlgorithms);
            this.Name = "ConfigurationForm";
            this.Text = "ConfigurationForm";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numberMaxIter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numberPop)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbAlgorithms;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lbFitness;
        private System.Windows.Forms.Label lbIteration;
        private System.Windows.Forms.NumericUpDown numberMaxIter;
        private System.Windows.Forms.NumericUpDown numberPop;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
    }
}