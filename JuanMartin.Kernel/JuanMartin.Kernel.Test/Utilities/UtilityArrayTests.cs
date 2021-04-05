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
        [Test()]
        public void BinarySearchOfNonExistingIntegerValue_ShouldReturnNegativeOne()
        {
            var actual_array = new int[] { 1, 2, 3, 4 };
            var actual_item = 10;
            var expected_index = -1;

            Assert.AreEqual(expected_index, UtilityArray.BinarySearch<int>(actual_array, actual_item));
        }

        [Test()]
        public void BinarySearchOfExistingStringValue_ShouldReturnItsIndexInArrsy()
        {
            var actual_array = new string[] { "foo1", "foo2", "foo3", "foo4", "foo5" };
            var actual_item = "foo3";
            var expected_index = 2;

            Assert.AreEqual(expected_index, UtilityArray.BinarySearch<string>(actual_array, actual_item));
        }
    }
}