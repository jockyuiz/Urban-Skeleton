using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace SD_Evaluation.Components.SimulationComponent
{
    public class InitializeSeedsComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the InitializeSeedsComponent class.
        /// </summary>
        public InitializeSeedsComponent()
          : base("Initialize Seeds", "Init",
              "Initialize seed points for circle packing simulation",
              "SD_Evaluation", "Simulate")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("PedDomain", "PD", "Pedestrian Domain", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Number", "N", "Initial seed numbers to start", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("Seeds", "Seeds", "Seeds positions", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // --- Input
            var domain = default(PedstrianDomain);
            int number = 0;

            if (!DA.GetData(0, ref domain)) return;
            if (!DA.GetData(1, ref number)) return;


            // --- Check
            if (number <= 0)
                throw new Exception(@"Seed number must be a positive value!");

            // --- Execute
            var breps = domain.ToBreps();
            var rawMesh = Utils.MeshFromBreps(breps);
            var seeds = Utils.RawPopulate(rawMesh, number);

            // --- Output
            DA.SetDataList(0, seeds);
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
            get { return new Guid("a4fe9ee9-d18f-4b3c-b87d-b6aebfb3fa1c"); }
        }
    }
}