using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;
using Rhino;
using Grasshopper;
using KangarooSolver;
using KangarooSolver.Goals;

namespace SD_Evaluation.CirclePackings
{
    class SimulationSettings
    {
        public PedstrianDomain Domain { get; }

        public List<Point3d> Seeds { get; }


        public int K { get; }

        public double Radii { get; set; }

        public double Strength { get; set; }

        public double PackingStrength { get; set; }

        private List<double> RadiiList()
        {
            var rList = new List<double>();
            for(int i = 0; i < K; ++i)
            {
                rList.Add(Radii);
            }
            return rList;
        }

        private SimulationSettings(PedstrianDomain domain,List<Point3d> seeds)
        {
            Domain = domain;
            Seeds = seeds;
            Radii = 6.0;
            K = seeds.Count;
            Strength = 1.0;
            PackingStrength = 1.5;

        }

        public static SimulationSettings Create(PedstrianDomain domain,List<Point3d> seeds)
        {
            return new SimulationSettings(domain, seeds);
        }

        public List<IGoal> CreateGoals()
        {
            Plane pl = Domain.DomainPlane;
            /*Offset domain curves for collision testing
            var allCrvs = new List<Curve>();
            var crvs = Domain.Regions;
           
            foreach(Curve crv in crvs)
            {
                Curve newCrv = crv.Offset(pl, 0.75 * Radii, 0.01, CurveOffsetCornerStyle.None)[0];
                allCrvs.Add(newCrv);
            }
            allCrvs.Add(Domain.Boundary.Offset(pl, -0.75 * Radii, 0.01, CurveOffsetCornerStyle.None)[0]);*/

            var allCrvs = Domain.AllDomainCurves();

            List<IGoal> goals = new List<IGoal>();


            //Append Sphere-curve collide goals
            foreach(Curve c in allCrvs)
            {
                goals.Add(new SphereCurveCollide(c, Seeds, RadiiList(), Strength));
            }

            //Append On Plane goals
            goals.Add(new OnPlane(Seeds, pl, Strength));

            //Append Collision goals

            goals.Add(new Collider(Seeds, RadiiList(), PackingStrength));

            return goals;
        }
    }
}
