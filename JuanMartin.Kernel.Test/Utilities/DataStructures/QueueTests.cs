using JuanMartin.Kernel.Utilities.DataStructures;
using NUnit.Framework;
using System;

namespace JuanMartin.Kernel.Utilities.DataStructures.Tests
{
    [TestFixture]
    class QueueTests
    {
        [Test]
        public static void ShouldThrowIndexOutOfRangeExceptionIfPeekOnEmptyQueue()
        {
            var actualQueue = new Queue<int>();

            Assert.IsTrue(actualQueue.IsEmpty());
            var actualOperationException = Assert.Throws<IndexOutOfRangeException>(() => actualQueue.Peek());
            Assert.IsTrue(actualOperationException.Message.Contains("cannot be inexed because it is empty"));
        }
    }
}
