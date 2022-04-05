using JuanMartin.Kernel.Utilities;
using NUnit.Framework;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Numerics;
using System.Collections.Generic;
using JuanMartin.Kernel.Extesions;                                                                                                                                                                                                                                                                                                                                                                                                  
using JuanMartin.Kernel.Utilities.DataStructures;

namespace JuanMartin.Kernel.Utilities.Tests
{
    [TestFixture]
    public class UtilityMathTests
    {
        [TestFixture]
        public class EvaluateSimpleArithmeticOerationsTests
        {
            [Test()]
            public void ShouldEvaluateSuccessfullyAllSipmpleArithmeticExpressionsWithFourSequentialDigitsAndFourOperandsOfTheFormllaobloclodOrlaoblolcodl()
            {
                // get all expressions
                var operandCombinations = UtilityMath.GetCombinationsOfK<int>(Enumerable.Range(1, 9).ToArray(), 4).ToArray();
                var operatorPermutationsWithRepetition = UtilityMath.GeneratePermutationsOfK<string>(new string[] { "+", "-", "*", "/" }, 3, true).ToArray();
                var expressions = new HashSet<string>();
                var expectedValue = double.NegativeInfinity;

                foreach (var operandEnumerable in operandCombinations)
                {
                    if (UtilityMath.ItemsArePositiveSequential(operandEnumerable))
                    {
                        var operands = string.Join("", operandEnumerable);
                        var operandPermutations = UtilityMath.GeneratePermutationsOfK<int>(operandEnumerable.ToArray(), 4);

                        foreach (var operandArray in operandPermutations)
                        {
                            var a = operandArray[0];
                            var b = operandArray[1];
                            var c = operandArray[2];
                            var d = operandArray[3];

                            foreach (var operatorSet in operatorPermutationsWithRepetition)
                            {
                                var o1 = operatorSet[0];
                                var o2 = operatorSet[1];
                                var o3 = operatorSet[2];
                                expressions.Add($"(({a}{o1}{b}){o2}{c}){o3}{d}");
                                expressions.Add($"({a}{o1}{b}){o2}({c}{o3}{d})");
                            }
                        }
                    }
                }

                foreach (var expression in expressions)
                {
                    var actualValue = UtilityMath.EvaluateSimpleArithmeticOerations(expression, 0);
                    Assert.Greater(actualValue, expectedValue, $"Successful evaluation of {expression}.");
                }

            }

            [Test()]
            public void ShouldHandleNegativeOperands()
            {
                var actualExpression = "(2-5)*(3-9)";
                var actualValue = UtilityMath.EvaluateSimpleArithmeticOerations(actualExpression);
                var expectedValue = 18;

                Assert.AreEqual(expectedValue, actualValue, "Negative operations.");

                actualExpression = "(-2-5)*(3--9)";
                actualValue = UtilityMath.EvaluateSimpleArithmeticOerations(actualExpression);
                expectedValue = -84;

                Assert.AreEqual(expectedValue, actualValue, "Negative operands.");
            }

            [Test()]
            public void ShouldTreatArithmeticOperationsWithLessThanOneDivisionsWhenNumberOfDecimalsAllowedForParsingIsZeroAsZero()
            {
                //int Evaluate(string expr, ExpressionEvaluator evaluator)
                //{
                //    evaluator.Parse(expr);
                //    Symbol eval = evaluator.Evaluate(new Dictionary<string, Symbol>());

                //    double result = (double)eval.Value.Result;
                //    if (!double.IsInfinity(result) && result == Math.Truncate(result) && result > 0) // is positive integer
                //        return (int)result;
                //    else
                //        return -1;
                //}
                //int v = Evaluate(actualExpression, new ExpressionEvaluator());
                var actualExpression = "(2-1)/(3*4)";
                double actualValue = UtilityMath.EvaluateSimpleArithmeticOerations(actualExpression, 0);
                double expectedValue = 0;

                Assert.AreEqual(expectedValue, actualValue, "Small number operations.");
            }

            [Test()]
            public void ShouldHandleOperationsAsNonIntegerArithmetic()
            {
                //int Evaluate(string expr, ExpressionEvaluator evaluator)
                var actualExpression = "(1.55+2.45)/(3/4)";
                double actualValue = UtilityMath.EvaluateSimpleArithmeticOerations(actualExpression);
                double expectedValue = 5.3333;

                Assert.AreEqual(expectedValue, actualValue, "Non-integer operations.");

                actualExpression = "((1-2)/3)+4";
                actualValue = UtilityMath.EvaluateSimpleArithmeticOerations(actualExpression);
                expectedValue = 3.6667;

                Assert.AreEqual(expectedValue, actualValue, "Non-integer negative operations.");
            }

            [Test]
            public static void ShouldThrowDivideByZeroExceptionIfRightHandOperatorOfDivisionIsZero()
            {
                var actualExpression = "(1+2)/(0*3)";

                var actualOperationException = Assert.Throws<DivideByZeroException>(() => UtilityMath.EvaluateSimpleArithmeticOerations(actualExpression));
                Assert.IsTrue(actualOperationException.Message.Contains($"Division by zero in"));
            }

            [Test]
            public static void ShouldThrowArgumentExceptionWhenExpressionHasUnmatchingParenthesesSets()
            {
                var actualExpression = "((1+2)+(3+4)";

                var actualOperationException = Assert.Throws<ArgumentException>(() => UtilityMath.EvaluateSimpleArithmeticOerations(actualExpression), "Incorrect ((");
                Assert.IsTrue(actualOperationException.Message.Contains($"is not a valid expression"), "Incorrect ((");

                actualExpression = "((1+2)*3))+4";

                actualOperationException = Assert.Throws<ArgumentException>(() => UtilityMath.EvaluateSimpleArithmeticOerations(actualExpression), "Incorrect ((");
                Assert.IsTrue(actualOperationException.Message.Contains($"is not a valid expression"), "Incorrect ))");

                actualExpression = "((1+2)*3+4";

                actualOperationException = Assert.Throws<ArgumentException>(() => UtilityMath.EvaluateSimpleArithmeticOerations(actualExpression), "Incorrect missing ).");
                Assert.IsTrue(actualOperationException.Message.Contains($"is not a valid expression"), "Incorrect missing )");
            }

            [Test]
            public static void ShouldThrowArgumentExceptionWhenExpressionHasNotSupportedOperators()
            {
                var actualExpression = "(1+*2)+(3+4)";

                var actualOperationException = Assert.Throws<ArgumentException>(() => UtilityMath.EvaluateSimpleArithmeticOerations(actualExpression));
                Assert.IsTrue(actualOperationException.Message.Contains($"is not a valid expression"));

                actualExpression = "((1#2)*3)+4)";

                actualOperationException = Assert.Throws<ArgumentException>(() => UtilityMath.EvaluateSimpleArithmeticOerations(actualExpression));
                Assert.IsTrue(actualOperationException.Message.Contains($"is not a valid expression"));
            }
        }

        [TestFixture]
        public class GeneratePermutationsOfKTests
        {
            [Test()]
            public void ShouldReturnSelectionsOfAllIntegerMembersInCollectionInLinearOrder()
            {
                var actualColection = Enumerable.Range(1, 2).ToArray();
                var actualK = 2;
                var actualPermutation = UtilityMath.GeneratePermutationsOfK<int>(actualColection, actualK);
                var expectedPermutation = new int[][]
                {
                new int[] {1,2 },
                new int[] {2,1 }
                };

                Assert.AreEqual(expectedPermutation, actualPermutation);
            }

            [Test()]
            public void ShouldReturnSelectionsOfAllStingMembersInCollectionInLinearOrder()
            {
                var actualColection = new string[] { "A", "B" };
                var actualK = 2;
                var actualPermutation = UtilityMath.GeneratePermutationsOfK<string>(actualColection, actualK);
                var expectedPermutation = new string[][]
                {
                new string[] {"A", "B" },
                new string[] {"B", "A" }
                };

                Assert.AreEqual(expectedPermutation, actualPermutation);
            }

            [Test()]
            public void ShouldReturnSelectionsOfAllStringMembersInCollectionIncludingRepetitionsInLinearOrderInAscendingOrder()
            {
                var actualColection = new string[] { "A", "B" };
                var actualK = 2;
                var actualPermutation = UtilityMath.GeneratePermutationsOfK<string>(actualColection, actualK, true);
                var expectedPermutation = new string[][]
                {
                new string[] {"A", "A" },
                new string[] {"A", "B" },
                new string[] {"B", "A" },
                new string[] {"B", "B" }
                };

                Assert.AreEqual(expectedPermutation, actualPermutation);
            }

            [Test()]
            public void ShouldReturnSelectionsOfOnlyKChoiceIntegerMembersInCollectionInLinearOrder()
            {
                var actualColection = Enumerable.Range(1, 3).ToArray();
                var actualK = 2;
                var actualPermutation = UtilityMath.GeneratePermutationsOfK<int>(actualColection, actualK);
                var expectedPermutation = new int[][]
                {
                new int[] {1,2 },
                new int[] {1,3 },
                new int[] {2,1 },
                new int[] {2,3 },
                new int[] {3,1 },
                new int[] {3,2 }
                };

                Assert.AreEqual(expectedPermutation, actualPermutation);
            }

            [Test()]
            public void ShouldReturnSelectionsOfOnlyKChoiceStringMembersInCollectionInLinearOrder()
            {
                var actualColection = new string[] { "A", "B", "C" };
                var actualK = 2;
                var actualPermutation = UtilityMath.GeneratePermutationsOfK<string>(actualColection, actualK);
                var expectedPermutation = new string[][]
                {
                new string[] {"A","B" },
                new string[] {"A","C" },
                new string[] {"B","A" },
                new string[] {"B","C" },
                new string[] {"C","A" },
                new string[] {"C","B" }
                };

                Assert.AreEqual(expectedPermutation, actualPermutation);
            }
        }

        [TestFixture]
        public class SqrtBySubstractionTests
        {
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
        }

        [TestFixture]
        public class GetNumericPatternsTests
        {

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
        }

        [TestFixture]
        public class GetPeriodicalSequenceTests
        {
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
        }

        [TestFixture]
        public class ProductSumTests
        {
            [Test()]
            public void ShouldHaveOneTwoDigitProductSumsIfNumberIsFour()
            {
                var p = UtilityMath.GetProductSums(4, 2);
                Assert.AreEqual(1, p.Count);
            }
        }

        [TestFixture]
        public class SquareRootDurationTests
        {
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
                Assert.LessOrEqual(actualMethodDotNetDuration, actualMethodOneDuration, "Babylonian is faster  than .Net Sqrt.");
                Assert.LessOrEqual(actualMethodDotNetDuration, actualMethodTwoDuration, "Newton  method is faster  than .Net Sqrt.");
            }
        }

        [TestFixture]
        public class PrimeFactorsTests
        {
            [Test()]
            public void ShouldHaveTwoPrimeFactorsForTenThousand()
            {
                var actualNumber = 1000;
                var actualFactors = UtilityMath.GetPrimeFactors(actualNumber, false, false);
                var actualFactorCount = actualFactors.Count();
                var expectedFactorCount = 2;

                UtilityHelper.MeasureInLoop(loopCount: 1000, seed: 2, (x) => UtilityMath.GetPrimeFactors(x, false, true), true);
                Assert.AreEqual(expectedFactorCount, actualFactorCount, $"{actualNumber} has  [{string.Join(",", actualFactors)}] factors.");
            }

            [Test()]
            public void ShouldHaveNoPrimeFactorsSetOfPrimeNumbers()
            {
                var actualNumber = 10000;
                var expectedFactorCount = 0;
                var primes = UtilityMath.ErathostenesSieve(actualNumber);

                foreach (var actualPrime in primes)
                {
                    var actualFactors = UtilityMath.GetPrimeFactors(actualPrime, false, false);
                    var actualFactorCount = actualFactors.Count();
                    Assert.AreEqual(expectedFactorCount, actualFactorCount, $"{actualPrime} has  [{string.Join(",", actualFactors)}] as factors.");
                }
            }
        }

        [TestFixture]
        public class GetCombinationsOfKTests
        {
            [Test()]
            public void ShouldReturOneCobinationWhenChoosingSameNumberOfElementsAsNumberOfDistinctObjectsInSet()
            {
                var actualCollection = Enumerable.Range(1, 3).ToArray();
                var actualCombination = UtilityMath.GetCombinationsOfK<int>(actualCollection, actualCollection.Length).ToArray();
                var expectedCombination = new int[][]
                {
                new int[] {1,2,3}
                };

                Assert.AreEqual(expectedCombination, actualCombination);
            }

            [Test()]
            public void ShouldReturnUniqueAndDisregardingOrderCombinationsInIncreasingOrderOfOneLessCountThanOriginalCollection()
            {
                var actualCollection = Enumerable.Range(1, 3).ToArray();
                var actualK = actualCollection.Length - 1;
                var actualCombination = UtilityMath.GetCombinationsOfK<int>(actualCollection, actualK).ToArray();
                var expectedCombination = new int[][]
                {
                new int[] { 1,2 },
                new int[] { 1,3 },
                new int[] { 2,3 }
                };

                Assert.AreEqual(expectedCombination, actualCombination);
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
        }

        [TestFixture]
        public class SquareDigitsTests
        {
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
            public void ShouldHaveEightElementsTheSquareDigitsChainOfEightyNine()
            {
                var actualNumber = 89;
                var (actualTerminator, actualChain) = UtilityMath.SquareDigitsChain(89);
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
        }

        [TestFixture]
        public class FibonacciLoopTests
        {
            [Test()]
            public void ShoulReturnValidResponseTheFibonacciLoopImplementationOfNumberOneHundred()
            {
                /// <seealso cref="http://www.maths.surrey.ac.uk/hosted-sites/R.Knott/Fibonacci/fibtable.html"/>
                string actualNumber = "354224848179261915075";

                var expectedLargeFibonacci = BigInteger.Parse(actualNumber);

                var actualLargeFibonacci = UtilityMath.FibonacciLoop(100);
                Assert.AreEqual(expectedLargeFibonacci, actualLargeFibonacci);
            }

            [Test()]
            public void ShouldBeFasterIfFibonacciIsMemoized()
            {
                void RegularFibonacci(int n)
                {
                    var f = Enumerable.Range(1, n).Select(i => UtilityMath.FibonacciRecursive(i)).ToList();
                }
                void MemoizedFibonacci(int n)
                {
                    var f = Enumerable.Range(1, n).Select(i => UtilityMath.FibonacciCached(i)).ToList();
                }
                var actualNumber = 10;
                var actualRegularFibonacciDuration = UtilityHelper.Measure(() => RegularFibonacci(actualNumber));
                var actualMemoizedFibonacciDuration = UtilityHelper.Measure(() => MemoizedFibonacci(actualNumber));
                Console.WriteLine($"{actualMemoizedFibonacciDuration} ms <= {actualRegularFibonacciDuration} ms");
                Assert.LessOrEqual(actualMemoizedFibonacciDuration, actualRegularFibonacciDuration);
            }
        }

        [TestFixture]
        public class ErathostenesSieveTests
        {
            [Test()]
            public void ShouldFlagAsPrimeCorreccctlyUsdingRabinMillerTest()
            {
                var actualNotPrimes = new long[] { 4, 6, 8, 9, 10, 12, 14, 15, 16, 18, 20, 21, 22, 24, 25, 26,
                                                    27, 28, 30, 32, 33, 34, 35, 36, 38, 39, 40, 42, 44, 45, 46, 48, 49,
                                                    50, 51, 52, 54, 55, 56, 57, 58, 60, 62, 30119 * 30119, 34883 * 34883};

                foreach (ulong n in actualNotPrimes)
                    Assert.IsFalse(UtilityMath.MillerRabin(n), $"{n} is not prime.");

                var actualPrimes = UtilityMath.ErathostenesSieve(10000);

                foreach (ulong n in actualPrimes)
                    Assert.IsTrue(UtilityMath.MillerRabin(n), $"{n} is prime.");

                actualPrimes = new long[] { 1262903, 1262917, 1262927, 1262929, 1262939, 1262941 };

                foreach (ulong n in actualPrimes)
                    Assert.IsTrue(UtilityMath.MillerRabin(n), $"{n} is prime.");
            }

            [Test()]
            public void ShouldContainOnlyPrimes()
            {
                var fileName = @"C:\GitRepositories\JuanMartin.Kernel\JuanMartin.Kernel.Test\data\first_million_primes.txt";
                var actualPrimes = UtilityFile.ReadTextToStringEnumerable(fileName).ToHashSet();

                var actualSieve = UtilityMath.ErathostenesSieve(1000000);

                foreach (var n in actualSieve)
                    Assert.IsTrue(actualPrimes.Contains(n.ToString()), $"{n} is prime."); //UtilityMath.IsPrime(n),$"{n} is prime.");
            }
        }

        [TestFixture]
        public class ItemsArePositiveSequentialTests
        {
            [Test()]
            public void ShouldBeTrueEnumerableHasSequentialPositiveIntegersAndCheckingOneAdditionalSequentialElement()
            {
                var actalCollection = new int[] { 4, 3, 2, 1 };
                var actualNewItem = 5;

                Assert.IsTrue(UtilityMath.ItemsArePositiveSequential(actalCollection, actualNewItem));
            }

            [Test()]
            public void ShouldBeTrueWhenEnumerableHasSequentialPositiveIntegersRegardlessOfOrder()
            {
                var actalAscendingCollection = new int[] { 1, 2, 3, 4 };
                var actalDescendingCollection = new int[] { 4, 3, 2, 1 };

                Assert.IsTrue(UtilityMath.ItemsArePositiveSequential(actalDescendingCollection) && UtilityMath.ItemsArePositiveSequential(actalAscendingCollection));
            }

            [Test()]
            public void ShouldBeFalseWhenEnumerableHasAtLeastOneNonSequentialPositiveInteger()
            {
                var actalCollection = new int[] { 4, 5, 2, 1 };

                Assert.IsFalse(UtilityMath.ItemsArePositiveSequential(actalCollection));
            }

            [Test()]
            public void ShouldBeFalseWhenEnumerableIsEmpty()
            {
                var actalCollection = new int[] { };

                Assert.IsFalse(UtilityMath.ItemsArePositiveSequential(actalCollection));
            }

            [Test()]
            public void ShouldBeTrueWhenEnumerableHasOneElement()
            {
                var actalCollection = new int[] { 1 };

                Assert.IsTrue(UtilityMath.ItemsArePositiveSequential(actalCollection));
            }

            [Test()]
            public void ShouldBeTrueWhenEnumerableIsEmptyAndThereIsOneAdditionalItem()
            {
                var actalCollection = new int[] { };
                var actualNewItem = 2;

                Assert.IsTrue(UtilityMath.ItemsArePositiveSequential(actalCollection, actualNewItem));
            }
        }

        [TestFixture]
        public class GetIscocelesTriangleAreaAndPerimeterUsingSidesOnlyTests
        {
            [Test]
            public void ShouldReturnValidNumberForAreaOfIscocelesTriangleWithLongSide()
            {
                int actualSide, actualBase;
                BigDecimal actualArea;

                actualSide = 333333333;
                actualBase = actualSide + 1;

                (actualArea, _) = UtilityMath.GetIscocelesTriangleAreaAndPerimeterUsingSidesOnly(actualBase, actualSide, false);

                Assert.IsTrue(actualArea.DecimalPlaces > 0, $"Area has a fraction, {actualArea}");
            }
        }

        [TestFixture]
        public class DcimalSquereRootTests
        {
            [Test()]
            public void ShouldCalculateSquareRootForLargeNumbers()
            {
                BigInteger big1 = new BigInteger(double.MaxValue);
                BigInteger big2 = new BigInteger(double.MaxValue);

                //BigInteger x = new BigInteger(6);
                //BigInteger y = new BigInteger(5);

                //BigInteger rem = BigInteger.DivRem(y, x, out BigInteger r);

                // Add the 2 values together.
                BigInteger actualNumber = BigInteger.Add(big1, big2);

                BigInteger? actualRoot = UtilityMath.BigNumberSquareRoot(actualNumber);

                Assert.IsTrue(actualRoot.HasValue, $"Number {actualNumber} has s square root.");

            }
        }

        [TestFixture]
        public class IsLargeNumberPerferctSquareTests
        {
            [Test()]
            public void ShouldIdentifyProductOfSameNumberTwiceAsPerfectSquare()
            {
                BigInteger actualNumber = 434 * 434;
                Assert.IsTrue(UtilityMath.IsLargeNumberPerferctSquare(actualNumber), $"{actualNumber} is a perfect square.");
            }
        }


        [TestFixture]
        public class DigitsSumTests
        {
            [Test()]
            public void ShouldBeTheFastestsDigitSumMethodTheArrayVariant()
            {
                void ArraySum(long[] numbers)
                {
                    var f = numbers.Select(i => UtilityMath.DigitsSum(i)).ToList();
                }
                void StringSum(long[] numbers)
                {
                    var f = numbers.Select(i => UtilityMath.DigitsSum(i.ToString())).ToList();
                }
                void ModulusSum(long[] numbers)
                {
                    var f = numbers.Select(i => UtilityMath.DigitsSum(i, addUpToSingleDigit: false)).ToList();
                }
                var actualNumbers = new long[] { 12345, 98765, 23456, 87654, 12345, 98765, 23456, 87654, 12345, 98765, 23456, 87654, 12345, 98765, 23456, 87654 };
                var actualArrayBasedDigitSumDuration = UtilityHelper.Measure(() => ArraySum(actualNumbers));
                var actualArithmeticBasedDigitSumDuration = UtilityHelper.Measure(() => ModulusSum(actualNumbers));
                var actualStringBasedDigitSumDuration = UtilityHelper.Measure(() => StringSum(actualNumbers));

                Assert.IsTrue(actualArithmeticBasedDigitSumDuration < actualArrayBasedDigitSumDuration, "Array variant is fastest.");
                Assert.IsTrue(actualArithmeticBasedDigitSumDuration > actualStringBasedDigitSumDuration, "String variant is slowest.");
            }
            [Test()]
            public void ShouldFinSumOfDigitsInNumberAsSingleDigit()
            {
                BigInteger actualNumber = 11111111111;
                int expectedSum = 2;

                var actualSum = UtilityMath.DigitsSum(actualNumber, true);

                Assert.AreEqual(expectedSum, actualSum);
            }
        }

        [TestFixture]
        public class AddLargeNumbersTests
        {
            [Test()]
            public void ShouldAddTwoNumbersWithoutDecimalTest()
            {
                var actualLeftNumber = "65";
                var actualRightNumber = "987";
                var expectedSum = "1052";

                var actualSum = UtilityMath.AddLargeNumbers(actualLeftNumber, actualRightNumber);

                Assert.AreEqual(expectedSum, actualSum);
            }

            [Test()]
            public void ShouldAddOneNegativeAndOnePositveNumbersWithoutDecimalsTest()
            {
                var actualLeftNumber = "5";
                var actualRightNumber = "-4";
                var expectedSum = "1";

                var actualSum = UtilityMath.AddLargeNumbers(actualLeftNumber, actualRightNumber);

                Assert.AreEqual(expectedSum, actualSum);
            }

            [Test()]
            public void ShouldPreserveOriginalNumberIfAddingZeroToOriginalTest()
            {
                var actualLeftNumber = "5";
                var actualRightNumber = "0";
                var expectedSum = actualLeftNumber;

                var actualSum = UtilityMath.AddLargeNumbers(actualLeftNumber, actualRightNumber);

                Assert.AreEqual(expectedSum, actualSum);
            }

            [Test()]
            public void ShouldAddTwoNegativeNumbersTest()
            {
                var actualLeftNumber = "-16";
                var actualRightNumber = "-5";
                var expectedSum = "-21";

                var actualSum = UtilityMath.AddLargeNumbers(actualLeftNumber, actualRightNumber);

                Assert.AreEqual(expectedSum, actualSum);
            }

            [Test()]
            public void ShouldAddTwoNumbersWithDecimalTest()
            {
                var actualLeftNumber = "0.65";
                var actualRightNumber = "98.7";
                var expectedSum = "99.35";

                var actualSum = UtilityMath.AddLargeNumbers(actualLeftNumber, actualRightNumber);

                Assert.AreEqual(expectedSum, actualSum);
            }

            [Test()]
            public void ShouldAddTwoNumbersWithDecimalAndMoveDecimalPlaceUpAccordinglyTest()
            {
                var actualRightNumber = "1.5";
                var actualLeftNumber = "1.5";
                var expectedSum = "3";

                var actualSum = UtilityMath.AddLargeNumbers(actualLeftNumber, actualRightNumber);

                Assert.AreEqual(expectedSum, actualSum);
            }

            [Test()]
            public void ShouldAddTwoNumbersWithDecimalAndIncreaseNumberOfDigitsTest()
            {
                var actualRightNumber = "99.9";
                var actualLeftNumber = "0.11";
                var expectedSum = "100.01";

                var actualSum = UtilityMath.AddLargeNumbers(actualLeftNumber, actualRightNumber);

                Assert.AreEqual(expectedSum, actualSum);
            }


            [Test()]
            public void ShouldAddTwoNumbersOneWithOneWithoutDecimalTest()
            {
                var actualLeftNumber = "0.65";
                var actualRightNumber = "987";
                var expectedSum = "987.65";

                var actualSum = UtilityMath.AddLargeNumbers(actualLeftNumber, actualRightNumber);

                Assert.AreEqual(expectedSum, actualSum);
            }

        }

        [TestFixture]
        public class SubstractLargeNumbersTests
        {
            [Test()]
            public void ShouldReturnZeroIfSubstractingSameTwoLargeNumbersTest()
            {
                var actualLeftNumber = long.MaxValue.ToString();
                var actualRightNumber = actualLeftNumber;
                var expectedSubs = "0";

                var actualSubs = UtilityMath.SubstractLargeNumbers(actualLeftNumber, actualRightNumber);

                Assert.AreEqual(expectedSubs, actualSubs);
            }

            [Test()]
            public void ShouldSubstractTwoNumbersTest()
            {
                var actualLeftNumber = "12";
                var actualRightNumber = "8";
                var expectedSubs = "4";

                var actualSubs = UtilityMath.SubstractLargeNumbers(actualLeftNumber, actualRightNumber);

                Assert.AreEqual(expectedSubs, actualSubs, $"{actualLeftNumber}>{actualRightNumber}.");

                actualLeftNumber = "4";
                actualRightNumber = "7";
                expectedSubs = "-3";

                actualSubs = UtilityMath.SubstractLargeNumbers(actualLeftNumber, actualRightNumber);

                Assert.AreEqual(expectedSubs, actualSubs, $"{actualLeftNumber}<{actualRightNumber}.");
            }

            [Test()]
            public void ShouldSubstractNegativeNumbersTest()
            {
                var actualLeftNumber = "-6";
                var actualRightNumber = "7";
                var expectedSubs = "-13";

                var actualSubs = UtilityMath.SubstractLargeNumbers(actualLeftNumber, actualRightNumber);

                Assert.AreEqual(expectedSubs, actualSubs, "left number is negative and smaller than right.");

                actualLeftNumber = "-8";
                actualRightNumber = "7";
                expectedSubs = "-1";

                actualSubs = UtilityMath.SubstractLargeNumbers(actualLeftNumber, actualRightNumber);

                Assert.AreEqual(expectedSubs, actualSubs, "left number is negative and larger than right.");

                actualLeftNumber = "-7";
                actualRightNumber = "7";
                expectedSubs = "0";

                actualSubs = UtilityMath.SubstractLargeNumbers(actualLeftNumber, actualRightNumber);

                Assert.AreEqual(expectedSubs, actualSubs, "left number is negative and same than right.");

                actualLeftNumber = "4";
                actualRightNumber = "-7";
                expectedSubs = "11";

                actualSubs = UtilityMath.SubstractLargeNumbers(actualLeftNumber, actualRightNumber);

                Assert.AreEqual(expectedSubs, actualSubs, "right number is negative and smaller than right.");

                actualLeftNumber = "8";
                actualRightNumber = "-7";
                expectedSubs = "15";

                actualSubs = UtilityMath.SubstractLargeNumbers(actualLeftNumber, actualRightNumber);

                Assert.AreEqual(expectedSubs, actualSubs, "right number is negative and larger than right.");
            }

            [Test()]
            public void ShouldSubstractTwoLargeNumbersTest()
            {
                var actualLeftNumber = long.MaxValue.ToString();
                var actualRightNumber = (long.MaxValue - 1).ToString();
                var expectedSubs = "1";

                var actualSubs = UtilityMath.SubstractLargeNumbers(actualLeftNumber, actualRightNumber);

                Assert.AreEqual(expectedSubs, actualSubs);
            }

            [Test()]
            public void ShouldProcessSubstractionsOfValuesLessThanOneTest()
            {
                var actualLeftNumber = "1";
                var actualRightNumber = "0.09";
                var expectedSubs = "0.91";

                var actualSubs = UtilityMath.SubstractLargeNumbers(actualLeftNumber, actualRightNumber);

                Assert.AreEqual(expectedSubs, actualSubs);
            }

            [Test()]
            public void ShouldGenerateNegativeResultTest()
            {
                var actualLeftNumber = "8";
                var actualRightNumber = "12";
                var expectedSubs = "-4";

                var actualSubs = UtilityMath.SubstractLargeNumbers(actualLeftNumber, actualRightNumber);


                Assert.AreEqual(expectedSubs, actualSubs);

                actualLeftNumber = "28";
                actualRightNumber = "32";
                expectedSubs = "-4";

                actualSubs = UtilityMath.SubstractLargeNumbers(actualLeftNumber, actualRightNumber);

                Assert.AreEqual(expectedSubs, actualSubs);

                actualLeftNumber = "0";
                actualRightNumber = "0.15";
                expectedSubs = "-0.15";

                actualSubs = UtilityMath.SubstractLargeNumbers(actualLeftNumber, actualRightNumber);

                Assert.AreEqual(expectedSubs, actualSubs);
            }
        }

        [TestFixture]
        public class MultiplyLargeNumbersTests
        {
            [Test()]
            public void ShoulProcessDecimalShiftsInMultiplicationOperandsTest()
            {
                var actualLeftNumber = "456";
                var actualRightNumber = "123";
                var expectedMult = "56088";

                var actualMult = UtilityMath.MultiplyLargeNumbers(actualLeftNumber, actualRightNumber);

                Assert.AreEqual(expectedMult, actualMult, $"{actualRightNumber}x{actualLeftNumber}");

                actualLeftNumber = "4.56";
                actualRightNumber = "123";
                expectedMult = "560.88";

                actualMult = UtilityMath.MultiplyLargeNumbers(actualLeftNumber, actualRightNumber);

                Assert.AreEqual(expectedMult, actualMult, $"{actualRightNumber}x{actualLeftNumber}");

                actualLeftNumber = "4.56";
                actualRightNumber = "1.23";
                expectedMult = "5.6088";

                actualMult = UtilityMath.MultiplyLargeNumbers(actualLeftNumber, actualRightNumber);

                Assert.AreEqual(expectedMult, actualMult, $"{actualRightNumber}x{actualLeftNumber}");

                actualLeftNumber = "0.456";
                actualRightNumber = "0.123";
                expectedMult = "0.056088";

                actualMult = UtilityMath.MultiplyLargeNumbers(actualLeftNumber, actualRightNumber);

                Assert.AreEqual(expectedMult, actualMult, $"{actualRightNumber}x{actualLeftNumber}");
            }

            [Test()]
            public void ShoulReturnNegativeResultIfOneOfTheOperandsIsNegativeTest()
            {
                var actualLeftNumber = "456";
                var actualRightNumber = "-123";
                var expectedMult = "-56088";

                var actualMult = UtilityMath.MultiplyLargeNumbers(actualLeftNumber, actualRightNumber);

                Assert.AreEqual(expectedMult, actualMult, $"{actualRightNumber}x{actualLeftNumber}");

                actualLeftNumber = "-4.56";
                actualRightNumber = "123";
                expectedMult = "-560.88";

                actualMult = UtilityMath.MultiplyLargeNumbers(actualLeftNumber, actualRightNumber);

                Assert.AreEqual(expectedMult, actualMult, $"{actualRightNumber}x{actualLeftNumber}");
            }

            [Test()]
            public void ShoulEliminateNegativeSignFromResultIfBothOperandsAreNegativeTest()
            {
                var actualLeftNumber = "-456";
                var actualRightNumber = "-123";
                var expectedMult = "56088";

                var actualMult = UtilityMath.MultiplyLargeNumbers(actualLeftNumber, actualRightNumber);

                Assert.AreEqual(expectedMult, actualMult, $"{actualRightNumber}x{actualLeftNumber}");
            }

            [Test()]
            public void ShoulRemoveDecimalPointIfGoingFromDecimalOnlyToIntegerOnlyTest()
            {
                var actualLeftNumber = "2";
                var actualRightNumber = "1.5";
                var expectedMult = "3";

                var actualMult = UtilityMath.MultiplyLargeNumbers(actualLeftNumber, actualRightNumber);

                Assert.AreEqual(expectedMult, actualMult, $"{actualRightNumber}x{actualLeftNumber}");
            }
        }

        [TestFixture]
        public class DivideLargeNumbersTests
        {
            [Test()]
            public void ShouldDivideSuccessfullyWholeNumbersTest()
            {
                var actualLeftNumber = "956";
                var actualRightNumber = "4";
                var expectedDiv = "239";

                var actualDiv = UtilityMath.DivideLargeNumbers(actualLeftNumber, actualRightNumber);

                Assert.AreEqual(expectedDiv, actualDiv, $"{actualLeftNumber}/{actualRightNumber}");

                actualLeftNumber = "5676";
                actualRightNumber = "12";
                expectedDiv = "473";

                actualDiv = UtilityMath.DivideLargeNumbers(actualLeftNumber, actualRightNumber);

                Assert.AreEqual(expectedDiv, actualDiv, $"{actualLeftNumber}/{actualRightNumber}");

                actualLeftNumber = "1328652";
                actualRightNumber = "234";
                expectedDiv = "5678";

                actualDiv = UtilityMath.DivideLargeNumbers(actualLeftNumber, actualRightNumber);

                Assert.AreEqual(expectedDiv, actualDiv, $"{actualLeftNumber}/{actualRightNumber}");
            }

            [Test()]
            public void ShouldProcessDivisionsOfAndByDecimalNumbersTest()
            {
                var actualLeftNumber = "16";
                var actualRightNumber = "5.3333";
                var expectedDiv = "3.18941368383552397";

                var actualDiv = UtilityMath.DivideLargeNumbers(actualLeftNumber, actualRightNumber, 17);

                Assert.AreEqual(expectedDiv, actualDiv, $"{actualLeftNumber}/{actualRightNumber}");

                actualLeftNumber = "9.152";
                actualRightNumber = "0.8";
                expectedDiv = "11.44";

                actualDiv = UtilityMath.DivideLargeNumbers(actualLeftNumber, actualRightNumber);

                Assert.AreEqual(expectedDiv, actualDiv, $"{actualLeftNumber}/{actualRightNumber}");

                actualLeftNumber = "1328652";
                actualRightNumber = "234";
                expectedDiv = "5678";

                actualDiv = UtilityMath.DivideLargeNumbers(actualLeftNumber, actualRightNumber);

                Assert.AreEqual(expectedDiv, actualDiv, $"{actualLeftNumber}/{actualRightNumber}");

                actualLeftNumber = "5";
                actualRightNumber = "0.5";
                expectedDiv = "10";

                actualDiv = UtilityMath.DivideLargeNumbers(actualLeftNumber, actualRightNumber);

                Assert.AreEqual(expectedDiv, actualDiv, $"{actualLeftNumber}/{actualRightNumber}");
            }

            [Test()]
            public void Should()
            {
            }

            [Test()]
            public void ShouldHandleCyclicalDecimalsInTheResults()
            {
                var actualLeftNumber = "5";
                var actualRightNumber = "7";
                var expectedDiv = "0.(714285)";

                var actualDiv = UtilityMath.DivideLargeNumbers(actualLeftNumber, actualRightNumber);

                Assert.AreEqual(expectedDiv, actualDiv, $"{actualLeftNumber}/{actualRightNumber}");

                actualLeftNumber = "3147";
                actualRightNumber = "990";
                expectedDiv = "3.1(78)";

                actualDiv = UtilityMath.DivideLargeNumbers(actualLeftNumber, actualRightNumber);

                Assert.AreEqual(expectedDiv, actualDiv, $"{actualLeftNumber}/{actualRightNumber}");

                actualLeftNumber = "29";
                actualRightNumber = "46";
                expectedDiv = "0.630434(782608695652234)";

                actualDiv = UtilityMath.DivideLargeNumbers(actualLeftNumber, actualRightNumber);

                Assert.AreEqual(expectedDiv, actualDiv, $"{actualLeftNumber}/{actualRightNumber}");

                actualLeftNumber = "5";
                actualRightNumber = "9";
                expectedDiv = "0.(5)";

                actualDiv = UtilityMath.DivideLargeNumbers(actualLeftNumber, actualRightNumber);

                Assert.AreEqual(expectedDiv, actualDiv, $"{actualLeftNumber}/{actualRightNumber}");

                actualLeftNumber = "2423";
                actualRightNumber = "450";
                expectedDiv = "5.38(4)";

                actualDiv = UtilityMath.DivideLargeNumbers(actualLeftNumber, actualRightNumber);

                Assert.AreEqual(expectedDiv, actualDiv, $"{actualLeftNumber}/{actualRightNumber}");
            }

            [Test()]
            public void ShouldExpressRemaindersAsDecimalsTest()
            {
                var actualLeftNumber = "5";
                var actualRightNumber = "2";
                var expectedDiv = "2.5";

                var actualDiv = UtilityMath.DivideLargeNumbers(actualLeftNumber, actualRightNumber);

                Assert.AreEqual(expectedDiv, actualDiv, $"{actualLeftNumber}/{actualRightNumber}");

                actualLeftNumber = "106408";
                actualRightNumber = "1139";
                expectedDiv = "93.422300263388937";

                actualDiv = UtilityMath.DivideLargeNumbers(actualLeftNumber, actualRightNumber, 15);

                Assert.AreEqual(expectedDiv, actualDiv, $"{actualLeftNumber}/{actualRightNumber}");

                actualLeftNumber = "2.87";
                actualRightNumber = "5";
                expectedDiv = "0.574";

                actualDiv = UtilityMath.DivideLargeNumbers(actualLeftNumber, actualRightNumber);

                Assert.AreEqual(expectedDiv, actualDiv, $"{actualLeftNumber}/{actualRightNumber}");

                actualLeftNumber = "121";
                actualRightNumber = "8";
                expectedDiv = "15.125";

                actualDiv = UtilityMath.DivideLargeNumbers(actualLeftNumber, actualRightNumber);

                Assert.AreEqual(expectedDiv, actualDiv, $"{actualLeftNumber}/{actualRightNumber}");

                actualLeftNumber = "471";
                actualRightNumber = "32";
                expectedDiv = "14.71875";

                actualDiv = UtilityMath.DivideLargeNumbers(actualLeftNumber, actualRightNumber);

                Assert.AreEqual(expectedDiv, actualDiv, $"{actualLeftNumber}/{actualRightNumber}");
            }

            [Test()]
            public void ShouldHandleNegativeNumbersForDivisionTest()
            {
                var actualLeftNumber = "10";
                var actualRightNumber = "-5";
                var expectedDiv = "-2";

                var actualDiv = UtilityMath.DivideLargeNumbers(actualLeftNumber, actualRightNumber);

                Assert.AreEqual(expectedDiv, actualDiv, $"{actualLeftNumber}/{actualRightNumber}");

                actualLeftNumber = "-10";
                actualRightNumber = "5";
                expectedDiv = "-2";

                actualDiv = UtilityMath.DivideLargeNumbers(actualLeftNumber, actualRightNumber);

                Assert.AreEqual(expectedDiv, actualDiv, $"{actualLeftNumber}/{actualRightNumber}");

                actualLeftNumber = "-10";
                actualRightNumber = "-5";
                expectedDiv = "2";

                actualDiv = UtilityMath.DivideLargeNumbers(actualLeftNumber, actualRightNumber);

                Assert.AreEqual(expectedDiv, actualDiv, $"{actualLeftNumber}/{actualRightNumber}");
            }
        }

        [TestFixture]
        public class IntegerDivisionTests
        {
            [Test()]
            public void ShouldCalculateHowManyTimesOneNumberFitsInAnotherTest()
            {
                var actualLeftNumber = "126";
                var actualRightNumber = "37";
                (string actualQuotient, _) = UtilityMath.Modulus(actualLeftNumber, actualRightNumber);
                var expectedQuotient = "3";

                Assert.AreEqual(expectedQuotient, actualQuotient, $"{actualLeftNumber}/{actualRightNumber}");
            }

            [Test()]
            public void ShouldCalculateModulusOfOneNumberFromAnotherTest()
            {
                var actualLeftNumber = "126";
                var actualRightNumber = "37";
                (_, string actualRemainder) = UtilityMath.Modulus(actualLeftNumber, actualRightNumber);
                var expectedRemainder = "15";

                Assert.AreEqual(expectedRemainder, actualRemainder, $"{actualRightNumber}%{actualLeftNumber}");
            }
        }

        [TestFixture]
        public class LoadAsMatrixTests
        {
            [Test()]
            public void ShouldConvertStringArrayOfNumbersIntoMatrixOfSingleDigitsTest()
            {
                var actualNumbers = new string[] { "123", "456" };
                var expectdedHeight = 2;
                var expectdedWidth = 3;
                var expectedTopLeft = 1;
                var expextedBottomRight = 6;

                var actualMatrix = UtilityMath.LoadAsMatrix(actualNumbers);

                Assert.AreEqual(expectdedHeight, actualMatrix.Length, "Height");
                Assert.AreEqual(expectdedWidth, actualMatrix[0].Length, "Width");
                Assert.AreEqual(expectedTopLeft, actualMatrix[0][0], "Top-left corner");
                Assert.AreEqual(expextedBottomRight, actualMatrix[1][2], "Bottom-right  corner");
            }
        }

        [TestFixture]
        public class AddMatrixTests
        {
            [Test()]
            public void ShouldAddArrayOfStringNumbersTest()
            {
                var actuaalNumbers = new string[] { "555", "666" };
                var expectdedSum = "1221";

                var actualSum = UtilityMath.AddMatrix(UtilityMath.LoadAsMatrix(actuaalNumbers)); // .AddLargeNumbers(actuaalNumbers);

                Assert.AreEqual(expectdedSum, actualSum);
            }
        }

        [TestFixture]
        public class BuildAmicableNumbersChainTests
        {
            [Test()]
            public void ShouldFormListOfAmicableNumbersTest()
            {
                const long limit = 1000000;
                long actualNumber;
                int expectedLength;

                actualNumber = 220;
                expectedLength = 2;
                Assert.AreEqual(expectedLength, UtilityMath.GetAmicableNumbersChain(actualNumber, limit).Count, $"chain for {actualNumber}");

                actualNumber = 12496;
                expectedLength = 5;
                Assert.AreEqual(expectedLength, UtilityMath.GetAmicableNumbersChain(actualNumber, limit).Count, $"chain for {actualNumber}");
            }
        }

        [TestFixture]
        public class GetFactorsTests
        {
            [Test()]
            public void ShoilGetAllProperDivisors()
            {
                long actualNumber;
                long[] expectedFactors, actualFactors;

                actualNumber = 28;
                expectedFactors = new long[] { 1, 2, 4, 7, 14 };
                actualFactors = UtilityMath.GetFactors(actualNumber).ToArray();
                Array.Sort(actualFactors);
                Assert.AreEqual(expectedFactors, actualFactors, $"{actualNumber}:  {string.Join(",", actualFactors.ToArray())} = {string.Join(",", expectedFactors.ToArray())}");
            }

            [Test()]
            public void ShouldReturnSameResultsGetProperDivisorsAndGetFactorsWithSelfNotIncludedTest()
            {
                void Factors(int n)
                {
                    var f = Enumerable.Range(1, n).Select(i => UtilityMath.GetFactors(i)).ToList();
                }
                void Divisors(int n)
                {
                    var f = Enumerable.Range(1, n).Select(i => UtilityMath.GetProperDivisors(i)).ToList();
                }
                var actualNumber = 1000;
                var actualFactorDuration = UtilityHelper.Measure(() => Factors(actualNumber));
                var actualDivisorsDuration = UtilityHelper.Measure(() => Divisors(actualNumber));

                //Assert.Less(actualDivisorsDuration, actualFactorDuration, $"{actualDivisorsDuration} ms < {actualFactorDuration} ms");

                for (int n = 4; n <= actualNumber; n++)
                {
                    if (!UtilityMath.IsPrimeUsingSquares(n))
                    {
                        var actualFactorList = UtilityMath.GetFactors(n).ToList();

                        actualFactorList.Sort();
                        var expectedFactorList = UtilityMath.GetProperDivisors(n).ToList();
                        Assert.AreEqual(expectedFactorList, actualFactorList, $"{n}: {string.Join(",", actualFactorList.ToArray())} = {string.Join(",", expectedFactorList.ToArray())}");
                    }
                }
            }
        }

        [TestFixture]
        public class PrimeFactorizationTests
        {
            System.Collections.Generic.Queue<long> actualPrimes;
            private void PrimeQueueSetup()
            {
                actualPrimes = new System.Collections.Generic.Queue<long>();
                foreach (var p in UtilityMath.ErathostenesSieve(10000))
                    actualPrimes.Enqueue(p);
            }
            [Test()]
            public void ShougldGetCompletePrimeFactorList()
            {
                List<long> expectedPrimeFactors, actualPrimeFactors;
                long actualNumber;

                actualNumber = 72;
                PrimeQueueSetup();
                expectedPrimeFactors = new List<long> { 2, 2, 2, 3, 3 };
                actualPrimeFactors = UtilityMath.PrimeFactorization(actualNumber, actualPrimes);

                Assert.AreEqual(expectedPrimeFactors, actualPrimeFactors, $"Factors of {actualNumber}");

                actualNumber = 28227;
                PrimeQueueSetup();
                expectedPrimeFactors = new List<long> { 3, 97, 97 };
                actualPrimeFactors = UtilityMath.PrimeFactorization(actualNumber, actualPrimes);

                Assert.AreEqual(expectedPrimeFactors, actualPrimeFactors, $"Factors of {actualNumber}");

                actualNumber = 7540;
                PrimeQueueSetup();
                expectedPrimeFactors = new List<long> { 2, 2, 5, 13, 29 };
                actualPrimeFactors = UtilityMath.PrimeFactorization(actualNumber, actualPrimes);

                Assert.AreEqual(expectedPrimeFactors, actualPrimeFactors, $"Factors of {actualNumber}");

                actualNumber = 15802;
                PrimeQueueSetup();
                expectedPrimeFactors = new List<long> { 2, 7901 };
                actualPrimeFactors = UtilityMath.PrimeFactorization(actualNumber, actualPrimes);

                Assert.AreEqual(expectedPrimeFactors, actualPrimeFactors, $"Factors of {actualNumber}");
            }

            [Test]
            public void ShouldCountFactorsUsingPrimeFactorization()
            {
                int expectedFactorCount, actualFactorCount;
                long actualNumber;

                actualNumber = 4;
                expectedFactorCount = 3;
                PrimeQueueSetup();

                actualFactorCount = UtilityMath.CountFactors(actualNumber, actualPrimes);

                Assert.AreEqual(expectedFactorCount, actualFactorCount, $"Factor count of {actualNumber}");

                actualNumber = 72;
                expectedFactorCount = 12;
                PrimeQueueSetup();

                actualFactorCount = UtilityMath.CountFactors(actualNumber, actualPrimes);

                Assert.AreEqual(expectedFactorCount, actualFactorCount, $"Factor count of {actualNumber}");

                actualNumber = 7540;
                expectedFactorCount = 24;
                PrimeQueueSetup();

                actualFactorCount = UtilityMath.CountFactors(actualNumber, actualPrimes);

                Assert.AreEqual(expectedFactorCount, actualFactorCount, $"Factor count of {actualNumber}");

                actualNumber = 15802;
                expectedFactorCount = 4;
                PrimeQueueSetup();

                actualFactorCount = UtilityMath.CountFactors(actualNumber, actualPrimes);

                Assert.AreEqual(expectedFactorCount, actualFactorCount, $"Factor count of {actualNumber}");
            }

            [Test]
            public void ShouldCountFactorsForLargeNumbersUsingPrimeFactorization()
            {
                int expectedFactorCount, actualFactorCount;
                BigInteger actualNumber;

                actualNumber = new BigInteger(4);
                expectedFactorCount = 3;
                PrimeQueueSetup();

                actualFactorCount = UtilityMath.CountLargeNumberFactorsUsingRecursion(actualNumber, actualPrimes);

                Assert.AreEqual(expectedFactorCount, actualFactorCount, $"Factor count of {actualNumber}");

                actualNumber = BigInteger.Pow(180180, 2);
                expectedFactorCount = 2025;
                PrimeQueueSetup();

                actualFactorCount = UtilityMath.CountLargeNumberFactorsUsingRecursion(actualNumber, actualPrimes);

                Assert.AreEqual(expectedFactorCount, actualFactorCount, $"Factor count of {actualNumber}");
            }
        }

        [TestFixture]
        public class IsBouncyNumberTests
        {
            [Test()]
            public void ShouldFlagNumberWithNoDigitExceededOrFollowedByTheDigitToItsLeftAsBouncyNumber()
            {
                int actualNumber;

                actualNumber = 538;

                Assert.IsTrue(UtilityMath.IsBouncyNumber(actualNumber), $"{actualNumber} is a bouncy number");

                actualNumber = 134468;

                Assert.IsFalse(UtilityMath.IsBouncyNumber(actualNumber), $"{actualNumber} is not a bouncy number");
                Assert.IsTrue(actualNumber.IsIncreasingNumber(), $"{actualNumber} is an increasing number");
                Assert.IsFalse(actualNumber.IsDecreasingNumber(), $"{actualNumber} is not a decreasing number");

                actualNumber = 66420;

                Assert.IsFalse(UtilityMath.IsBouncyNumber(actualNumber), $"{actualNumber} is not a bouncy number");
                Assert.IsTrue(actualNumber.IsDecreasingNumber(), $"{actualNumber} is an decreasing number");
                Assert.IsFalse(actualNumber.IsIncreasingNumber(), $"{actualNumber} is not an increasing number");

                actualNumber = 155349;

                Assert.IsTrue(UtilityMath.IsBouncyNumber(actualNumber), $"{actualNumber} is a bouncy number");
                Assert.IsFalse(actualNumber.IsIncreasingNumber(), $"{actualNumber} is not an increasing number");
                Assert.IsFalse(actualNumber.IsDecreasingNumber(), $"{actualNumber} is not a decreasing number");
            }
        }
    }
}