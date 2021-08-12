using JuanMartin.Kernel.Utilities.DataStructures;
using NUnit.Framework;
using System;

namespace JuanMartin.Kernel.Utilities.DataStructures.Tests
{

    [TestFixture]
    class BigDecimalTests
    {
        [Test]
        public static void ShouldEvaluateEqualityOfSimpleNonDecimalNumbersTest()
        {
            var actualRightNumber = new BigDecimal("123");
            var actualLeftNumber = new BigDecimal("456");

            Assert.IsTrue(actualLeftNumber == actualLeftNumber, $"{actualLeftNumber}={actualLeftNumber}");
            Assert.IsFalse(actualLeftNumber == actualRightNumber, $"{actualLeftNumber}!={actualRightNumber}");
        }
    }
}

