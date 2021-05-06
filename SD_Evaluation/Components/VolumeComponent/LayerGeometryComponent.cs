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
    public class LayerGeometryComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the CurvesFromLayerComponent class.
        /// </summary>
        public LayerGeometryComponent()
          : base("Layer Geometry", "LayerGeo",
              "Extract geometry information from Layer",
              "SD_Evaluation", "Utils")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Reference Toggle", "Toggle", "Toggle for referencing objects by layer.", (GH_ParamAccess)0);
            pManager.AddTextParameter("Layer Name", "Layer", "The name of the layer to reference.", (GH_ParamAccess)1, "Building Footprints");
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.Register_GenericParam("Geometry", "Geo", "Geometry on the Layer");
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        /// 
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool flag = false;
            List<string> stringList = new List<string>();
            // ISSUE: cast to a reference type
            DA.GetData<bool>(0, ref flag);
            DA.GetDataList<string>(1, (List<String>)stringList);

            RhinoDoc activeDoc = RhinoDoc.ActiveDoc;
            if (flag)
            {
                try
                {
                    DataTree<GeometryBase> dataTree1 = new DataTree<GeometryBase>();

                    int num1 = checked(stringList.Count - 1);
                    int index1 = 0;
                    while (index1 <= num1)
                    {
                        string str1 = "{" + Convert.ToString(index1) + "}";
                        GH_Path ghPath1 = new GH_Path();
                        ghPath1.FromString(str1);
                        string str2 = stringList[index1];
                        int num2 = activeDoc.Layers.Find(str2,true);
                        if (num2 >= 0)
                        {
                            Layer layer = activeDoc.Layers[num2];
                            RhinoObject[] byLayer = activeDoc.Objects.FindByLayer(layer);
                            if (byLayer == null)
                                return;
                            int num3 = 0;
                            RhinoObject[] rhinoObjectArray = byLayer;
                            int index2 = 0;
                            while (index2 < rhinoObjectArray.Length)
                            {
                                RhinoObject rhinoObject = rhinoObjectArray[index2];
                                string str3 = "{" + Convert.ToString(index1) + ";" + Convert.ToString(num3) + "}";
                                string str4 = rhinoObject.Id.ToString();
                                string name1 = rhinoObject.Name;
                                dataTree1.Add(rhinoObject.Geometry, ghPath1);

                                checked { ++num3; }
                                checked { ++index2; }
                            }
                        }
                        checked { ++index1; }
                    }
                    DA.SetDataTree(0, (IGH_DataTree)dataTree1);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(Convert.ToInt32(ex.ToString()));
                }
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
            get { return new Guid("eb094ee7-7b84-4dfa-a68a-0a1041119b4a"); }
        }
    }
}