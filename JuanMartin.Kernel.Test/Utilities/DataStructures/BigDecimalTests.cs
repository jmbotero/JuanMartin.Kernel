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
            var actualLeftNumber = new BigDecimal("123");

            Assert.IsTrue(actualLeftNumber == actualRightNumber, $"{actualLeftNumber}={actualLeftNumber}");

            actualRightNumber = new BigDecimal("123");
            actualLeftNumber = new BigDecimal("456");

            Assert.IsFalse(actualLeftNumber == actualRightNumber, $"{actualLeftNumber}!={actualRightNumber}");
        }

        [Test]
        public static void ShouldEvaluateGreaterThanComparisonOfSimpleNonDecimalNumbersTest()
        {
            var actualRightNumber = new BigDecimal(long.MaxValue - 1);
            var actualLeftNumber = new BigDecimal(long.MaxValue);

            Assert.IsTrue(actualLeftNumber > actualRightNumber, $"{actualLeftNumber}>{actualLeftNumber}");
        }

        [Test]
        public static void ShouldEvaluateLessThanComparisonOfSimpleNonDecimalNumbersTest()
        {
            var actualRightNumber = new BigDecimal(long.MaxValue);
            var actualLeftNumber = new BigDecimal(long.MaxValue - 1);

            Assert.IsTrue(actualLeftNumber < actualRightNumber, $"{actualLeftNumber}<{actualLeftNumber}");
        }
    }
}

