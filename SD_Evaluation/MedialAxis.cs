using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;
using Grasshopper.Kernel.Geometry;
using Grasshopper.Kernel;


namespace SD_Evaluation
{
    /// <summary>
    /// Class deals with geometrical problems in generating Medial Axis. 
    /// The geometry will be used in the MedialAxisGraph class for topology processing
    /// </summary>
    public class MedialAxis
    {

        public List<Line> SkeletonLines(Curve sidewalk,IEnumerable<Curve> bldgs,double res, Rhino.Geometry.Plane plane) {
            List<Point3d> p = new List<Point3d>();
            List<Line> MA = new List<Line>();


            p.AddRange(CurveUtil.SamplesFromCurve(sidewalk, res));
            foreach (Curve bld in bldgs)
            {
                p.AddRange(CurveUtil.SamplesFromCurve(bld, res));
            }

            List<Polyline> polys = Voronoi(p);
            Transform projection = Transform.PlanarProjection(plane);

            for (int i = 0; i < polys.Count; ++i)
            {
                Polyline pl = polys[i];
                Line[] lns = pl.GetSegments();
                int segCount = lns.Length;
                for (int j = 0; j < segCount; ++j)
                {
                    Line ln = lns[j];
                    ln.Transform(projection);
                    if (CurveUtil.LineInsideRegion(ln, sidewalk, bldgs)) MA.Add(ln);
                }
            }
            return MA;
        }
        private List<Polyline> Voronoi(List<Point3d> nodePts)
        {
            BoundingBox bb = new BoundingBox(nodePts);

            Transform scale = Transform.Scale(bb.Center, 5.0);
            bb.Transform(scale);

            Vector3d d = bb.Diagonal;
            double dl = d.Length;
            double f = dl / 15;
            bb.Inflate(f, f, f);
            Point3d[] bbCorners = bb.GetCorners();

            //# Create a list of nodes
            Node2List nodes = new Node2List();
            foreach (Point3d p in nodePts)
            {
                Node2 n = new Node2(p.X, p.Y);
                nodes.Append(n);
            }

            //Create a list of outline nodes using the BB
            Node2List outline = new Node2List();
            foreach (Point3d p in bbCorners)
            {
                Node2 n = new Node2(p.X, p.Y);
                outline.Append(n);
            }


            //# Calculate the delaunay triangulation
            var delaunay = Grasshopper.Kernel.Geometry.Delaunay.Solver.Solve_Connectivity(nodes, 0.1, false);

            // # Calculate the voronoi diagram
            var voronoi = Grasshopper.Kernel.Geometry.Voronoi.Solver.Solve_Connectivity(nodes, delaunay, outline);

            //# Get polylines from the voronoi cells and return them to GH
            List<Polyline> polys = new List<Polyline>();
            foreach (var c in voronoi)
            {

                Polyline pl = c.ToPolyline();

                polys.Add(pl);
            }
            return polys;
        }

    }
}
