using System;

namespace JuanMartin.Kernel.Utilities.DataStructures
{
    public static class GenericLimit<T>
    {
        public static readonly T MinValue = (T)MinValue.GetType().GetField("MinValue").GetValue(MinValue);
        public static readonly T MaxValue = (T)MaxValue.GetType().GetField("MaxValue").GetValue(MaxValue);
    }

    /// <summary>
    /// Class for a node in a tree.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TreeNode<T> where T : IComparable<T>
    {
        private T _item;
        private readonly long _index;
        private TreeNode<T> _left;
        private TreeNode<T> _right;
        private bool _visited;

        public T Item
        {
            get { return _item; }
            set { _item = value; }
        }

        public TreeNode<T> Left
        {
            get { return _left; }
            set { _left = value; }
        }

        public TreeNode<T> Right
        {
            get { return _right; }
            set { _right = value; }
        }

        public bool Visited
        {
            get { return _visited; }
            set { _visited = value; }
        }

        public long Index
        {
            get { return _index; }
        }

        public TreeNode(T value, long index)
        {
            _item = value;
            _index = index;
            _left = null;
            _right = null;
            _visited = false;
        }

        public static bool operator >(TreeNode<T> n1, TreeNode<T> n2)
        {
            bool isGreaterThan = false;

            if (n1 != null && n2 != null)
                isGreaterThan = (dynamic)n1.Item > (dynamic)n2.Item;

            return isGreaterThan;
        }

        public static bool operator >(TreeNode<T> n1, T v2)
        {
            bool isGreaterThan = false;

            if (n1 != null)
                isGreaterThan = (dynamic)n1.Item > (dynamic)v2;

            return isGreaterThan;
        }

        public static bool operator <(TreeNode<T> n1, TreeNode<T> n2)
        {
            bool isLessThan = false;

            if (n1 != null && n2 != null)
                isLessThan = (dynamic)n1.Item < (dynamic)n2.Item;

            return isLessThan;
        }

        public static bool operator <(TreeNode<T> n1, T v2)
        {
            bool isLessThan = false;

            if (n1 != null)
                isLessThan = (dynamic)n1.Item < (dynamic)v2;

            return isLessThan;
        }

        public bool Equal(TreeNode<T> node)
        {
            bool isEqual = false;

            if (node != null && this != null)
                isEqual = (dynamic)_item == (dynamic)node.Item;

            return isEqual;
        }

        public bool Equal(T value)
        {
            bool isEqual = false;

            if (this != null)
                isEqual = (dynamic)_item == (dynamic)value;

            return isEqual;
        }

        public override string ToString()
        {
            return Convert.ToString(_item);
        }
    }
}
