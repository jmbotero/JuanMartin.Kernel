using JuanMartin.Kernel.Utilities.DataStructures;
using NUnit.Framework;
using System;

namespace JuanMartin.Kernel.Utilities.DataStructures.Tests
{
    [TestFixture]
    class LinkedListTests
    {
        [Test]
        public static void LinkedListTest_Initialized_WithAnArray_ShouldLinkedLkistWithSameElements()
        {
            var expectedArray = new int[] { 1, 2, 3, 4 };
            var actualList = new LinkedList<int>(expectedArray);

            var actualArray = actualList.ToArray();

            Assert.AreEqual(actualArray.Length, actualList.Length);
            Assert.AreEqual(actualArray, expectedArray);
        }

        [Test]
        public static void LinkedListTest_Remove_OnlyElementInList_ShouldGenerateAnEmptyList()
        {
            var actualList = new LinkedList<int>();

            actualList.Add(5);
            actualList.Remove(5);
            Assert.IsTrue(actualList.IsEmpty());
        }

        [Test]
        public static void LinkedListTest_Remove_OnAnEmpyList_ShouldThrowInvalidOperationException()
        {
            var actualList = new LinkedList<int>();

            var ex = Assert.Throws<InvalidOperationException>(() => actualList.Remove(5));
        }

        [Test]
        public static void LinkedListTest_RemoveByKey_KeyGreaterThanListLength_ShouldThrowIndexOutOfRangeException()
        {
            var actualList = new LinkedList<int>();


            actualList.Add(1);
            actualList.Add(2);
            actualList.Add(3);

            var ex = Assert.Throws<IndexOutOfRangeException>(() => actualList.RemoveByKey(5));
            Assert.IsTrue(ex.Message.Contains("Index specified is greater than or equal"));
        }

        [Test]
        public static void LinkedListTest_Append_OnNotEmptyList_ShouldAddElementAtEndOfList()
        {
            const int expectedValue = 3;
            const int expectedIndex = 2;
            var actualList = new LinkedList<int>();

            actualList.Append(1);                // index 0 element
            actualList.Append(2);                // index 1 element
            actualList.Append(expectedValue);    // index 2 element, last element

            Assert.AreEqual(expectedValue, actualList[expectedIndex].Item);
        }

        [Test]
        public static void LinkedListTest_Add_OnNotEmptyList_ShouldAddElementAtBeginningOfList()
        {
            const int expectedValue = 3;
            const int expectedIndex = 0;
            var actualList = new LinkedList<int>();

            actualList.Add(1);                // index 2 element
            actualList.Add(2);                // index 1 element
            actualList.Add(expectedValue);    // index 0 element, first element

            Assert.AreEqual(expectedValue, actualList[expectedIndex].Item);
        }


        [Test]
        public static void LinkedListTest_ToArray_AWellFormedList_ShouldReturnArrayWithSameElementsAsListAndInSameOrder()
        {
            var expectedArray = new int[] { 4, 2, 1, 3 };
            var actualList = new LinkedList<int>();

            actualList.Add(3);    // last element
            actualList.Add(1);
            actualList.Add(2);
            actualList.Add(4);    // first element

            var actualArray = actualList.ToArray();
            Assert.AreEqual(expectedArray, actualArray);
        }

        [Test]
        public static void LinkedListTest_Clone_AWellFormedList_ShouldGenerateNewListDuplicate()
        {
            var actualList = new LinkedList<int>();

            actualList.Add(1);
            actualList.Add(2);
            actualList.Add(3);

            var expectedList = (LinkedList<int>)actualList.Clone();
            Assert.AreEqual(expectedList.Length, actualList.Length);
            Assert.AreEqual(expectedList.ToArray(), actualList.ToArray());
        }

        [Test]
        public static void LinkedListTest_QuickSort_ListWithItemsAddedOutOfOrder_ShouldReturrnNewListWithItemsInAscendingOrder()
        {
            var actualArray = new int[] { 5, 2, 7, 6, 1, 9, 4, 8 };
            var expectedArray = new int[] { 1, 2, 4, 5, 6, 7, 8, 9 };
            var actualList = new LinkedList<int>(actualArray);

            var expectedList = actualList.QuickSort();

            Assert.AreEqual(expectedList.ToArray(), expectedArray);
        }

        [Test]
        public static void LinkedListTest_Concatenate_SameListToItself_ShouldDoubleLengthAndHaveAllElementsDuplicated()
        {
            var actualList = new LinkedList<int>();

            actualList.Add(1);
            actualList.Add(2);

            var actualLength = actualList.Length;
            var expectedLength = actualLength * 2;

            actualList += actualList;

            Assert.AreEqual(expectedLength, actualList.Length);
            for (int i = 0; i < actualLength; i++)
            {
                Assert.AreEqual(actualList[i + actualLength].Item, actualList[i].Item);
            }
        }

        [Test]
        public static void LinkedListTest_Concatenate_TwoSeparateLists_ShouldAppendAllElementsOfTheSecondListToTheFirst()
        {
            var actualList1 = new LinkedList<int>();
            var actualList2 = new LinkedList<int>();

            var actualLength1 = 2;
            var actualLength2 = 3;

            actualList1.Append(4);
            actualList1.Append(5);
            actualList2.Append(6);
            actualList2.Append(7);
            actualList2.Append(8);

            actualList1 += actualList2;

            var expectedLength = actualList1.Length;

            Assert.AreEqual(expectedLength, actualLength1 + actualLength2);
            for (int i = 0; i < actualLength1; i++)
            {
                Assert.AreEqual(actualList1[i + actualLength2 - 1].Item, actualList2[i].Item);
            }
        }
    }
}
