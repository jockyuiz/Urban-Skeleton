using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using KangarooSolver;
using KangarooSolver.Goals;
using SD_Evaluation.CirclePackings;


namespace SD_Evaluation.Components.SimulationComponent
{
    public class IterSimulateComponent : GH_Component
    {

        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public IterSimulateComponent()
          : base("Simulate Circle Packing", "SimuCP",
              "circle packing simulation based on a given iteration number",
              "SD_Evaluation", "Simulate")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Simulation Settings", "Setting", "Settings for simulation", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Iterations", "iter", "Iteration number", GH_ParamAccess.item,1);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("Out Points", "Points", "Center positions for circle packings", GH_ParamAccess.list);
            pManager.AddCircleParameter("Out Circles", "Circles", "Circles packed together", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Max Capacity", "Capacity", "Maximum capacity after circle packing", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // --- Input
            var setting = default(SimulationSettings);
            int iter = 0;

            if (!DA.GetData(0, ref setting)) return;

            DA.GetData(1, ref iter);

            // --- Check
            if (iter < 0 )
                throw new Exception(@"Iteration must be a positive values");

            // --- Execute
            var mySimulation = new PackingSimulation(setting);

            mySimulation.RunIterativeSimulation(iter);
            mySimulation.PostProcessing();

            var pts = mySimulation.SatisfiedPositions;
            var circles = mySimulation.SatisfiedCircles;

            // --- Output
            DA.SetDataList(0, pts);
            DA.SetDataList(1, circles);
            DA.SetData(2, pts.Count);

        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("6ba1d50e-9f73-4899-af1e-9eb970187ae3"); }
        }
    }
}