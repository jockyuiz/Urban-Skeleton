using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace SD_Evaluation.Components.VisualizationComponent
{
    public class PathScoreVisualizationComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the PathScoreVisualization class.
        /// </summary>
        public PathScoreVisualizationComponent()
          : base("Path Score Visualization", "Path",
              "Visualize the path based on its social distancing score",
               "SD_Evaluation", "Visualization")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Segment Paths", "Seg", "Path segments to be evaluated", GH_ParamAccess.list);
            pManager.AddNumberParameter("Thredhold Scores", "Scores", "Minimum people allowed to pass each segment, varied from 1 to 5", GH_ParamAccess.list);
            pManager.AddNumberParameter("Multiplier", "Mult", "Height multiplier for extrusion", GH_ParamAccess.item,100);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("Path Score Mesh", "Mesh Path", "Colored Mesh represent comfort scored mesh", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // --- Input
            var segs = new List<Curve>();
            var scores = new List<double>();
            var height = default(double);

            DA.GetDataList(0, segs);
            DA.GetDataList(1, scores);
            if (!DA.GetData(2, ref height)) return;

            // --- Check

            if (segs.Count != scores.Count)
            {
                throw new Exception(@"Segments and Scores must be corresponding");
            }

            // --- Execute

            var mesh = VisualUtils.PathScoringMesh(scores, segs, height);

            // --- Output

            DA.SetData(0, mesh);



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
            get { return new Guid("76f3ead3-b705-4d26-b805-29ee4d57ca91"); }
        }
    }
}