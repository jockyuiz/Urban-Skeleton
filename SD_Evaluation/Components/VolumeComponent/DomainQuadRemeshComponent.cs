using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace SD_Evaluation.Components.VolumeComponent
{
    public class DomainQuadRemeshComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the DomainQuadRemesh class.
        /// </summary>
        public DomainQuadRemeshComponent()
          : base("Domain Quad Remesh", "DMesh",
              "Regularly quad remeshing a domain",
              "SD_Evaluation", "Volume Creation")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("PedDomain", "PD", "Pedestrian Domain", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("DomainQuadMesh", "DQMesh", "", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // --- Input
            var domain = default(PedstrianDomain);

            if (!DA.GetData(0, ref domain)) return;

            // --- Execute

            var mesh = domain.DomainQuadMesh();

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
            get { return new Guid("77b5a9d6-5986-4451-a0b7-ba7f3ad2b02c"); }
        }
    }
}