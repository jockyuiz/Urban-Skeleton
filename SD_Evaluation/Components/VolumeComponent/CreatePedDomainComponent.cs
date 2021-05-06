using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace SD_Evaluation.Components.VolumeComponent
{
    public class CreatePedDomainComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the CreatePedRegionComponent class.
        /// </summary>
        public CreatePedDomainComponent()
          : base("Pedestrian Domain", "PedDomain",
              "Create the pedestrian domain for evaluation",
              "SD_Evaluation", "Volume Creation")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Sidewalk", "Sidewalk", "Sidewalk boundary, must be a closed curve", GH_ParamAccess.item);
            pManager.AddCurveParameter("Buildings", "Bldgs", "Building list inside the sidewalk boundary", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("PedDomain", "PD", "Pedestrian Domain for analysis", GH_ParamAccess.item);
            pManager.AddBrepParameter("DomainSurface","DSrf","Surface representation of pedestrian domain", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // --- Input
            var boundary = default(Curve);
            var bldgs = new List<Curve>();

            if (!DA.GetData(0, ref boundary)) return;
            if (!DA.GetDataList(1, bldgs)) return;
            //if (!DA.GetData(2, ref res)) return;
            if (!boundary.IsClosed)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Input boundary must be closed");
                return;
            }

            if (!boundary.TryGetPlane(out Plane plane))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Input boundary must be planar");
                return;
            }

            // --- Execute

            var fitRegions = PedstrianDomain.MultipleBldgsToRegions(bldgs, boundary);
            var domain = PedstrianDomain.Create(boundary, fitRegions);
            var domainSrf = domain.ToBreps();

            // --- Output

            DA.SetData(0, domain);
            DA.SetDataList(1, domainSrf);
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
            get { return new Guid("0ba096d5-3dab-493e-87c4-d4fbd199d75b"); }
        }
    }
}