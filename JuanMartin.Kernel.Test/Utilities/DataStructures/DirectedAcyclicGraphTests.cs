using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using JuanMartin.Kernel.Extesions;
using JuanMartin.Kernel.Utilities.DataStructures;

namespace JuanMartin.Kernel.Utilities.DataStructures.Tests
{
    [TestFixture()]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "<Pending>")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0018:Inline variable declaration", Justification = "<Pending>")]
    public class DirectedAcyclicGraphTests
    {
        private Vertex<int> v1, v2, v3, v0;
        private int expected_vertex_count = 6, expected_outgoing_edge_count = 6;
        private DirectedAcyclicGraph<int> graph;

        [SetUp]
        public void Init()
        {
            graph = CreateNumericTestGraph();
        }

        [TearDown]
        public void Dispose()
        {
            v1 = null;
            v2 = null;
            v3 = null;
            v0 = null;

            graph = null;
        }


        [Test()]
        public void TwoVerticesConnectedByAnEdge_MustBeAdjacent()
        {
            Assert.IsTrue(graph.Adjacent(v1, v2));

        }

        [Test()]
        public void TwoVerticesConnectedByAnEdge_MustBeNeighbors()
        {
            // there is an edge from v1 to v2
            var o = v1.OutgoingNeighbors();
            var i = v2.IncomingNeighbors();
            Assert.IsTrue(o.Contains(v2) && i.Contains(v1));
        }

        [Test()]
        public void VertexCount_MustCountAllVerticesInGraph()
        {
            Assert.AreEqual(expected_vertex_count, graph.VertexCount());
        }

        [Test()]
        public void EdgeCount_MustCountAllEdgesInGraph()
        {
            Assert.AreEqual(expected_outgoing_edge_count, graph.EdgeCount(Edge<int>.EdgeType.outgoing));
        }

        [Test()]
        public void AddDuplicateVertex_MustAddNode()
        {
            // if name is be unique change graph dups status
            Assert.IsFalse(graph.HasDuplicateVertexNames,"no duplicates");
            Assert.IsNotNull(graph.AddVertex(1000, v1.Name),"add duplicate with v1 name");
            Assert.IsTrue(graph.HasDuplicateVertexNames,"one duplicate");
        }

        [Test()]
        public void GettingByNameEdgeRepeatedInGraph_MustUseFromVertexNameToDisambiguate()
        {
            var vertex_add_1 = graph.GetEdge("add", type: Edge<int>.EdgeType.outgoing); //belongs to v1
            var vertex_add_2 = graph.GetEdge("add", "two", type: Edge<int>.EdgeType.outgoing); //belongs to v2
            var vertex_add_3 = graph.GetEdge("add", "four", type: Edge<int>.EdgeType.outgoing); //belongs to v4
            var add_edges = graph.GetOutgoingEdges().Where(e => e.Name.Contains("add")).ToList();

            Assert.AreEqual(3, add_edges.Count, "Graph has two 'add' eges");
            Assert.AreNotEqual(vertex_add_1.From, vertex_add_2.From, "Both 'add' edges belong to different vertices");
            Assert.AreEqual("two", vertex_add_2.From.Name, "'vertex_add_2' comes from vertex 'two'");
            Assert.AreEqual("four", vertex_add_3.From.Name, "'vertex_add_3' comes from vertex 'four'");
        }

        [Test()]
        public void ReAddingEdgeBetweenTwoVerticesWithSameName_ShouldIncreaseByOne()
        {
            // check initial weight on edge
            var edge = graph.GetEdge("substract", type: Edge<int>.EdgeType.outgoing);
            Assert.AreEqual(2, edge.Weight);
            // readd edge between same nodes and with same name
            graph.AddEdge(from: v1, to: v2, type: Edge<int>.EdgeType.outgoing, name: "substract");

            // check new weight
            edge = graph.GetEdge("substract", type: Edge<int>.EdgeType.outgoing );

            const int WeightAterReAdd = 3;
            Assert.AreEqual(WeightAterReAdd, edge.Weight);
        }

        [Test()]
        public void MustRemoveVerticesByName()
        {
            var deleted_v1_vertex = graph.RemoveVertex(v1.Name);

            Assert.AreEqual(v1, deleted_v1_vertex);
            Assert.IsFalse(graph.Vertices.Contains(v1));
        }

        [Test()]
        public void AddCyclicEdge_MustThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => graph.AddEdge(from: v3, to: v3, type: Edge<int>.EdgeType.outgoing, name: "cycle", weight: 1));
        }

        [Test()]
        public void AddEdgeWithNoWeight_MustThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => graph.AddEdge(from: v3, to: v3, type: Edge<int>.EdgeType.outgoing, name: "noweight"));
        }

        [Test()]
        public void AddEdgeWithUnexistingFromVertex_MustThrowArgumentException()
        {
            var v = new Vertex<int>(-1, "uknown");

            Assert.Throws<ArgumentException>(() => graph.AddEdge(from: v, to: v3, type: Edge<int>.EdgeType.outgoing, name: "unexisting vertex"));
        }

        [Test()]
        public void AddEdgeWithUnexistingToVertex_MustThrowArgumentException()
        {
            var v = new Vertex<int>(1, "uknown");

            Assert.Throws<ArgumentException>(() => graph.AddEdge(from: v3, to: v, type: Edge<int>.EdgeType.outgoing, name: "unexisting vertex"));
        }

        [Test()]
        public void AddingBidirectionalEdge_MustAddNeighborsAndEgesInBothVerticesInEdge()
        {
            var v1 = new Vertex<string>("A");
            var v2 = new Vertex<string>("B");
            var g = new DirectedAcyclicGraph<string>(new List<Vertex<string>> { v1, v2 });

            g.AddEdge(v1, v2, "connect", Edge<string>.EdgeType.outgoing, Edge<string>.EdgeDirection.bidirectional,1);

            Assert.IsTrue((v1.Neighbors.Count == 2) && (v2.Neighbors.Count == 2),"Neighbor Count");
            Assert.IsTrue((v1.Edges.Count == 2) && (v2.Edges.Count == 2),"Total edge count");
            Assert.IsTrue((v1.OutgoingEdges().Count == 1) && (v1.IncomingEdges().Count == 1) && (v2.OutgoingEdges().Count == 1) && (v2.IncomingEdges().Count == 1),"Edge counnt by typr");
        }

        [Test()]
        public void IfVertexValueIsDuplicate_MustRemoveNothing()
        {
            var deleted_v1_vertex = graph.RemoveVertex(v1.Value);

            Assert.AreEqual(deleted_v1_vertex, null);
            Assert.IsTrue(graph.Vertices.Contains(v1));
        }

        [Test()]
        public void RemoveEdges_MustOnlyDeleteEdgesBetweenTwoGivenVertices()
        {
            // v1 has two eges to v2 and one to v4
            Assert.AreEqual(3, v1.OutgoingEdges().Count, "Initially there should be three outgoing edges from.");

            // delete edges between v1 and v2
            Assert.AreEqual(2, graph.RemoveEdges(v1, v2,Edge<int>.EdgeType.outgoing).Count, "Between v1 and v2 there are two edges.");
            Assert.AreEqual(1, v1.OutgoingEdges().Count, "After removing the edges from v1 that go to v2 only one remains.");
        }


        [Test()]
        public void GetVertex_MustFindAVertexByName()
        {
            var v1actual = graph.GetVertex(v1.Name);

            Assert.AreEqual(v1, v1actual);
        }

        [Test()]
        public void GetEdgesByName_MustReturnAllEdgesInGraphThatHaveTheSameName()
        {
            var edges = graph.GetEdgesByName("add");

            Assert.AreEqual(3, edges.Count);
        }

        [Test()]
        public void GetRoot_MustRetunOnlyVerticesWithNoIncomingEdges()
        {
            var vertices = graph.GetRoot();

            Assert.AreEqual(1, vertices.Count, "There should be only one vertex returned");
            Assert.AreEqual(v0.Name, vertices[0].Name, "The single root should be the test vertex named zero");

        }


        [Test()]
        public void MultiplePathTestGraph_MustHaveOnlyOneRoot()
        {
            var g = Create_MultiplePathsTestNumericGraph();
            var vertices = g.GetRoot();

            Assert.AreEqual(1, vertices.Count, "There should be only one vertex returned");
            Assert.AreEqual("0", vertices[0].Name, "The single root should be the vertex named '0'");

        }

        [Test()]
        public void StringTestGraph_MustHaveTwoPaths()
        {
            var g = CreateStringTestGraph();
            var paths = g.GetPaths();
            Assert.AreEqual(2, paths.Count);
        }

        [Test()]
        public void NumericTestGraph_MustHaveSixPaths()
        {
            var g = Create_MultiplePathsTestNumericGraph();
            var paths = g.GetPaths();
            Assert.AreEqual(6, paths.Count);
        }

        [Test()]
        public void GetLongestPath_MustGetPathWithLargestWeight()
        {
            var g = CreateStringTestGraph();
            var path = g.GetLongestPath();
            Assert.AreEqual(5, path.Weight);
        }

        [Test()]
        public void ToString_MustReturnStrinRepresentationOfThePlanarizedGraphAsCommaSeparatedListOfItsVerticesWithEdges()
        {
            var expected_representation = "a:[ a-n:(oneword):a-n:outgoing:1 ],n:[ a-n:(oneword):a-n:incoming:1, n-d:(oneword):n-d:outgoing:1 ]d:[ n-d:(oneword):n-d:incoming:1 ]";
            var g = CreateStringTestSinglePathGraph();
            var actual_representation = g.ToString(true);

            Assert.AreEqual(expected_representation, actual_representation);
        }


        [Test()]
        public void MustGetSingleShortestPathBetweenTwoGivenVertices()
        {
            //Assert.Pass();
            var expected_distance = 7;
            var expected_path = "A,D,E,C";
            var g = CreateComputerScienceGraph();

            var (actual_distance, actual_path) = g.GetDijkstraSingleShortestPath("A", "C");
            var actual_shortest_path = string.Join(",", actual_path.Vertices.Select(v => v.Name));

            Assert.AreEqual(expected_distance, actual_distance, "Computer Science graph: A-C shortest distance");
            Assert.AreEqual(expected_path, actual_shortest_path, "Computer Science graph: A-C shortest path");

            expected_distance = 10;
            expected_path = "A,B,G";
            g = CreateSWEGraph();

            (actual_distance, actual_path) = g.GetDijkstraSingleShortestPath("A", "G");
            actual_shortest_path = string.Join(",", actual_path.Vertices.Select(v => v.Name));

            Assert.AreEqual(expected_distance, actual_distance, "SWE graph: A-G shortest distance");
            Assert.AreEqual(expected_path, actual_shortest_path, "SWE graph: A-G shortest path");

            expected_distance = 19;
            expected_path = "0,1,3,4,6";
            g = CreaateFreeCodeCampGraph();

            (actual_distance, actual_path) = g.GetDijkstraSingleShortestPath("0", "6");
            actual_shortest_path = string.Join(",", actual_path.Vertices.Select(v => v.Name));

            Assert.AreEqual(expected_distance, actual_distance, "Free Code Camp graph: 0-6 shortest distance");
            Assert.AreEqual(expected_path, actual_shortest_path, "Free Code Camp graph: 0-6 shortest path");
        }

        [Test()]
        public void AllEdgesInGraphMustHaveCorrspondingNeighbors()
        {
            var g = CreateComputerScienceGraph();

            foreach(var V in g.Vertices)
            {
                foreach(var E in V.Edges)
                {
                    var vertex_source = E.From;
                    var vertex_target = E.To;

                    if (E.Type == Edge<string>.EdgeType.outgoing)
                        Assert.IsTrue(V.Neighbors.Where(n => n.Type == Neighbor<string>.NeighborType.outgoing && n.Node.Guid == vertex_target.Guid).FirstOrDefault() != null,"outgoing check");

                    if (E.Type == Edge<string>.EdgeType.incoming)
                        Assert.IsTrue(V.Neighbors.Where(n => n.Type == Neighbor<string>.NeighborType.incoming && n.Node.Guid == vertex_source.Guid).FirstOrDefault() != null, "inconcoming check");
                }
            }
        }


        private DirectedAcyclicGraph<string> CreateStringTestGraph()
        {
            var vertices = new List<Vertex<string>>();

            var a = new Vertex<string>("a");
            var n = new Vertex<string>("n");
            var d = new Vertex<string>("d");
            var e = new Vertex<string>("e");
            var w = new Vertex<string>("w");
            vertices.Add(a);
            vertices.Add(n);
            vertices.Add(d);
            vertices.Add(e);
            vertices.Add(w);

            var g = new DirectedAcyclicGraph<string>(vertices);
            g.AddEdge(from: a, to: n, name: "oneword", weight: 1);
            g.AddEdge(from: n, to: d, name: "oneword", weight: 1);
            g.AddEdge(from: n, to: e, name: "twoword", weight: 2);
            g.AddEdge(from: e, to: w, name: "twoword", weight: 2);

            return g;
        }

        private DirectedAcyclicGraph<string> CreateStringTestSinglePathGraph()
        {
            var vertices = new List<Vertex<string>>();

            var a = new Vertex<string>("a");
            var n = new Vertex<string>("n");
            var d = new Vertex<string>("d");
            vertices.Add(a);
            vertices.Add(n);
            vertices.Add(d);

            var g = new DirectedAcyclicGraph<string>(vertices);
            g.AddEdge(from: a, to: n, name: "oneword", weight: 1);
            g.AddEdge(from: n, to: d, name: "oneword", weight: 1);

            return g;
        }

        private static readonly int[] n = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

        /// <summary>
        /// Numeric graph with with six paths one root and one leaf 
        /// </summary>
        /// <returns></returns>
        private DirectedAcyclicGraph<int> Create_MultiplePathsTestNumericGraph()
        {
            var vertices = new List<Vertex<int>>();

            foreach (var i in n)
            {
                vertices.Add(new Vertex<int>(i));
            }

            var g = new DirectedAcyclicGraph<int>(vertices);

            g.AddEdge(vertices[0], vertices[1], name: null, weight: 1);
            g.AddEdge(vertices[0], vertices[2], name: null, weight: 2);
            g.AddEdge(vertices[0], vertices[3], name: null, weight: 3);

            g.AddEdge(vertices[1], vertices[4], name: null, weight: 4);
            g.AddEdge(vertices[1], vertices[5], name: null, weight: 5);

            g.AddEdge(vertices[2], vertices[6], name: null, weight: 6);
            g.AddEdge(vertices[2], vertices[7], name: null, weight: 7);

            g.AddEdge(vertices[3], vertices[8], name: null, weight: 8);
            g.AddEdge(vertices[3], vertices[9], name: null, weight: 9);

            g.AddEdge(vertices[4], vertices[10], name: null, weight: 10);
            g.AddEdge(vertices[5], vertices[10], name: null, weight: 11);
            g.AddEdge(vertices[6], vertices[10], name: null, weight: 12);
            g.AddEdge(vertices[7], vertices[10], name: null, weight: 13);
            g.AddEdge(vertices[8], vertices[10], name: null, weight: 14);
            g.AddEdge(vertices[9], vertices[10], name: null, weight: 15);

            return g;
        }

        /// <summary>FreeCodeCamp's Dijstra's algorithm video
        /// <see cref=@"https://www.freecodecamp.org/news/dijkstras-shortest-path-algorithm-visual-introduction/#:~:text=Dijkstra's%20Algorithm%20finds%20the%20shortest,node%20and%20all%20other%20nodes."/>
        /// </summary>
        /// <returns></returns>
        private DirectedAcyclicGraph<string> CreaateFreeCodeCampGraph()
        {
            var vertices = new List<Vertex<string>>();

            foreach (var i in Enumerable.Range(0, 7))
            {
                vertices.Add(new Vertex<string>(i.ToString()));
            }

            var g = new DirectedAcyclicGraph<string>(vertices);


            g.AddEdge(vertices[0], vertices[1], name: null, weight: 2);
            g.AddEdge(vertices[0], vertices[2], name: null, weight: 6);

            g.AddEdge(vertices[1], vertices[3], name: null, weight: 5);
            g.AddEdge(vertices[2], vertices[3], name: null, weight: 8);

            g.AddEdge(vertices[3], vertices[4], name: null, weight: 10);
            g.AddEdge(vertices[3], vertices[5], name: null, weight: 15);

            g.AddEdge(vertices[4], vertices[5], name: null, weight: 6);
            g.AddEdge(vertices[4], vertices[6], name: null, weight: 2);

            g.AddEdge(vertices[5], vertices[6], name: null, weight: 6);

            return g;
        }

        /// <summary>ComputerScience's Dijstra's algorithm video
        /// <see cref="https://www.youtube.com/watch?v=pVfj6mxhdMw"/>
        /// </summary>
        /// <returns></returns>
        private DirectedAcyclicGraph<string> CreateComputerScienceGraph()
        {
            var vertices = new List<Vertex<string>>();

            foreach (var i in Enumerable.Range(0, 5))
            {
                vertices.Add(new Vertex<string>(Convert.ToChar(i + 65).ToString()));
            }

            var g = new DirectedAcyclicGraph<string>(vertices);


            g.AddEdge(vertices[0], vertices[1], name: null, weight: 6);
            g.AddEdge(vertices[0], vertices[3], name: null, weight: 1);

            g.AddEdge(vertices[3], vertices[4], name: null, weight: 1);
            g.AddEdge(vertices[3], vertices[1], name: null, weight: 2);

            g.AddEdge(vertices[4], vertices[1], name: null, weight: 2);
            g.AddEdge(vertices[4], vertices[2], name: null, weight: 5);

            g.AddEdge(vertices[1], vertices[2], name: null, weight: 5);



            return g;
        }


        /// <summary>Back To Back SWE's Dijstra's algorithm video
        /// <see cref="https://www.youtube.com/watch?v=K_1urzWrzLs"/>
        /// </summary>
        /// <returns></returns>
        private DirectedAcyclicGraph<string> CreateSWEGraph()
        {
            var vertices = new List<Vertex<string>>();

            foreach (var i in Enumerable.Range(0, 7))
            {
                vertices.Add(new Vertex<string>(Convert.ToChar(i + 65).ToString()));
            }

            var g = new DirectedAcyclicGraph<string>(vertices);


            g.AddEdge(vertices[0], vertices[1], name: null, weight: 2);
            g.AddEdge(vertices[0], vertices[2], name: null, weight: 4);
            g.AddEdge(vertices[0], vertices[3], name: null, weight: 7);
            g.AddEdge(vertices[0], vertices[5], name: null, weight: 5);

            g.AddEdge(vertices[1], vertices[3], name: null, weight: 6);
            g.AddEdge(vertices[1], vertices[4], name: null, weight: 3);
            g.AddEdge(vertices[1], vertices[6], name: null, weight: 8);

            g.AddEdge(vertices[2], vertices[5], name: null, weight: 6);

            g.AddEdge(vertices[3], vertices[5], name: null, weight: 1);
            g.AddEdge(vertices[3], vertices[6], name: null, weight: 6);

            g.AddEdge(vertices[4], vertices[6], name: null, weight: 7);

            g.AddEdge(vertices[5], vertices[6], name: null, weight: 6);

            return g;
        }

        /// <summary>
        /// Create numeric values graph with three paths and one root
        /// </summary
        private DirectedAcyclicGraph<int> CreateNumericTestGraph()
        {
            var vertices = new List<Vertex<int>>();

            v0 = new Vertex<int>(0, "zero"); // this will be the only root of this graph: no incoming edges to it
            v1 = new Vertex<int>(1, "one");
            v2 = new Vertex<int>(2, "two");
            v3 = new Vertex<int>(3, "three");
            var v4 = new Vertex<int>(1, "four");
            var v5 = new Vertex<int>(5, "five");
            vertices.Add(v0);
            vertices.Add(v1);
            vertices.Add(v2);
            vertices.Add(v3);
            vertices.Add(v4);
            vertices.Add(v5);
            expected_vertex_count = vertices.Count;

            var g = new DirectedAcyclicGraph<int>(vertices);
            g.AddEdge(from: v0, to: v1, name: "start", weight: 5);
            g.AddEdge(from: v1, to: v2, name: "add", weight: 4);
            g.AddEdge(from: v2, to: v3, name: "add", weight: 1);
            g.AddEdge(from: v1, to: v2, name: "substract", weight: 2);
            g.AddEdge(from: v1, to: v4, name: "copy", weight: 3);
            g.AddEdge(from: v4, to: v5, name: "add", weight: 6);

            expected_outgoing_edge_count = 6;

            return g;
            // graph:
            // vertex (neighbors,edges)
            // vertex -weight-> vertex
            //
            // v0 (1,1) -5-> v1 (3,3) -2-> v2 (2,1) -1-> v3 (1,0)
            //              |            ^                
            //              |_____4______|                
            //              |-----3---------> v4 (2,1) --6-> v5,0)
        }
    }
}