using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JuanMartin.Kernel.Utilities.DataStructures
{
    public class UndirectedGraph<T> : Graph<T>, IGraph<T>
    {
        public UndirectedGraph()
        {
            Vertices = new HashSet<Vertex<T>>();
            VertexUris = new Dictionary<string, string>();
        }
        public UndirectedGraph(IEnumerable<Vertex<T>> nodes = null) : this()
        {
            foreach (var node in nodes)
                VertexUris.Add(node.Guid, node.Name);

            Vertices = nodes?.ToHashSet();
        }

        public new bool AddEdge(Vertex<T> from, Vertex<T> to, string name, Edge<T>.EdgeType type = Edge<T>.EdgeType.none, Edge<T>.EdgeDirection direction = Edge<T>.EdgeDirection.undirected, double weight = 0)
        {
            return base.AddEdge(from, to, name, type, direction, weight);
        }

        public bool AddEdge(Edge<T> edge)
        {
            return AddEdge(edge.From, edge.To, edge.Name, edge.Type, edge.Direction, edge.Weight);
        }

        public bool AddEdge(string nameFrom, string nameTo, string name, Edge<T>.EdgeType type = Edge<T>.EdgeType.none, Edge<T>.EdgeDirection direction = Edge<T>.EdgeDirection.undirected, double weight = 0)
        {
            return AddEdge(GetVertex(nameFrom), GetVertex(nameTo), name, type, direction, weight);
        }

        public new Edge<T> RemoveEdge(Vertex<T> from, Vertex<T> to, string name, double weight, Edge<T>.EdgeType type, Edge<T>.EdgeDirection direction = Edge<T>.EdgeDirection.none)
        {
            //  undirected edges have an outgoing and incoming couhterparts
            base.RemoveEdge(from, from, name, weight, Edge<T>.EdgeType.incoming, direction);
            return base.RemoveEdge(from, to, name, weight, Edge<T>.EdgeType.outgoing, direction);
        }

        public void AddEdges(double[][] matrix)
        {
            throw new NotImplementedException();
        }

        public List<Edge<T>> GetUndirectedEdges()
        {
            //  undirected edges have an outgoing and incoming couhterparts
            var edges = new List<Edge<T>>();

            foreach (var v in Vertices)
                edges.AddRange(v.Edges.Where(e=>e.Direction==Edge<T>.EdgeDirection.undirected  && e.Type == Edge<T>.EdgeType.outgoing));

            return edges;
        }

        /// <summary>
        /// Where, the value a ij equals the number of edges from the vertex i to j. For an undirected 
        /// graph, the value a ij = a ji for all i, j , so that the adjacency matrix becomes a symmetric 
        /// matrix:   <see cref="https://byjus.com/maths/symmetric-matrix/#:~:text=A%20matrix%20is%20called%20a%20symmetric%20matrix%20if,is%20any%20matrix%2C%20and%20AT%20is%20its%20transpose."/>
        /// 
        /// </summary>
        /// <returns></returns>
        public double[][] GetAdjacencyMatrix()
        {
            int nodes = Vertices.Count;
            var matrix = new double[nodes][];

            for (int i = 0; i < nodes; i++)
            {
                var from = GetVertex(i);
                if (from == null)
                    throw new ArgumentNullException($"Vertex for Index ( {i}) not defined.");

                matrix[i] = new double[nodes];

                for (int j = 0; j < nodes; j++)
                {
                    var to = GetVertex(j);

                    if (to == null)
                        throw new ArgumentNullException($"Vertex for Index ( {j}) not defined.");

                    string n = $"{from.Name}-{to.Name}";
                    var edge = GetEdge(name: n, direction: Edge<T>.EdgeDirection.undirected); ;
                    double w = 0;

                    if (edge != null)
                        w = edge.Weight;

                    matrix[i][j] = w;
                    // with undirected edges add i,j and j,i simultaneously: symmetry
                    if (j < i)
                    {
                        var m = matrix[j][i];
                        if (m != 0)
                            matrix[i][j] = m ;
                    }
                }
            }
            return matrix;
        }

        /// <summary>
        /// <see cref="https://www.geeksforgeeks.org/detect-cycle-undirected-graph/"/>
        /// </summary>
        public bool IsCyclic()
        {
            UnVisitedVertices();

            // Call recursive helper to chek for cycles in every DDFS tree
            foreach (var v in Vertices)
            {
                if (!v.IsVisited)
                {
                    if (DetectCycleWithDepthFirstTraversal(v, null))
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// c<see cref="https://en.wikipe]dia.org/wiki/Minimum_spanning_tree"/>
        /// Implemented through Kruskal's algotithm  
        /// </summary>
        /// <returns></returns>
        public UndirectedGraph<T> GetMinimumSpanningTree()
        {
            var forest = new UndirectedGraph<T>();

            foreach (var v in Vertices)
                forest.AddVertex(new Vertex<T>(value: v.Value, name: v.Name));

            int trees = forest.VertexCount();
            var edges = GetUndirectedEdges();

            edges.Sort(); // default sort by weight ascending
            var count = 0;
            int outgoing = 0;
            while (count < edges.Count && outgoing != trees - 1)
            {
                var e = edges[count];
                // replaces edge's to/from with corressssponding trees in new forest
                e.To = forest.GetVertex(e.To.Name);
                e.From = forest.GetVertex(e.From.Name);

                forest.AddEdge(e);
                count++;

                if (forest.IsCyclic())
                    forest.RemoveEdge(e.From, e.To, e.Name, e.Weight, e.Type, Edge<T>.EdgeDirection.undirected);

                outgoing = forest.GetUndirectedEdges().Count;
            }

            return forest;
        }

        /// <summary>

        /// Translate adjacency matrix into adjacency list <seealso cref="https://www.section.io/engineering-education/graphs-in-data-structure-using-cplusplus/"/>,
        /// referencing vertices by their Index.
        /// <seealso cref="https://www.geeksforgeeks.org/convert-adjacency-matrix-to-adjacency-list-representation-of-graph/#:~:text=To%20convert%20an%20adjacency%20matrix%20to%20the%20adjacency,at%20i-th%20position%20in%20the%20array%20of%20lists."/>
        /// </summary>
        /// <param name="adjacency">adjacency matrix</param>
        /// <param name="translateIndicesToNodeNames">Dictionary values are the matrix columns indices, so covert them to their associated vertex names </param>
        /// <returns></returns>
        public Dictionary<int, List<int>> GetAdjacencyList(double[][] adjacency)//, bool translateIndicesToNodeNames=false)
        {
            // Created the jagged array. It contains the vertexes and how they are to be connected.
            // E.g. new int[]{ 1,2}, means 1 is to be connected to 2
            // This is the dictionary for storing the adjacency list.
            // It is of the type, int that will hold a node and List<int> that will hold all other nodes 
            // attached to the int node.
            Dictionary<int, List<int>> ls = new Dictionary<int, List<int>>();
            int[][] graph;
            var g = new List<int[]>();

            for (int i = 0; i < adjacency.Length; i++)
            {
                for (int j = 0; j < adjacency[i].Length; j++)
                {
                    if (adjacency[i][j] > 0)  // != 0)
                    {
                        var adjacent = new List<int> { i, j };
                        g.Add(adjacent.ToArray());
                    }
                }
            }
            graph = g.ToArray();
            // Loop through our jagged array, graph.
            for (int i = 0; i < graph.Length; i++)
            {
                if (graph[i].Length == 0)
                    continue;

                // As we loop, check whether our dictionary already contains the node at index[i][0]
                // of our jagged array, graph. If it is not there, we add it to the dictionary, ls
                if (!ls.ContainsKey(graph[i][0]))
                {
                    ls.Add(graph[i][0], new List<int>());
                }
                // this line of code will connect the nodes. E.g. If we are given { 1,2}, we added 1 to our dictionary
                // on the line  ls.Add(graph[i][0], new List<int>());
                //Therefore, in this next line,ls[graph[i][0]].Add(graph[i][1]); we connect the 1 to the 2
                ls[graph[i][0]].Add(graph[i][1]);
            }

            return ls;
        }

        #region Support Methods
        private bool DetectCycleWithDepthFirstTraversal(Vertex<T> v, object p)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
