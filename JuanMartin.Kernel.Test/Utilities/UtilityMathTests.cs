using NUnit.Framework;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Numerics;

namespace JuanMartin.Kernel.Utilities.Tests
{
    [TestFixture]
    public class UtilityMathTests
    {
        #region SqrtBySubstraction tests
        [Test]
        public void ShouldBeEqualToOriginalNumberTheSquareRootResultMultipliedByItself()
        {
            for (var actualNumber = 0.5; actualNumber < 10; actualNumber += 0.5)
            {
                var actualSqrt = UtilityMath.GetSqrtBySubstraction(actualNumber, 5);
                var expectedNumber = Math.Round(actualSqrt * actualSqrt, 3);

                Assert.AreEqual(expectedNumber, actualNumber);
            }
        }
        #endregion

        #region GetNumericPatterns tests
        [Test]
        public void ShouldHaveOneGroupPatternForAtLeastTwoDigitsRepeatingInNumberOnlyString()
        {
            var masks = UtilityMath.GetNumericPatterns(123426);
            Assert.IsTrue(masks.Where(m => Regex.Matches(m, @"\\1").Count == 1).Count() == 1);

            var masks2 = UtilityMath.GetNumericPatterns(223426);
            Assert.IsFalse(masks2.Where(m => Regex.Matches(m, @"\\1").Count == 1).Count() == 1);

        }

        [Test]
        public void ShouldReturnOneValidRegexPatternForEverySetofRepeatedDigitsInNumberOnlyString()
        {
            var masks = UtilityMath.GetNumericPatterns(121313);

            Assert.AreEqual(2, masks.Count);
            Assert.IsTrue(masks.Contains(@"\d\d\d(\d)\d\1"));
            Assert.IsTrue(masks.Contains(@"(\d)\d\1\d\1\d"));
        }
        #endregion

        #region GetPeriodicalSequence tests
        [Test()]
        public void ShouldRetunEmptyStringOPeriodicalStrigOfNumbersInDecimal()
        {
            // receive string of decimals as array
            var numbers = UtilityMath.GetDecimalList(1, 3, 2000).ToArray();
            var expectedSequence = "3";
            var actualSequence = UtilityMath.GetPeriodicalSequence(numbers);

            Assert.AreEqual(expectedSequence, actualSequence);

            numbers = UtilityMath.GetDecimalList(7, 12, 2000).ToArray();
            expectedSequence = "3";
            actualSequence = UtilityMath.GetPeriodicalSequence(numbers);

            Assert.AreEqual(expectedSequence, actualSequence);

            numbers = UtilityMath.GetDecimalList(1, 6, 2000).ToArray();
            expectedSequence = "6";
            actualSequence = UtilityMath.GetPeriodicalSequence(numbers);

            Assert.AreEqual(expectedSequence, actualSequence);

            numbers = UtilityMath.GetDecimalList(1, 81, 2000).ToArray();
            expectedSequence = "012345679";
            actualSequence = UtilityMath.GetPeriodicalSequence(numbers);

            Assert.AreEqual(expectedSequence, actualSequence);

            numbers = UtilityMath.GetDecimalList(1, 893, 2000).ToArray();
            expectedSequence = "001119820828667413213885778275475923852183650615901455767077267637178051511758118701007838745800671892497200447928331466965285554311310190369540873460246360582306830907054871220604703247480403135498320268756998880179171332586786114221724524076147816349384098544232922732362821948488241881298992161254199328107502799552071668533034714445688689809630459126539753639417693169092945128779395296752519596864501679731243";
            actualSequence = UtilityMath.GetPeriodicalSequence(numbers);

            Assert.AreEqual(expectedSequence, actualSequence);

        }
        #endregion

        #region ProductSum tests
        [Test()]
        public void NumberFourShouldHaveOneTwoDigitProductSums()
        {
            var p = UtilityMath.GetProductSums(4, 2);
            Assert.AreEqual(1, p.Count);
        }
        #endregion

        #region Sqrt Execution Measure consumers
        [Test()]
        public void ShouldBeTheFastestMethodToCalCulateTheSquareRootTheNativeDotNetVersusTheBabylonianOrNewtonian()
        {
            void DotNetSquareRootLoop(int n)
            {
                var f = Enumerable.Range(1, n).Select(i => Math.Sqrt(i)).ToList();
            }
            void BabylonianSquareRootLoop(int n)
            {
                var f = Enumerable.Range(1, n).Select(i => UtilityMath.GetSqrtBabylonianMethod(i)).ToList();
            }
            void NewtonSquareRootLoop(int n)
            {
                var f = Enumerable.Range(1, n).Select(i => UtilityMath.GetSqrtUsingNewtonMethod(i)).ToList();
            }
            var count = 10000;
            var actualMethodDotNetDuration = UtilityHelper.Measure(() => DotNetSquareRootLoop(count));
            var actualMethodOneDuration = UtilityHelper.Measure(() => BabylonianSquareRootLoop(count));
            var actualMethodTwoDuration = UtilityHelper.Measure(() => NewtonSquareRootLoop(count));
            Assert.Less(actualMethodDotNetDuration, actualMethodOneDuration,  "Babylonian is faster  than .Net Sqrt.");
            Assert.Less(actualMethodDotNetDuration, actualMethodTwoDuration,  "Newton  method is faster  than .Net Sqrt.");
        }

        #endregion

        #region Prime factors tests
        [Test()]
        public void ShouldHaveTwoPrimeFactorsForOneThousand()
        {
            var actualFactors = UtilityMath.GetPrimeFactors(1000, false, true);
            var expectedFactorsCount = 2;
            UtilityHelper.MeasureInLoop(loopCount: 1000, seed: 2, (x) => UtilityMath.GetPrimeFactors(x, false, true), true);
            Assert.AreEqual(expectedFactorsCount, actualFactors.Count());
        }

        [Test()]
        public void ShouldHaveNoPrimeFactorsSetOfPrimeNumbers()
        {
            var expectedFactorsCount = 0;
            var primes = UtilityMath.ErathostenesSieve(1000);

            foreach (var p in primes)
            {
                var actualFactors = UtilityMath.GetPrimeFactors(p, false, true);
                Assert.AreEqual(expectedFactorsCount, actualFactors.Count());
            }
        }

        [Test()]
        public void GetCombinationsExecutionDurationTest()
        {
            void CombLoop(int n, int d)
            {
                var f = Enumerable.Range(2, n).Select(i => UtilityMath.GetCombinationsOfK(Enumerable.Range(1, i).ToArray(), d)).ToList();
            }
            void NCombLoop(int n, int d)
            {
                var f = Enumerable.Range(2, n).Select(i => UtilityMath.GetCombinationsOfKRecursive(Enumerable.Range(1, i), d)).ToList();
            }

            var s = 6;
            var count = 12000;
            var actualMethodOneDuration = UtilityHelper.Measure(() => NCombLoop(count, s));
            var actualMethodTwoDuration = UtilityHelper.Measure(() => CombLoop(count, s));

            Console.WriteLine($"Plain: {actualMethodOneDuration}, sorted: {actualMethodOneDuration}");
        }

        #endregion

        #region SquareDigit tests
        [Test()]
        public void ShouldBeOneTheSquareDigitsOfOne()
        {
            var expectedSum = 1;
            var actualSum = UtilityMath.SquareDigits(1);

            Assert.AreEqual(expectedSum, actualSum);
        }

        [Test()]
        public void ShouldBeGreaterThanZeroAnySquareDigitsUptoOneHundredThousand()
        {
            int limit = 10000;

            for (int number = 2; number <= limit; number++)
            {
                var actualSum = UtilityMath.SquareDigits(number);

                Assert.Greater(actualSum, 0);
            }
        }

        [Test()]
        public void ShouldBeTwentyNineSquareDigitsOfTwentyFive()
        {
            var expectedSum = 29;
            var actualSum = UtilityMath.SquareDigits(25);

            Assert.AreEqual(expectedSum, actualSum);
        }

        [Test()]
        public void ShouldHaveOneElementTheSquareDigitsChainOfOne()
        {
            var (_, actualChain) = UtilityMath.SquareDigitsChain(1);
            var expectedLength = 1;


            Assert.AreEqual(expectedLength, actualChain.Count());
        }

        [Test()]
        public void SShouldHaveEightElementsTheSquareDigitsChainOfEightyNine()
        {
            var actualNumber = 89;
            var (actualTerminator, actualChain ) = UtilityMath.SquareDigitsChain(89);
            var expectedLength = 8;

            // check duration
            UtilityHelper.MeasureInLoop(loopCount: 10, actualNumber, (x) => UtilityMath.SquareDigitsChain(x), true);
            Assert.AreEqual(expectedLength, actualChain.Count());
        }

        [Test()]
        public void ShouldHaveAsTerminatorOneTheSquareDigitsChainOfFourtyFour()
        {
            var (actualTerminator, _) = UtilityMath.SquareDigitsChain(44);
            var expectedTerminator = 1;

            Assert.AreEqual(expectedTerminator, actualTerminator);
        }

        [Test()]
        public void ShouldHaveAsTerminatorEightyNineTheSquareDigitsChainOfOneHundredFortyFive()
        {
            var (actualTerminator, _) = UtilityMath.SquareDigitsChain(145);
            var expectedTerminator = 89;

            Assert.AreEqual(expectedTerminator, actualTerminator);
        }

        [Test()]
        public void ShouldHaveAsTerminatorEightyNineTheSquareDigitsChainOfEightyFive()
        {
            var (actualTerminator, _) = UtilityMath.SquareDigitsChain(85);
            var expectedTerminator = 89;

            Assert.AreEqual(expectedTerminator, actualTerminator);
        }
        #endregion

        #region  Fibonacci tests
        [Test()]
        public void ShoulReturnValidResponseTheFibonacciLoopImplementationOfNumberOneHundred()
        {
            /// <seealso cref="http://www.maths.surrey.ac.uk/hosted-sites/R.Knott/Fibonacci/fibtable.html"/>
            var expectedFibonacciAsString = BigInteger.Parse("354224848179261915075");
            var actualFibonacciAsString = UtilityMath.FibonacciLoop(100);
            Assert.AreEqual(expectedFibonacciAsString,actualFibonacciAsString);
        }
        #endregion
    }
}