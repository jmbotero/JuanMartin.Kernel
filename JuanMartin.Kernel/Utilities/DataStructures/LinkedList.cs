using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JuanMartin.Kernel.Utilities.DataStructures
{
    /// <summary>
    /// Class implementing the liked list data structure,
    /// zero-based index
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LinkedList<T> : ICloneable where T : IComparable<T>
    {
        private const string LinkedListType = "LinkedList";
        private int _size;
        private readonly string _type;
        private readonly Dictionary<Guid, Link<T>> _bag; // add link to dictionary to allow to store multiple values

        public LinkedList(string listType, T[] values)
        {
            if (values == null)
            {
                _type = listType;
                _size = 0;
                First = null;
                Last = null;
                _bag = new Dictionary<Guid, Link<T>>();
            }
            else
            {
                var len = values.Length;
                var nodes = new Link<T>[len];
                _type = char.ToUpper(listType[0]) + listType.Substring(1);
                _bag = new Dictionary<Guid, Link<T>>();

                // initialize nodes with values and array index for key
                for (int i = 0; i < len; i++)
                {
                    nodes[i] = new Link<T>(values[i]);
                    _bag.Add(nodes[i].Key, nodes[i]);
                }

                // set node links set head and tail
                for (int i = 0; i < len; i++)
                {
                    if (i == 0)
                    {
                        if (len > 1)
                            nodes[i].Next = nodes[i + 1];
                        First = nodes[i];
                    }
                    else if (i == len - 1)
                    {
                        if (len > 1)
                            nodes[i].Previous = nodes[i - 1];
                        Last = nodes[i];
                    }
                    else if (i > 0 && i < len)
                    {
                        nodes[i].Next = nodes[i + 1];
                        nodes[i].Previous = nodes[i - 1];
                    }
                }
                _size = len;
            }
        }

        public LinkedList(string listType) : this(listType, null)
        { }

        public LinkedList() : this(LinkedListType)
        { }

        public LinkedList(T[] values) : this(LinkedListType, values)
        { }

        public bool IsEmpty()
        {
            return First == null;
        }

        /// <summary>
        /// Add node (for value) at end of list.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Returns node just added coontaining the new value, last property of list pinting to it and it pointing to nothing.</returns>
        public Link<T> Append(T value)
        {
            //if(value.GetType()!=this.GetType().GetGenericArguments().Single())
            //{
            //    throw new InvalidCastException(string.Format("Type of object being added to list ({0}) must be the same type as the " + type + "'s type ({1}). ", value.GetType(), this.GetType()));
            //}
            var node = new Link<T>(value);

            if (node == null)
                throw new StackOverflowException(string.Format("Stack overflow when creating node for ({0}).", value));

            if (Last == null)
            {
                Last = node;
            }
            else
            {
                Last.Next = node;
                node.Previous = Last;
                Last = node;

                //index  new node appropriately
                node.Index = node.Previous.Index + 1;
            }
            if (First == null)
            {
                First = node;
            }
            _size++;

            _bag.Add(node.Key, node);

            return node;
        }

        /// <summary>
        /// Add node (for value) at beginning of list.
        /// </summary>
        /// <param name="value">Value to add</param>
        /// <returns>Returns node just added coontaining the value added and a pointer to the firrst element before the addition.</returns>
        public Link<T> Add(T value)
        {
            //if (value.GetType() != this.GetType().GetGenericArguments().Single())
            //{
            //    throw new InvalidCastException(string.Format("Type of object being added to list ({0}) must be the same type as the " + type + "'s type ({1}). ", value.GetType(), this.GetType()));
            //}
            var node = new Link<T>(value);

            if (node == null)
            {
                throw new StackOverflowException(string.Format("Stack overflow when creating node for ({0}).", value));
            }
            if (First == null)
            {
                First = node;
                node.Index = 0;
            }
            else
            {
                node.Next = First;
                if (Last.Previous == null)
                    Last.Previous = node;
                First = node;
                //index  new node appropriately
                node.Index = 0;
                ReIndexListNodes(node);
            }
            if (Last == null)
            {
                Last = node;
            }

            _size++;
            _bag.Add(node.Key, node);

            return node;
        }

        /// <summary>
        /// Look for a value in the list and delete the corresponging node setting
        /// it to null and linking  over it.
        /// </summary>
        /// <param name="value"></param>
        public void Remove(T value)
        {
            if (IsEmpty())
                throw new InvalidOperationException(_type + " is empty.");
            else
            {
                var node = First;
                while (node != null)
                {
                    if (node.Item.Equals(value))
                    {

                        if(node==First)
                        {
                            First = node.Next;
                            // reindex
                             node.Index = 0;
                            ReIndexListNodes(node);
                        }
                        else if(node==Last)
                        {
                            Last.Previous = Last;
                        }
                        else
                        {
                            var previous = node.Previous;
                            previous.Next = node.Next;
                            ReIndexListNodes(previous);
                        }

                        _bag.Remove(node.Key);
                        _size--;
                        if (_size == 0)
                        {
                            First = null;
                            Last = null;
                        }

                        node = null;
                    }
                    if(node != null)
                        node = node.Next;
                }
            }
        }

        /// <summary>
        /// Delete the corresponging node indexed in list (linking  over it).
        /// </summary>
        /// <param name="position">index in list</param>
        public void RemoveByIndex(int position)
        {
            var node = Get(position);
            if (node != null)
                Remove(node.Item);
        }

        /// <summary>
        /// Remove the last node of the list, by making the current previous to last node the new last.
        /// </summary>
        /// <returns></returns>
        public void RemoveLast()
        {
            if (IsEmpty())
                throw new InvalidOperationException(_type + " is empty.");
            else
            {
                if (Last != null)
                {
                    if (Last.Index == -1)
                        throw new ArgumentException("node index has not been set.");
                    _bag.Remove(Last.Key);
                    _size--;
                    var previuos = Last.Previous;
                    Last = previuos;
                    if(Last != null)
                        Last.Next = null;
                    if(_size == 0)
                    {
                        Last = null;
                        First = null;
                    }
                }
                else
                    throw new ArgumentException("Last node is undefined:  result of  possible linking issue.");
            }
        }

        /// <summary>
        /// Get a string representation of linked list as a comma separated list of values in nodes
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var node = First;
            if (IsEmpty())
                throw new InvalidOperationException(_type + " is empty.");

            var builder = new StringBuilder();

            while (node != null)
            {
                builder.Append(node.ToString() + ((node.Next != null) ? "," : string.Empty));
                node = node.Next;
            }
            string s = builder.ToString();
            return s;
        }

        /// <summary>
        /// Number of nodes minus one in list
        /// </summary>
        public int Length
        {
            get { return _size; }
        }

        /// <summary>
        /// Indexing off zero based list
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public Link<T> this[int position]
        {
            get
            {
                return Get(position);
            }
        }

        public Link<T> Last { get; private set; }
        public Link<T> First { get; private set; }

        public static LinkedList<T> operator +(LinkedList<T> l1, LinkedList<T> l2)
        {
            if (l1 == null && l2 != null)
                return l2;
            else if (l1 != null && l2 == null)
                return l1;
            else
            {
                var list = l1; // new LinkedList<T>();
                var lemgth = l2.Length;
                for (int i = 0; i < lemgth; i++)
                {
                    list.Append(l2[i].Item);
                }
                return list;
            }
        }


        /// <summary>
        /// Put value of every node in list in an array, following same order
        /// </summary>
        /// <returns></returns>
        public T[] ToArray()
        {
            var array = new T[_size];
            var i = 0;

            var node = First.Next;

            array[i] = First.Item;
            i++;
            while (node != null)
            {
                array[i] = node.Item;
                node = node.Next;
                i++;
            }

            return array;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public LinkedList<T> QuickSort()
        {
            return Qsort(this);
        }

        /// <summary>
        /// Return true/false if value is contained in list
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Contains(T value)
        {
            var element = _bag.Where(i => i.Value.Item.Equals(value)).FirstOrDefault().Value;

            return element != null;
        }

        /// <summary>
        /// Create a copy in memory of the linked list
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            var list = new LinkedList<T>(_type);
            foreach (var item in _bag.Values)
                list.Add(item.Item);

            return list;
            //return this.MemberwiseClone();
        }

        /// <summary>
        /// Get node indicated with index or position counting linked nodes from _first,
        /// its a zero-bsed count.
        /// </summary>
        /// <param name="position">Integer position of node starting in zero to one less than length</param>
        /// <returns></returns>
        private Link<T> Get(int position)
        {
            var node = First;
            var i = 0;

            if (IsEmpty())
                throw new IndexOutOfRangeException(_type + " cannot be inexed because it is empty.");

            if (position > _size - 1 || position < 0)
            {
                throw new IndexOutOfRangeException($"Index specified [{position}] is out of list bounds 0...{_size - 1}.");
            }
            while (node != null && i <= position)
            {
                if (i == position)
                {
                    return node;
                }
                i++;
                node = node.Next;
            }
            return null;
        }

        /// <summary>
        /// Recalculate indexes o elements in list starting in 'n' sequentially. 
        /// </summary>
        /// <param name="n"></param>
        private void ReIndexListNodes(Link<T> n)
        {
            while (n.Next != null)
            {
                n.Next.Index = n.Index + 1;
                n = n.Next;
            }
        }

        private LinkedList<T> Qsort(LinkedList<T> list)
        {
            var len = list.Length;
            if (len <= 1)
                return list;
            else
            {
                var i = 0;

                var pivot = (dynamic)list[new Random().Next(0, len)].Item;

                var left = new LinkedList<T>();
                var right = new LinkedList<T>();

                // create two partitions based on pivot
                while (left.Length + right.Length < len - 1)
                {
                    var item = list[i].Item;
                    if (item < pivot)
                    {
                        left.Append(item);
                    }
                    else if (item > pivot)
                    {
                        right.Append(item);
                    }
                    i++;
                }

                left = Qsort(left);
                right = Qsort(right);

                // merge partitions
                left.Append(pivot);
                left += right;

                return left;
            }
        }
    }
}








