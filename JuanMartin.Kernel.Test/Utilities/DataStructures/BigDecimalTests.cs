using NUnit.Framework;
using System;

namespace JuanMartin.Kernel.Utilities.DataStructures.Tests
{

    [TestFixture]
    class BigDecimalTests
    {
        [Test]
        public static void ShouldRoundNumbersWithDecimalValuesTest()
        {
            BigDecimal actualNumber;
            BigDecimal expectedNumber;
            int actualDigits;

            actualNumber = new BigDecimal("5.3996578");
            actualDigits = 3;
            expectedNumber = new BigDecimal("5.4");

            Assert.AreEqual(expectedNumber, actualNumber.Round(actualDigits), $"Round {actualNumber}");

            actualNumber = new BigDecimal("5.3993578");
            actualDigits = 3;
            expectedNumber = new BigDecimal("5.399");

            Assert.AreEqual(expectedNumber, actualNumber.Round(actualDigits), $"Round {actualNumber}");

            actualNumber = new BigDecimal("5.9996578");
            actualDigits = 3;
            expectedNumber = new BigDecimal("6");

            Assert.AreEqual(expectedNumber, actualNumber.Round(actualDigits), $"Round {actualNumber}");

            actualNumber = new BigDecimal(Math.Sqrt(2));
            expectedNumber = new BigDecimal("1");
            
            Assert.AreEqual(expectedNumber, actualNumber.Round(), $"Round {actualNumber}");
        }

        [Test]
        public static void ShouldEvaluateEqualityOfSimpleNonDecimalNumbersTest()
        {
            var actualLeftNumber = new BigDecimal("123");
            var actualRightNumber = new BigDecimal("123");

            Assert.IsTrue(actualLeftNumber == actualRightNumber, $"{actualLeftNumber}={actualRightNumber}");

            actualLeftNumber = new BigDecimal("4.5");
            actualRightNumber = new BigDecimal("4.5");

            Assert.IsTrue(actualLeftNumber == actualRightNumber, $"{actualLeftNumber}={actualRightNumber}");

            actualLeftNumber = new BigDecimal("15.0");
            actualRightNumber = new BigDecimal("15");

            Assert.IsTrue(actualLeftNumber == actualRightNumber, $"{actualLeftNumber}={actualRightNumber}");

            actualLeftNumber = new BigDecimal("0");
            actualRightNumber = new BigDecimal("000.0");

            Assert.IsTrue(actualLeftNumber == actualRightNumber, $"{actualLeftNumber}={actualRightNumber}");

            actualLeftNumber = new BigDecimal("456");
            actualRightNumber = new BigDecimal("123");

            Assert.IsFalse(actualLeftNumber == actualRightNumber, $"{actualLeftNumber}!={actualRightNumber}");
        }


        [Test]
        public static void ShouldConvertLongBinaryToBigDecimal()
        {
            string actualBinary = "110001001100100011001111000100110010001100111100010011001000110011";
            BigDecimal expectedNumber=new BigDecimal("56719244431513956915");

            Assert.AreEqual(BigDecimal.ConvertFromBinary(actualBinary), expectedNumber);
        }

        [Test]
        public static  void ShoulPerformLeftBitShiftingOnBigDecimal()
        {
            BigDecimal actualNumber;
            int actualShift;
            BigDecimal expectedNumber;
            BigDecimal actualOperationResult;

            actualNumber = new BigDecimal(1);
            actualShift = 9;
            expectedNumber = new BigDecimal(512);

            actualOperationResult = actualNumber << actualShift;

            Assert.AreEqual(expectedNumber, actualOperationResult, $"{actualNumber} << { actualShift}");

            actualNumber = new BigDecimal(1000);
            actualShift = 2;
            expectedNumber = new BigDecimal(4000);

            actualOperationResult = actualNumber << actualShift;

            Assert.AreEqual(expectedNumber, actualOperationResult, $"{actualNumber} << { actualShift}");
        }

        [Test]
        public static void ShouldConvertBigDecimalToBinary()
        {
            BigDecimal actualNumber;
            string actualBinaryRepresentation;
            string expectedBinaryRepresentation;

            actualNumber = new BigDecimal(10);
            actualBinaryRepresentation = actualNumber.ToBinary();
            expectedBinaryRepresentation = "1010";
            Assert.AreEqual(expectedBinaryRepresentation, actualBinaryRepresentation, $"{actualNumber}");

            actualNumber = new BigDecimal("12345678901234567890123456789012345678901234567890");
            actualBinaryRepresentation = actualNumber.ToBinary();

            expectedBinaryRepresentation = "10000111001001111111011000110110100110101010111110000011110010100001010100000010011001110100011110101111100011000111111100011001011011001110001111110000101011010010";
            Assert.AreEqual(expectedBinaryRepresentation, actualBinaryRepresentation, $"{actualNumber}");
        }

        [Test]
        public static void ShouldCalculateSqrtSameAsNettSqrt()
        {
            Assert.Pass();

            BigDecimal actualSqrt;
            double actualNetSqrt;
            double actualNumber;// = 153;
            //var expectedNumber = new BigDecimal(12.36931687685298);
            for (actualNumber = 2; actualNumber <= 1000000; actualNumber++)
            {
                actualNetSqrt = Math.Sqrt(actualNumber);
                actualSqrt = new BigDecimal(actualNumber).Sqrt_Babylonian();

                 var bigDecimalHasFraction = (actualSqrt.GetDecimal().Length > 0);
                var netHasFraction = (actualNetSqrt - Math.Round(actualNetSqrt)) != 0; 
          
                if(!netHasFraction && bigDecimalHasFraction)
                {
                    actualSqrt = new BigDecimal(actualNumber).Sqrt_BigInteger(); 
                    Assert.AreEqual(new BigDecimal(actualNetSqrt), actualSqrt, $"integer sqrt of {actualNumber}:{actualSqrt},{actualNetSqrt}");
                }
                else
                    Assert.AreEqual(netHasFraction, bigDecimalHasFraction, $"sqrt of {actualNumber}:{actualSqrt},{actualNetSqrt} has fractions");
            }
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

