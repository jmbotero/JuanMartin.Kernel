using NUnit.Framework;
using JuanMartin.Kernel.Utilities.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JuanMartin.Kernel.Utilities.DataStructures.Tests
{
    [TestFixture()]
    public class StackTests
    {
        [Test()]
        public void ShouldBeEmptyWhenCreatingNewStack()
        {
            var actualStack = new Stack<int>();
            var expectedLength = 0;

            Assert.IsTrue(actualStack.IsEmpty(), "Stack is empty.");
            Assert.AreEqual(expectedLength, actualStack.Length, $"Stack length is {expectedLength}.");
        }

        [Test()]
        public void ShouldIncreaseLengthByOneWhenPushingAnElement()
        {
            var actualStack = new Stack<int>();
            var actualItem = 5;
            var expectedLength = 1;

            actualStack.Push(actualItem);

            Assert.AreEqual(expectedLength, actualStack.Length, $"Stack length is {expectedLength}.");

            expectedLength = 2;

            actualStack.Push(actualItem);

            Assert.AreEqual(expectedLength, actualStack.Length, $"Stack length is {expectedLength}.");
        }

        [Test()]
        public void ShouldReduceLegthAsElementsArePopped()
        {
            var actualStack = new Stack<int>(new int[] { 1, 2, 3 });

            var expectedLength = 2;

            actualStack.Pop();
            Assert.AreEqual(expectedLength, actualStack.Length, $"Stack length is {expectedLength}.");
            expectedLength = 0;

            actualStack.Pop();
            actualStack.Pop();
            Assert.AreEqual(expectedLength, actualStack.Length, $"Stack length is {expectedLength}.");
        }

        [Test()]
        public void ShouldBeAbleToReadLastValueInStackWithoutRemovingIt()
        {
            var actualStack = new Stack<int>();
            var expectedItem = 3;

            actualStack.Push(1);
            actualStack.Push(2);
            actualStack.Push(expectedItem);

            var actualItem = actualStack.Peek();

            Assert.AreEqual(expectedItem, actualItem, $"Next value to be popped is {expectedItem}.");
            Assert.AreEqual(expectedItem, actualStack[0].Item, $"{actualItem} is present in stack.");
        }

        [Test()]
        public void ShouldThrowInvalidOperationExceptionWhenTryingToPopItemsFronEmptyList ()
        {
            var actualStack = new Stack<int>();
             
            var actualOperationException = Assert.Throws<InvalidOperationException>(() => actualStack.Pop(), "Invalid Pop()");
            Assert.IsTrue(actualOperationException.Message.Contains("Cannot pop a item  from  an empty stack"), "Invalid Pop()");
            Assert.IsTrue(actualStack.IsEmpty(), "Stack is empty.");
        }

        [Test()]
        public void  ShouldOutputPalindromeOfStringByPoppingAllCharactesPushedIntoStack()
        {
            var actualStack = new Stack<char>();
            var baseWord = "abc";
            var actualWord = "";
            var expectedWord = "cba";

            foreach (var letter in baseWord)
                actualStack.Push(letter);

            while(actualStack.Length>0)
            //while(!actualStack.IsEmpty())
                actualWord += actualStack.Pop().ToString();

            Assert.AreEqual(expectedWord, actualWord);
        }
    }
}