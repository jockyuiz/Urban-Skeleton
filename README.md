# KPFui_CodeExam


___KPFui Computational Designer Challenge - Zhenxian Huang___



___Research Questions___

Evaluate the "width" property given domain with the topology of an outside closed loop and several inside obstacles.

Naturally the "width" problem can be related to a problem of Medial Axis Transform, which gets the skeleton of geometry.Then analysis on either intersection with obstacles or number of related circle packings can be made to imply the attribute across the whole domain.



___Tools Used___

Boolean operation on closed curves from clipper(Angus Johnson) to get Pedestrian Domain

Voronoi algorithm from Grasshopper.Kernel() for Medial Axis Extraction

Graph traversal for getting singularities of skeleton(Self wrote)

Physical System Simulation from KangarooSolver for Circle Packing Approximation

RTree search for final mesh visualization(Self wrote)



___WorkFlow___

1.Information Extraction:
	Extracting building footprints from layer

2.Generate Pedestrian Domain(PD)
	Use Boolean Difference between a selected boundary curve and auto-detective footprints inside the boundary to create the analysi volume.

3.Create Medial Axis Skeleton(MAS)
	Use voronoi diagram for basic MAS lines and then rebuild it based on graph connectivity. Take sample segments from original lines or rebuilt result.

4.Simulate Circle Packing
	Use physical system to process collisions of circles. The collisions will take some seeds outside of the domain and what remains is thus the result of circle packing. Maximum capacity can be informed.

5.Evaluate Segment Score
	Use either the density of circle packing centers or the rays generated from perpendicular direction of each segment to evaluate score on each sample segment.Social distancing value(set as (6+1.7+1.7)=9.4) is an input at this stage

6.Visualize Analysis
	Use a good-quality mesh or directly the sample segments to visualize the result. RTree search is performed to get nearest segments for a sample point at the mesh vertex.



___Conclusions___

1.Summary
This plugin mainly takes two algorithms for evaluating a pedestrian domain: Medial Axis Skeleton for extracting path segments; and Circle Packing for generating maximum capacity. Several derived and helper functions were provided to help realize the algorithm as well as visualize the result. The final visualization gives an indication of Score distribution around the pedestrian domain.

2.Known bugs and potential fix

---The boolean operation cannot perform correctly when there is overlap between buildings and boundary
Possible Solution: Try to extend the PedestrianDomain class to accompany for this scenario

---The smoothing operation of MAS often goes wrong because of the quality of the bad quality of initial MAS
Possible Solution: Use a different algorithm other than voronoi, i.e. Tracing Paths

---The evaluation result can have some extremeties because of:
	1. the perpendicular direction of a MAS segment is actually along the path
	2. the physical simulation have some accidents during collision
Possible Solution: Expand the search range for each analysis point on the domain and apply anti-aliasing

