using System;
using System.Collections.Generic;
using System.Text;

namespace JuanMartin.Kernel.Utilities.DataStructures
{
    /// <summary>
    /// Class implementing the list data structure
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LinkedList<T> : ICloneable where T : IComparable<T>
    {
        private const string LinkedListType = "LinkedList";
        private int _size;
        private Link<T> _first;
        private Link<T> _last;
        private readonly string _type;
        private readonly Dictionary<int, T> _bag;

        private LinkedList(string listType, Link<T> first, Link<T> last, int size)
        {
            _type = char.ToUpper(listType[0]) + listType.Substring(1);
            _size = size;
            _first = first;
            _last = last;
            _bag = new Dictionary<int, T>();
        }

        public LinkedList(string listType, T[] values)
        {
            var len = values.Length;
            var nodes = new Link<T>[len];
            _type = char.ToUpper(listType[0]) + listType.Substring(1);
            _bag = new Dictionary<int, T>();

            // initialize nodes with values only
            for (int i = 0; i < len; i++)
            {
                nodes[i] = new Link<T>(values[i]);
                _bag.Add(i + 1, values[i]); // list indexes are one lessb than their array counterparts
            }

            // set node links set head and tail
            for (int i = 0; i < len; i++)
            {
                if (i == 0)
                {
                    if (len > 1)
                        nodes[i].Next = nodes[i + 1];
                    _first = nodes[i];
                }
                else if (i == len - 1)
                {
                    if (len > 1)
                        nodes[i].Previous = nodes[i - 1];
                    _last = nodes[i];
                }
                else if (i > 0 && i < len)
                {
                    nodes[i].Next = nodes[i + 1];
                    nodes[i].Previous = nodes[i - 1];
                }
            }
            _size = len;
        }

        public LinkedList(string listType) : this(listType, null, null, 0)
        { }

        public LinkedList() : this(LinkedListType)
        { }

        public LinkedList(T[] values) : this(LinkedListType, values)
        { }

        public bool IsEmpty()
        {
            return _first == null;
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
                throw new StackOverflowException(string.Format("Stack overflow when creating noe for ({0}).", value));

            if (_last == null)
            {
                _last = node;
            }
            else
            {
                _last.Next = node;
                node.Previous = _last;
                _last = node;
            }
            if (_first == null)
            {
                _first = node;
            }
            _size++;
            _bag.Add(_size, value);

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
                throw new StackOverflowException(string.Format("Stack overflow when creating noe for ({0}).", value));
            }

            if (_first == null)
            {
                _first = node;
            }
            else
            {
                _first.Previous = node;
                node.Next = _first;
                _first = node;
            }
            if (_last == null)
            {
                _last = node;
            }
            _size++;
            _bag.Add(_size, value);

            return node;
        }

        /// <summary>
        /// Look for a value in the list and delete the corresponging node (linking  over it).
        /// </summary>
        /// <param name="value"></param>
        public void Remove(T value)
        {
            if (IsEmpty())
                throw new InvalidOperationException(_type + " is empty.");
            else
            {
                var node = _first;
                var position = 0;
                while (node != null)
                {
                    if (node.Item.Equals(value))
                    {
                        var p = node.Previous;

                        if (p != null)
                        {
                            var n = node.Next;
                            p.Next = node;
                            if (n != null)
                            {
                                n.Previous = node;
                            }
                            node = null;
                        }
                        else
                        {
                            // the value to the list corresponded to the first element in the list. The new first is the next of the original first
                            var n = _first.Next;
                            _first = null;
                            _first = n;
                        }
                        _bag.Remove(position);
                    }
                    node = node.Next;
                    position++;
                }
            }
        }

        /// <summary>
        /// Look for a value in the list and delete the corresponging node (linking  over it).
        /// </summary>
        /// <param name="key">todo: describe key parameter on RemoveByKey</param>
        public void RemoveByKey(int key)
        {
            if (IsEmpty())
                throw new InvalidOperationException(_type + " is empty.");
            else
            {
                var node = Get(key);
                if (node != null)
                {
                    var p = node.Previous;
                    var n = node.Next;
                    p.Next = n;
                    n.Previous = p;
                    node = null;

                    _bag.Remove(key);
                }
            }
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
                var previuos = _last.Previous;
                _last = previuos;
                if (_last == null)
                {
                    _first = null;
                }
            }
        }

        /// <summary>
        /// Get a string representation of linked list as a comma separated list of values in nodes
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var node = _first;
            var s = string.Empty;

            if (IsEmpty())
                throw new InvalidOperationException(_type + " is empty.");

            var builder = new StringBuilder();

            while (node != null)
            {
                builder.Append(node + ((node.Next != null) ? "," : string.Empty));
                node = node.Next;
            }
            s = builder.ToString();

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
        /// <param name="key"></param>
        /// <returns></returns>
        public Link<T> this[int key]
        {
            get
            {
                return Get(key);
            }
        }

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

            var node = _first.Next;

            array[i] = _first.Item;
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
            return _bag.ContainsValue(value);
        }

        /// <summary>
        /// Create a copy in memory of the linked list
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            var list = new LinkedList<T>(_type, this.ToArray());

            return list;
        }

        /// <summary>
        /// Get node indicated with index key counting linked nodes from _first.
        /// </summary>
        /// <param name="key">Integer position of node starting in zero to one less than length</param>
        /// <returns></returns>
        private Link<T> Get(int key)
        {
            var node = _first;
            var position = 0;

            if (IsEmpty())
                throw new IndexOutOfRangeException(_type + " cannot be inexed because it is empty.");

            if (key >= _size)
            {
                throw new IndexOutOfRangeException("Index specified is greater than or equal to " + _type + " size.");
            }
            while (node != null && position <= key)
            {
                if (position == key)
                {
                    return node;
                }
                position++;
                node = node.Next;
            }
            return null;
        }

        private LinkedList<T> Qsort(LinkedList<T> list)
        {
            var len = list.Length;
            if (len <= 1)
                return list;
            else
            {
                var i = 0;
                var rnd = new Random();

                var pivot = (dynamic)list[rnd.Next(0, len)].Item;

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








