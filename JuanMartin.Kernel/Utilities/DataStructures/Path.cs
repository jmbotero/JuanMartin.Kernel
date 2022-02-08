using JuanMartin.Kernel.Extesions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace JuanMartin.Kernel.Utilities.DataStructures
{
    [DebuggerDisplay("{GetDebuggerDisplay(),nq}")]
    public class Path<T>
    {
        public Path()
        {
            Vertices = new List<Vertex<T>>();
            Weight = 0;
        }

        public List<Vertex<T>> Vertices { get; set; }
        public int Weight { get; set; }
        public bool IsComplete()
        {
            if (Vertices == null)
                return false;
            return Vertices.IsComplete();
        }

        public bool IsLast(string name)
        {
            if (VertexCount == 0)
                return false;
            return (Vertices.Last().Name == name);
           //return (Vertices.FirstOrDefault(v => v.Name == Vertices.Last().Name && v.Name == name) != null);
        }
        public int VertexCount => (Vertices != null) ? Vertices.Count : 0;

        public bool IsSingle()
        {
            if (Vertices == null)
                return false;

            return Vertices.IsSingle();
        }

        public void AddVertex(Vertex<T> v)
        {
            //check not duplicate by guid
            if (v !=   null &&  Vertices.FirstOrDefault(item => item.Guid == v.Guid) == null)
                Vertices.Add(v);
        }

        public void Append(Path<T> p)
        {
            Vertices.AddRange(p.Vertices);
        }

        public bool ContainsByName(string name)
        {
            return (Vertices.FirstOrDefault(Vertices => Vertices.Name == name) != null);
        }

        public bool ContainsByGuid(string guid)
        { 
            return (Vertices.FirstOrDefault(Vertices => Vertices.Guid == guid) != null);
        }

        // Define the indexer to allow client code to use [] notation.
        public Vertex<T> this[int i]
        {
            get { return Vertices[i]; }
        }

        public void RefreshWeight()
        {
            var weight = 0;

            for (var i = 0; i < VertexCount - 1; i++)
            {
                Vertex<T> v1 = Vertices[i];
                Vertex<T> v2 = Vertices[i + 1];
                if (UtilityType.IsNumericType(v1.Value.GetType()))
                    weight += Convert.ToInt32(v1.Value);
                else
                {
                    //var  edgeName = v2.Notes;  // get edge used to travel from v2 to v1
                    //var edge = v1.Edges.FirstOrDefault(e => e.Name.Contains(edgeName) && (v1.Name == null || (v1?.Name != null && e.From != null && e    == v1.Name)));
                    var edge = v1.Edges.FirstOrDefault(e => e.Type == Edge<T>.EdgeType.outgoing && e.From.Guid == v1.Guid && e.To.Guid == v2.Guid);

                    if (edge == null)

                        throw new NullReferenceException($"Incorrect vertex sequence in path, {v1.Name} to {v2.Name}, caused edge not to be found.");

                    weight += (int)edge.Weight;
                }
            }
            Weight = weight;
        }

        public void Reverse()
        {
            Vertices.Reverse();
        }

        public string ToString(string separator="")
        {
            return string.Join(separator, (IEnumerable<string>)Vertices.Select(v => v.Name).ToArray());
        }

        private string GetDebuggerDisplay()
        {
            return ToString(","); // string.Empty;
        }
    }
}
