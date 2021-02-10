using JuanMartin.Kernel.Extesions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace JuanMartin.Kernel.Utilities.DataStructures
{
    /// <summary>
    /// Implementation of a generic vertex to be used in any graph,
    /// copied from https://codereview.stackexchange.com/questions/131583/generic-graph-implementation-in-c
    /// </summary>

    public class Vertex<T> : ICloneable
    {
        public Vertex(T value, string name = null, string guid = null, IEnumerable<Neighbor<T>> neighbors = null)
        {
            if (guid == null)
                Guid = System.Guid.NewGuid().ToString();
            else
                Guid = guid;

            if (name == null)
                name = value.ToString();

            Name = name;
            Value = value;
            Neighbors = neighbors?.ToList() ?? new List<Neighbor<T>>();
            Edges = new List<Edge<T>>();
            IsVisited = false;
        }

        public string Guid { get; set; }
        public string Name { get; set; }
        public string Notes { get; set; }

        public T Value { get; }   // can be made writable

        public List<Neighbor<T>> Neighbors { get; }

        public List<Edge<T>> Edges { get; }

        public bool IsVisited { get; set; }
        public List<Vertex<T>> UnVisitedNeighbors(Neighbor<T>.NeighborType type=Neighbor<T>.NeighborType.any)
        {
            if (type==Neighbor<T>.NeighborType.any)
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

        public void AddEdge(Vertex<T> to, Edge<T>.EdgeType type, Edge<T>.EdgeDirection direction, string name, double weight = 0)
        {
            // assume edges  have a unique id on name
            if (Edges.Contains(new Edge<T>(this, to, name, type), new EdgeNameComparer<T>()))
            {
                var edge = Edges.FirstOrDefault(e => e.Name.Contains(name) && e.To.Equals(to) && e.Type == type);

                if (edge != null)
                {
                    edge.Weight++;
                }
            }
            else
            {
                // add outgoing edge
                if(Neighbors.Count == 0 || (Neighbors.Count > 0 && Neighbors.FirstOrDefault(n=>n.Node.Name == to.Name && n.Type==Neighbor<T>.NeighborType.outgoing) == null))
                    Neighbors.Add(new Neighbor<T> { Node = to, Type = Neighbor<T>.NeighborType.outgoing });
                Edges.Add(new Edge<T>(this, to, weight, name, Edge<T>.EdgeType.outgoing, direction));
                // if it is outgoing for the source vertex it is incoming for the target one
                if (to.Neighbors.Count == 0 || (to.Neighbors.Count > 0 && to.Neighbors.FirstOrDefault(n => n.Node.Name == this.Name && n.Type == Neighbor<T>.NeighborType.incoming) == null))
                    to.Neighbors.Add(new Neighbor<T> { Node =  this, Type = Neighbor<T>.NeighborType.incoming });
                to.Edges.Add(new Edge<T>(this, to, weight, name, Edge<T>.EdgeType.incoming, direction));
            }
        }

        public bool RemoveNeigbor(Vertex<T> neighbor, Neighbor<T>.NeighborType type = Neighbor<T>.NeighborType.both)
        {
            var result = false;

            switch (type)
            {
                case Neighbor<T>.NeighborType.incoming:
                    {
                        result = IncomingNeighbors().Remove(neighbor);
                        break;
                    }
                case Neighbor<T>.NeighborType.outgoing:
                    {
                        result = OutgoingNeighbors().Remove(neighbor);
                        break;
                    }
                case Neighbor<T>.NeighborType.both:
                    {
                        result = IncomingNeighbors().Remove(neighbor);
                        result &= OutgoingNeighbors().Remove(neighbor);
                        break;
                    }
            }
            return result;
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

        public override string ToString()
        {
            //return Neighbors.Aggregate(new StringBuilder($"{Name}: {Value} - ["), (sb, n) => sb.Append($"{n.Name}/{n.Value}")).ToString();
            return $"{Name}:{Value}:[ {(string.Join(", ", OutgoingNeighbors().Select(n => n.Value)))} ]";
        }

        public object Clone()
        {
            var vertex = this.MemberwiseClone();

            return vertex;
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
