using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace SD_Evaluation.Components.VisualizationComponent
{
    public class SafetyVisualizationComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the SafetyEvaluationComponent class.
        /// </summary>
        public SafetyVisualizationComponent()
          : base("Safety Visualization", "Safety",
              "Visualize a safety-based value map on a given mesh",
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
            pManager.AddMeshParameter("Mesh", "Mesh", "Basic mesh to give color visualization", GH_ParamAccess.item);
            pManager.AddIntegerParameter("KNeighbours", "K", "Number of segs allowed for each grid point to search for", GH_ParamAccess.item,5);
            pManager.AddIntegerParameter("Standard", "Standard", "Minimum number of people allowed to pass a path", GH_ParamAccess.item,2);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh Safety", "Mesh Safety", "Colored Mesh represent safety scored mesh", GH_ParamAccess.item);
            pManager.AddNumberParameter("Vertex Safety Value", "Value", "Value for average safetyness at each vertex", GH_ParamAccess.list);
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
            var mesh = default(Mesh);
            var k = default(int);
            var sta = default(int);

            DA.GetDataList(0, segs);
            DA.GetDataList(1, scores);
            if(!DA.GetData(2, ref mesh))return;
            DA.GetData(3, ref k);
            DA.GetData(4, ref sta);

            // --- Check

            if (segs.Count != scores.Count)
            {
                throw new Exception(@"Segments and Scores must be corresponding");
            }

            if(k <=0 || sta <=0)
                throw new Exception(@"Number of neighbourhood or minimum passable people must be a positive value!");

            // --- Execute

            VisualUtils.Eligibility(scores, segs, mesh, k, sta, out Mesh cMesh, out List<double> vertexScores);

            // --- Output

            DA.SetData(0, cMesh);
            DA.SetDataList(1, vertexScores);

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
            get { return new Guid("e511dd10-5667-45af-b8d6-adb7c12a63a0"); }
        }
    }
}