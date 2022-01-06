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
        public const int INFINITY = int.MaxValue / 2;

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

        public HashSet<Vertex<T>> Vertices { get; }

        public bool Adjacent(Vertex<T> v1, Vertex<T> v2)
        {
            if (v1 is null || v2 is null)
                return false;

            return v1.OutgoingNeighbors().Contains(v2);
        }

        public List<Vertex<T>> OutgoingNeighbors(Vertex<T> v)
        {
            return v?.OutgoingNeighbors();
        }

        public List<Vertex<T>> IncomingNeighbors(Vertex<T> v)
        {
            return v?.IncomingNeighbors();
        }

        public int VertexCount()
        {
            return Vertices != null ? Vertices.Count : 0;
        }

        public int EdgeCount(Edge<T>.EdgeType type = Edge<T>.EdgeType.both)
        {
            var incomingCount = 0;
            var outgoingCount = 0;

            foreach (var vertex in Vertices)
            {
                incomingCount += vertex.IncomingEdges().Count;
                outgoingCount += vertex.OutgoingEdges().Count;
            }

            switch (type)
            {
                case Edge<T>.EdgeType.incoming:
                    return incomingCount;
                case Edge<T>.EdgeType.outgoing:
                    return outgoingCount;
                default:
                    return 0;
            }
        }

        private readonly Dictionary<string, string> VertexUris;
        public bool HasDuplicateVertexNames => Vertices.Count != Vertices.Select(v => v.Name).Distinct().Count();

        public Vertex<T> this[int index]
        {
            get => (Vertex<T>)Vertices.ElementAt(index);
        }

        public bool Contains(T value) => (Vertices.FirstOrDefault(v => v.Value.Equals(value)) != null);
        public bool Contains(Vertex<T> value) => Vertices.Contains(value);
        public Vertex<T> AddVertex(Vertex<T> v)
        {
            if (true) //Vertices.Count(i=>i.Name==v.Name) == 0) // ensure uniqueness
            {
                VertexUris.Add(v.Guid, v.Name);
                v.Index = Vertices.Count;
                Vertices.Add(v);
                return v;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="name">Ensure uniqueness by name</param>
        /// <returns></returns>
        public Vertex<T> AddVertex(T value, string name = null, string guid = null)
        {
            if (name == null)
                name = value.ToString();

            Vertex<T> added;
            try
            {
                added = new Vertex<T>(value, name, guid);
                AddVertex(added);
            }
            catch (Exception)
            {
                return null;
            }

            return added;
        }

        public bool AddEdge(string nameFrom, string nameTo, string name = null, Edge<T>.EdgeType type = Edge<T>.EdgeType.outgoing, Edge<T>.EdgeDirection direction = Edge<T>.EdgeDirection.unidirectional
    , double weight = 0)
        {
            return AddEdge(GetVertex(nameFrom), GetVertex(nameTo), name, type, direction, weight);
        }
        public bool AddEdge(Vertex<T> from, Vertex<T> to, string name = null, Edge<T>.EdgeType type = Edge<T>.EdgeType.outgoing, Edge<T>.EdgeDirection direction = Edge<T>.EdgeDirection.unidirectional
            , double weight = 0)
        {
            if (from.Equals(to))
                throw new ArgumentException("A cycle edge, from and to the same vertex cannot be added in a directed-acyclic graph.");

            if (Vertices.Contains(from) && Vertices.Contains(to))
            {
                try
                {
                    string n = name ?? string.Empty;
                    n = $"{from.Name}-{to.Name}:({n})";

                    from.AddEdge(to, type, direction, n, weight);

                    // add this as neigbor of vertex connected and if bidirectional duplicate edge in target
                    if (direction == Edge<T>.EdgeDirection.bidirectional)
                    {
                        n = name ?? string.Empty;
                        n = $"{to.Name}-{from.Name}:({n})";
                        //if (to.Edges.Where(e => e.Name == n).FirstOrDefault() == null)
                        to.AddEdge(from, Edge<T>.EdgeType.incoming, direction, n, weight);
                    }

                }
                catch (Exception)
                {
                    throw;
                    //return false;
                }
            }
            else
            {
                throw new ArgumentException("To Add edge between a and b, both vertices must be added before.");
            }

            return true;
        }

        public bool AddEdge(Edge<T> edge)
        {
            return this.AddEdge(edge.From, edge.To, edge.Name, edge.Type, edge.Direction, edge.Weight);
        }

        /// <summary>
        /// Add outgoing edges to graph based on Adjacency Matrix A V*V binary matrix is an adjacency matrix. 
        /// There is an edge that is connecting vertex i and vertex j, element Ai,j is the weight
        /// of the edge, otherwise Ai,j is 0.
        /// </summary>
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

        public Vertex<T> RemoveVertex(string name)
        {
            // do not delete vertex if multiple vertices have same name if so do not remove
            if (Vertices.Count(v => v.Name == name) > 1)
                return null;

            var vertex = GetVertex(guid: null, name: name);

            if (vertex is null)
                return null;

            if (!RemoveSingleVertex(vertex))
                return null;

            return vertex;
        }

        public Vertex<T> RemoveVertex(T value)
        {
            // do not delete vertex if multiple vertices have same value if so do not remove
            if (Vertices.Count(v => v.Value.Equals(value)) > 1)
                return null;

            var vertex = Vertices.FirstOrDefault(v => v.Value.Equals(value));

            if (vertex is null)
                return null;

            if (!RemoveSingleVertex(vertex))
                return null;

            return vertex;
        }

        public Vertex<T> RemoveVertexByGuid(string guid)
        {
            return RemoveVertexByGuid(Guid.Parse(guid));
        }

        public Vertex<T> RemoveVertexByGuid(Guid guid)
        {
            var v = GetVertex(guid);

            if (v == null)
                return null;

            if (!RemoveSingleVertex(v))
                return null;

            return v;
        }
        private bool RemoveSingleVertex(Vertex<T> vertex)
        {
            var i = Vertices.Remove(vertex);

            if (i == false)
                return false;

            foreach (var v in Vertices)
            {
                v.RemoveNeigbor(vertex);
                v.Edges.RemoveAll(e => e.From.Equals(vertex) || e.To.Equals(vertex));
            }

            return true;
        }

        public Edge<T> RemoveEdge(string name)
        {
            foreach (var v in Vertices)
            {
                var edge = v.Edges.FirstOrDefault(e => e.Name.Contains(name));

                if (edge != null)
                {
                    v.Edges.Remove(edge);
                    return edge;
                }
            }

            return null;
        }

        public List<Edge<T>> RemoveEdges(Vertex<T> from, Vertex<T> to, Edge<T>.EdgeType type = Edge<T>.EdgeType.none)
        {
            if (type == Edge<T>.EdgeType.none)
                throw new ArgumentException("To remove edges a type, either incoming, outgoing or both, must be speciFied.");

            if (from is null || to is null)
                return null;

            var edges = GetEdges(from, to, type);

            foreach (var e in edges)
                from.Edges.Remove(e);

            return edges;
        }

        /// <summary>
        /// If name is repeated use name of from vertex to disambiguate
        /// </summary>
        /// <param name="name"></param>
        /// <param name="fromName"></param>
        /// <returns></returns>
        public Edge<T> GetEdge(string name, string fromName = null, Edge<T>.EdgeType type = Edge<T>.EdgeType.none)
        {
            if (type == Edge<T>.EdgeType.none)
                throw new ArgumentException("To select an edge a type, either incoming or outgoing, must be speciFied.");

            foreach (var v in Vertices)
            {
                var edge = v.Edges.FirstOrDefault(e => e.Name.Contains(name) && (fromName == null || (fromName != null && e.From != null && e.From.Name == fromName)) && e.Type == type);
                if (edge != null)
                    return edge;
            }
            return null;
        }

        public List<Edge<T>> GetEdges(Vertex<T> from, Vertex<T> to, Edge<T>.EdgeType type = Edge<T>.EdgeType.none)
        {
            if (type == Edge<T>.EdgeType.none)
                throw new ArgumentException("To select edges a type, either incoming, outgoing or both, must be speciFied.");

            var edges = new List<Edge<T>>();

            if (from != null && to != null)
            {
                switch (type)
                {
                    case Edge<T>.EdgeType.incoming:
                        edges = from.Edges.Where(e => e.From.Equals(from) && e.To.Equals(to) && e.Type == type).ToList();
                        break;
                    case Edge<T>.EdgeType.outgoing:
                        edges = from.Edges.Where(e => e.From.Equals(from) && e.To.Equals(to) && e.Type == type).ToList();
                        break;
                    case Edge<T>.EdgeType.both:
                        edges = from.Edges.Where(e => e.From.Equals(from) && e.To.Equals(to)).ToList();
                        break;
                    case Edge<T>.EdgeType.none:
                        break;
                    default:
                        break;
                }
            }
            return edges;
        }

        /// <summary>
        /// Get all edges with same name, and  if name is null return all edges in graph
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public List<Edge<T>> GetEdgesByName(string name = null, Edge<T>.EdgeType type = Edge<T>.EdgeType.outgoing)
        {
            var edges = new List<Edge<T>>();

            foreach (var v in Vertices)
            {
                edges.AddRange((name == null) ? v.Edges : v.Edges.Where(e => e.Name.Contains(name) && e.Type == type));
            }

            return edges;
        }
        public List<Edge<T>> GetOutgoingEdges()
        {
            var edges = new List<Edge<T>>();

            foreach (var v in Vertices)
                edges.AddRange(v.OutgoingEdges());

            return edges;
        }
        public List<Edge<T>> GetIncomingEdges()
        {
            var edges = new List<Edge<T>>();

            foreach (var v in Vertices)
                edges.AddRange(v.IncomingEdges());

            return edges;
        }

        /// <summary>
        /// Allow for there may be duplicate vertices byy name 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public List<Vertex<T>> GetVertices(string name)
        {
            return Vertices.Where(v => v.Name == name).ToList();
        }

        public Vertex<T> GetVertex(string name, string guid = null)
        {
            if (name == null)
            {
                if (guid == null)
                    return null;
                else
                    return Vertices.FirstOrDefault(v => v.Guid == guid);
            }
            else
            {
                if (guid == null)
                    return Vertices.FirstOrDefault(v => v.Name == name);
                else
                    return Vertices.FirstOrDefault(v => v.Name == name && v.Guid == guid);
            }
        }

        public Vertex<T> GetVertex(Guid guid)
        {
            return Vertices.FirstOrDefault(v => v.Guid == guid.ToString());
        }

        public Vertex<T> GetVertex(int index)
        {
            return Vertices.FirstOrDefault(v => v.Index == index);
        }

        public List<Vertex<T>> GetRoot()
        {
            var vertices = new List<Vertex<T>>();

            foreach (var v in Vertices)
            {
                int incoming = v.IncomingEdges().Count;
                int outgoing = v.OutgoingEdges().Count;
                if (incoming == 0 && outgoing > 0)
                    vertices.Add(v);
            }

            return vertices;
        }

        public List<Vertex<T>> VisitedVertices() => Vertices.Where(v => v.IsVisited == true).ToList();
        public List<Vertex<T>> UnVisitedVertices() => Vertices.Where(v => v.IsVisited == false).ToList();

        public string ToString(bool includeEdges = false)
        {
            UnvisitAllVertices();
            var roots = GetRoot();

            if (roots.Count > 0)
            {
                var vertex = roots[0];

                return ToString(vertex, includeEdges);
            }
            else
                return string.Empty;
        }
        public List<Path<T>> GetPaths(Vertex<T> start = null, Vertex<T> end = null)
        {
            var paths = new List<Path<T>>();
            var path = new Path<T>();

            if (start != null && end != null && start.Guid == end.Guid) // since there are no cycles in this graph return zero paths
                return paths;

            if (start == null)
                start = GetRoot()[0];

            UnvisitAllVertices();
            GetPaths(paths, path, start, end);

            return paths;
        }

        private void GetPaths(List<Path<T>> paths, Path<T> path, Vertex<T> start, Vertex<T> end)
        {
            // avoid loops
            if (!path.ContainsByGuid(start.Guid))
                path.AddVertex(start);

            var edges = start.OutgoingEdges();

            foreach (var (e, index) in edges.Enumerate()) // scan all outgoing edges
            {
                Vertex<T> v = e.To;

                v.Notes = e.Name; // save the edge id/name used to travel to this vertex
                var outgoingEdgesCount = v.OutgoingEdges().Count;

                // build all possible paths from this point                                                                                                                                                                             )
                DefinePaths(paths, path);
                // get currrent path to add nodes to
                path = SelectPath(paths, path, start);

                // avoid loops
                if (!path.ContainsByGuid(v.Guid))
                {
                    GetPaths(paths, path, v, end);
                }
                else
                    continue;

                if ((end != null && v.Guid == end.Guid) || (end == null && outgoingEdgesCount == 0))
                {
                    continue;
                }
                // when all edges have been processed mark all neighboring vertices as visited
                //if (index == edges.Count - 1)
                //{
                //    UnvisitAllVertices(v.OutgoingNeighbors());
                //}
            }
        }


        public Path<T> GetCriticalPath(CriticalPathType type, Vertex<T> start = null, Vertex<T> end = null)
        {
            var paths = GetPaths(start, end);

            return GetCriticalPath(type, paths);
        }

        public Path<T> GetCriticalPath(CriticalPathType type, List<Path<T>> paths)
        {
            Path<T> criticalPath = null;
            var limit = 0;
            switch (type)
            {
                case CriticalPathType.longest:
                    {
                        limit = int.MinValue;
                        break;
                    }
                case CriticalPathType.shortest:
                    {
                        limit = int.MaxValue;
                        break;
                    }

                default:
                    break;
            }

            foreach (var p in paths)
            {
                p.RefreshWeight();

                if ((type == CriticalPathType.longest && p.Weight > limit) ||
                   (type == CriticalPathType.shortest && p.Weight < limit))
                {
                    limit = p.Weight;
                    criticalPath = p;
                }
            }

            return criticalPath;
        }

        public Path<T> GetLongestPath(Vertex<T> start = null, Vertex<T> end = null)
        {
            var paths = GetPaths(start, end);

            return GetCriticalPath(CriticalPathType.longest, paths);
        }
        public Path<T> GetShortestPath(Vertex<T> start = null, Vertex<T> end = null)
        {
            var paths = GetPaths(start, end);

            return GetCriticalPath(CriticalPathType.shortest, paths);
        }

        /// <summary>
        /// Get next unvisited graph node with minimum distance in dijkstra's shortest path log
        /// </summary>
        /// <returns></returns>
        private Vertex<T> GetNextVertex(Vertex<T> a, Dictionary<string, DijkstraNode> distance, bool isNumericNode)
        {
            Vertex<T> v = null;

            // if vertex is numeric then check minimum weight on neighbors, else check weight in edges
            if (isNumericNode)
            {
                var neighbors = a.UnVisitedNeighbors(Neighbor<T>.NeighborType.outgoing).OrderByDescending(n => n.Value).ToList();

                foreach (var n in neighbors)
                {
                    int d = Convert.ToInt32(n.Value);

                    if (distance[a.Guid].Distance + d < distance[n.Guid].Distance)
                    {
                        v = n;
                        distance[v.Guid].Distance = distance[a.Guid].Distance + d;
                        distance[v.Guid].Previous = a.Guid;
                    }
                }

            }
            else
            {
                var edges = a.OutgoingEdges().OrderByDescending(e => e.Weight);

                foreach (var e in edges)
                {
                    if (!e.To.IsVisited)
                    {
                        int d = (int)e.Weight;

                        if (distance[a.Guid].Distance + d < distance[e.To.Guid].Distance)
                        {
                            v = e.To;
                            distance[v.Guid].Distance = distance[a.Guid].Distance + d;
                            distance[v.Guid].Previous = a.Guid;
                        }
                    }
                }
            }
            a.IsVisited = true;

            return v;
        }

        public List<string> DuplicateVertices()
        {
            var dups = new List<string>();

            foreach (var v in Vertices)
            {
                if (!dups.Contains(v.Name) && Vertices.Count(n => n.Name == v.Name) > 1)
                    dups.Add(v.Name);
            }
            return dups;
        }

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
                    var edge = GetEdge(n, type: Edge<T>.EdgeType.outgoing);
                    double w = 0;


                    if (edge != null)
                        w = edge.Weight;

                    matrix[i][j] = w;
                }
            }
            return matrix;
        }

        /// <summary>
        /// c<see cref="https://en.wikipedia.org/wiki/Minimum_spanning_tree"/>
        /// Implemented through Kruskal's algotithm 
        /// </summary>
        /// <returns></returns>
        public DirectedAcyclicGraph<T> GetMinimumSpanningTree()
        {
            var forest = new DirectedAcyclicGraph<T>();

            foreach(var v in Vertices)
                forest.AddVertex(new Vertex<T>(value: v.Value, name: v.Name));

            int trees = forest.VertexCount();
            var edges = GetOutgoingEdges();

            edges.Sort(); // default sort by weight ascending
            var count = 0;
            while (count < edges.Count || forest.GetOutgoingEdges().Count<trees-1)
            {
                var e = edges[count];
                // replaces edge's to/from with corressssponding trees in new forest
                e.To = forest.GetVertex(e.To.Name);
                e.From = forest.GetVertex(e.From.Name);

                forest.AddEdge(e);
                count++;
                
                
                var adjacency = forest.GetAdjacencyMatrix();
                if(forest.DetectCycle(adjacency,count))
                {
                    forest.RemoveEdge(e.Name);
                    count--;
                }
            }

            return forest;
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
        public bool DetectCycle(double[][] adjacency, int edges)
        {
            int nodes = adjacency.Length;

            /// translate adjacency matrix into adjacency list <seealso cref="https://www.section.io/engineering-education/graphs-in-data-structure-using-cplusplus/"/>,
            /// referencing vertices by their Index

            // Created the jagged array. It contains the vertexes and how they are to be connected.
            // E.g. new int[]{ 1,2}, means 1 is to be connected to 2
            int[][] graph;
            var g = new List<int[]>();

            for(int i=0;i<adjacency.Length;i++)
            {
                for(int j=0;j<adjacency[i].Length;j++)
                {
                    if (adjacency[i][j] != 0)
                    {
                        var adjacent = new List<int> { i, j };
                        g.Add(adjacent.ToArray());
                    }
                }
            }
            graph = g.ToArray();
            // This is the dictionary for storing the adjacency list.
            //It is of the type, int that will hold a node and List<int> that will hold all other nodes 
            //attached to the int node.
            /* E.G. This is how our graph will look like 
                               1-> 2,3,4,5
                               2-> 6,7
                               3-> 4
                               4-> 1
                           */
            Dictionary<int, List<int>> ls = new Dictionary<int, List<int>>();
            // We declare a visited bool array variable. We will store the visited nodes in it.
            bool[] visited = new bool[nodes];
            // We declare a path bool array variable. We will store all nodes in our current path here.
            bool[] path = new bool[nodes];
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
            // We start our traversal here. We could also say that this is where we start our path from.
            for (int i = 0; i < nodes; i++)
            {
                // We do our Dfs starting from the node at i in this case our start point will be 0;
                // For each Dfs, we are checking if we will find a cycle. If yes, we immediately return true.
                // A cycle has been found.
                if (Dfs(ls, i, visited, path))

                    return true;

            }
            // If in our for loop above, we never found a cycle, then we will return false.
            // A cycle was not detected.
            return false;

        }

        /// <summary>
        /// Function that detects whether there is a cycle or not. Below is the code for the same. It uses recursion for backtracking.
        /// If a cycle is detected, we return true, otherwise, we return false.
        /// </summary>
        /// <param name="graph">Jagged array representation of adjacency matrix</param>
        /// <param name="start">Index in graph of fitrst node in path</param>
        /// <param name="visited">The visited status of nodes</param>
        /// <param name="path">True or false if node represented by indeex in current path/param>
        /// <returns></returns>
        private static bool Dfs(Dictionary<int, List<int>> graph, int start, bool[] visited, bool[] path)
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
                foreach(var item in graph[start])
               
               {
                    //We do our recursion
                    // At this point, if the start node returned a true in our recursive call, then we say that cycle has been
                    // found. We return true immediately.
                    if (Dfs(graph, item, visited, path))
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




        /// <summary>
        /// Dijkstra algorithm implementation that calculates the shortest d[=istance and path from 'start' node
        /// to all other nodes in graph, but this method returns only values to 'end'.
        /// </suvmmary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public (int Distance, Path<T> ShortestPath) GetDijkstraSingleShortestPath(string start, string end)
        {
            Dictionary<string, DijkstraNode> dist = new Dictionary<string, DijkstraNode>();
            Path<T> shortestPath = new Path<T>();

            // initialize distances with all vertices
            foreach (var vertex in Vertices)
                dist.Add(vertex.Guid, new DijkstraNode { Distance = INFINITY, Previous = string.Empty });

            Vertex<T> a, b;
            try
            {
                a = GetVertices(name: start).First();
                b = GetVertices(name: end).Last();
            }
            catch (Exception)
            {
                return (INFINITY, shortestPath);
            }
            bool IsNumericNode = UtilityType.IsNumericType(a.Value.GetType());
            Vertex<T> startNode = a, endNode = b;
            // the distance for starting node is the first nodes value if node's weight is in its value, if not, since
            // no vertices come into it, is zero
            dist[a.Guid].Distance = IsNumericNode ? Convert.ToInt32(a.Value) : 0;
            
            UnvisitAllVertices();
            while (true)
            {
               if (a.Guid == b.Guid)
                    break;

                a = GetNextVertex(a, dist, IsNumericNode);

                if (a == null)
                    break;
            }

            if (dist[b.Guid].Previous != string.Empty)
            {
                // get shortest path between start vertex and end vertex from Dijkstra'sinformation log
                shortestPath.AddVertex(endNode);

                Vertex<T> currentNode;
                var currentId = endNode.Guid;
                while (currentId != startNode.Guid && currentId != string.Empty)
                {
                    if (dist.ContainsKey(currentId))
                    {
                        currentId = dist[currentId].Previous;
                        currentNode = GetVertex(name: null, guid: currentId.ToString());

                        shortestPath.AddVertex(currentNode);
                    }
                }
                shortestPath.AddVertex(startNode);

                shortestPath.Reverse();
            } 

            return (dist[b.Guid].Distance, shortestPath);
        }

        private int[][] _distance;
        /// <summary>
        /// Implementation of BellmanFord algrithm  to find shortest path (distance) between source vertex  and 
        /// other nodes in graph.
        /// </summary>
        /// <returns></returns>
        public int[][] GetBellmanFordSingleShortestPath(string start, Vertex<T>[][] matrix)
        {
            //TODO: implement based on weights
            int h = matrix.Length;
            int w = matrix[0].Length;
                _distance = new int[h][];
            for (int i = 0; i < h; i++)
            {
                _distance[i] = new int[w];
                _distance[i] = Enumerable.Repeat(INFINITY, w).ToArray();
            }

            Vertex<T> a;
            try
            {
                a = GetVertices(name: start).First();
            }
            catch (Exception)
            {
                return _distance;
            }

            bool IsNumericNode = UtilityType.IsNumericType(a.Value.GetType());

            // BellmanFord algorithm: https://www.youtube.com/watch?v=hxMWBBCpR6A
             _distance[0][0] = IsNumericNode ? Convert.ToInt32(a.Value) : 0;
            for (int i = 0; i < w * h; i++)
            {
                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        var d = IsNumericNode ? Convert.ToInt32(matrix[x][y].Value.ToString()) : 0;

                        int temp = INFINITY;
                        temp = Math.Min(GetDistance(x - 1, y), temp);
                        temp = Math.Min(GetDistance(x + 1, y), temp);
                        temp = Math.Min(GetDistance(x, y - 1), temp);
                        temp = Math.Min(GetDistance(x, y + 1), temp);
                        _distance[x][y] = Math.Min(d + temp, _distance[x][y]);
                    }
                }
            }
            return _distance;
        }
        private int GetDistance(int x, int y)
        {
            if (x < 0 || x >= _distance.Length || y < 0 || y >= _distance[x].Length)
            {
                return INFINITY;
            }
            else
            {
                return _distance[x][y];
            }
        }

        private string ToString(Vertex<T> start, bool includeEdges = false, bool IsLast=false)
        {
            var sBuilder = new StringBuilder();

            if (start == null)
            {
                sBuilder.Append(string.Empty);
            }
            else
            {
                if (!start.IsVisited)
                {
                    start.IsVisited = true;
                    sBuilder.Append(start.Name);
                    if (includeEdges)
                        sBuilder.Append($":[ {(string.Join(", ", start.Edges.Select(e => e.ToString())))} ]");
                    if (!IsLast) sBuilder.Append(",");
                    var vertices = start.OutgoingNeighbors();
                    foreach (var v in vertices)
                    { 
                        sBuilder.Append(ToString(v, includeEdges,v==vertices.Last()));
                    }
                }
                else
                    sBuilder.Append(string.Empty);
            }
            return sBuilder.ToString();
        }

        /// <summary>
        /// Define paths replicating path specified, a path splits when it gets
        /// to a vertex that has multiple outgoing edges
        /// </summary>
        /// <param name="paths"></param>
        /// <param name="currentPath"></param>
          /// <returns></returns>
        private void DefinePaths(List<Path<T>> paths, Path<T> currentPath)
        {
            Vertex<T> splitPoint = currentPath.Vertices.Last();

            var edges = splitPoint.OutgoingEdges();
            var pathCount = edges.Count;
            var splitThePath = pathCount > 1;

            if (!splitThePath && currentPath.IsComplete() && currentPath.IsSingle())
            {
                paths.Add(currentPath);
            }
            else if (splitThePath)
            {
                if (paths.Count == 0)
                    paths.Add(currentPath);

                // one path, ending in splitPoint, must splitted, copied over, for the 
                // number of its outgoing edges
                for (var i = 0; i < paths.Count; i++)
                {
                    var p = paths[i];

                    if (p.IsLast(splitPoint.Name))
                    {
                        paths.Remove(p);
                        for (var j = 0; j < pathCount; j++)
                        {
                            var newPath = new Path<T>();
                            newPath.Append(p);
                            paths.Add(newPath);
                        }
                        currentPath = paths.Last();
                        break;
                    }
                }
            }
        }

        private Path<T> SelectPath(List<Path<T>> paths, Path<T> currentPath, Vertex<T> splitPoint)
        {
            var edges = splitPoint.OutgoingEdges();
            var pathCount = edges.Count;
            var splitThePath = pathCount > 1;

            if (splitThePath)
            {
                foreach (var p in paths)
                {
                    if (p.IsLast(splitPoint.Name))
                    {
                        currentPath = p;
                        break;
                    }
                }
            }

            return currentPath;
        }

        private void UnvisitAllVertices()
        {
            UnvisitAllVertices(Vertices);
        }

        private void UnvisitAllVertices(IEnumerable<Vertex<T>> vertices)
        {
            if (vertices.Count(n => n.IsVisited) > 1)
            {
                foreach (var v in vertices)
                    v.IsVisited = false;
            }
        }
    }

}
