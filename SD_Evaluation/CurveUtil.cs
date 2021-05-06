using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;
using Rhino;
using Grasshopper.Kernel.Types;
using ClipperLib;

namespace SD_Evaluation
{
    public static class CurveUtil
    {
        private static readonly double EPS = 1e-5;
        private static readonly Rhino.Geometry.Plane XY = Rhino.Geometry.Plane.WorldXY;

        /// <summary>
        /// Decomposite a set of curves into middle and naked parts based on branching points
        /// </summary>
        /// <param name="BP">Branching points on curves</param>
        /// <param name="NP">Naked(End) points on curves</param>
        /// <param name="c">A set of interconnected curves</param>
        /// <param name="M">Middle parts of these curves</param>
        /// <param name="N">Naked parts of these curves</param>
        public static void DecompositeCurves(List<Point3d> BP,List<Point3d> NP,List<Curve> c, out Curve[] M, out Curve[] N) {

            List<int> branchesOnCurves = new List<int>();

            List<Curve> endCand = new List<Curve>();
            List<Curve> middleCand = new List<Curve>();

            List<Curve> naked = new List<Curve>();
            List<Curve> paths = new List<Curve>();

            foreach (Curve crv in c)
            {
                int k = numPtsOnCurve(BP, crv);
                int n = numPtsOnCurve(NP, crv);
                if (k != 1 && n != 0) endCand.Add(crv);
                else if (k != 1 && n == 0) middleCand.Add(crv);
                else naked.Add(crv);
                branchesOnCurves.Add(k);
            }


            foreach (Curve crv in middleCand)
            {
                if (crv.IsClosed)
                {
                    List<double> ts = SegPositions(BP, crv);
                    int count = ts.Count;
                    for (int i = 0; i < count; ++i)
                    {
                        Curve cc = crv.Trim(ts[i], ts[(i + 1) % count]);
                        paths.Add(cc);
                    }
                }
                else
                {
                    List<double> ts = SegPositions(BP, crv);
                    int count = ts.Count - 1;
                    for (int i = 0; i < count; ++i)
                    {
                        Curve cc = crv.Trim(ts[i], ts[i + 1]);
                        paths.Add(cc);
                    }
                }
            }

            foreach (Curve crv in endCand)
            {
                List<double> ts = SegPositions(BP, crv);
                double t_min = crv.Domain.Min;
                double t_max = crv.Domain.Max;
                int count = ts.Count - 1;
                if (ts[0] == t_min)
                {
                    for (int i = 0; i < count; ++i)
                    {
                        Curve cc = crv.Trim(ts[i], ts[i + 1]);
                        paths.Add(cc);
                    }
                    Curve nn = crv.Trim(ts[count], t_max);
                    naked.Add(nn);
                }
                else
                {
                    for (int i = 0; i < count; ++i)
                    {
                        Curve cc = crv.Trim(ts[i], ts[i + 1]);
                        paths.Add(cc);
                    }
                    Curve nn = crv.Trim(t_min, ts[0]);
                    naked.Add(nn);
                }
            }

            M = paths.ToArray();
            N = naked.ToArray();
        }

        /// <summary>
        /// return the sorted t values for points on curves
        /// </summary>
        /// <param name="BP">A set of points to separate the curves</param>
        /// <param name="crv">A collection of curves to be segmented</param>
        /// <returns></returns>
        public static List<double> SegPositions(List<Point3d> BP, Curve crv)
        {
            List<double> ts = new List<double>();
            foreach (Point3d P in BP)
            {
                crv.ClosestPoint(P, out double t);
                Vector3d vec = crv.PointAt(t) - P;
                double dist = vec.Length;
                if (dist < EPS)
                {
                    ts.Add(t);
                }
            }
            ts.Sort();
            return ts;
        }

        /// <summary>
        /// return the number of intersections on a curve based on a given list of points
        /// </summary>
        /// <param name="BP">A list of points to pick the curve</param>
        /// <param name="crv">A collection of curves to be segmented</param>
        /// <returns></returns>
        public static int numPtsOnCurve(List<Point3d> BP, Curve crv)
        {
            int count = 0;
            foreach (Point3d P in BP)
            {
                crv.ClosestPoint(P, out double t);
                Vector3d vec = crv.PointAt(t) - P;
                double dist = vec.Length;
                if (dist < EPS)
                {
                    count++;
                }

            }
            return count;
        }

        public static List<Point3d> SamplesFromCurve(Curve c, double res)
        {
            List<Point3d> samples = new List<Point3d>();
            int num = numSegsOnCurve(c, res);
            Curve rec = c.Rebuild(num, 1, false);
            double t0 = rec.Domain.Max;
            double t1 = rec.Domain.Min;
            double dt = dt = (t1 - t0) / (double)(num - 1.0);

            for (int i = 0; i < num; ++i)
            {
                double t = t0 + i * dt;
                Point3d p = rec.PointAt(t);
                samples.Add(p);
            }

            return samples;
        }

        public static List<Curve> SegsFromCurve(Curve c,double res)
        {
            List<Curve> samples = new List<Curve>();
            int num = numSegsOnCurve(c, res);
            Curve rec = c.Rebuild(num, 1, false);
            double t0 = rec.Domain.Min;
            double t1 = rec.Domain.Max;
            double dt = dt = (t1 - t0) / (double)num;

            for (int i = 0; i < num; ++i)
            {
                double t_a = t0 + i * dt;
                double t_b = t0 + (i + 1) * dt;
                Curve crv = rec.Trim(t_a, t_b);

                samples.Add(crv);
            }

            return samples;
        }

        public static IEnumerable<Curve> Boolean(ClipType operation, PolyFillType fillType, IEnumerable<Curve> curvesA, IEnumerable<Curve> curvesB, Plane? plane)
        {
            var closedA = curvesA.Where(o => o.IsClosed);
            var closedB = curvesB.Where(o => o.IsClosed);

            if (!plane.HasValue)
            {
                foreach (var curve in closedA)
                {
                    if (!curve.TryGetPlane(out var curvePlane))
                        continue;

                    plane = curvePlane;
                }
            }

            if (!plane.HasValue)
                plane = Plane.WorldXY;


            var polylinesA = new List<List<Point2d>>();
            var polylinesB = new List<List<Point2d>>();

            foreach (var curve in closedA)
            {
                if (!curve.TryGetPolyline(out var polyline))
                    continue;

                polylinesA.Add(polyline.Select(o => o.Map2D(plane.Value)).ToList());
            }

            foreach (var curve in closedB)
            {
                if (!curve.TryGetPolyline(out var polyline))
                    continue;

                polylinesB.Add(polyline.Select(o => o.Map2D(plane.Value)).ToList());
            }


            var minX = polylinesA.Union(polylinesB).SelectMany(o => o).Min(o => o.X);
            var minY = polylinesA.Union(polylinesB).SelectMany(o => o).Min(o => o.Y);
            var maxX = polylinesA.Union(polylinesB).SelectMany(o => o).Max(o => o.X);
            var maxY = polylinesA.Union(polylinesB).SelectMany(o => o).Max(o => o.Y);

            var unit = Math.Max(maxX - minX, maxY - minY) / (2 * 4.6e+18);

            var midX = (minX + maxX) / 2.0;
            var midY = (minY + maxY) / 2.0;


            var polygonsA = polylinesA.Select(o => o.Select(p => new IntPoint((p.X - midX) / unit, (p.Y - midY) / unit)).ToList())
                                      .ToList();

            var polygonsB = polylinesB.Select(o => o.Select(p => new IntPoint((p.X - midX) / unit, (p.Y - midY) / unit)).ToList())
                                      .ToList();


            var clipper = new Clipper();

            clipper.AddPaths(polygonsA, PolyType.ptSubject, true);
            clipper.AddPaths(polygonsB, PolyType.ptClip, true);

            var solution = new List<List<IntPoint>>();

            clipper.Execute(operation, solution, fillType, fillType);


            return solution.Select(o =>
            {
                var points = o.Select(p => plane.Value.Origin + (p.X * unit + midX) * plane.Value.XAxis + (p.Y * unit + midY) * plane.Value.YAxis)
                              .ToList();

                if (points.Count > 0 && points.First() != points.Last())
                    points.Add(points[0]);

                return new PolylineCurve(points);
            });
        }

        public static void RebuildCurveList(Curve[] crvs, double res)
        {
            for (int i = 0; i < crvs.Length; ++i)
            {
                Curve seg = crvs[i];
                crvs[i] = seg.Rebuild(numSegsOnCurve(seg, res), 3, true);
            }
        }

        public static int numSegsOnCurve(Curve c,double res)
        {
            return (int)Math.Ceiling(c.GetLength() / res);
        }

        public static List<Line> RemoveDupLn2(List<Line> lines, double tolerance)
        {
            return ((IEnumerable<Line>)lines).Distinct<Line>((IEqualityComparer<Line>)new CurveUtil.LineEqualityComparer(tolerance)).ToList<Line>();
        }

        public class LineEqualityComparer : IEqualityComparer<Line>
        {
            private double t;

            public LineEqualityComparer()
            {
            }

            public LineEqualityComparer(double _t)
            {
                this.t = _t;
            }

            public bool Equals(Line L1, Line L2)
            {
                return CurveUtil.OrthoClose(L1.From,L2.From, this.t) && CurveUtil.OrthoClose(L1.To,L2.To, this.t) || CurveUtil.OrthoClose(L1.To,L2.From, this.t) && CurveUtil.OrthoClose(L1.From,L2.To, this.t);
            }

            public int GetHashCode(Line obj)
            {
                return 0;
            }
        }

        public static bool OrthoClose(Point3d Point1, Point3d Point2, double t)
        {
            return Math.Abs( Point1.X - Point2.X) < t && Math.Abs(Point1.Y - Point2.Y) < t && Math.Abs(Point1.Z- Point2.Z) < t;
        }

        public static bool LineInsideCurve(Line ln, Curve bound)
        {
            Point3d p1 = ln.From;
            Point3d p2 = ln.To;
            Point3d p3 = (p1 + p2) / 2;
            return (bound.Contains(p1, XY, EPS) == PointContainment.Inside) && (bound.Contains(p2, XY, EPS) == PointContainment.Inside) && (bound.Contains(p3, XY, EPS) == PointContainment.Inside);
        }

        public static bool LineOutsideCurve(Line ln, Curve bound)
        {
            Point3d p1 = ln.From;
            Point3d p2 = ln.To;
            Point3d p3 = (p1 + p2) / 2;
            return (bound.Contains(p1, XY, EPS) == PointContainment.Outside) && (bound.Contains(p2, XY, EPS) == PointContainment.Outside &&(bound.Contains(p3, XY, EPS) == PointContainment.Outside));
        }

        public static bool LineInsideRegion(Line ln, Curve outer, IEnumerable<Curve> inner)
        {
            if (!LineInsideCurve(ln, outer)) return false;
            foreach (Curve c in inner)
            {
                if (!LineOutsideCurve(ln, c)) return false;
                else continue;
            }
            return true;
        }

        public static bool PointInsideRegion(Point3d p, Curve outer, IEnumerable<Curve> inner, Plane plane)
        {
            if (outer.Contains(p, plane, 0.01) != PointContainment.Inside) return false;

            foreach(Curve c in inner)
            {
                if (c.Contains(p, plane, 0.01) != PointContainment.Outside) return false;
                else continue;
            }
            return true;
        }

        public static Point2d Map2D(this Point3d point, Plane plane)
        {
            var p = point - plane.Origin;

            var x = p * plane.XAxis;
            var y = p * plane.YAxis;

            return new Point2d(x, y);
        }
    }
}
