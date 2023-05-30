using Grasshopper;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;
using Grasshopper.Kernel.Special;
using MetaComponent.Models;
using MetaComponent.Views;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace MetaComponent
{
    public class MetaComponent : GH_Component
    {
        internal ConfigurationForm form;
        internal GrasshopperInput grasshopperInput;

        public MetaComponent()
          : base("MetaComponent", "MetaAlgo",
            "Using Metaheuristic for solving Generative Designs",
            "Params", "Util")
        {
            // TODO: Create Views Windows Form here
            form = null;
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            // Use the pManager object to register your input parameters.
            // You can often supply default values when creating parameters.
            // All parameters must have the correct access type. If you want 
            // to import lists or trees of values, modify the ParamAccess flag.
            pManager.AddNumberParameter("Variables", "Vars", "The variables what we find to archieve optimal solution", GH_ParamAccess.list);
            pManager.AddNumberParameter("Objectives", "Obj", "The objectives that we need to optimize", GH_ParamAccess.item);
            // If you want to change properties of certain parameters, 
            // you can use the pManager instance to access them by index:
            //pManager[0].Optional = true;
        }


        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
        }

        // For Double-Click
        public override void CreateAttributes()
        {
            m_attributes = new Attribute(this);
        }

        internal class Attribute : GH_ComponentAttributes
        {
            public Attribute(IGH_Component component) : base(component)
            {
            }

            public override GH_ObjectResponse RespondToMouseDoubleClick(GH_Canvas sender, GH_CanvasMouseEvent e)
            {
                // Show Windows Here
                ((MetaComponent)Owner).ShowWindowForm();
                return GH_ObjectResponse.Handled;
            }
        }

        // Set up show Window Form
        public void ShowWindowForm()
        {
            var owner = Grasshopper.Instances.DocumentEditor;

            // Check if current window is showing
            if(form == null || form.IsDisposed )
            {
                form = new ConfigurationForm(this)
                {
                    StartPosition = FormStartPosition.Manual
                };

                // register this form to GrassHopper
                GH_WindowsFormUtil.CenterFormOnWindow(form, owner, true);
                owner.FormShepard.RegisterForm(form);
            }
            form.Show(owner);
        }

        //Create GhInOut
        public void GrasshopperInput_Instantiate()
        {
            grasshopperInput = new GrasshopperInput(this);
        }

        /// <summary>
        /// The Exposure property controls where in the panel a component icon 
        /// will appear. There are seven possible locations (primary to septenary), 
        /// each of which can be combined with the GH_Exposure.obscure flag, which 
        /// ensures the component will only be visible on panel dropdowns.
        /// </summary>
        public override GH_Exposure Exposure => GH_Exposure.primary;

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// You can add image files to your project resources and access them like this:
        /// return Resources.IconForThisComponent;
        /// </summary>
        protected override System.Drawing.Bitmap Icon => null;

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid => new Guid("7e744bdc-6723-4113-9b74-c62efb18eba4");
    }

    // This class is to process the Inputs (Varibles) and Outputs (Objectives) of Grasshopper Component
    internal class GrasshopperInput
    {
        private readonly List<Guid> _inputGuids;
        private IGH_Param _output;
        private List<IGH_Param> _outputs;
        private readonly GH_Document _doc;
        
        // Grasshopper Component
        public MetaComponent Component { get; private set; }

        // Get Component Directory
        public string ComponentFolder { get; private set; }

        // Sliders
        public List<GH_NumberSlider> Sliders { get; private set; }
        public List<Variable> Variables { get; private set; }
        public int VariableN { get { return Variables.Count; } }

        public GrasshopperInput(MetaComponent component)
        {
            Component = component;
            ComponentFolder = Path.GetDirectoryName(Grasshopper.Instances.ComponentServer.FindAssemblyByObject(Component).Location);

            _doc = Component.OnPingDocument();
            _inputGuids = new List<Guid>();
        }

        public bool SetInputs()
        {
            Sliders = new List<GH_NumberSlider>();

            var s = new GH_NumberSlider();

            // Get Variables from GrassHopper Component
            foreach (var source in Component.Params.Input[0].Sources)
            {
                var guid = source.InstanceGuid;
                _inputGuids.Add(guid);
            }

            // Checking the number of inputs must be large than 0
            if (_inputGuids.Count == 0)
            {
                MessageBox.Show("Provide at least one variable input", "Input Variables Error");
                return false;
            }

            foreach (var guid in _inputGuids)
            {
                // Find object that maches the ID. Learn more: https://developer.rhino3d.com/api/grasshopper/html/M_Grasshopper_Kernel_GH_Document_FindObject_1.htm
                var input = _doc.FindObject(guid, true);

                if (input == null)
                {
                    MessageBox.Show(
                        "The variables are inconsistent. This error typically occurs after removing one or more connected number sliders. Please consider deleting and setting up all variables connections again.",
                        "Variables Inconsistency Error");
                    return false;
                }

                // If 
                if (input.ComponentGuid == s.ComponentGuid)
                {
                    var slider = (GH_NumberSlider)input;
                    Sliders.Add(slider);
                }
            }

            SetVariables();
            return true;
        }

        public bool SetOutput()
        {
            if (Component.Params.Input[1].Sources.Count != 1)
            {
                MessageBox.Show("Provide at least one objective", "Output Objective Error");
                return false;
            }


            //foreach (var source in Component.Params.Input[1].Sources)
            //{
            //    if (source == null) return false;
            //    _outputs.Add(source);
            //}



            // Test for only one output
            _output = Component.Params.Input[1].Sources[0];
            
            if (_output == null)
            {
                return false;
            }


            return true;
        }

        //Get Variable String
        public void SetVariables()
        {
            var variables = new List<Variable>();

            //Sliders
            foreach (var slider in Sliders)
            {
                //Slider Type
                var min = slider.Slider.Minimum;
                var max = slider.Slider.Maximum;

                decimal lowerB;
                decimal upperB;
                bool integer;

                switch (slider.Slider.Type)
                {
                    case Grasshopper.GUI.Base.GH_SliderAccuracy.Even:
                        lowerB = min / 2;
                        upperB = max / 2;
                        integer = true;
                        break;
                    case Grasshopper.GUI.Base.GH_SliderAccuracy.Odd:
                        lowerB = (min - 1) / 2;
                        upperB = (max - 1) / 2;
                        integer = true;
                        break;
                    case Grasshopper.GUI.Base.GH_SliderAccuracy.Integer:
                        lowerB = min;
                        upperB = max;
                        integer = true;
                        break;
                    default:
                        lowerB = min;
                        upperB = max;
                        integer = false;
                        break;
                }

                variables.Add(new Variable(lowerB, upperB, integer));
            }
            Variables = variables;
        }

        //Get Variable Values
        public decimal[] GetSliderValues()
        {
            return Sliders.Select(slider => slider.CurrentValue).ToArray();
        }

        public double[] GetSliderValuesDouble()
        {
            return Array.ConvertAll(GetSliderValues(), x => (double)x);
        }

        //Set Variable Values
        public bool SetSliderValues(List<decimal> parameters)
        {
            // Check the value from Variables which are beyond the boundaries
            if (!CheckVariableValues(parameters)) return false;

            var i = 0;

            //Sliders
            foreach (var slider in Sliders)
            {
                if (slider == null)
                {
                    MessageBox.Show("Slider is null", "Set Value Error");
                    return false;
                }

                decimal val;

                switch (slider.Slider.Type)
                {
                    case Grasshopper.GUI.Base.GH_SliderAccuracy.Even:
                        val = (int)parameters[i++] * 2;
                        break;
                    case Grasshopper.GUI.Base.GH_SliderAccuracy.Odd:
                        val = (int)(parameters[i++] * 2) + 1;
                        break;
                    case Grasshopper.GUI.Base.GH_SliderAccuracy.Integer:
                        val = (int)parameters[i++];
                        break;
                    case Grasshopper.GUI.Base.GH_SliderAccuracy.Float:
                        val = parameters[i++];
                        break;
                    default:
                        val = parameters[i++];
                        break;
                }

                slider.Slider.RaiseEvents = false;
                slider.SetSliderValue(val);
                slider.ExpireSolution(false);
                slider.Slider.RaiseEvents = true;
            }

            return true;
        }



        //Check Variable Values
        private bool CheckVariableValues(List<decimal> parameters)
        {

            //Check Variable Number
            if (parameters.Count != VariableN)
            {
                MessageBox.Show(String.Format("Wrong number of parameters({0}): {1}" + Environment.NewLine + "Parameters: {2}", Sliders.Count, parameters.Count, VariableN), "Value Error");
                return false;
            }


            for (var i = 0; i < VariableN; i++)
                if (CheckVariableValue(parameters[i], Variables[i]) == false) return false;

            return true;
        }

        private static bool CheckVariableValue(decimal param, Variable variable)
        {
            var lowerB = variable.LowerB;
            var upperB = variable.UpperB;
            var integer = variable.Integer;

            //Check Integer
            if (integer && param % 1 != 0)
            {
                MessageBox.Show(String.Format("Wrong parameter type(int: {0})", param), "Value Error");
                return false;
            }
            //Check lower Bound
            if (param < lowerB)
            {
                MessageBox.Show(String.Format("Parameter is too small (lower Bound {0}: {1})", lowerB, param), "Value Error");
                return false;
            }
            //Check upper Bound
            if (param > upperB)
            {
                MessageBox.Show(String.Format("Parameter is too large (upper Bound {0}: {1})", upperB, param), "Value Error");
                return false;
            }

            return true;
        }

        //Get Objective Value
        public double GetObjectiveValue()
        {
            double objectiveValue;

            if (_output == null)
            {
                MessageBox.Show("No objective value found", "Objective Error");
                return double.NaN;
            }

            if (_output.VolatileDataCount < 1)
            {
                MessageBox.Show("Please provide at least one objective value, instead of " + _output.VolatileDataCount + " values.", "Objective Error");
                return double.NaN;
            }

            var objectiveGoo = _output.VolatileData.AllData(false).First();

            if (objectiveGoo == null)
            {
                MessageBox.Show("Objective value is null", "Objective Error");
                return double.NaN;
            }

            var bolCast = objectiveGoo.CastTo<double>(out objectiveValue);

            // Test
            objectiveValue = double.Parse(objectiveGoo.ToString());

            if (bolCast) return objectiveValue;
            // MessageBox.Show(double.Parse(objectiveGoo.ToString()).ToString(), "FrOG Error");
            // MessageBox.Show("Failed to cast objective value to double", "FrOG Error");

            //return double.NaN;
            return double.Parse(objectiveGoo.ToString());
        }

        //Recalculate Grasshopper Solution
        public void Recalculate()
        {
            //Wait until the grasshopper solution in finished
            while (_doc.SolutionState != GH_ProcessStep.PreProcess || _doc.SolutionDepth != 0) { }

            //var strMessage = "Starting new solution" + Environment.NewLine;
            //MessageBox.Show(strMessage);

            _doc.NewSolution(true);

            //strMessage = "Started new solution" + Environment.NewLine;
            //MessageBox.Show(strMessage);

            //Wait until the grasshopper solution in finished
            while (_doc.SolutionState != GH_ProcessStep.PostProcess || _doc.SolutionDepth != 0) { }

            //strMessage += "Finished solution" + Environment.NewLine;
            //MessageBox.Show(strMessage);
        }

        //New Solution
        public void NewSolution(List<decimal> parameters)
        {
            //MessageBox.Show("Setting values!");
            SetSliderValues(parameters);
            //MessageBox.Show("Recalculating!");
            Recalculate();
        }
    }
}