using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using KangarooSolver;
using KangarooSolver.Goals;
using SD_Evaluation.CirclePackings;

namespace SD_Evaluation.Components.SimulationComponent
{
    public class SettingComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the CirclePackingComponent class.
        /// </summary>
        public SettingComponent()
          : base("Circle Packing settings", "Setting",
              "Settings for circle packing simulation",
              "SD_Evaluation", "Simulate")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("PedDomain", "PD", "Pedestrian Domain", GH_ParamAccess.item);
            pManager.AddNumberParameter("Radii", "Radii", "Circle radii based on social distancing", GH_ParamAccess.item, 4.6);
            pManager.AddPointParameter("Seeds", "Seed", "Initial seeds to start", GH_ParamAccess.list);
            pManager.AddNumberParameter("Packing Force", "P_Force", "Initial seeds to start", GH_ParamAccess.item,1.1);
            pManager.AddNumberParameter("Constraining Force", "C_Force", "Initial seeds to start", GH_ParamAccess.item,1.0);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Simulation Setting", "Setting", "Settings for simulation", GH_ParamAccess.item);
            pManager.AddGenericParameter("Goal Objects", "Goals", "Goal Objects in Kangaroo", GH_ParamAccess.list);
        }


        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // --- Input
            var domain = default(PedstrianDomain);
            double radii= 0.0;
            var seeds = new List<Point3d>();
            double PF = 0.0;
            double CF = 0.0;

            if (!DA.GetData(0, ref domain)) return;
            if (!DA.GetData(1, ref radii)) return;
            if (!DA.GetDataList(2, seeds)) return;

            DA.GetData(3, ref PF);
            DA.GetData(4, ref CF);


            // --- Check
            if (radii <= 0)
                throw new Exception(@"Radii must be positive values!");

            // --- Execute

            var setting = SimulationSettings.Create(domain, seeds);

            if(DA.GetData(3, ref PF) && PF > 0.1) {
                setting.PackingStrength = PF;
            }

            if (DA.GetData(4, ref CF) && CF > 0.1)
            {
                setting.Strength = CF;
            }
            var goals = setting.CreateGoals();

            // --- Output
            DA.SetData(0, setting);
            DA.SetDataList(1, goals);
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
            get { return new Guid("4342cef2-b4d3-43cc-a511-54fd6cb5465d"); }
        }
    }
}