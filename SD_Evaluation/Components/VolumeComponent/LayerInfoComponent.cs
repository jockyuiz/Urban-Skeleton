using System;
using System.Collections.Generic;
using System.Drawing;
using System.Collections.Specialized;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Rhino.DocObjects;
using Rhino;
using Grasshopper;
using Grasshopper.Kernel.Data;

namespace SD_Evaluation.Components.VolumeComponent
{
    public class LayerInfoComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the LayerInfo class.
        /// </summary>
        public LayerInfoComponent()
          : base("Layer Information", "LayerInformation",
              "Extract name information from Layer",
              "SD_Evaluation", "Utils")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Reference Toggle", "Toggle", "Toggle for referencing objects by layer.", (GH_ParamAccess)0);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.Register_GenericParam("Names", "Names", "Name of the layers.");
            pManager.Register_GenericParam("GUID", "ID", "Name of the layers.");
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool flag = false;
            // ISSUE: cast to a reference type
            DA.GetData<bool>(0, ref flag);
            RhinoDoc activeDoc = RhinoDoc.ActiveDoc;
            if (!flag)
                return;
            try
            {
                var layers = activeDoc.Layers;
                List<string> stringList1 = new List<string>();
                List<string> stringList2 = new List<string>();
                int num1 = checked(layers.Count - 1);
                int num2 = 0;
                while (num2 <= num1)
                {
                    Layer layer = layers[num2];
                    stringList1.Add(layer.Name);
                    stringList2.Add(layer.Id.ToString());
                    checked { ++num2; }
                }
                DA.SetDataList(0, stringList1);
                DA.SetDataList(1, stringList2);
            }
            catch (Exception ex)
            {
                Console.WriteLine (Convert.ToInt32(ex.ToString()));
            }
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
            get { return new Guid("51574100-b3c5-492d-aed6-bf8ab12138a3"); }
        }
    }
}