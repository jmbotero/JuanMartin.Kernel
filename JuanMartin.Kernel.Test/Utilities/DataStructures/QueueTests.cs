using JuanMartin.Kernel.Utilities.DataStructures;
using NUnit.Framework;
using System;

namespace JuanMartin.Kernel.Utilities.DataStructures.Tests
{
    [TestFixture]
    class QueueTests
    {
        [Test]
        public static void QueueTest_Peek_EmptyQueue_ShouldThrowIndexOutOfRangeException()
        {
            var q = new Queue<int>();

            Assert.IsTrue(q.IsEmpty());
            var ex = Assert.Throws<IndexOutOfRangeException>(() => q.Peek());
            Assert.IsTrue(ex.Message.Contains("cannot be inexed because it is empty"));
        }
    }
}
