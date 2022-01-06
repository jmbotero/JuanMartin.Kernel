using System;

namespace JuanMartin.Kernel.Utilities.DataStructures
{
    public class Edge<T> : IComparable<Edge<T>>
    {
        // Edges can either be unidirectional or bidirectional. If they are unidirectional, the graph is called a directed graph. 
        // If they are bidirectional (meaning they go both ways), the graph is called a undirected graph. In the case where some 
        // edges are directed and others are not, the bidirectional edges should be swapped out for 2 directed edges that fulfill 
        // the same functionality. That graph is now fully directed.
        public enum EdgeType
        {
            incoming = 1,
            outgoing = 2, 
            both = 3,
            none = 0
        };

        public enum EdgeDirection
        {
            undirected = 0,
            unidirectional = 1,
            bidirectional = 2
        };

        public Vertex<T> From { get; set; }
        public Vertex<T> To { get; set; }
        public EdgeType Type { get; private set; }
        public double Weight { get; set; }
        public string Name { get; set; }
        public EdgeDirection Direction { get; }

         public Edge(Vertex<T> f, Vertex<T> t, string name, EdgeType type = EdgeType.none, EdgeDirection direction = EdgeDirection.undirected)
        {
            if (type == EdgeType.none)
                throw new ArgumentException("An edge must be defined with a particular  type: either incoming or outgoing.");

            if (type == EdgeType.both)
                throw new ArgumentException("An edge cannot be defined with type of both it must be either incoming or outgoing.");

            if (f is null || t is null)
                throw new ArgumentNullException("To Add edge must be created with a from (origin) and a to (target) vertices.");

            From = f;
            To = t;
            Type = type;
            Name = name;
            Direction = direction;
        }

        public Edge(Vertex<T> f, Vertex<T> t, double wgt, string name, EdgeType type = EdgeType.none, EdgeDirection direction = EdgeDirection.undirected) : this(f, t, name, type, direction)
        {
            if (wgt < 0)
                throw new ArgumentException("An edge must have a weight greater than or equal to zero.");

            Weight = wgt;
        }

        // Comparator function used for
        // sorting edgesbased on their weight
        public int CompareTo(Edge<T> That)
        {
            if (this.Weight < That.Weight) return -1;
            else if (this.Weight > That.Weight) return 1;
            else return 0;
        }
        public override string ToString()
        {
            return $"{Name}:{From.Name}-{To.Name}:{Type}:{Weight}";
        }
    }
}