using JuanMartin.Kernel.Extesions;
using System;

namespace JuanMartin.Kernel.Utilities.DataStructures
{

    public class Tree<T> where T : IComparable<T>
    {
        #region Private members
        private long _size;
        private T _sum;

        private TreeNode<T> _root;

        /// <summary>
        /// Only method to create a new treenode to be added
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private TreeNode<T> NewNode(T value)
        {
            //create a new tree node
            TreeNode<T> node = new TreeNode<T>(value, _size + 1);

            if (node == null)
                throw new InvalidOperationException("Cannot add new tree node due to stack overflow.");

            //add new value to total tree addition
            _sum = UtilityMath.AddGenericValues<T>(_sum, value);
            //increase tree sizxe in 1
            _size++;

            return node;
        }

        private TreeNode<T> Add(TreeNode<T> node, T value)
        {
            TreeNode<T> AuxNode = null;

            if (!Exists(value))
            {
                //new node to add to tree
                AuxNode = NewNode(value);

                Add(node, AuxNode);
            }

            return AuxNode;
        }

        //private void Add(TreeNode<T> node, T value)
        //{
        //    if (!Exists(value))
        //    {
        //        //new node to add to tree
        //        TreeNode<T> AuxNode = NewNode(value);

        //        Add(node, AuxNode);
        //    }
        //}

        private void Add(TreeNode<T> root, TreeNode<T> node)
        {
            bool isGreater = false;

            if (node > root)
                isGreater = true;
            else if (root.Item.CompareTo(node.Item) < 0)
                isGreater = true;

            //if new node item is geater than the root item add it to its right otherwise add it to the left
            if (isGreater)
            {
                if (root.Right == null)
                    root.Right = node;
                else
                    Add(root.Right, node);
            }
            else
            {
                if (root.Left == null)
                    root.Left = node;
                else
                    Add(root.Left, node);
            }
        }

        private string ToString(TreeNode<T> node)
        {
            string s = string.Empty;

            if (node != null)
            {
                s = node.ToString();

                if (node.Left != null)
                    s += "," + ToString(node.Left);
                if (node.Right != null)
                    s += "," + ToString(node.Right);

            }
            return s;
        }

        //define an indexer
        private TreeNode<T> GetNode(long index, TreeNode<T> root)
        {
            if (index == root.Index)
            {
                return root;
            }
            else
            {
                if (root.Left != null) return GetNode(index, root.Left);
                if (root.Right != null) return GetNode(index, root.Right);
            }

            return null; //unreachable
        }
        #endregion

        #region Public members
        public Tree()
        {
            _root = null;
            _size = 0;
        }

        public Tree(T[] numbers)
        {
            LoadArrayTree(numbers);
        }

        public TreeNode<T> Root
        {
            get { return _root; }
        }

        public long Size
        {
            get { return _size; }
        }

        public T Sum
        {
            get { return _sum; }
        }

        public void Reset()
        {
            _root = null;
            _size = 0;
            _sum = (dynamic)0;
        }

        public bool IsEmpty()
        {
            return _root == null;
        }

        public System.Collections.Generic.List<T> GetLeafNodes()
        {
            return GetLeafNodes(_root);
        }

        public System.Collections.Generic.List<T> GetLeafNodes(TreeNode<T> node)
        {
            System.Collections.Generic.List<T> leafs = new System.Collections.Generic.List<T>();

            GetLeafNodes(node, leafs);

            return leafs;
        }

        private void GetLeafNodes(TreeNode<T> node, System.Collections.Generic.List<T> leafs)
        {
            if (node != null && node.Right == null && node.Left == null)
                leafs.Add(node.Item);
            else
            {
                if (node.Right != null)
                    GetLeafNodes(node.Right, leafs);
                if (node.Left != null)
                    GetLeafNodes(node.Left, leafs);
            }

        }

        public bool Exists(T value)
        {
            bool exist = false;
            TreeNode<T> node = _root;
            long i = 0;

            while (i < _size && !exist)
            {
                if (node.Equal(value))
                    exist = true;
                else if (node > value)
                {
                    if (node.Left == null)
                        break;
                    else
                        node = node.Left;
                }
                else
                {
                    if (node.Right == null)
                        break;
                    else
                        node = node.Right;
                }
                i++;
            }

            return exist;
        }

        public TreeNode<T> Add(T value)
        {
            if (!Exists(value))
            {
                //new node to add to tree
                TreeNode<T> node = NewNode(value);

                if (!IsEmpty())
                {
                    Add(_root, node);
                }
                else
                {
                    //tree is empty: adding node as root
                    _root = node;
                }

                return node;
            }

            return null;
        }

        public override string ToString()
        {
            return ToString(_root);
        }

        public TreeNode<T> this[long index]
        {
            get
            {
                if (index == 0)
                    throw new IndexOutOfRangeException();

                if (this.IsEmpty())
                    return null;
                else
                    return GetNode(index, _root);
            }
        }

        public T Min()
        {
            TreeNode<T> node = _root;

            while (node.Left != null)
                node = node.Left;

            return node.Item;
        }

        public T Max()
        {
            TreeNode<T> node = _root;

            while (node.Right != null)
                node = node.Right;

            return node.Item;
        }

        #endregion

        public void LoadArrayTree(T[] numbers)
        {
            if (_root == null)
            {
                _root = NewNode(numbers[0]);
                numbers = numbers.RemoveAt(0);
            }
            LoadArrayTree(_root, numbers);
        }

        private void LoadArrayTree(TreeNode<T> root, T[] values)
        {
            TreeNode<T> node = root;

            foreach (T number in values)
            {
                node = Add(node, number);
            }
        }

        public void LoadFactorTree(T number)
        {
            if (_root == null)
                _root = NewNode(number);

            LoadFactorTree(number, _root);
        }

        private void LoadFactorTree(T number, TreeNode<T> node)
        {
            if (number is long)
            {
                long i = 2;

                do
                {
                    if ((dynamic)number % i == 0)
                    {
                        if ((dynamic)number <= 2)
                            break;

                        var numberLeft = (dynamic)i;
                        var numberRight = (dynamic)number / i;

                        if (numberLeft > numberRight)
                        {
                            long aux = numberLeft;
                            numberLeft = numberRight;
                            numberRight = aux;
                        }
                        if (numberRight > 1)
                        {
                            node.Right = NewNode(numberRight);
                            LoadFactorTree(numberRight, node.Right);
                        }
                        if (numberLeft > 1)
                        {
                            node.Left = NewNode(numberLeft);
                            LoadFactorTree(numberLeft, node.Left);
                        }
                        return;
                    }
                    else
                        i++;
                } while (i < (dynamic)number);
            }

        }




    }
}


