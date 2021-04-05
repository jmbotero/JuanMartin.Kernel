using JuanMartin.Kernel.Utilities;
using NUnit.Framework;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace JuanMartin.Kernel.Utilities.Tests
{
    [TestFixture]
    public class UtilityMathTests
    {
        #region Sqrt_By_Substraction tests
        [Test]
        public void SqrtBySubstraction_SqrtResultMultipliedByItselfShouldBeEqualToOriginalNumber()
        {
            for (var expected_number = 0.5; expected_number < 10; expected_number += 0.5)
            {
                var actual_sqrt = UtilityMath.Sqrt_By_Substraction(expected_number, 5);
                var actual_number = Math.Round(actual_sqrt * actual_sqrt, 3);

                Assert.AreEqual(expected_number, actual_number);
            }
        }
        #endregion

        #region GetNumericPatterns tests
        [Test]
        public void NumberShouldHaveOnePatternForAtLeastTwoDigitsRepeatinng()
        {
            var masks = UtilityMath.GetNumericPatterns(123426);
            Assert.IsTrue(masks.Where(m => Regex.Matches(m, @"\\1").Count == 1).Count() == 1);

            var masks2 = UtilityMath.GetNumericPatterns(223426);
            Assert.IsFalse(masks2.Where(m => Regex.Matches(m, @"\\1").Count == 1).Count() == 1);

        }

        [Test]
        public void NumberShouldReturnOneValidRegexPatternForEverySetofRepeatedDigits()
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
            var expected_sequence = "3";
            var actual_sequence = UtilityMath.GetPeriodicalSequence(numbers);

            Assert.AreEqual(expected_sequence, actual_sequence);

            numbers = UtilityMath.GetDecimalList(7, 12, 2000).ToArray();
            expected_sequence = "3";
            actual_sequence = UtilityMath.GetPeriodicalSequence(numbers);

            Assert.AreEqual(expected_sequence, actual_sequence);

            numbers = UtilityMath.GetDecimalList(1, 6, 2000).ToArray();
            expected_sequence = "6";
            actual_sequence = UtilityMath.GetPeriodicalSequence(numbers);

            Assert.AreEqual(expected_sequence, actual_sequence);

            numbers = UtilityMath.GetDecimalList(1, 81, 2000).ToArray();
            expected_sequence = "012345679";
            actual_sequence = UtilityMath.GetPeriodicalSequence(numbers);

            Assert.AreEqual(expected_sequence, actual_sequence);

            numbers = UtilityMath.GetDecimalList(1, 893, 2000).ToArray();
            expected_sequence = "001119820828667413213885778275475923852183650615901455767077267637178051511758118701007838745800671892497200447928331466965285554311310190369540873460246360582306830907054871220604703247480403135498320268756998880179171332586786114221724524076147816349384098544232922732362821948488241881298992161254199328107502799552071668533034714445688689809630459126539753639417693169092945128779395296752519596864501679731243";
            actual_sequence = UtilityMath.GetPeriodicalSequence(numbers);

            Assert.AreEqual(expected_sequence, actual_sequence);

        }
        #endregion

        #region Square Root tests
        [Test()]
        public void DotNetBabylonianSquareRootMethodShouldBeTheFastest()
        {
            void DotNetSquareRootLoop(int n)
            {
                var f = Enumerable.Range(1, n).Select(i => Math.Sqrt(i)).ToList();
            }
            void BabylonianSquareRootLoop(int n)
            {
                var f = Enumerable.Range(1, n).Select(i => UtilityMath.Sqrt_Babylonian(i)).ToList();
            }
            void NewtonSquareRootLoop(int n)
            {
                var f = Enumerable.Range(1, n).Select(i => UtilityMath.Sqrt_Newton(i)).ToList();
            }
            var count = 10000;
            var actualMethodDotNetDuration = UtilityHelper.Measure(() => DotNetSquareRootLoop(count));
            var actualMethodOneDuration = UtilityHelper.Measure(() => BabylonianSquareRootLoop(count));
            var actualMethodTwoDuration = UtilityHelper.Measure(() => NewtonSquareRootLoop(count));
            Assert.Less(actualMethodDotNetDuration, actualMethodOneDuration);
            Assert.Less(actualMethodDotNetDuration, actualMethodTwoDuration);
        }

        [Test()]
        public void ManyRunsOfGetPrimeFactors_ShouldTakeAtMost50Miliseconds()
        {
            void FactorPrimeLoop(int n)
            {
                var f = Enumerable.Range(1, n).Select(i => UtilityMath.GetPrimeFactors(i, false, true)).ToList();
            }

            var actualDuration = UtilityHelper.Measure(() => FactorPrimeLoop(100000));
            var expectedDuration = 70;

            Assert.LessOrEqual(actualDuration, expectedDuration);
        }

        #endregion
        [Test()]
        public void NumberFourShouldHaveOneTwoDigitProductSums()
        {
#pragma warning disable CS8321 // Local function is declared but never used
            void print(string operation, List<IEnumerable<int>> info)
#pragma warning restore CS8321 // Local function is declared but never used
            {
                foreach (var l in info)
                {
                    Console.WriteLine(string.Join(operation, l.ToArray()));
                }
            };

            var p = UtilityMath.GetProductSums(4, 2);
            //print("+", p);
            Assert.AreEqual(1, p.Count);
        }

        [Test()]
        public void GetCombinationsConsumer()
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
    }
}