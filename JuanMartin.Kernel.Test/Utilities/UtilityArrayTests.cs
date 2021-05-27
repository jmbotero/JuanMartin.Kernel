using NUnit.Framework;
using JuanMartin.Kernel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JuanMartin.Kernel.Utilities.Tests
{
    [TestFixture()]
    public class UtilityArrayTests
    {
        #region  Binary Search tests
        [Test()]
        public void ShouldReturnNegativeOneWhenSeachingForNonExistingIntegerValue()
        {
            var actualArray = new int[] { 1, 2, 3, 4 };
            var actualItem = 10;
            var expectedIndex = -1;

            Assert.AreEqual(expectedIndex, UtilityArray.BinarySearch<int>(actualArray, actualItem));
        }

        [Test()]
        public void ShouldReturnItsIndexInArrayWhenSearchingForAnExistingStringValue()
        {
            var actualArray = new string[] { "foo1", "foo2", "foo3", "foo4", "foo5" };
            var actualItem = "foo3";
            var expectedIndex = Array.IndexOf(actualArray, actualItem);

            Assert.AreEqual(expectedIndex, UtilityArray.BinarySearch<string>(actualArray, actualItem));
        }
        #endregion
    }
}