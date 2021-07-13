using System;

namespace JuanMartin.Kernel.Utilities.DataStructures
{
    /// <summary>
    /// Class defining a node for a linked list data,  stack, or queue structure
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Link<T> where T : IComparable<T>
    {
        public Link(T value)
        {
            Key = Guid.NewGuid();
            Index = -1; //initilize as not defined
            Item = value;
            Next = null;
            Previous = null;
        }

        /// <summary>
        /// Commonly a link may be stored in a list or dictionary, this
        /// property allows to store in the link object its external
        /// key. 
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Commonly a link may be stored in a list or dictionary, this
        /// property allows to store in the link object its external
        /// index - (position in external collection) 
        /// </summary>
        public int Index { get; set; }

        public T Item { get; set; }

        public Link<T> Next { get; set; }

        public Link<T> Previous { get; set; }

        public override string ToString() => Convert.ToString(Item);

        public static bool operator >(Link<T> n1, Link<T> n2)
        {
            var isGreaterThan = false;

            if (n1 != null && n2 != null)
                isGreaterThan = (dynamic)n1.Item > (dynamic)n2.Item;

            return isGreaterThan;
        }

        public static bool operator >(Link<T> n1, T v2)
        {
            var isGreaterThan = false;

            if (n1 != null)
                isGreaterThan = (dynamic)n1.Item > (dynamic)v2;

            return isGreaterThan;
        }

        public static bool operator <(Link<T> n1, Link<T> n2)
        {
            var isLessThan = false;

            if (n1 != null && n2 != null)
                isLessThan = (dynamic)n1.Item < (dynamic)n2.Item;

            return isLessThan;
        }

        public static bool operator <(Link<T> n1, T v2)
        {
            var isLessThan = false;

            if (n1 != null)
                isLessThan = (dynamic)n1.Item < (dynamic)v2;

            return isLessThan;
        }

        public bool Equal(Link<T> node)
        {
            var isEqual = false;

            if (node != null && this != null)
                isEqual = (dynamic)Item == (dynamic)node.Item;

            return isEqual;
        }

        public bool Equal(T value)
        {
            var isEqual = false;

            if (this != null)
                isEqual = (dynamic)Item == (dynamic)value;

            return isEqual;
        }


    }

}
