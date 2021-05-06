using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KangarooSolver;
using KangarooSolver.Goals;
using Rhino.Geometry;

namespace SD_Evaluation.CirclePackings
{
    class PackingSimulation
    {
        public SimulationSettings Settings;


        public List<IGoal> Goals { get; private set; }
        public PackingSimulation()
        {

        }

        public PackingSimulation(SimulationSettings settings)
        {
            Settings = settings;
            PS = new KangarooSolver.PhysicalSystem();
            counter = 0;
            CalculatedPositions = new List<Point3d>();
            ProjectedPositions = new List<Point3d>();
            SatisfiedPositions = new List<Point3d>();
            ProjectedCircles = new List<Circle>();
            SatisfiedCircles = new List<Circle>();

            
        }

        public KangarooSolver.PhysicalSystem PS { get; private set; }
        public int counter = 0;


        public List<Point3d> CalculatedPositions { get; private set; }
        public List<Point3d> ProjectedPositions { get; private set; }
        public List<Circle> ProjectedCircles { get; private set; }
        public List<Point3d> SatisfiedPositions{ get; private set; }
        public List<Circle> SatisfiedCircles { get; private set; }
        private void InitGoals()
        {
            Goals = Settings.CreateGoals();
        }
        public void RunIterativeSimulation(int iter) {
            
            InitGoals();
            PS = new KangarooSolver.PhysicalSystem();
            double tol = 0.0001; // points closer than this will get combined into a single particle
            var GoalList = new List<IGoal>();

            //Assign indexes to the particles in each Goal:
            foreach (IGoal G in Goals)
            {
                PS.AssignPIndex(G, tol);
                GoalList.Add(G);
            }

            double threshold = 1e-4;
            do
            {
                PS.Step(GoalList, false, threshold); // The step will iterate until either reaching 15ms or the energy threshold
                counter++;
            } while (PS.GetvSum() > threshold && counter < iter); //GetvSum returns the current kinetic energy

            CalculatedPositions = PS.GetPositions().ToList();
            ProjectResult();
        }

        private void ProjectResult()
        {
            var z = Settings.Domain.DomainPlane.OriginZ;
            foreach (Point3d p in CalculatedPositions)
            {
                var projected = new Point3d(p.X,p.Y,z);
                ProjectedPositions.Add(projected);
                ProjectedCircles.Add(new Circle(Settings.Domain.DomainPlane, p, Settings.Radii));
            }
        }

        public void PostProcessing()
        {
            var outside = Settings.Domain.Boundary;
            var inside = Settings.Domain.Regions;

            for(int i = 0; i < CalculatedPositions.Count; i++)
            {
                Point3d p = ProjectedPositions[i];
                Circle cr = ProjectedCircles[i];
                if (CurveUtil.PointInsideRegion(p, outside, inside, Settings.Domain.DomainPlane)){
                    SatisfiedPositions.Add(p);
                    SatisfiedCircles.Add(cr);
                }
            }
        }

    }
}
