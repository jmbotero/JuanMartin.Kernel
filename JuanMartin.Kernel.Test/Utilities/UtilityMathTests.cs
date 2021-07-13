using JuanMartin.Kernel.Utilities;
using NUnit.Framework;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Numerics;
using System.Collections.Generic;
using JuanMartin.Kernel.RuleEngine;

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
                    var actualValue = UtilityMath.EvaluateSimpleArithmeticOerations(expression,0);
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
                double actualValue = UtilityMath.EvaluateSimpleArithmeticOerations(actualExpression);
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
            public void ShouldReturnSelectionsOfAllStingMembersInCollectionIncludingRepetitionsInLinearOrderInAscendingOrder()
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
                Assert.Less(actualMethodDotNetDuration, actualMethodOneDuration, "Babylonian is faster  than .Net Sqrt.");
                Assert.Less(actualMethodDotNetDuration, actualMethodTwoDuration, "Newton  method is faster  than .Net Sqrt.");
            }
        }

        [TestFixture]
        public class PrimeFactorsTests
        {
            [Test()]
            public void ShouldHaveTwoPrimeFactorsForOneThousand()
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
            public void ShouldReturOneCobinationWhenchoosingSameNumberOfElementsAsNumberOfDistinctObjectsInSet()
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
                var expectedFibonacciAsString = BigInteger.Parse("354224848179261915075");
                var actualFibonacciAsString = UtilityMath.FibonacciLoop(100);
                Assert.AreEqual(expectedFibonacciAsString, actualFibonacciAsString);
            }
        }

        [TestFixture]
        public class ErathostenesSieveTests
        {
            [Test()]
            public void ShouldContainOnlyPrimes()
            {
                var fileName = @"C:\GitRepositories\JuanMartin.Kernel\JuanMartin.Kernel.Test\data\primes_under_100000.txt";
                var actualPrimes = UtilityFile.ReadTextToStringEnumerable(fileName).ToHashSet(); // load 100000 primes

                var actualSieve = UtilityMath.ErathostenesSieve(10000);

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

    }
}