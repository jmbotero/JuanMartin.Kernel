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
            var actualLeftNumber = new BigDecimal("123");
            var actualRightNumber = new BigDecimal("123");

            Assert.IsTrue(actualLeftNumber == actualRightNumber, $"{actualLeftNumber}={actualLeftNumber}");

            actualLeftNumber = new BigDecimal("4.5");
            actualRightNumber = new BigDecimal("4.5");

            Assert.IsTrue(actualLeftNumber == actualRightNumber, $"{actualLeftNumber}={actualRightNumber}");
  
            //actualLeftNumber = new BigDecimal("0");
            //actualRightNumber = new BigDecimal("000.0");

            //Assert.IsTrue(actualLeftNumber == actualRightNumber, $"{actualLeftNumber}={actualRightNumber}");

            actualLeftNumber = new BigDecimal("456");
            actualRightNumber = new BigDecimal("123");

            Assert.IsFalse(actualLeftNumber == actualRightNumber, $"{actualLeftNumber}!={actualRightNumber}");
        }

        [Test]
        /// <see cref="https://www.mathgoodies.com/lessons/decimals/compare#:~:text=Decimal%20numbers%20are%20compared%20in%20the%20same%20way,is%20helpful%20to%20write%20one%20below%20the%20other."/>
        public static void ShouldEvaluateGreaterThanComparisonOfDecimalNumbersTest()
        {
            var actualLeftNumber = new BigDecimal("27.18");
            var actualRightNumber = new BigDecimal("2.718");

            Assert.IsTrue(actualLeftNumber > actualRightNumber, $"{actualLeftNumber}>{actualRightNumber}");

            actualLeftNumber = new BigDecimal("4.1");
            actualRightNumber = new BigDecimal("4.01");

            Assert.IsTrue(actualLeftNumber > actualRightNumber, $"{actualLeftNumber}>{actualRightNumber}");

            actualLeftNumber = new BigDecimal("0.17");
            actualRightNumber = new BigDecimal("0.9");

            Assert.IsFalse(actualLeftNumber > actualRightNumber, $"{actualLeftNumber}>{actualRightNumber}");

            actualLeftNumber = new BigDecimal("0.17");
            actualRightNumber = new BigDecimal("0.09");

            Assert.IsTrue(actualLeftNumber > actualRightNumber, $"{actualLeftNumber}>{actualRightNumber}");
        }

        [Test]
        public static void ShouldEvaluateGreaterThanComparisonOfNonDecimalNumbersTest()
        {
            var actualLeftNumber = new BigDecimal(long.MaxValue);
            var actualRightNumber = new BigDecimal(long.MaxValue - 1);

            Assert.IsTrue(actualLeftNumber > actualRightNumber, $"{actualLeftNumber}>{actualRightNumber}");

            actualLeftNumber = new BigDecimal("27");
            actualRightNumber = new BigDecimal("2");

            Assert.IsTrue(actualLeftNumber > actualRightNumber, $"{actualLeftNumber}>{actualRightNumber}");
        }

        [Test]
        public static void ShouldEvaluateLessThanComparisonOfNonDecimalNumbersTest()
        {
            var actualLeftNumber = new BigDecimal(long.MaxValue - 1);
            var actualRightNumber = new BigDecimal(long.MaxValue);

            Assert.IsTrue(actualLeftNumber < actualRightNumber, $"{actualLeftNumber}<{actualRightNumber}");

            actualLeftNumber = new BigDecimal("28");
            actualRightNumber = new BigDecimal("32");

            Assert.IsTrue(actualLeftNumber < actualRightNumber, $"{actualLeftNumber}<{actualRightNumber}");
        }

        [Test]
        /// <see cref="https://www.mathgoodies.com/lessons/decimals/compare#:~:text=Decimal%20numbers%20are%20compared%20in%20the%20same%20way,is%20helpful%20to%20write%20one%20below%20the%20other."/>
        public static void ShouldEvaluateLessThanComparisonOfDecimalNumbersTest()
        {
            var actualLeftNumber = new BigDecimal("0");
            var actualRightNumber = new BigDecimal("0.15");

            Assert.IsTrue(actualLeftNumber < actualRightNumber, $"{actualLeftNumber}<{actualRightNumber}");

            actualLeftNumber = new BigDecimal("0.15");
            actualRightNumber = new BigDecimal("0.3");

            Assert.IsTrue(actualLeftNumber < actualRightNumber, $"{actualLeftNumber}<{actualRightNumber}");

            actualLeftNumber = new BigDecimal("0.549");
            actualRightNumber = new BigDecimal("0.57");

            Assert.IsTrue(actualLeftNumber < actualRightNumber, $"{actualLeftNumber}<{actualRightNumber}");

            actualLeftNumber = new BigDecimal("3.05");
            actualRightNumber = new BigDecimal("3.5");

            Assert.IsTrue(actualLeftNumber < actualRightNumber, $"{actualLeftNumber}<{actualRightNumber}");

            actualLeftNumber = new BigDecimal("5.43");
            actualRightNumber = new BigDecimal("5.45");

            Assert.IsTrue(actualLeftNumber < actualRightNumber, $"{actualLeftNumber}<{actualRightNumber}");

            actualLeftNumber = new BigDecimal("5.5");
            actualRightNumber = new BigDecimal("6");

            Assert.IsTrue(actualLeftNumber < actualRightNumber, $"{actualLeftNumber}<{actualRightNumber}");
        }

        [Test]
        /// <see cref=""/>
        public static void ShouldEvaluateLessThanComparisonOfNegativeDecimalNumbersTest()
        {
            var actualLeftNumber = new BigDecimal("28");
            var actualRightNumber = new BigDecimal("32");

            Assert.IsTrue(actualLeftNumber < actualRightNumber, $"{actualLeftNumber}<{actualRightNumber}");

            actualLeftNumber = new BigDecimal("-28");
            actualRightNumber = new BigDecimal("32");

            Assert.IsTrue(actualLeftNumber < actualRightNumber, $"{actualLeftNumber}<{actualRightNumber}");

            actualLeftNumber = new BigDecimal("28");
            actualRightNumber = new BigDecimal("-32");

            Assert.IsTrue(actualLeftNumber > actualRightNumber, $"{actualLeftNumber}>{actualRightNumber}");

            actualLeftNumber = new BigDecimal("-28");
            actualRightNumber = new BigDecimal("-32");

            Assert.IsTrue(actualLeftNumber > actualRightNumber, $"{actualLeftNumber}>{actualRightNumber}");

            actualLeftNumber = new BigDecimal("5.5");
            actualRightNumber = new BigDecimal("6");

            Assert.IsTrue(actualLeftNumber < actualRightNumber, $"{actualLeftNumber}<{actualRightNumber}");

            actualLeftNumber = new BigDecimal("-5.5");
            actualRightNumber = new BigDecimal("6");

            Assert.IsTrue(actualLeftNumber < actualRightNumber, $"{actualLeftNumber}<{actualRightNumber}");

            actualLeftNumber = new BigDecimal("5.5");
            actualRightNumber = new BigDecimal("-6");

            Assert.IsTrue(actualLeftNumber > actualRightNumber, $"{actualLeftNumber}>{actualRightNumber}");

            actualLeftNumber = new BigDecimal("-5.5");
            actualRightNumber = new BigDecimal("-6");

            Assert.IsTrue(actualLeftNumber > actualRightNumber, $"{actualLeftNumber}>{actualRightNumber}");
        }
    }
}

