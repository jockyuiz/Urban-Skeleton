using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace SD_Evaluation.Components.ThredholdComponent
{
    public class EvaluateComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public EvaluateComponent()
          : base("Evaluate", "Eval",
              "Evaluate the score of path segments in regard to some social distancing thredhold based on the perpendicular direction",
              "SD_Evaluation", "Evaluation")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("PedDomain", "PD", "Pedestrian Domain", GH_ParamAccess.item);
            pManager.AddCurveParameter("Segments", "Seg", "Path segments to be evaluated", GH_ParamAccess.list);
            pManager.AddNumberParameter("Thredhold", "Thr", "(Optional)thredhold value for social distancing. Default set as 9.2 feet", GH_ParamAccess.item,9.2);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddLineParameter("Passibles", "Pass", "Clearance segments for paths", GH_ParamAccess.list);
            pManager.AddNumberParameter("Distances", "Dist", "Length of clearance segments", GH_ParamAccess.list);
            pManager.AddNumberParameter("Scores", "Score", "Evaluation Scores", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // --- Input
            var domain = default(PedstrianDomain);
            var segs = new List<Curve>();
            var thredhold = default(double);

            if (!DA.GetData(0, ref domain)) return;
            if (!DA.GetDataList(1, segs)) return;

            DA.GetData(2, ref thredhold);

            // --- Check
            if(thredhold <= 0)
                throw new Exception(@"Thredhold must be a positive value!");

            // --- Execute
            var boundary = domain.Boundary;
            var bldgs = domain.Regions;

            var eva = ThresholdEvaluation.Create(boundary, bldgs, segs);
            eva.Thredhold = thredhold;

            eva.Evaluate();
            var passes = eva.DistanceLines;
            var distances = eva.Distances;
            var scores = eva.DistanceScore;

            // --- Output

            DA.SetDataList(0, passes);
            DA.SetDataList(1, distances);
            DA.SetDataList(2, scores);

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
            get { return new Guid("c52e80d6-0c1c-40d5-89d3-172dfe1b7d74"); }
        }
    }
}