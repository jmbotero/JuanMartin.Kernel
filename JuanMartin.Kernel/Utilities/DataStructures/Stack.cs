using System;

namespace JuanMartin.Kernel.Utilities.DataStructures
{
    /// <summary>
    /// Class defining first-in-last-out implementation of linked list data structure
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class Stack<T> where T : IComparable<T>
    {
        private readonly LinkedList<T> filo;

        public Stack()
        {
            filo = new LinkedList<T>("stack");
        }

        public bool IsEmpty()
        {
            return filo.IsEmpty();
        }

        public int Length
        {
            get { return filo.Length; }
        }

        public Link<T> this[int key]
        {
            get
            {
                return filo[key];
            }
        }
        public override string ToString()
        {
            return filo.ToString();
        }

        /// <summary>
        /// Add a new element to the top of the stack
        /// </summary>
        /// <param name="value"></param>
        public void Push(T value)
        {
            filo.Append(value);
        }

        /// <summary>
        /// Removes and returns the top element from the stack
        /// </summary>
        /// <returns></returns>
        public Link<T> Pop()
        {
            var node = filo[0];

            filo.RemoveByKey(0);

            return node;
        }

        /// <summary>
        /// Returns the top elment without removing it from the stack
        /// </summary>
        /// <returns></returns>
        public T Peek()
        {
            var node = filo[0];

            return node.Item;
        }
    }
}
