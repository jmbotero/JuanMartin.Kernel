using JuanMartin.Kernel.Extesions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JuanMartin.Kernel.Utilities;

namespace JuanMartin.Kernel.Utilities.DataStructures
{
    /// <summary>
    /// Implementation of a undirected graph
    /// copied from https://codereview.stackexchange.com/questions/131583/generic-graph-implementation-in-c
    /// </summary>
    /// <seealso cref="https://youtu.be/KmVSCv6Bn8E"/>  
    /// <seealso cref="https://docs.microsoft.com/en-us/previous-versions/ms379574(v=vs.80)?redirectedfrom=MSDN"/>
    public class DirectedAcyclicGraph<T> : Graph<T>, IGraph<T>
    {
        public DirectedAcyclicGraph()
        {
            Vertices = new HashSet<Vertex<T>>();
            VertexUris = new Dictionary<string, string>();
        }
        public DirectedAcyclicGraph(IEnumerable<Vertex<T>> nodes = null) : this()
        {
            foreach (var node in nodes)
                VertexUris.Add(node.Guid, node.Name);

            Vertices = nodes?.ToHashSet();
        }

        public bool AddEdge(Edge<T> edge)
        {
            return AddEdge(edge.From, edge.To, edge.Name, edge.Type, edge.Direction, edge.Weight);
        }

        public bool AddEdge(string nameFrom, string nameTo, string name = null, Edge<T>.EdgeType type = Edge<T>.EdgeType.outgoing, Edge<T>.EdgeDirection direction = Edge<T>.EdgeDirection.unidirectional
, double weight = Edge<T>.EdgeWeightDefault)
        {
            return AddEdge(GetVertex(nameFrom), GetVertex(nameTo), name, type, direction, weight);
        }
        public new bool AddEdge(Vertex<T> from, Vertex<T> to, string name = null, Edge<T>.EdgeType type = Edge<T>.EdgeType.outgoing, Edge<T>.EdgeDirection direction = Edge<T>.EdgeDirection.unidirectional
            , double weight = Edge<T>.EdgeWeightDefault)
        {
            if(direction==Edge<T>.EdgeDirection.undirected)
                throw new ArgumentException("Undirected edges cannot be added to a directed-acyclic graph.");

            if (from.Equals(to))
                throw new ArgumentException("A loop edge, from and to the same vertex cannot be added in a directed-acyclic graph.");

            // default for directed cyclic graph is direction composite
            base.AddEdge(from, to, name, type, Edge<T>.EdgeDirection.composite, weight);
            try
            {
                if (direction == Edge<T>.EdgeDirection.bidirectional)
                {
                     // translate into four edges ; inverting to and from (therefore neighbors too)
                    // add outgoing edge
                    to.AddEdge(to, from, Edge<T>.EdgeType.outgoing, direction, name, weight);
                    to.AddNeighbor(from, Neighbor<T>.NeighborType.outgoing);
                    // if it is outgoing for the source vertex it is incoming for the target one
                    from.AddEdge(to, from, Edge<T>.EdgeType.incoming, direction, name, weight);
                    from.AddNeighbor(to, Neighbor<T>.NeighborType.incoming);
                }
            }
            catch (Exception)
            {
                throw;
                //return false;
            }

            return true;
        }

        /// <summary>
        /// Add outgoing edges to graph based on Adjacency Matrix A V*V binary matrix is an adjacency matrix. 
        /// There is an edge that is connecting vertex i and vertex j, element Ai,j is the weight
        /// of the edge, otherwise Ai,j is 0.
        /// </summary>
        ///
        /// <param name="matrix"></param>
        public void AddEdges(double[][] matrix)
        {
            int dimension = matrix.Length;

            if (dimension != Vertices.Count)
                throw new IndexOutOfRangeException($"Matrix size ({dimension}) does not match number of vertices defined in this graph ({Vertices.Count}).");

            for (int i = 0; i < dimension; i++)
            {
                for (int j = 0; j < dimension; j++)
                {
                    double w = (double)matrix[i][j];
                    if (w > 0)
                    {
                        var from = GetVertex(i);
                        var to = GetVertex(j);
                        string n = $"{from.Name}-{to.Name}";
                        var e = new Edge<T>(from, to, w, n, Edge<T>.EdgeType.outgoing, Edge<T>.EdgeDirection.unidirectional);
                        AddEdge(e);
                    }
                }
            }
        }

        /// <summary>
        /// An adjacency matrix is a matrix that contains rows and columns used to represent a simple 
        /// labeled graph with the numbers 0 and the edge weight in the position of (V­I, Vj), according 
        /// to the condition of whether or not the two Vi ­and Vj are adjacent.
        /// For a directed graph, if there is an edge exists between vertex i or Vi to Vertex j or Vj, 
        /// then the value of A[Vi][Vj] = Edge weight, otherwise the value will be 0.
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
                    var edge = GetEdge(name: n, type: Edge<T>.EdgeType.outgoing);
                    double w = 0;


                    if (edge != null)
                        w = edge.Weight;

                    matrix[i][j] = w;
                }
            }
            return matrix;
        }

        /// <summary>
        /// Translate adjacency matrix into adjacency list <seealso cref="https://www.section.io/engineering-education/graphs-in-data-structure-using-cplusplus/"/>,
        /// referencing vertices by their Index
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
                    if (adjacency[i][j] != 0)
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

        /// <summary>
        /// Implementing cycle detection using DFS:
        /// To detect a cycle in a graph, we visit the node, mark it as visited. Then visit all 
        /// the nodes connected through it. A cycle will be detected when visiting a node 
        /// that has been marked as visited and part of the current path.
        /// <see cref="https://www.section.io/engineering-education/graph-cycle-detection-csharp/#:~:text=Cycle%20detection%20in%20a%20directed%20graph%20To%20detect,as%20visited%20and%20part%20of%20the%20current%20path."/>
        /// </summary>
        /// <param name="adjacency">Adjacency matrix that identifies graph</param>
        /// <returns></returns>
        public bool DetectCycle(Dictionary<int, List<int>> adjacency, int nodes)
        {
            //nodes = adjacency.Length;
            // We declare a visited bool array variable. We will store the visited nodes in it.
            bool[] visited = new bool[nodes];
            // We declare a path bool array variable. We will store all nodes in our current path here.
            bool[] path = new bool[nodes];

            // We start our traversal here. We could also say that this is where we start our path from.
            for (int i = 0; i < nodes; i++)
            {
                // We do our Dfs starting from the node at i in this case our start point will be 0;
                // For each Dfs, we are checking if we will find a cycle. If yes, we immediately return true.
                // A cycle has been found.
                if (DetectCycleWithDepthFirstTraversal(adjacency, i, visited, path))

                    return true;

            }
            // If in our for loop above, we never found a cycle, then we will return false.
            // A cycle was not detected.
            return false;

        }

        #region Support Methods
        /// <summary>
        /// Search for a cycle using deppth search algorithm.
        /// Function that detects whether there is a cycle or not. Below is the code for the same. It uses recursion for backtracking.
        /// If a cycle is detected, we return true, otherwise, we return false.
        /// <see cref="https://www.section.io/engineering-education/graph-cycle-detection-csharp/#:~:text=Cycle%20detection%20in%20a%20directed%20graph%20To%20detect,as%20visited%20and%20part%20of%20the%20current%20path."/>
        /// </summary>
        /// <param name="graph">Jagged array representation of adjacency matrix</param>
        /// <param name="start">Index in graph of fitrst node in path</param>
        /// <param name="visited">The visited status of nodes</param>
        /// <param name="path">True or false if node represented by indeex in current path/param>
        /// <returns></returns>
        private static bool DetectCycleWithDepthFirstTraversal(Dictionary<int, List<int>> graph, int start, bool[] visited, bool[] path)
        {
            // If we find that we marked path[start] true, we return true.
            // This means that we have come back to the node we started from hence a cycle has
            // been found.
            if (path[start])
            {
                return true;
            }
            // If we didn't find a cycle from the code block above, we mark visited[start] to true.
            visited[start] = true;

            //  We also mark path[start] to true. This will help us know that the node start is on our
            //  current path.
            path[start] = true;

            // We check whether our graph contains the start node. Sometimes the start node is not in our graph.
            // Thus, if we do our traversal on such a node, an exception will be thrown. This is because the node does
            // not exist.
            if (graph.ContainsKey(start))
            {
                // We start our traversal from our start node of the graph.
                foreach (var item in graph[start])

                {
                    //We do our recursion
                    // At this point, if the start node returned a true in our recursive call, then we say that cycle has been
                    // found. We return true immediately.
                    if (DetectCycleWithDepthFirstTraversal(graph, item, visited, path))
                    {
                        return true;
                    }

                }

            }
            // If we have traversed the whole path from the start node and never found a cycle, we start removing
            // those nodes from this path. This is done recursively using c# inbuilt stack also called the call stack.
            path[start] = false;
            // If we did not find a cycle, we return false.
            return false;

        }

        #endregion
    }
}