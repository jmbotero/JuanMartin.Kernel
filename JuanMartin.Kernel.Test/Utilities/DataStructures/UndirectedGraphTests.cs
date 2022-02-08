using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JuanMartin.Kernel.Utilities.DataStructures.Tests
{
    [TestFixture()]
    public class UndirectedGraphTests
    {
        private int expectedVertexCount;
        private int expectedUndirectedEdgeCount;
        private UndirectedGraph<int> actualGraph;

        [SetUp]
        public void Init()
        {
        }

        [TearDown]
        public void Dispose()
        {
            actualGraph = null;
        }
        [Test()]
        public void ShouldCreateAnAdjacencyMatrixWithAllUndirectedEdgesInGraph()
        {
            double[][] actualMatrix;
            double[][] expectedMatrix = new double[][]
              {
                        new double[]{ 0,1,2,0,0,0 },
                        new double[]{ 1,0,3,0,0,0 },
                        new double[]{ 2,3,0,4,5,6 },
                        new double[]{ 0,0,4,0,7,0 },
                        new double[]{ 0,0,5,7,0,8 },
                        new double[]{ 0,0,6,0,8,0 },

            };

            actualGraph = CreateGatevidyalayKruskalsAlgorithmExampleGraph01();

            actualMatrix = actualGraph.GetAdjacencyMatrix();

            Assert.AreEqual(expectedMatrix, actualMatrix, "Adjacency matrix");
            //due to symmetry divideactual count by two
            Assert.AreEqual(expectedUndirectedEdgeCount, actualMatrix.Sum(row => row.Count(cell => cell > 0)) / 2, "Edge count");
            Assert.AreEqual(expectedVertexCount, actualMatrix.Length, "Vertex count");
        }

        [Test()]
        public void ShholdConvertAdjacencyMatrixToListTest()
        {
            double[][] actualMatrix;
            Dictionary<int, List<int>> expectedList = new Dictionary<int, List<int>>
            {
                { 0, new List<int> { 1,2 } },
                { 1, new List<int> { 0,2 } },
                { 2, new List<int> { 0,1,3,4,5 } },
                { 3, new List<int> { 2,4 } },
                { 4, new List<int> { 2,3,5 } },
                { 5, new List<int> { 2,4 } }
            };
            //beware that ditionary uses matrix indices (node index) instead of the actual node names

            actualGraph = CreateGatevidyalayKruskalsAlgorithmExampleGraph01();
            actualMatrix = actualGraph.GetAdjacencyMatrix();
            var actullList = actualGraph.GetAdjacencyList(actualMatrix);

            Assert.AreEqual(expectedList, actullList);
        }

        [Test()]
        public void ShouldDetectCycleInGraph()
        {
            bool actualHasCycle;

            actualGraph = CreateCycleTestGrah();
            actualHasCycle = actualGraph.IsCyclic();

            Assert.IsTrue(actualHasCycle, "Graph should have a cycle.");

            actualGraph = CreateSimpleGrah();
            actualHasCycle = actualGraph.IsCyclic();

            Assert.IsFalse(actualHasCycle, "Graph should not have a cycle.");

            actualGraph = CreateSimpleGrah(true);
            actualHasCycle = actualGraph.IsCyclic();

            Assert.IsTrue(actualHasCycle, "Graph should have a cycle.");
        }

        [Test()]
        public void ShouldGetMinimumSpanningTreeTest()
        {
            UndirectedGraph<int> actualSubsetGraph;
            double expectedWeight;
            List<Edge<int>> actualUndirectedEdges;
            double actualWeight;

            actualGraph = CreateGatevidyalayKruskalsAlgorithmExampleGraph02();
            actualSubsetGraph = actualGraph.GetMinimumSpanningTreeWithPrimAlgorithm();
            expectedWeight = 99;

            actualUndirectedEdges = actualSubsetGraph.GetUndirectedEdges().ToList();
            actualWeight = actualUndirectedEdges.Sum(e => e.Weight);

            Assert.AreEqual(expectedWeight, actualWeight, "Prim Problem 01 graph");

            actualGraph = CreateGatevidyalayPrimsAlgorithmExampleGraph02();
            actualSubsetGraph = actualGraph.GetMinimumSpanningTreeWithPrimAlgorithm();
            expectedWeight = 26;

            actualUndirectedEdges = actualSubsetGraph.GetUndirectedEdges().ToList();
            actualWeight = actualUndirectedEdges.Sum(e => e.Weight);

            Assert.AreEqual(expectedWeight, actualWeight, "Prim Problem 02 graph");

            actualGraph = CreateMinimumSpanningTreeTestGraph();
            actualSubsetGraph = actualGraph.GetMinimumSpanningTreeWithKruskalAlgorithm();
            expectedWeight = 37;

            actualUndirectedEdges = actualSubsetGraph.GetUndirectedEdges().ToList();
            actualWeight = actualUndirectedEdges.Sum(e => e.Weight);

            Assert.AreEqual(expectedWeight, actualWeight, "Kruskal GfG graph");

            actualGraph = CreateGatevidyalayKruskalsAlgorithmExampleGraph01();
            actualSubsetGraph = actualGraph.GetMinimumSpanningTreeWithKruskalAlgorithm();
            expectedWeight = 18;

            actualUndirectedEdges = actualSubsetGraph.GetUndirectedEdges().ToList();
            actualWeight = actualUndirectedEdges.Sum(e => e.Weight);

            Assert.AreEqual(expectedWeight, actualWeight, "Kruskal Concept 01 graph");

            actualGraph = CreateGatevidyalayKruskalsAlgorithmExampleGraph02();
            actualSubsetGraph = actualGraph.GetMinimumSpanningTreeWithKruskalAlgorithm();
            expectedWeight = 99;

            actualUndirectedEdges = actualSubsetGraph.GetUndirectedEdges().ToList();
            actualWeight = actualUndirectedEdges.Sum(e => e.Weight);

            Assert.AreEqual(expectedWeight, actualWeight,"Kruskal Problem 02 graph");
        }

        #region Support Methods
        /// <summary>
        /// <see cref="https://www.gatevidyalay.com/tag/kruskals-algorithm-example-with-solution/"/>
        /// </summary>
        /// <returns></returns>
        private UndirectedGraph<int> CreateGatevidyalayKruskalsAlgorithmExampleGraph01()
        {
            var graph = new UndirectedGraph<int>();

            foreach (var i in Enumerable.Range(0, 6))
            {
                graph.AddVertex(new Vertex<int>(value: i, name: Convert.ToChar(i + 65).ToString()));
            }
            expectedVertexCount = 6;

            var vA = graph.GetVertex(name: "A");
            var vB = graph.GetVertex(name: "B");
            var vC = graph.GetVertex(name: "C");
            var vD = graph.GetVertex(name: "D");
            var vE = graph.GetVertex(name: "E");
            var vF = graph.GetVertex(name: "F");

            graph.AddEdge(vA, vB, name: null, weight: 1);
            graph.AddEdge(vA, vC, name: null, weight: 2);
            graph.AddEdge(vB, vC, name: null, weight: 3);
            graph.AddEdge(vC, vD, name: null, weight: 4);
            graph.AddEdge(vC, vF, name: null, weight: 6);
            graph.AddEdge(vC, vE, name: null, weight: 5);
            graph.AddEdge(vD, vE, name: null, weight: 7);
            graph.AddEdge(vE, vF, name: null, weight: 8);
            expectedUndirectedEdgeCount = 8;

            return graph;
        }

        private UndirectedGraph<int> CreateGatevidyalayPrimsAlgorithmExampleGraph02()
        {
            var graph = new UndirectedGraph<int>();
            expectedVertexCount = 7;

            foreach (var i in Enumerable.Range(0, expectedVertexCount))
            {
                graph.AddVertex(new Vertex<int>(value: i, name: Convert.ToChar(i + 97).ToString()));
            }

            var va = graph.GetVertex(name: "a");
            var vb = graph.GetVertex(name: "b");
            var vc = graph.GetVertex(name: "c");
            var vd = graph.GetVertex(name: "d");
            var ve = graph.GetVertex(name: "e");
            var vf = graph.GetVertex(name: "f");
            var vg = graph.GetVertex(name: "g");

            graph.AddEdge(va, vb, name: null, weight: 1);
            graph.AddEdge(va, vc, name: null, weight: 5);
            graph.AddEdge(vb, vc, name: null, weight: 4);
            graph.AddEdge(vb, vd, name: null, weight: 8);
            graph.AddEdge(vb, ve, name: null, weight: 7);
            graph.AddEdge(vc, vd, name: null, weight: 6);
            graph.AddEdge(vc, vf, name: null, weight: 2);
            graph.AddEdge(vd, ve, name: null, weight: 11);
            graph.AddEdge(vd, vf, name: null, weight: 9);
            graph.AddEdge(ve, vg, name: null, weight: 10);
            graph.AddEdge(ve, vf, name: null, weight: 3);
            graph.AddEdge(vf, vg, name: null, weight: 12);
            expectedUndirectedEdgeCount = 12;

            return graph;
        }

        private UndirectedGraph<int> CreateGatevidyalayKruskalsAlgorithmExampleGraph02()
        {
            var graph = new UndirectedGraph<int>();

            foreach (var i in Enumerable.Range(1, 7))
            {
                graph.AddVertex(new Vertex<int>(value: i, name: i.ToString()));
            }
            expectedVertexCount = 7;

            var v1 = graph.GetVertex(name: "1");
            var v2 = graph.GetVertex(name: "2");
            var v3 = graph.GetVertex(name: "3");
            var v4 = graph.GetVertex(name: "4");
            var v5 = graph.GetVertex(name: "5");
            var v6 = graph.GetVertex(name: "6");
            var v7 = graph.GetVertex(name: "7");

            graph.AddEdge(v1, v2, name: null, weight: 28);
            graph.AddEdge(v2, v3, name: null, weight: 16);
            graph.AddEdge(v2, v7, name: null, weight: 14);
            graph.AddEdge(v3, v4, name: null, weight: 12);
            graph.AddEdge(v4, v7, name: null, weight: 18);
            graph.AddEdge(v4, v5, name: null, weight: 22);
            graph.AddEdge(v5, v7, name: null, weight: 24);
            graph.AddEdge(v5, v6, name: null, weight: 25);
            graph.AddEdge(v6, v1, name: null, weight: 10);
            expectedUndirectedEdgeCount = 9;

            return graph;
        }

        /// <summary>
        /// Graph to test Kruskal algorithm
        /// <see cref="https://www.geeksforgeeks.org/kruskals-minimum-spanning-tree-algorithm-greedy-algo-2/"/>
        /// 
        /// </summary>
        /// <returns></returns>
        private UndirectedGraph<int> CreateMinimumSpanningTreeTestGraph()
        {
            var graph = new UndirectedGraph<int>();

            foreach (var i in new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 })
            {
                graph.AddVertex(new Vertex<int>(value: i));
            }
            expectedVertexCount = 9;

            graph.AddEdge(graph[7], graph[6], name: null, weight: 1);
            graph.AddEdge(graph[8], graph[2], name: null, weight: 2);
            graph.AddEdge(graph[6], graph[5], name: null, weight: 2);
            graph.AddEdge(graph[0], graph[1], name: null, weight: 4);
            graph.AddEdge(graph[2], graph[5], name: null, weight: 4);
            graph.AddEdge(graph[8], graph[6], name: null, weight: 6);
            graph.AddEdge(graph[2], graph[3], name: null, weight: 7);
            graph.AddEdge(graph[7], graph[8], name: null, weight: 7);
            graph.AddEdge(graph[0], graph[7], name: null, weight: 8);
            graph.AddEdge(graph[1], graph[2], name: null, weight: 8);
            graph.AddEdge(graph[3], graph[4], name: null, weight: 9);
            graph.AddEdge(graph[5], graph[4], name: null, weight: 10);
            graph.AddEdge(graph[1], graph[7], name: null, weight: 11);
            graph.AddEdge(graph[3], graph[5], name: null, weight: 14);
            expectedUndirectedEdgeCount = 14;

            return graph;
        }

        private UndirectedGraph<int> CreateSimpleGrah(bool addCycle = false)
        {
            var graph = new UndirectedGraph<int>();

            foreach (var i in Enumerable.Range(0, 4))
            {
                graph.AddVertex(new Vertex<int>(value: i, name: Convert.ToChar(i + 65).ToString()));
            }
            expectedVertexCount = 4;


            graph.AddEdge(graph[0], graph[1], name: null, weight: 1);
            graph.AddEdge(graph[1], graph[2], name: null, weight: 2);
            graph.AddEdge(graph[2], graph[3], name: null, weight: 3);
            expectedUndirectedEdgeCount = 3;

            if(addCycle)
            {
                graph.AddEdge(graph[3], graph[0], name: null, weight: 3);
                expectedUndirectedEdgeCount = 4;
            }

            return graph;
        }

        /// <summary>
        ///  <see cref="https://www.geeksforgeeks.org/detect-cycle-undirected-graph/"/>
        /// </summary>
        /// <returns></returns>
        private UndirectedGraph<int> CreateCycleTestGrah()
        {
            var graph = new UndirectedGraph<int>();

            foreach (var i in Enumerable.Range(0, 5))
            {
                graph.AddVertex(new Vertex<int>(value: i, name: i.ToString()));
            }
            expectedVertexCount = 5;

            var v0 = graph.GetVertex(name: "0");
            var v1 = graph.GetVertex(name: "1");
            var v2 = graph.GetVertex(name: "2");
            var v3 = graph.GetVertex(name: "3");
            var v4 = graph.GetVertex(name: "4");

            graph.AddEdge( v1, v0, name: null, weight: 1);
            graph.AddEdge(v1, v2, name: null, weight: 3);
            graph.AddEdge(v0 , v2, name: null, weight: 2);
            graph.AddEdge(v0, v3, name: null, weight: 2);
            graph.AddEdge(v3, v4, name: null, weight: 2);
            expectedUndirectedEdgeCount = 4;

            return graph;
        }

        #endregion
    }
}