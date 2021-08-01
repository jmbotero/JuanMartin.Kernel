using System;

namespace JuanMartin.Kernel.Utilities.DataStructures
{
    /// <summary>
    /// Class defining first-in-last-out implementation of linked list data structure
    /// </summary>
    /// <typeparam name="T"></typeparam>
   
    public  class Stack<T> where T : IComparable<T>
    {
        private readonly LinkedList<T> filo;

        public Stack()
        {
            filo = new LinkedList<T>("stack");
        }

        public Stack(T[] values) : this()
        {
            foreach (var v in values)
                Push(v);
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
            filo.Add (value);
        }

        /// <summary>
        /// Removes and returns the last element added to the stack: first-i-last-out
        /// </summary>
        /// <returns></returns>
        public Link<T> Pop()
        {
            if (filo.IsEmpty())
                throw new InvalidOperationException("Cannot pop a item  from  an empty stack.");

            var node = filo.First;

            filo.RemoveFirst();

            return node;
        }

        /// <summary>
        /// Returns the top elment without removing it from the stack
        /// </summary>
        /// <returns></returns>
        public T Peek()
        {
            var node = filo.First;

            return node.Item;
        }
    }
}
