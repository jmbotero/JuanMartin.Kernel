using NUnit.Framework;
using JuanMartin.Kernel.Extesions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JuanMartin.Kernel.Utilities.DataStructures;

namespace JuanMartin.Kernel.Extesions.Tests
{
    [TestFixture()]
    public class CollectionExtensionsTests
    {
        [Test()]
        public void ShouldRemoveSingleElements()
        {
            var actualArray = new int[] { 1, 2, 3, 4, 5 };
            var expectedArray = new int[] { 1, 2, 4, 5 };

            var actualLength = actualArray.Length;
            var expectedLenth = actualLength - 1;            
            var modifiedArray = CollectionExtensions.Remove(actualArray, 3);

            Assert.AreEqual(expectedArray, modifiedArray);
            Assert.AreEqual(expectedLenth, modifiedArray.Length);
        }

        [Test]
        public static void ShouldReindexElementsAfterRemovedElement()
        {
            var actualAray = new String[] { "foo1", "foo2", "foo3" };
            var actualItem = "foo3";
            var removeItem = "foo2";
            var staticItem = "foo1";
            var removeIndex = Array.IndexOf<String>(actualAray, removeItem);

            var expectedArray = CollectionExtensions.Remove<String>(actualAray, removeItem);

            Assert.AreNotEqual(removeItem,expectedArray[removeIndex],"Removed elements index is reused.");
            Assert.AreEqual(-1, Array.IndexOf<String>(expectedArray, removeItem),"Removed element is not indexed anymore.");
            Assert.AreEqual(Array.IndexOf<String>(expectedArray, staticItem), Array.IndexOf<String>(actualAray, staticItem), "Element before removed element kept index.");
            Assert.AreNotEqual(Array.IndexOf<String>(expectedArray, actualItem), Array.IndexOf<String>(actualAray, actualItem), "Element after removed element is reindexed.");
        }

        [Test]
        public static void ShouldDetermineIndexIsNegativeOneWhenRemovingByIndex()
        {
            var actualArray = new String[] { "foo1", "foo2", "foo3" };
            var actualItem = "foo1";
            var actualIndex = 0;
            var expectedIndex = -1;

            actualArray = CollectionExtensions.RemoveAt<String>(actualArray, actualIndex);
            Assert.AreEqual(expectedIndex, Array.IndexOf<String>(actualArray, actualItem));
            Assert.IsFalse(actualArray.Contains(actualItem));
        }

        [Test()]
        public void ShouldMultiplyAllItemsInCollectionWhenApplyingMultiplicationExtension()
        {
            var actualList = new List<int> { 2, 3, 4, 5 };
            var expectedMultiplication = 120;

            Assert.AreEqual(expectedMultiplication, actualList.Multiply());
        }

        [Test()]
        public void ShouldMultiplyOnlyPropertiesIndicatedByPropertyInPredicate()
        {
            var vertices = new List<Vertex<int>>();

            foreach (var i in new int[] {1, 2, 3, 4, 5 })
            {
                vertices.Add(new Vertex<int>(i));
            }

            var actualCollectionOfObjects = new DirectedAcyclicGraph<int>(vertices);
            var expectedMultiplicationOfValues = 120;
            var expectedMultiplicationOfNames = 120;

            Assert.AreEqual(expectedMultiplicationOfNames, actualCollectionOfObjects.Vertices.Multiply(v => Convert.ToInt32(v.Name)), "Multiplication of Vertex String names converted to integers");
            Assert.AreEqual(expectedMultiplicationOfValues, actualCollectionOfObjects.Vertices.Multiply(v => v.Value), "Multiplication of Vertex Integer values");
        }

        [Test()]
        public void ShouldReturnZeroWithMultiplicationExtensionIfCollectionIsmpty()
        {
            var source = new List<int> { };
            var expectedMultiplication = 0;

            Assert.AreEqual(expectedMultiplication, source.Multiply());
        }

        [Test()]
        public void ShouldRaiseExeptionWhhenTryingToMultiplyStringCollection()
        {
            var actualArray = new String[] { "foo1", "foo2", "foo3" };

            Assert.Throws<ArgumentException>(()=> actualArray.Multiply<string>());
        }
    }
}