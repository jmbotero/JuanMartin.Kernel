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

        public new bool AddEdge(Vertex<T> from, Vertex<T> to, string name, Edge<T>.EdgeType type = Edge<T>.EdgeType.none, Edge<T>.EdgeDirection direction = Edge<T>.EdgeDirection.undirected, double weight = Edge<T >.EdgeWeightDefault)
        {
            var path = $"({from.Name}-{to.Name})";

            if (name + "" == "")
                name = path;
            else if (!name.Contains(path))
                name += path;

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

        /// <summary>
        /// Get edge by searching under both vertices, undirected edges can be declared A->B or B->A 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="fromName"></param>
        /// <param name="toName"></param>
        /// <param name="type"></param>
        /// <param name="direction"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public new Edge<T> GetEdge(string name, string fromName, string toName, Edge<T>.EdgeType type, Edge<T>.EdgeDirection direction, double weight)
        { 
            var path = $"({fromName}-{toName})";
            var edge = base.GetEdge(path, fromName, toName, Edge<T>.EdgeTypeDefault, direction, weight);

            if (edge == null)
            {
                path = $"({toName}-{fromName})";
                edge = base.GetEdge(path, toName, fromName, Edge<T>.EdgeTypeDefault, direction, weight);
            }

            return edge;
        }
        public new Edge<T> RemoveEdge(Edge<T> edge)
        {
            return RemoveUndirectedEdge(edge.From, edge.To, edge.Name, edge.Weight);
        }
        private Edge<T> RemoveUndirectedEdge(Vertex<T> from, Vertex<T> to, string name, double weight)
        {
            if (from == null || to == null)
                throw new ArgumentNullException("Both vertices must be defined.");

            Edge<T> edge;

            //  undirected edges have an outgoing and incoming counterparts
            // there are two neighbors as well
            from.RemoveNeigbor(to, Neighbor<T>.NeighborType.outgoing);
            to.RemoveNeigbor(from, Neighbor<T>.NeighborType.incoming);

            //  the undirected graph edge is represented by two edges
            edge = UtilityGraph<T>.NewEdge(name, from, to, Edge<T>.EdgeType.incoming, Edge<T>.EdgeDirection.undirected, weight);
            //var edge = GetEdge(name, from.Name, to.Name, Edge<T>.EdgeType.incoming, direction, weight);
            to.RemoveEdge(edge);
            //edge = GetEdge(name, from.Name, to.Name, Edge<T>.EdgeType.outgoing, direction, weight);
            edge = UtilityGraph<T>.NewEdge(name, from, to, Edge<T>.EdgeType.outgoing, Edge<T>.EdgeDirection.undirected, weight);
            from.RemoveEdge(edge);


            return edge;
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
        /// Determine if graph has cycles
        /// <see cref="https://www.geeksforgeeks.org/detect-cycle-undirected-graph/"/>
        /// </summary>
        public bool IsCyclic()
        {

            // Call recursive helper to chek for cycles in every DDFS tree
            // cycles  need to checked for each path, DFS tree, independently,
            // so unmark visited vertices on every loop
            foreach (var v in Vertices)
            {
                UnvisitAllVertices();

                if (DetectCycleWithDepthFirstTraversal(v, null))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// c<see cref="https://en.wikipe]dia.org/wiki/Minimum_spanning_tree"/>
        /// <seealso cref="https://www.gatevidyalay.com/kruskals-algorithm-kruskals-algorithm-example/"/>
        /// Implemented through Kruskal's algotithm  
        /// </summary>
        /// <returns></returns>
        public UndirectedGraph<T> GetMinimumSpanningTreeWithKruskalAlgorithm()
        {
            var forest = new UndirectedGraph<T>();

            foreach (var v in Vertices)
                forest.AddVertex(new Vertex<T>(value: v.Value, name: v.Name, guid: v.Guid, index: v.Index));

            int trees = forest.VertexCount();
            var edges = GetUndirectedEdges();

            edges.Sort(); // default sort by weight ascending
            var count = 0;
            int outgoing = 0;
            while (count < edges.Count && outgoing != trees - 1)
            {
                var e = edges[count];
                // update  edge vertices to match new graph
                var toName = e.To.Name;
                var fromName = e.From.Name;

                e.To =forest.GetVertex(name: toName);
                e.From = forest.GetVertex(name: fromName);

                forest.AddEdge(e);
                count++;

                if (forest.IsCyclic())
                    forest.RemoveEdge(e);

                outgoing = forest.GetUndirectedEdges().Count;
            }

            return forest;
        }

        /// <summary>
        /// 
        /// <see cref="https://www.gatevidyalay.com/prims-algorithm-prim-algorithm-example/"/>
        /// </summary>
        /// <returns></returns>
        public UndirectedGraph<T> GetMinimumSpanningTreeWithPrimsAlgorithm()
        {
            var forest = new UndirectedGraph<T>();
            int outgoing = 0;

            int trees = VertexCount();
            // Randomly choose any vertex: The vertex connecting to the edge having least weight is usually selected.
            var v = GetMinimumWeightVertex();
            forest.AddVertex(value: v.Value, name: v.Name, guid: v.Guid);
            // current must always be a vertex of the original graph
            var current = v;

            while (forest.VertexCount() <= trees && outgoing != trees - 1)
            {
                // Find all the edges that connect the tree to new vertices. Find the
                // least weight edge among those edges and include it in the existing tree.
                Edge<T> least = null;
                double minWeight = double.MaxValue;
                var adjacents = current.AllNeighbors();
                var edges = current.IncomingEdges().ToList();

                var allEdges = edges.Where(e => adjacents.Contains(e.From)).ToList();

                edges = current.OutgoingEdges().ToList();
                edges = edges.Where(e => adjacents.Contains(e.From)).ToList();

                allEdges.AddRange(edges);

                allEdges = allEdges.OrderBy(e => e.Weight).ToList();
                int count = 0;
                for (var i = 0; i < allEdges.Count; i++)
                {
                    var e = allEdges[0];

                    minWeight = e.Weight;
                    least = e;
                    if (e.Type == Edge<T>.EdgeType.incoming)
                        current = GetVertex(name: e.From.Name);
                    else if (e.Type == Edge<T>.EdgeType.outgoing)
                        current = GetVertex(name: e.To.Name);

                    if (i == allEdges.Count)
                        throw new ApplicationException("Could not find a minimum weight edge that did not generate a cycle.");

                    // add current vertex so we can add edge
                    forest.AddVertex(value: current.Value, name: current.Name, guid: current.Guid);

                    // update  edge vertices to match new graph
                    var toName = least.To.Name;
                    var fromName = least.From.Name;

                    least.To = forest.GetVertex(name: toName);
                    least.From = forest.GetVertex(name: fromName);

                    forest.AddEdge(least);

                    if (forest.IsCyclic())
                    {
                        // If including that edge creates a cycle, then reject
                        // that edge and look for the next least weight edge.
                        forest.RemoveEdge(least);
                        forest.RemoveVertex(current.Name);
                        count++;
                    }
                    else
                        break;
                }

                outgoing = forest.GetUndirectedEdges().Count;
            }

            return forest;
        }

        public Vertex<T> GetMinimumWeightVertex(HashSet<Vertex<T>>  adjacents = null)
        {
            HashSet<Vertex<T>> vertices = Vertices;
            if (adjacents != null)
                vertices = adjacents;

            Vertex<T> minV = null;
            double min = double.MaxValue;

            foreach(var v in vertices)
            {
                var w = v.Edges.Min(e => e.Weight);

                if (w < min)
                {
                    min = w;
                    minV = v;
                }
            }
            return minV;
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
        private bool DetectCycleWithDepthFirstTraversal(Vertex<T> v, Vertex<T> parent)
        {
            // Mark the current node as visited
            v.IsVisited = true;

            List<Vertex<T>> adjacents = v.AllNeighbors();                

            // Recur for all the vertices adjacent to this vertex
            foreach (var u in adjacents)
            {
                if (u.Equals(parent))
                    continue;

                // If an adjacent is not visited, then recur for that adjacent
                if (!u.IsVisited)
                {
                    if (DetectCycleWithDepthFirstTraversal(u, v))
                        return true;
                }
                // If an adjacent is visited and ot parent of current vertex, then there is a cycle.
                else if (parent!=null && !u .Equals(parent))
                    return true;
            }

            return false;
        }

        #endregion
    }
}
