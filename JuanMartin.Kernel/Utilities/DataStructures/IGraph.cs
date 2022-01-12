﻿using System;
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
        Vertex<T> AddVertex(Vertex<T> v);
        Vertex<T> AddVertex(T value, string name, string guid);
        bool AddEdge(Vertex<T> from, Vertex<T> to, string name, Edge<T>.EdgeType type, Edge<T>.EdgeDirection direction, double weight);
        Vertex<T> RemoveVertex(string name);
        Vertex<T> RemoveVertex(T value);
        Edge<T> RemoveEdge(string name);
        List<Edge<T>> RemoveEdges(Vertex<T> from, Vertex<T> to, Edge<T>.EdgeType type);
        Edge<T> GetEdge(string name, string fromName    , Edge<T>.EdgeType type);
        List<Edge<T>> GetEdges(Vertex<T> from, Vertex<T> to, Edge<T>.EdgeType type);
        List<Edge<T>> GetOutgoingEdges();       
        List<Edge<T>> GetIncomingEdges();
        bool Contains(T value);
        bool Contains(Vertex<T> value);
        Vertex<T> GetVertex(string name);
        Vertex<T> GetVertex(Guid guid);
        Vertex<T> GetVertex(int index);
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

        public Graph(IEnumerable<Vertex<T>> nodes = null)
        {

        }
    }
}



