using System;

namespace JuanMartin.Kernel.Utilities.DataStructures
{
    /// <summary>
    /// Class defining first-in-first-out implementation of linked list data structure
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Queue<T> where T : IComparable<T>
    {
        private readonly LinkedList<T> fifo;

        public Queue()
        {
            fifo = new LinkedList<T>("queue");
        }

        public bool IsEmpty()
        {
            return fifo.IsEmpty();
        }

        public int Length
        {
            get { return fifo.Length; }
        }

        public Link<T> this[int key]
        {
            get
            {
                return fifo[key];
            }
        }
        public override string ToString()
        {
            return fifo.ToString();
        }

        /// <summary>
        /// Add a new ekement at the end of the queue
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Element added></returns>
        public Link<T> EnQueue(T value)
        {
            return fifo.Append(value);
        }

        /// <summary>
        /// Remove element from the front of the queue
        /// </summary>
        /// <returns>Element removed></returns>
        public Link<T> DeQueue()
        {
            var node = fifo[0];

            fifo.RemoveByIndex(0);

            return node;
        }

        /// <summary>
        /// Returns the top elment without removing it from the front of queue
        /// </summary>
        /// <returns></returns>
        public T Peek()
        {
            var node = fifo[0];

            return node.Item;
        }
    }
}
