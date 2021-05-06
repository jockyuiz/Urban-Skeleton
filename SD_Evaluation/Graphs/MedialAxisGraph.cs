using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace SD_Evaluation.Graphs
{
    class MedialAxisGraph
    {
        public UndirectedGraph BasicGraphFromLines(List<Line> l,double tolerence)
        {
            List<Point3d> cands = new List<Point3d>();
            for (int i = 0; i < l.Count; ++i)
            {
                Line li = l[i];
                Point3d pa = li.From;
                Point3d pb = li.To;
                cands.Add(pa);
                cands.Add(pb);
            }
            List<Point3d> v = new List<Point3d>(Point3d.CullDuplicates(cands, 1.5 * tolerence));
            PointCloud vcloud = new PointCloud(v);

            //Construct Graph
            UndirectedGraph g = new UndirectedGraph(vcloud.Count);
            g.points = v;
            for(int i = 0; i < v.Count; ++i)
            {
                g.InsertVertex(i.ToString());
            }

            for (int i = 0; i < l.Count; ++i)
            {
                Line li = l[i];
                Point3d pa = li.From;
                Point3d pb = li.To;
                int indexFrom = vcloud.ClosestPoint(pa);
                int indexTo = vcloud.ClosestPoint(pb);
                g.InsertEdge(indexFrom.ToString(), indexTo.ToString());
            }
            g.SetAttribute((object)vcloud);
            return g;
        }


        public List<Curve> GraphLines(UndirectedGraph sg)
        {
            var curves = new List<Curve>();
            sg.GetLines().ForEach(line => curves.Add(new LineCurve(line)));
            return curves;
        }

        public List<Point3d> BranchPoints(UndirectedGraph sg)
        {
            List<Point3d> pts = new List<Point3d>();
            foreach(String s in sg.GetVertices())
            {
                if (sg.Degree(s) >= 3) pts.Add(sg.points[Int32.Parse(s)]);
            }
            return pts;
        }

        public List<Point3d> NakedPoints(UndirectedGraph sg)
        {
            List<Point3d> pts = new List<Point3d>();
            foreach (String s in sg.GetVertices())
            {
                if (sg.Degree(s) <= 1) pts.Add(sg.points[Int32.Parse(s)]);
            }
            return pts;
        }
    }
}
