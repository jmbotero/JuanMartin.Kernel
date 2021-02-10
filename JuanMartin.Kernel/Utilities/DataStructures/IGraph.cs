using System;
using System.Collections.Generic;

namespace JuanMartin.Kernel.Utilities.DataStructures
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="https://en.wikipedia.org/wiki/Graph_(abstract_data_type)"/>
    public interface IGraph<T>
    {
        bool Adjacent(Vertex<T> v1, Vertex<T> v2);
        List<Vertex<T>> IncomingNeighbors(Vertex<T> v);
        List<Vertex<T>> OutgoingNeighbors(Vertex<T> v);
        int VertexCount();
        int EdgeCount(Edge<T>.EdgeType type);
        bool AddVertex(Vertex<T> v);
        bool AddVertex(T value, string name, string guid);
        bool AddEdge(Vertex<T> from, Vertex<T> to, Edge<T>.EdgeType type, Edge<T>.EdgeDirection direction, string name, double weight);
        Vertex<T> RemoveVertex(string name);
        Vertex<T> RemoveVertex(T value);
        Edge<T> RemoveEdge(string name);
        List<Edge<T>> RemoveEdges(Vertex<T> from, Vertex<T> to, Edge<T>.EdgeType type);
        Edge<T> GetEdge(string name, string from_name, Edge<T>.EdgeType type);
        List<Edge<T>> GetEdges(Vertex<T> from, Vertex<T> to, Edge<T>.EdgeType type);
        List<Edge<T>> GetOutgoingEdges();
        List<Edge<T>> GetIncomingEdges();
        Vertex<T> GetVertex(string name, string guid);
        Vertex<T> GetVertex(Guid guid);
        List<Vertex<T>> GetVertices(string name);
        List<Vertex<T>> GetRoot();
        List<Vertex<T>> VisitedVertices();
        List<Vertex<T>> UnVisitedVertices();
        List<Path<T>> GetPaths(Vertex<T> start, Vertex<T> end);
        Path<T> GetLongestPath(Vertex<T> start, Vertex<T> end);
        Path<T> GetShortestPath(Vertex<T> start, Vertex<T> end);
        string ToString(bool includeEdges);
    }

    public abstract class Graph<T>
    {
        public enum CriticalPathType
        {
            shortest = 0,
            longest = 1
        };

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        public Graph(IEnumerable<Vertex<T>> nodes = null)
        {

        }
    }
}



