using Grasshopper;
using Grasshopper.Kernel.Data;
using Rhino.Geometry;
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
    public static class LineGraph
    {
        public static Tuple<Point3d[], List<string>, List<int>, List<int>, List<int>, DataTree<int>> GetGraphData(List<Line> lines)
        {

            UndirectedGraph g = LinesToUndirectedGrap(lines);

            DataTree<int> dataTreeA = new DataTree<int>();
            List<String> vertices = g.GetVertices();
            for (int i = 0; i < vertices.Count; i++)
                dataTreeA.AddRange(g.GetNeighbours(vertices[i]), new Grasshopper.Kernel.Data.GH_Path(i));

            DataTree<int> dataTreeB = new DataTree<int>();
            List<Graphs.Edge> edges = g.GetEdges();

            List<int> u = new List<int>();
            List<int> v = new List<int>();
            List<int> w = new List<int>();
            for (int i = 0; i < edges.Count; i++)
            {
                u.Add(edges[i].u);
                v.Add(edges[i].v);
                w.Add(1);
                dataTreeB.Add(edges[i].u, new Grasshopper.Kernel.Data.GH_Path(i));
                dataTreeB.Add(edges[i].v, new Grasshopper.Kernel.Data.GH_Path(i));
            }

            PointCloud pointCloud = ((PointCloud)g.GetAttribute());


            return new Tuple<Point3d[], List<string>, List<int>, List<int>, List<int>, DataTree<int>>(pointCloud.GetPoints(), g.GetVertices(), u, v, w, dataTreeA);


        }


        public static SD_Evaluation.Graphs.UndirectedGraph LinesToUndirectedGrap(List<Line> lines)
        {

            List<Point3d> pts = new List<Point3d>();

            foreach (Line l in lines)
            {
                pts.Add(l.From);
                pts.Add(l.To);
            }

            //Sorting
            var edges = new List<int>();

            var allPoints = new List<Point3d>(pts); //naked points

            int i = 0;

            while (allPoints.Count != 0)
            {
                Point3d pt = allPoints[0];
                allPoints.RemoveAt(0);


                for (int d = 0; d < pts.Count; d++)
                {
                    if (pt.Equals(pts[d]))
                    {
                        edges.Add(d);
                        break;
                    }
                }

                i++;
            }

            var uniqueVertices = new HashSet<int>(edges).ToList();

            //Creating typological points
            var topologyPoints = new PointCloud();

            foreach (int k in uniqueVertices)
                topologyPoints.Add(pts[k]);

            //var vertices = Enumerable.Range(0, uniqueVertices.Count);

            for (int k = 0; k < uniqueVertices.Count; k++)
                if (uniqueVertices.ElementAt(k) != k)
                    for (int l = 0; l < edges.Count; l++)
                        if (edges[l] == uniqueVertices[k])
                            edges[l] = k;

            //Create graph
            Graphs.UndirectedGraph g = new Graphs.UndirectedGraph(uniqueVertices.Count);

            for (int k = 0; k < uniqueVertices.Count; k++)
                g.InsertVertex(k.ToString());


            for (int k = 0; k < edges.Count; k += 2)
                g.InsertEdge(edges[k].ToString(), edges[k + 1].ToString());

            g.SetAttribute((object)topologyPoints);


            return g;

        }


    }
}
