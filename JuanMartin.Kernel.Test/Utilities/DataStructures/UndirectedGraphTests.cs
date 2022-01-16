using NUnit.Framework;
using JuanMartin.Kernel.Utilities.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JuanMartin.Kernel.Utilities.DataStructures.Tests
{
    [TestFixture()]
    public class UndirectedGraphTests
    {
        private int expectedVertexCount = 0, expectedUndirectedEdgeCount = 0;
        private UndirectedGraph<int> actualGraph;

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

            actualGraph = CreateGatevidyalayKruskalsAlgorithmExampleGraph01();
            actualHasCycle = actualGraph.IsCyclic();

            Assert.IsTrue(actualHasCycle, "Graph should have a cycle.");
        }

        [Test()]
        public void ShouldGetMinimumSpanningTreeTest()
        {
            actualGraph = CreateGatevidyalayKruskalsAlgorithmExampleGraph01();

            var actualSubsetGraph = actualGraph.GetMinimumSpanningTree();
            double expectedWeight = 18;

            var actualUndirectedEdges = actualSubsetGraph.GetUndirectedEdges().ToList();
            var actualWeight = actualUndirectedEdges.Sum(e => e.Weight);

            Assert.AreEqual(expectedWeight, actualWeight);
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


            graph.AddEdge(graph[0], graph[1], name: null, weight: 1);
            graph.AddEdge(graph[0], graph[2], name: null, weight: 2);
            graph.AddEdge(graph[1], graph[2], name: null, weight: 3);
            graph.AddEdge(graph[2], graph[3], name: null, weight: 4);
            graph.AddEdge(graph[2], graph[5], name: null, weight: 6);
            graph.AddEdge(graph[2], graph[4], name: null, weight: 5);
            graph.AddEdge(graph[3], graph[4], name: null, weight: 7);
            graph.AddEdge(graph[4], graph[5], name: null, weight: 8);
            expectedUndirectedEdgeCount = 8;

            return graph;
        }
        #endregion
    }
}