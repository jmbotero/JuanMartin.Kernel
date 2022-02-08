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
    public class VertexTests
    {
        private UndirectedGraph<int> actualGraph;
        private Vertex<int> actualFrom, actualTo;

        [SetUp]
        public void Init()
        {
            actualGraph = new UndirectedGraph<int>();

            foreach (var i in Enumerable.Range(0, 2))
            {
                actualGraph.AddVertex(new Vertex<int>(value: i, name: Convert.ToChar(i + 97).ToString()));
            }

            actualFrom = actualGraph.GetVertex(name: "a");
            actualTo = actualGraph.GetVertex(name: "b");
        }

        [TearDown]
        public void Dispose()
        {
            actualGraph = null;
            actualFrom = null;
            actualTo = null;
        }

        [Test()]
        public void ShouldThrowArgumentExceptionIfNoGetEdgePropertiesAreSpecified()
        {
            Assert.Throws<ArgumentException>(() => actualGraph.GetEdge());
        }

        [Test()]
        public void ShouldGetEdgeFilteringOnAllEdgeProperties()
        {
            string actualName = "test";
            Edge<int> expectedEdgeAll, expectedEdgeOne, expectedEdgeTwo, expectedEdgeThree, expectedEdgeFour, expectedEdgeFive;
            Edge<int> actualEdge = UtilityGraph<int>.NewEdge(name: actualName,
                                                                                    to: actualTo,
                                                                                    from: actualFrom,
                                                                                    // by default undirectedgraph adds both incoming and outgoing but this isb overwritten if original type is any 
                                                                                    type: Edge<int>.EdgeType.any,
                                                                                    // by default undirectedgraph sets direction to 'undirected'
                                                                                    direction: Edge<int>.EdgeDirection.undirected,
                                                                                    weight: 5);
            _ = actualGraph.AddEdge(actualEdge);

            expectedEdgeAll = actualFrom.GetEdge(actualName, actualFrom.Name, actualTo.Name, Edge<int>.EdgeType.any, Edge<int>.EdgeDirection.undirected, 5);
            expectedEdgeOne = actualFrom.GetEdge(actualName);
            expectedEdgeTwo = actualFrom.GetEdge(actualName, actualFrom.Name);
            expectedEdgeThree = actualFrom.GetEdge(actualName, actualFrom.Name, actualTo.Name);
            expectedEdgeFour = actualFrom.GetEdge(actualName, actualFrom.Name, actualTo.Name, Edge<int>.EdgeType.any);
            expectedEdgeFive = actualFrom.GetEdge(actualName, actualFrom.Name, actualTo.Name, Edge<int>.EdgeType.any, Edge<int>.EdgeDirection.undirected);

            Assert.IsTrue(expectedEdgeAll.Equals(actualEdge), "All property get");
            Assert.AreEqual(expectedEdgeAll, expectedEdgeOne, "One property get");
            Assert.AreEqual(expectedEdgeAll, expectedEdgeTwo, "Two property get");
            Assert.AreEqual(expectedEdgeAll, expectedEdgeThree, "Three property get");
            Assert.AreEqual(expectedEdgeAll, expectedEdgeFour, "Four property get");
            Assert.AreEqual(expectedEdgeAll, expectedEdgeFive, "Five property get");
        }
    }
}