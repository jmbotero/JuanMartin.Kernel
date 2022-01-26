using System.Diagnostics;

namespace JuanMartin.Kernel.Utilities.DataStructures
{
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public class Neighbor<T>
    {
        public enum NeighborType
        {
            incoming = 0,
            outgoing = 1,
            both = 2,
            none = 8
        };

        public Vertex<T> Node { get; set; }

        public NeighborType Type { get; set; }

        public override string ToString()
        {
            return $"{Node}:{Type}";
        }

        private string GetDebuggerDisplay()
        {
            return ToString();
        }
    }
}
