using JuanMartin.Kernel.Utilities.DataStructures;
using NUnit.Framework;
using System;

namespace JuanMartin.Kernel.Utilities.DataStructures.Tests
{
    [TestFixture]
    class LinkedListTests
    {
        [Test]
        public static void ShouldInitializeLinkedListWithAnArray()
        {
            var expectedArray = new int[] { 1, 2, 3, 4 };
            var actualList = new LinkedList<int>(expectedArray);

            var actualArray = actualList.ToArray();

            Assert.AreEqual(expectedArray, actualArray);
        }

        [Test]
        public static void ShouldLaveAnEmptyListAfterRemovingOnlyElementInList()
        {
            var actualList = new LinkedList<int>();

            actualList.Add(5);
            actualList.Remove(5);
            Assert.IsTrue(actualList.IsEmpty());
        }

        [Test]
        public static void ShouldThrowInvalidOperationExceptionWhenAttemptinToRemoveAnElementInAnEmpyList()
        {
            var actualList = new LinkedList<int>();

            var ex = Assert.Throws<InvalidOperationException>(() => actualList.Remove(5));
        }

        [Test]
        public static void ShouldThrowIndexOutOfRangeExceptionWhenRemovingByKeyAndKeyIsGreaterThanListLength()
        {
            var actualList = new LinkedList<int>();
            var actualKey = 5;

            actualList.Add(1);
            actualList.Add(2);
            actualList.Add(3);

            var ex = Assert.Throws<IndexOutOfRangeException>(() => actualList.RemoveByKey(actualKey));
            Assert.IsTrue(ex.Message.Contains($"Index specified [{actualKey}] is out of list bounds 0...{actualList.Length - 1}."));
        }

        [Test]
        public static void ShouldAddElementAtEndOfListWhenAppendingToLinkedList()
        {
            var expectedValue = 3;
            var actualList = new LinkedList<int>();
    
            actualList.Append(1);                // index 0 element
            actualList.Append(2);                // index 1 element
            actualList.Append(expectedValue);    // index 2 element, last element
            var expectedIndex = actualList.Length - 1;

            Assert.AreEqual(expectedValue, actualList[expectedIndex].Item);
        }

        [Test]
        public static void ShouldAddElementAtBegginningOfListWhenAddingToLinkedList()
        {
            var expectedValue = 3;
            var expectedIndex = 0;
            var actualList = new LinkedList<int>();

            actualList.Add(1);                // index 2 element
            actualList.Add(2);                // index 1 element
            actualList.Add(expectedValue);    // index 0 element, first element

            Assert.AreEqual(expectedValue, actualList[expectedIndex].Item);
        }

        [Test]
        public static void ShouldGenerateNewAndIndependentListDuplicateWhenCloning()
        {
            var actualList = new LinkedList<int>();

            actualList.Add(1);
            actualList.Add(2);
            actualList.Add(3);

            var expectedList = (LinkedList<int>)actualList.Clone();
            //TODO: failed comparing full object
            //Assert.AreEqual(expectedList, actualList,"List and its clone are identical objects.");
            Assert.AreEqual(expectedList.Length, actualList.Length, "Lists  have same length");
            Assert.AreEqual(expectedList.ToString(), actualList.ToString(), "Lists have same strig representation.");

            var actualItem = 5;
            actualList.Add(actualItem);
            Assert.IsFalse(expectedList.Contains(actualItem), "Original and clone lists are independent.");
        }

        [Test]
        public static void ShouldReturnNewListWithItemsInAscendingOrderAfterQuickSortingListWithItemsAddedOutOfOrder()
        {
            var actualArray = new int[] { 5, 2, 7, 6, 1, 9, 4, 8 };
            var expectedArray = new int[] { 1, 2, 4, 5, 6, 7, 8, 9 };
            var actualList = new LinkedList<int>(actualArray);

            var expectedList = actualList.QuickSort();

            Assert.AreEqual(expectedList.ToArray(), expectedArray);
        }

        [Test]
        public static void ShouldAppendAllElementsOfTheSecondListToTheFirstWhenConcatenatingTwoSeparateLists()
        {
            var actualList1 = new LinkedList<int>();
            var actualList2 = new LinkedList<int>();

            actualList1.Append(4);
            actualList1.Append(5);
            actualList2.Append(6);
            actualList2.Append(7);
            actualList2.Append(8);

            var actualLength1 = actualList1.Length;
            var actualLength2 = actualList2.Length;

            actualList1 += actualList2;

            var expectedLength = actualList1.Length;

            Assert.AreEqual(expectedLength, actualLength1 + actualLength2, "Lenth of conatenated list is the addition of the legths of the two source lists.");

            for (int i = 0; i < actualLength2; i++)
            {
                var expectedIndex = i + actualLength2 - 1;
                Assert.AreEqual(actualList1[expectedIndex].Item, actualList2[i].Item,$"Item   ({expectedIndex}), {actualList2[i]}, minplaced.");
            }
        }
    }
}
