using JuanMartin.Kernel.Extesions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace JuanMartin.Kernel.Utilities.DataStructures
{
    /// <summary>
    /// Implementation of a generic vertex to be used in any graph,
    /// copied from https://codereview.stackexchange.com/questions/131583/generic-graph-implementation-in-c
    /// </summary>

    public class Vertex<T> : ICloneable
    {
        public Vertex(T value, string name = null, string guid = null, IEnumerable<Neighbor<T>> neighbors = null, int index = -1)
        {
            if (guid == null)
                Guid = System.Guid.NewGuid().ToString();
            else
                Guid = guid;

            if (name == null)
                name = value.ToString();

            if (index == -1)
            {
                Type methodType = typeof(T);

                if (methodType == typeof(int))
                    Index =Convert.ToInt32(value);
            }
            else
                Index = index;

            Name = name;
            Value = value;
            Neighbors = neighbors?.ToList() ?? new List<Neighbor<T>>();
            Edges = new List<Edge<T>>();
            IsVisited = false;
        }

        public string Guid { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// Zero-based adjacency matrix column/line index
        /// </summary>
        public  int Index { get; set; }
        public string Notes { get; set; }

        public T Value { get; }   // can be made writable

        public List<Neighbor<T>> Neighbors { get; }

        public List<Edge<T>> Edges { get; }

        /// <summary>
        /// Use an edge default vaLues to search by different settings
        /// If name is repeated use name of from vertex to disambiguate
        /// </summary>
        /// <param name="name"></param>
        /// <param name="fromName"></param>
        /// <param name="toName"></param>
        /// <param name="type"></param>
        /// <param name="direction"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public Edge<T> GetEdge(string name = Edge<T>.EdgeNameDefault, string fromName = Edge<T>.EdgeVertexFromNameDefault, string toName = Edge<T>.EdgeVertexToNameDefault,
                                                Edge<T>.EdgeType type = Edge<T>.EdgeTypeDefault, Edge<T>.EdgeDirection direction = Edge<T>.EdgeDirectionDefault,
                                                double weight = Edge<T>.EdgeWeightDefault)
        {
            if (name == Edge<T>.EdgeNameDefault && 
                fromName == Edge<T>.EdgeVertexFromNameDefault && 
                toName == Edge<T>.EdgeVertexToNameDefault &&
                type == Edge<T>.EdgeTypeDefault && 
                direction == Edge<T>.EdgeDirectionDefault &&
                weight == Edge<T>.EdgeWeightDefault)
                throw new ArgumentNullException("At least one earch attribute must be defined.");

            var edge = from e in Edges
                        where (name != Edge<T>.EdgeNameDefault ?e.Name.Contains(name) :true) &&
                                    (fromName != Edge<T>.EdgeVertexFromNameDefault ? e.From.Name == fromName : true)  &&
                                    (toName != Edge<T>.EdgeVertexToNameDefault ? e.To.Name == toName : true) &&
                                    (direction != Edge<T>.EdgeDirectionDefault ? e.Direction == direction : true) &&
                                    (type != Edge<T>.EdgeTypeDefault ? e.Type == type : true) &&
                                    (weight != Edge<T>.EdgeWeightDefault ? e.Weight == weight : true)
                        select e;

            return edge.FirstOrDefault();
        }


        public bool IsVisited { get; set; }
        public List<Vertex<T>> UnVisitedNeighbors(Neighbor<T>.NeighborType type=Neighbor<T>.NeighborType.both)
        {
            if (type == Neighbor<T>.NeighborType.none)
                return null;

            if (type == Neighbor<T>.NeighborType.both)
                return Neighbors.Where(n => n.Node.IsVisited == false).Select(n => n.Node).ToList();
            else  
                return Neighbors.Where(n => n.Node.IsVisited == false && n.Type==type).Select(n => n.Node).ToList();
        }


        //public int Index { get; set; }

        public List<Vertex<T>> IncomingNeighbors()
        {
            return Neighbors.Where(n => n.Type == Neighbor<T>.NeighborType.incoming).Select(n => n.Node).ToList();
        }

        public List<Vertex<T>> OutgoingNeighbors()
        {
            return Neighbors.Where(n => n.Type == Neighbor<T>.NeighborType.outgoing).Select(n => n.Node).ToList();
        }

        public List<Vertex<T>> AllNeighbors()
        {
            return Neighbors.Select(n => n.Node).Distinct().ToList();
        }

        public List<Edge<T>> OutgoingEdges()
        {
            //return v.Edges.Where(e => e.From.Equals(v)).ToList();
            return Edges.Where(e => e.Type == Edge<T>.EdgeType.outgoing).ToList();
        }
        public List<Edge<T>> IncomingEdges()
        {
            //var edges = new List<Edge<T>>();

            //foreach (var neighbor in v.IncomingNeighbors())
            //{
            //    edges.AddRange(neighbor.Edges.Where(e => e.From.Equals(neighbor) && e.To.Equals(v)));
            //}

            //return edges;
            return Edges.Where(e => e.Type == Edge<T>.EdgeType.incoming).ToList();
        }

        public void AddNeighbor(Vertex<T> v, Neighbor<T>.NeighborType neighborType)
        {
            bool exists = Neighbors.FirstOrDefault(n => n.Node.Name == v.Name && n.Type == neighborType) != null;

            if(!exists)
                Neighbors.Add(new Neighbor<T> { Node = v, Type = neighborType });
        }

        public void AddEdge(Vertex<T> from, Vertex<T> to, Edge<T>.EdgeType type, Edge<T>.EdgeDirection direction, string name, double weight = 0)
        {
            // assume edges  have a unique id on name
            if (Edges.Contains(new Edge<T>(from, to, name, type), new EdgeNameComparer<T>()))
            {
                var edge = Edges.FirstOrDefault(e => e.Name.Contains(name) && e.To.Equals(to) && e.Type == type);

                if (edge != null)
                {
                    edge.Weight++;
                }
            }
            else
            {
                Edges.Add(new Edge<T>(from, to, weight, name, type, direction));
            }
        }

        public bool RemoveNeigbor(Vertex<T> node, Neighbor<T>.NeighborType type = Neighbor<T>.NeighborType.both)
        {
            if (type == Neighbor<T>.NeighborType.none)
                throw new ArgumentException("Inorder to remove a neighbor it has to to have a type.");

            if (type != Neighbor<T>.NeighborType.both)
            {
                Neighbor<T> aux = Neighbors.FirstOrDefault(n => n.Node.Name == node.Name && n.Type == type);
                return Neighbors.Remove(aux);
            }
            else
                return RemoveNeigbor(node, Neighbor<T>.NeighborType.incoming) && RemoveNeigbor(node, Neighbor<T>.NeighborType.outgoing);
        }

        public void RemoveConnection(Vertex<T> neighbor)
        {
            RemoveNeigbor(neighbor, Neighbor<T>.NeighborType.outgoing);

            var edges = Edges.Where(e => e.To == neighbor).ToList();
            foreach (var e in edges)
            {
                Edges.Remove(e);
            }
        }

        public bool RemoveEdge(Edge<T> edge)
        {
            var aux = Edges.FirstOrDefault(e => e.Name == edge.Name &&
                                                                    e.From.Name == edge.From.Name &&
                                                                    e.To.Name  == edge.To.Name &&
                                                                    e.Direction ==  edge.Direction &&
                                                                    e.Type == edge.Type &&
                                                                    e.Weight == edge.Weight);
                
            return Edges.Remove(aux);
        }

        public override string ToString()
        {
            //return Neighbors.Aggregate(new StringBuilder($"{Name}: {Value} - ["), (sb, n) => sb.Append($"{n.Name}/{n.Value}")).ToString();
            return $"{Name}:{Value}:[ {(string.Join(", ", OutgoingNeighbors().Select(n => n.Name)))} ]";
        }

        public object Clone()
        {
            var vertex = this.MemberwiseClone();

            return vertex;
        }

        public bool Equals(Vertex<T> v)
        {
            Vertex<T> u = this;

            if (v != null && u != null &&
                string.Equals(u.Name, v.Name, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(u.Guid, v.Guid, StringComparison.OrdinalIgnoreCase) &&
                u.Value.Equals(v.Value))
            {
                return true;
            }
            return false;
        }
    }

    internal class EdgeNameComparer<T> : IEqualityComparer<Edge<T>>
    {
        /// <summary>
        /// Two edges are equal  if it's name and it's to and from vertices are the same (use name as unique identifier)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Equals(Edge<T> x, Edge<T> y)
        {
            if (string.Equals(x.Name, y.Name, StringComparison.OrdinalIgnoreCase) && string.Equals(x.From.Guid, y.From.Guid, StringComparison.OrdinalIgnoreCase) && string.Equals(x.To.Guid, y.To.Guid, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            return false;
        }

        public int GetHashCode(Edge<T> obj)
        {
            return obj.Name.GetHashCode();
        }
    }

    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public class DijkstraNode
    {
        public int Distance { get; set; }
        public string Previous { get; set; }

        public override string ToString()
        {
            return $"{Distance}:{Previous}";
        }

        private string GetDebuggerDisplay()
        {
            return ToString();
        }
    }
}
