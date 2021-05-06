using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace SD_Evaluation.Components.VisualizationComponent
{
    public class ComfortVisualizationComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the ComfortVisualizationComponent class.
        /// </summary>
        public ComfortVisualizationComponent()
          : base("Comfort Visualization", "Comfort",
              "Visualize a comfort-based value map on a given mesh",
              "SD_Evaluation", "Visualization")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Segment Paths", "Seg", "Path segments to be evaluated", GH_ParamAccess.list);
            pManager.AddNumberParameter("Distances", "Dist", "Passable distance at each path segment", GH_ParamAccess.list);
            pManager.AddMeshParameter("Mesh", "Mesh", "Basic mesh to give color visualization", GH_ParamAccess.item);
            pManager.AddIntegerParameter("KNeighbours", "K", "Number of segs allowed for each grid point to search for", GH_ParamAccess.item, 5);
            pManager.AddNumberParameter("Thredhold", "Thred", "Thredhold distance for social distancing", GH_ParamAccess.item, 9.2);
            pManager.AddNumberParameter("ClampDown", "Down", "multiplition of minimum value for clamping", GH_ParamAccess.item, 1.1);
            pManager.AddNumberParameter("ClampUp", "Up", "multiplition of minimum value for clamping", GH_ParamAccess.item, 0.9);


        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh Comfort", "Mesh Comfort", "Colored Mesh represent comfort scored mesh", GH_ParamAccess.item);
            pManager.AddNumberParameter("Vertex Safety Value", "Value", "Value for average comfortness at each vertex", GH_ParamAccess.list);
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
            var sta = default(double);
            var down = default(double);
            var up = default(double);

            DA.GetDataList(0, segs);
            DA.GetDataList(1, scores);
            if (!DA.GetData(2, ref mesh)) return;
            DA.GetData(3, ref k);
            DA.GetData(4, ref sta);
            DA.GetData(5, ref down);
            DA.GetData(6, ref up);

            // --- Check

            if (segs.Count != scores.Count)
            {
                throw new Exception(@"Segments and Scores must be corresponding");
            }

            if (k <= 0 || sta <= 0)
                throw new Exception(@"Number of neighbourhood or thredhold must be a positive value!");

            // --- Execute

            VisualUtils.RelativeComfortness(scores, segs, mesh, k, sta, down,up,out Mesh cMesh, out List<double> vertexScores);

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
            get { return new Guid("50392a94-4faf-4a7a-9f6d-f15af5456b57"); }
        }
    }
}