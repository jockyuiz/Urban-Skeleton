using System;
using System.Collections;
using System.Collections.Generic;

using Rhino;
using Rhino.Geometry;

using KangarooSolver;
using KangarooSolver.Goals;

namespace SD_Evaluation.CirclePackings
{
    class SphereCurveCollide:GoalObject
    {
        Curve C;
        double strength;
        double[] radii;

        public SphereCurveCollide(Curve Crv, List<Point3d> Pts, List<double> Radii, double k)
        {
            C = Crv;
            int L = Pts.Count;
            PPos = Pts.ToArray();
            Move = new Vector3d[L];
            Weighting = new double[L];
            radii = Radii.ToArray();
            strength = k;
        }

        public override void Calculate(List<KangarooSolver.Particle> p)
        {
            for (int i = 0; i < PIndex.Length; i++)
            {
                Point3d Pt = p[PIndex[i]].Position;
                if (C.ClosestPoint(Pt, out double t, radii[i]))
                {
                    Point3d Closest = C.PointAt(t);
                    Vector3d Push = Pt - Closest;
                    Push.Unitize();
                    Push *= radii[i];
                    Point3d Target = Closest + Push;
                    Move[i] = Target - Pt;
                    Weighting[i] = strength;
                }
                else
                {
                    Move[i] = Vector3d.Zero;
                    Weighting[i] = 0;
                }
            }
        }
    }
}
