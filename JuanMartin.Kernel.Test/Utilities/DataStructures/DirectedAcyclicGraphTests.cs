using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using JuanMartin.Kernel.Extesions;
using JuanMartin.Kernel.Utilities.DataStructures;

namespace JuanMartin.Kernel.Utilities.DataStructures.Tests
{
    [TestFixture()]
    public class DirectedAcyclicGraphTests
    {
        private Vertex<int> v1, v2, v3, v0;
        private int expectedVertexCount = 0, expectedOutgoingEdgeCount = 0;
        private DirectedAcyclicGraph<int> actualGraph;

        [SetUp]
        public void Init()
        {
            expectedVertexCount = 6;
            expectedOutgoingEdgeCount = 6;
            actualGraph = CreateNumericTestGraph();
        }

        [TearDown]
        public void Dispose()
        {
            v1 = null;
            v2 = null;
            v3 = null;
            v0 = null;

            actualGraph = null;
        }

        [Test()]
        public void ShouldCreateAnAdjacencyMatrixWithAllOutgoingEdgesInGraph()
        {
            double[][] actualMatrix;
            double[][] expectedMatrix = new double[][]
              {
                        new double[]{ 0,1,1,1,1,0,0 },
                        new double[]{ 0,0,0,0,0,1,1 },
                        new double[]{ 0,0,0,1,0,0,0 },
                        new double[]{ 1,0,0,0,0,0,0 },
                        new double[]{ 0,0,0,0,0,0,0 },
                        new double[]{ 0,0,0,0,0,0,0 },
                        new double[]{ 0,0,0,0,0,0,0 },
            };

            actualGraph = CreateCycleTestGraph();

            actualMatrix = actualGraph.GetAdjacencyMatrix();
            var actualAdjacencyCount = expectedMatrix.Sum(row => (row.Count(value => value != 0)));

            Assert.AreEqual(expectedMatrix, actualMatrix, "Adjacency matrix");
            Assert.AreEqual(expectedOutgoingEdgeCount, actualAdjacencyCount, "Adjacent nodes");
        }

        [Test()]
        public void ShouldBeConsideredAsAdjacentTwoVerticesThatAreConnectedByAnEdge()
        {
            Assert.IsTrue(actualGraph.Adjacent(v1, v2));

        }

        [Test()]
        public void ShouldBeIdentifiedAsNeighborsTwoVerticesThatAreConnectedByAnEdge()
        {
            // there is an edge from v1 to v2
            var actualOutgoingNeighbors = v1.OutgoingNeighbors();
            var actualIncomingNeighbors = v2.IncomingNeighbors();
            Assert.IsTrue(actualOutgoingNeighbors.Contains(v2) && actualIncomingNeighbors.Contains(v1), "v2 is neighbor of v1.");

            // there is no edge between v3 and v1
            Assert.IsFalse(actualOutgoingNeighbors.Contains(v3) && v3.IncomingNeighbors().Contains(v1), "v3 is not neighbor of v1.");
        }
        [Test()]
        public void ShouldGetACountOfAllVerticesInGraphWithVertexCountProperty()
        {
            Assert.AreEqual(expectedVertexCount, actualGraph.VertexCount());
        }

        [Test()]
        public void ShouldGetACountOfAllEdgeWithEdgeCountProperty()
        {
            Assert.AreEqual(expectedOutgoingEdgeCount, actualGraph.EdgeCount(Edge<int>.EdgeType.outgoing));
        }

        [Test()]
        public void ShouldBeIndicatedIfGraphAddVertexMethodAddedAnyDuplicates()
        {
            // if name is be unique change graph dups status
            Assert.IsFalse(actualGraph.HasDuplicateVertexNames, "no duplicates");
            Assert.IsNotNull(actualGraph.AddVertex(1000, v1.Name), "add duplicate with v1 name");
            Assert.IsTrue(actualGraph.HasDuplicateVertexNames, "one duplicate");
        }

        [Test()]
        public void ShouldUseFromVertexNameToDisambiguateWhenGettingByNameEdgeRepeatedInGraph()
        {
            var firstAddEdge = actualGraph.GetEdge(name: "add", type: Edge<int>.EdgeType.outgoing); //belongs to v1
            var secondAddEdge = actualGraph.GetEdge(name: "add", fromName: "two", type: Edge<int>.EdgeType.outgoing); //belongs to v2
            var ThirdAddEdge = actualGraph.GetEdge(name: "add",fromName:  "four", type: Edge<int>.EdgeType.outgoing); //belongs to v4
            var actualEdgesNamedAdd = actualGraph.GetOutgoingEdges().Where(e => e.Name.Contains("add")).ToList();

            Assert.AreEqual(3, actualEdgesNamedAdd.Count, "Graph has two 'add' eges");
            Assert.AreNotEqual(firstAddEdge.From, secondAddEdge.From, "Both 'add' edges belong to different vertices");
            Assert.AreEqual("two", secondAddEdge.From.Name, "'secondAddEdge' comes from vertex 'two'");
            Assert.AreEqual("four", ThirdAddEdge.From.Name, "'theirdAddEdge' comes from vertex 'four'");
        }

        [Test()]
        public void ShouldIncreaseEdgeWeightByOneWhenAddingEdgeBetweenTwoVerticesWithSameName()
        {
            // check initial weight on edge
            var actualEdge = actualGraph.GetEdge(name: "substract", type: Edge<int>.EdgeType.outgoing);
            Assert.AreEqual(2, actualEdge.Weight);
            // readd edge between same nodes and with same name
            actualGraph.AddEdge(from: v1, to: v2, type: Edge<int>.EdgeType.outgoing, name: "substract");

            // check new weight
            actualEdge = actualGraph.GetEdge(name: "substract", type: Edge<int>.EdgeType.outgoing);

            const int expectedWeightAterReAdd = 3;
            Assert.AreEqual(expectedWeightAterReAdd, actualEdge.Weight);
        }

        [Test()]
        public void ShouldBeAbleToRemoveVerticesByName()
        {
            var actualDeletedVertex = actualGraph.RemoveVertex(v1.Name);

            Assert.AreEqual(v1, actualDeletedVertex);
            Assert.IsFalse(actualGraph.Vertices.Contains(v1));
        }

        [Test()]
        public void ShouldThrowArgumentExceptionWhenAddinngCyclicEdge()
        {
            Assert.Throws<ArgumentException>(() => actualGraph.AddEdge(from: v3, to: v3, type: Edge<int>.EdgeType.outgoing, name: "cycle", weight: 1));
        }

        [Test()]
        public void ShouldThrowArgumentExceptionWhenAddingEdgeWithNoWeight()
        {
            Assert.Throws<ArgumentException>(() => actualGraph.AddEdge(from: v3, to: v3, type: Edge<int>.EdgeType.outgoing, name: "noweight"));
        }

        [Test()]
        public void ShouldThrowArgumentExceptionWhenAddingEdgeWithUnexistingFromVertex()
        {
            var v = new Vertex<int>(-1, "uknown");

            Assert.Throws<ArgumentException>(() => actualGraph.AddEdge(from: v, to: v3, type: Edge<int>.EdgeType.outgoing, name: "unexisting vertex"));
        }

        [Test()]
        public void ShouldThrowArgumentExceptionWhenAddingEdgeWithUnexistingToVertex()
        {
            var v = new Vertex<int>(1, "uknown");

            Assert.Throws<ArgumentException>(() => actualGraph.AddEdge(from: v3, to: v, type: Edge<int>.EdgeType.outgoing, name: "unexisting vertex"));
        }

        [Test()]
        public void ShouldAddNeighborsAndEgesInBothVerticesInEdgeIfAddingBidirectionalEdge()
        {
            var v1 = new Vertex<string>("A");
            var v2  = new Vertex<string>("B");
            var actualGraph = new DirectedAcyclicGraph<string>(new List<Vertex<string>> { v1, v2 });

            actualGraph.AddEdge(v1, v2, "connect", Edge<string>.EdgeType.outgoing, Edge<string>.EdgeDirection.bidirectional, 1);

            Assert.IsTrue((v1.Neighbors.Count == 2) && (v2.Neighbors.Count == 2), "Neighbor Count");
            Assert.IsTrue((v1.Edges.Count == 2) && (v2.Edges.Count == 2), "Total edge count");
            Assert.IsTrue((v1.OutgoingEdges().Count == 1) && (v1.IncomingEdges().Count == 1) && (v2.OutgoingEdges().Count == 1) && (v2.IncomingEdges().Count == 1), "Edge counnt by typr");
        }

        [Test()]
        public void ShouldNotRemoveAnyVertexIfItsValueIsDuplicate()
        {
            var actualValue = v1.Value;
            var actualVertex = actualGraph.RemoveVertex(actualValue);

            Assert.AreEqual(null, actualVertex, "Deleted nothing.");
            Assert.AreEqual(2, actualGraph.Vertices.Count(v => v.Value == actualValue), $"Two vertices with value ({actualValue}) exist.");
            Assert.IsTrue(actualGraph.Vertices.Contains(v1), "V1 exists.");
        }

        [Test()]
        public void ShouldRemoveEdgesOnlyBetweenTwoGivenVertices()
        {
            // v1 has two eges to v2 and one to v4
            Assert.AreEqual(3, v1.OutgoingEdges().Count, "Initially there should be three outgoing edges from.");

            // delete edges between v1 and v2
            Assert.AreEqual(2, actualGraph.RemoveEdges(v1, v2, Edge<int>.EdgeType.outgoing).Count, "Between v1 and v2 there are two edges.");
            Assert.AreEqual(1, v1.OutgoingEdges().Count, "After removing the edges from v1 that go to v2 only one remains.");
        }

        [Test()]
        public void ShouldBeAbleToGetAVertexByName()
        {
            var expectedVertex = actualGraph.GetVertex(v1.Name);

            Assert.AreEqual(expectedVertex, v1);
        }

        [Test()]
        public void ShouldGetAListWithAllEdgesInGraphThatHaveTheSameName()
        {
            var expectedEdges = actualGraph.GetEdgesByName("add");

            Assert.AreEqual(3, expectedEdges.Count);
        }

        [Test()]
        public void ShouldUseGetRootToRetrieveAListOfAllVerticesWithNoIncomingEdges()
        {
            var expectedVertices = actualGraph.GetRoot();

            Assert.AreEqual(1, expectedVertices.Count, "There should be only one vertex returned");
            Assert.AreEqual(v0.Name, expectedVertices[0].Name, "The single root should be the test vertex named zero");

        }

        [Test()]
        public void ShouldOnlyHaveOnlyOneRootASimpleTreeLikeGraph()
        {
            var g = CreateMultiplePathsTestNumericGraph();
            var expectedRoot = g.GetRoot();
            var expectedVertices = expectedRoot.Count;

            Assert.AreEqual(1, expectedVertices, "There should be only one vertex returned");
            Assert.AreEqual("0", expectedRoot[0].Name, "The single root should be the vertex named '0'");

        }

        [Test()]
        public void ShouldHaveFixedNumberOfPaths()
        {
            var g = CreateStringTestGraph();
            var actualPathCount = g.GetPaths().Count;
            double excpectedPathCount = 2;
            Assert.AreEqual(excpectedPathCount, actualPathCount);

            var otherGraph = CreateMultiplePathsTestNumericGraph();
            actualPathCount = otherGraph.GetPaths().Count;
            excpectedPathCount = 6;
            Assert.AreEqual(excpectedPathCount, actualPathCount);
        }

        [Test()]
        public void ShouldGetAsLongestPathThePathWithLargestWeight()
        {
            const int expectedPathWeight = 5;

            var g = CreateStringTestGraph();
            var path = g.GetLongestPath();
            Assert.AreEqual(expectedPathWeight, path.Weight);
        }

        [Test()]
        public void ShouldGraphHaveStringRepresentationAsCommaSeparatedListOfVerticesWithEdges()
        {
            // TODO: Remove hard-coding
            var expectedRepresentation = "a:[oneword(a-n):outgoing:1],n:[oneword(a-n):incoming:1, oneword(n-d):outgoing:1]d:[oneword(n-d):incoming:1]";
            var g = CreateStringTestSinglePathGraph();
            
            var actualRepresentation = g.ToString(true);

            Assert.AreEqual(expectedRepresentation, actualRepresentation);
        }

        [Test()]
        public void ShouldGetShortestPathBetweenTwoGivenVerticesFollowingDikstraAlgorithm()
        {
            //Assert.Pass();
            var expectedDistance = 7;
            var expectedPath = "A,D,E,C";
            var g = CreateComputerScienceGraph();

            var (actualDistance, actualPath) = g.GetDijkstraSingleShortestPath("A", "C");
            var actualShortestPath = string.Join(",", actualPath.Vertices.Select(v => v.Name));

            Assert.AreEqual(expectedDistance, actualDistance, "Computer Science graph: A-C shortest distance");
            Assert.AreEqual(expectedPath, actualShortestPath, "Computer Science graph: A-C shortest path");

            expectedDistance = 10;
            expectedPath = "A,B,G";
            g = CreateSWEGraph();

            (actualDistance, actualPath) = g.GetDijkstraSingleShortestPath("A", "G");
            actualShortestPath = string.Join(",", actualPath.Vertices.Select(v => v.Name));

            Assert.AreEqual(expectedDistance, actualDistance, "SWE graph: A-G shortest distance");
            Assert.AreEqual(expectedPath, actualShortestPath, "SWE graph: A-G shortest path");

            expectedDistance = 19;
            expectedPath = "0,1,3,4,6";
            g = CreaateFreeCodeCampGraph();

            (actualDistance, actualPath) = g.GetDijkstraSingleShortestPath("0", "6");
            actualShortestPath = string.Join(",", actualPath.Vertices.Select(v => v.Name));

            Assert.AreEqual(expectedDistance, actualDistance, "Free Code Camp graph: 0-6 shortest distance");
            Assert.AreEqual(expectedPath, actualShortestPath, "Free Code Camp graph: 0-6 shortest path");
        }

        [Test()]
        public void ShoulGetShortestPathDistancBetweenSameGivenVertexAsZero()
        {
            var expectedDistance = 0;
            var expectedPath = "";
            var v1 = new Vertex<string>("A");
            var g = new DirectedAcyclicGraph<string>(new List<Vertex<string>> { v1 });

            var (actualDistance, actualPath) = g.GetDijkstraSingleShortestPath("A", "A");
            var actualShortestPath = string.Join(",", actualPath.Vertices.Select(v => v.Name));

            Assert.AreEqual(expectedDistance, actualDistance, "1 vertex graph: 0 shortest distance");
            Assert.AreEqual(expectedPath, actualShortestPath, "1 vertex graph: no shortest path");
        }

        [Test()]
        public void ShouldHaveCorrspondingNeighborsForAllEdgesInGraph()
        {
            var g = CreateComputerScienceGraph();

            foreach (var V in g.Vertices)
            {
                foreach (var E in V.Edges)
                {
                    var actualEdgeFromVertex = E.From;
                    var actualEdgeToVertex = E.To;

                    if (E.Type == Edge<string>.EdgeType.outgoing)
                        Assert.IsTrue(V.Neighbors.Where(n => n.Type == Neighbor<string>.NeighborType.outgoing && n.Node.Guid == actualEdgeToVertex.Guid).FirstOrDefault() != null, "outgoing check");

                    if (E.Type == Edge<string>.EdgeType.incoming)
                        Assert.IsTrue(V.Neighbors.Where(n => n.Type == Neighbor<string>.NeighborType.incoming && n.Node.Guid == actualEdgeFromVertex.Guid).FirstOrDefault() != null, "inconcoming check");
                }
            }
        }

        [Test()]
        public void ShholdConvertAdjacencyMatrixToListTest()
        {
            double[][] actualMatrix;
            Dictionary<int, List<int>> expectedList = new Dictionary<int, List<int>>
            {
                { 0, new List<int> { 1,2,3,4 } },
                { 1, new List<int> { 5,6 } },
                { 2, new List<int> { 3 } },
                { 3, new List<int> { 0 } }
            };
            //beware that ditionary uses matrix indices (node index) instead of the actual node names

            actualGraph = CreateCycleTestGraph();
            actualMatrix = actualGraph.GetAdjacencyMatrix();
            var actullList = actualGraph.GetAdjacencyList(actualMatrix);

            Assert.AreEqual(expectedList, actullList);
        }

        [Test()]
        public void ShouldDetectCycleInGraph()
        {
            double[][] actualMatrix;
            bool actualHasCycle;
            Dictionary<int, List<int>> actualAdjacency;

            actualGraph = CreateCycleTestGraph();
            actualMatrix = actualGraph.GetAdjacencyMatrix();
            actualAdjacency = actualGraph.GetAdjacencyList(actualMatrix);
            actualHasCycle = actualGraph.DetectCycle(actualAdjacency, expectedVertexCount);

            Assert.IsTrue(actualHasCycle, "Graph should have a cycle.");
        }

        [Test()]
        public void ShouodMatchVertexNameAndIndex()
        {
            DirectedAcyclicGraph<int> actalGraph;
            int expectedIndex;
            string expectedName;

            actalGraph = new DirectedAcyclicGraph<int>();
            actalGraph.AddVertex(new Vertex<int>(100, "first"));
            expectedName = "second";
            expectedIndex = 1;
            actalGraph.AddVertex(new Vertex<int>(200, expectedName));

            var actualName = actalGraph.GetVertexName(expectedIndex);

            Assert.AreEqual(expectedName, actualName, "Vertex Name");
        }

        #region Support Methods
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

        /// <summary>
        /// Numeric graph with with six paths one root and one leaf 
        /// </summary>
        /// <returns></returns>
        private DirectedAcyclicGraph<int> CreateMultiplePathsTestNumericGraph()
        {
            var vertices = new List<Vertex<int>>();

            foreach (var i in new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 })
            {
                vertices.Add(new Vertex<int>(i));
            }

            var g = new DirectedAcyclicGraph<int>(vertices);

            g.AddEdge(vertices[0], vertices[1], name: null , weight: 1);
            g.AddEdge(vertices[0], vertices[2], name: null , weight: 2);
            g.AddEdge(vertices[0], vertices[3], name: null , weight: 3);

            g.AddEdge(vertices[1], vertices[4], name: null , weight: 4);
            g.AddEdge(vertices[1], vertices[5], name: null , weight: 5);

            g.AddEdge(vertices[2], vertices[6], name: null , weight: 6);
            g.AddEdge(vertices[2], vertices[7], name: null , weight: 7);

            g.AddEdge(vertices[3], vertices[8], name: null , weight: 8);
            g.AddEdge(vertices[3], vertices[9], name: null , weight: 9);

            g.AddEdge(vertices[4], vertices[10], name: null , weight: 10);
            g.AddEdge(vertices[5], vertices[10], name: null , weight: 11);
            g.AddEdge(vertices[6], vertices[10], name: null , weight: 12);
            g.AddEdge(vertices[7], vertices[10], name: null , weight: 13);
            g.AddEdge(vertices[8], vertices[10], name: null , weight: 14);
            g.AddEdge(vertices[9], vertices[10], name: null , weight: 15);

            return g;
        }

        private DirectedAcyclicGraph<int> CreateCycleTestGraph()
        {


            var g = new DirectedAcyclicGraph<int>();

            foreach (var i in new int[] { 1, 2, 3, 4, 5, 6, 7 })
            {
                g.AddVertex(new Vertex<int>(i));
            }
            expectedVertexCount = 7;

            g.AddEdge(g[0], g[1], name: null, weight: 1);
            g.AddEdge(g[0], g[2], name: null, weight: 1);
            g.AddEdge(g[0], g[3], name: null, weight: 1);
            g.AddEdge(g[0], g[4], name: null, weight: 1);
            g.AddEdge(g[1], g[5], name: null, weight: 1);
            g.AddEdge(g[1], g[6], name: null, weight: 1);
            g.AddEdge(g[2], g[3], name: null, weight: 1);
            g.AddEdge(g[3], g[0], name: null, weight: 1);
            expectedOutgoingEdgeCount = 8;

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
            expectedVertexCount = vertices.Count;

                var g = new DirectedAcyclicGraph<int>(vertices);
                g.AddEdge(from: v0, to: v1, name: "start",  weight: 5);
                g.AddEdge(from: v1, to: v2, name: "add" , weight: 4);
                g.AddEdge(from: v2, to: v3, name: "add" , weight: 1);
                g.AddEdge(from: v1, to: v2, name: "substract" , weight: 2);
                g.AddEdge(from: v1, to: v4, name: "copy" , weight: 3);
                g.AddEdge(from: v4, to: v5, name: "add" , weight: 6);

            expectedOutgoingEdgeCount = 6;

            return g;
            // graph:
            // vertex (neighbors,edges)
            // vertex -weight-> vertex
            //
            // v0 (1,1) -5-> v1 (3,3) -2-> v2 (2,1) -1-> v3 (1,0)
            //              |            ^                
            //              |_____4______|                
            //              |-----3---------> v4 (2,1) --6-> v5,0)fc0
        }

        #endregion    
    }
    }