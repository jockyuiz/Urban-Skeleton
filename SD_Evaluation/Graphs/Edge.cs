using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// /// Class definition in NGon.Core from NGON (by petrasvestartas). 
/// Project Website:https://github.com/petrasvestartas/NGons
/// </summary>
namespace SD_Evaluation.Graphs
{
    public class Edge
    {
        public int u;
        public int v;
        public int wt;

        public Rhino.Geometry.Line lnOriginal;
        public Rhino.Geometry.Line ln;

        public Rhino.Geometry.Curve crv;
        public List<Rhino.Geometry.Point3d> controls;

        public Edge(int u, int v, int wt)
        {
            this.u = u;
            this.v = v;
            this.wt = wt;
        }
    }
}
