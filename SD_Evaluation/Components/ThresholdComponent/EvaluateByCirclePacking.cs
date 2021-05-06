using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace SD_Evaluation.Components.ThredholdComponent
{
    public class EvaluateByCirclePacking : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the EvaluateByCirclePacking class.
        /// </summary>
        public EvaluateByCirclePacking()
          : base("Evaluate By Circle Packing", "EvalCP",
              "Evaluate the score of path segments in regard to some social distancing threshold by points from circle packing",
              "SD_Evaluation", "Evaluation")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)

        {
            pManager.AddGenericParameter("Pedestrian Domain", "PD", "Pedestrian Domain", GH_ParamAccess.item);
            pManager.AddCurveParameter("Segments", "Seg", "Path segments to be evaluated", GH_ParamAccess.list);
            pManager.AddNumberParameter("Thredhold", "Thr", "(Optional)threshold value for social distancing. Default set as 9.2 feet", GH_ParamAccess.item, 9.2);
            pManager.AddPointParameter("Circle Packing Grid", "Samples", "Point grid from circle packing", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
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
            var threshold = default(double);
            var pts = new List<Point3d>();

            if (!DA.GetData(0, ref domain)) return;
            if (!DA.GetDataList(1, segs)) return;
            if (!DA.GetDataList(3, pts)) return;

            DA.GetData(2, ref threshold);

            // --- Check
            if (threshold <= 0)
                throw new Exception(@"Thredhold must be a positive value!");

            // --- Execute
            var boundary = domain.Boundary;
            var bldgs = domain.Regions;

            var eva = ThresholdEvaluation.Create(boundary, bldgs, segs);
            eva.Thredhold = threshold;

            eva.EvaluateByPacking(pts);

            var scores = eva.DistanceScore;

            // --- Output

            DA.SetDataList(0, scores);
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
            get { return new Guid("873bb281-9cc5-44d4-b2af-e297f27534f3"); }
        }
    }
}