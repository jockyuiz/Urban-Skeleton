using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Linq;

namespace SD_Evaluation.Components.VolumeComponent
{
    public class MedialAxisSmoothComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public MedialAxisSmoothComponent()
          : base("Medial Axis Smooth", "MA",
              "Simplify a series of medial axis skeleton to a curved network",
              "SD_Evaluation", "Medial Axis")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddLineParameter("Medial Axis Skeletons", "MAS", "MA Skeleton", GH_ParamAccess.list);
            pManager.AddNumberParameter("Rebuild Resolution", "res", "rebuild resolution", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("Branching Points", "BP", "Branching points in the Medial Axis Graph", GH_ParamAccess.list);
            pManager.AddPointParameter("Naked Points", "NP", "Naked points in the Medial Axis Graph", GH_ParamAccess.list);
            pManager.AddCurveParameter("Skeleton Curves", "SC", "Skeleton curves in the Medial Axis Graph", GH_ParamAccess.list);
            pManager.AddCurveParameter("Naked Curves", "NC", "Naked curves in the Medial Axis Graph", GH_ParamAccess.list);
            pManager.AddCurveParameter("Skeleton Segments", "Seg", "Skeleton Segmented Sample Curves", GH_ParamAccess.list);
            pManager.AddCurveParameter("All Segments", "AllSeg", "All Segmented Sample Curves", GH_ParamAccess.list);

        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // --- Input

            var skeletons = new List<Line>();
            var res = default(double);

            if (!DA.GetDataList(0, skeletons)) return;
            if (!DA.GetData(1, ref res)) return;

            // --- Execute

            var MAG = new Graphs.MedialAxisGraph();
            var sg = MAG.BasicGraphFromLines(skeletons,res/3.0);
            var branches = MAG.BranchPoints(sg);
            var naked = MAG.NakedPoints(sg);
            var graphLines = MAG.GraphLines(sg);

            var joinedCurves = Curve.JoinCurves(graphLines).ToList();

            List<Curve> sample = new List<Curve>();

            CurveUtil.DecompositeCurves(branches, naked, joinedCurves, out Curve[] middleSegs, out Curve[] nakedSegs);
            CurveUtil.RebuildCurveList(middleSegs, res);
            CurveUtil.RebuildCurveList(nakedSegs, res);
            Array.ForEach(middleSegs, seg => sample.AddRange(CurveUtil.SegsFromCurve(seg, res)));

            List<Curve> allsamples = new List<Curve>(sample);
            Array.ForEach(nakedSegs, seg => allsamples.AddRange(CurveUtil.SegsFromCurve(seg, res)));

            // --- Output
            DA.SetDataList(0, branches);
            DA.SetDataList(1, naked);
            DA.SetDataList(2, middleSegs);
            DA.SetDataList(3, nakedSegs);
            DA.SetDataList(4, sample);
            DA.SetDataList(5, allsamples);
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
            get { return new Guid("d7d5e026-ce4f-4170-a258-203d5d0b0153"); }
        }
    }
}