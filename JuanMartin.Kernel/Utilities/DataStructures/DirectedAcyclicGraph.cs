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
            var incoming_count = 0;
            var outgoing_count = 0;

            foreach (var vertex in Vertices)
            {
                incoming_count += vertex.IncomingEdges().Count;
                outgoing_count += vertex.OutgoingEdges().Count;
            }

            switch (type)
            {
                case Edge<T>.EdgeType.incoming:
                    return incoming_count;
                case Edge<T>.EdgeType.outgoing:
                    return outgoing_count;
                default:
                    return 0;
            }
        }

        private Dictionary<string, string> VertexUris;
        public bool HasDuplicateVertexNames => Vertices.Count != Vertices.Select(v => v.Name).Distinct().Count();

        public Vertex<T> this[int index]
        {
            get => (Vertex<T>)Vertices.ElementAt(index);
        }

        public bool Contains(T value) => (Vertices.FirstOrDefault(v=>v.Value.Equals(value))!=null);
        public bool Contains(Vertex<T> value) => Vertices.Contains(value);
        public Vertex<T> AddVertex(Vertex<T> v)
        {
            if (true) //Vertices.Count(i=>i.Name==v.Name) == 0) // ensure uniqueness
            {
                //v.Index = VertexCount();
                Vertices.Add(v);
                VertexUris.Add(v.Guid, v.Name);
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
        /// <param name="from_name"></param>
        /// <returns></returns>
        public Edge<T> GetEdge(string name, string from_name = null, Edge<T>.EdgeType type = Edge<T>.EdgeType.none)
        {
            if (type == Edge<T>.EdgeType.none)
                throw new ArgumentException("To select an edge a type, either incoming or outgoing, must be speciFied.");

            foreach (var v in Vertices)
            {
                var edge = v.Edges.FirstOrDefault(e => e.Name.Contains(name) && (from_name == null || (from_name != null && e.From != null && e.From.Name == from_name)) && e.Type == type);
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
                var outgoing_edges_count = v.OutgoingEdges().Count;

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

                if ((end != null && v.Guid == end.Guid) || (end == null && outgoing_edges_count == 0))
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
            Path<T> critical_path = null;
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
                    critical_path = p;
                }
            }

            return critical_path;
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
            Path<T> shortest_path = new Path<T>();

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
                return (INFINITY, shortest_path);
            }
            bool IsNumericNode = UtilityType.IsNumericType(a.Value.GetType());
            Vertex<T> start_v = a, end_v = b;
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
                shortest_path.AddVertex(end_v);

                Vertex<T> current_node;
                var current_id = end_v.Guid;
                while (current_id != start_v.Guid && current_id != string.Empty)
                {
                    if (dist.ContainsKey(current_id))
                    {
                        current_id = dist[current_id].Previous;
                        current_node = GetVertex(name: null, guid: current_id.ToString());

                        shortest_path.AddVertex(current_node);
                    }
                }
                shortest_path.AddVertex(start_v);

                shortest_path.Reverse();
            } 

            return (dist[b.Guid].Distance, shortest_path);
        }

        private int[][] _distance;
        /// <summary>
        /// Implementation of Bellman_Ford algrithm  to find shortest path (distance) between source vertex  and 
        /// other nodes in graph.
        /// </summary>
        /// <returns></returns>
        public int[][] GetBellman_FordSingleShortestPath(string start, Vertex<T>[][] matrix)
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

            // Bellman–Ford algorithm: https://www.youtube.com/watch?v=hxMWBBCpR6A
             _distance[0][0] = IsNumericNode ? Convert.ToInt32(a.Value) : 0;
            for (int i = 0; i < w * h; i++)
            {
                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        var d = IsNumericNode ? Convert.ToInt32(matrix[x][y].Value.ToString()) : 0;

                        int temp = INFINITY;
                        temp = Math.Min(getDistance(x - 1, y), temp);
                        temp = Math.Min(getDistance(x + 1, y), temp);
                        temp = Math.Min(getDistance(x, y - 1), temp);
                        temp = Math.Min(getDistance(x, y + 1), temp);
                        _distance[x][y] = Math.Min(d + temp, _distance[x][y]);
                    }
                }
            }
            return _distance;
        }
        private int getDistance(int x, int y)
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
            var sgraph = new StringBuilder();

            if (start == null)
            {
                sgraph.Append(string.Empty);
            }
            else
            {
                if (!start.IsVisited)
                {
                    start.IsVisited = true;
                    sgraph.Append(start.Name);
                    if (includeEdges)
                        sgraph.Append($":[ {(string.Join(", ", start.Edges.Select(e => e.ToString())))} ]");
                    if (!IsLast) sgraph.Append(",");
                    var vertices = start.OutgoingNeighbors();
                    foreach (var v in vertices)
                    { 
                        sgraph.Append(ToString(v, includeEdges,v==vertices.Last()));
                    }
                }
                else
                    sgraph.Append(string.Empty);
            }
            return sgraph.ToString();
        }

        /// <summary>
        /// Define paths replicating path specified, a path splits when it gets
        /// to a vertex that has multiple outgoing edges
        /// </summary>
        /// <param name="paths"></param>
        /// <param name="current_path"></param>
          /// <returns></returns>
        private void DefinePaths(List<Path<T>> paths, Path<T> current_path)
        {
            Vertex<T> split_point = current_path.Vertices.Last();

            var edges = split_point.OutgoingEdges();
            var path_count = edges.Count;
            var split_paths = path_count > 1;

            if (!split_paths && current_path.IsComplete() && current_path.IsSingle())
            {
                paths.Add(current_path);
            }
            else if (split_paths)
            {
                if (paths.Count == 0)
                    paths.Add(current_path);

                // one path, ending in split_point, must splitted, copied over, for the 
                // number of its outgoing edges
                for (var i = 0; i < paths.Count; i++)
                {
                    var p = paths[i];

                    if (p.IsLast(split_point.Name))
                    {
                        paths.Remove(p);
                        for (var j = 0; j < path_count; j++)
                        {
                            var new_path = new Path<T>();
                            new_path.Append(p);
                            paths.Add(new_path);
                        }
                        current_path = paths.Last();
                        break;
                    }
                }
            }
        }

        private Path<T> SelectPath(List<Path<T>> paths, Path<T> current_path, Vertex<T> split_point)
        {
            var edges = split_point.OutgoingEdges();
            var path_count = edges.Count;
            var split_paths = path_count > 1;

            if (split_paths)
            {
                foreach (var p in paths)
                {
                    if (p.IsLast(split_point.Name))
                    {
                        current_path = p;
                        break;
                    }
                }
            }

            return current_path;
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
