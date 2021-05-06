using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace SD_Evaluation.Components.VolumeComponent
{
    public class MedialAxisLinesComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public MedialAxisLinesComponent()
          : base("Medial Axis Lines", "MAL",
              "Construct the basic Medial Axis Skeletons of a given boundary and buildings based on voronoi diagram",
              "SD_Evaluation", "Medial Axis")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("PedDomain", "PD", "Pedestrian Domain", GH_ParamAccess.item);
            pManager.AddNumberParameter("Rebuild Resolution", "res", " Resolution for MAS. If too small, an adaptive value will be given", GH_ParamAccess.item,0.0);
            pManager[1].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddLineParameter("Medial Axis Skeletons", "MAS", "Medial Axis Skeleton lines", GH_ParamAccess.list);
            pManager.AddNumberParameter("Adapted Resolution", "AdaptiveRes", "Adapted resolution", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // --- Input
            var domain = default(PedstrianDomain);
            var res = default(double);

            if (!DA.GetData(0, ref domain)) return;

            var boundary = domain.Boundary;
            var bldgs = domain.Regions;
            var plane = domain.DomainPlane;
            var adapt = DA.GetData(1, ref res);


            // --- Execute


            var MA = new MedialAxis();
            var refRes = (boundary.GetLength()) / 200.0;
            if (res < 0.2 * refRes || !adapt) res = refRes;
            var skeletons = MA.SkeletonLines(boundary, bldgs, res,plane);
            skeletons = CurveUtil.RemoveDupLn2(skeletons, 0.01);

            // --- Output
            DA.SetDataList(0, skeletons);
            DA.SetData(1, refRes);
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
            get { return new Guid("fb8315f3-89e7-4003-b040-82702a8a1633"); }
        }
    }
}