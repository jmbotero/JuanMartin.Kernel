   using JuanMartin.Kernel.Extesions;
using JuanMartin.Kernel.RuleEngine;
using JuanMartin.Kernel.Utilities.DataStructures;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CellList = System.Collections.Generic.Dictionary<string, JuanMartin.Kernel.Utilities.DataStructures.Cells>;


namespace JuanMartin.Kernel.Utilities
{
    public class UtilityMath
    {
        #region Calculate min and max values for generic types
        public static class GenericLimit<T>
        {
            public static readonly T MinValue = (T)MinValue.GetType().GetField("MinValue").GetValue(MinValue);
            public static readonly T MaxValue = (T)MaxValue.GetType().GetField("MaxValue").GetValue(MaxValue);
        }
        #endregion

        #region Rule engine math macro operations
        [Macro]
        public static Value Add(Symbol op1, Symbol op2)
        {
            var result = new Value();

            try
            {
                var o1 = op1.Value.Result;
                var o2 = op2.Value.Result;

                if (o1 is string || o2 is string)
                    result.Result = (string)(o1.ToString() + o2.ToString());
#pragma warning disable IDE0038 // Use pattern matching
                else if (o1 is double && o2 is double)
                    result.Result = (double)((double)o1 + (double)o2);
                else if (o1 is int && o2 is int)
                    result.Result = (int)((int)o1 + (int)o2);
                else if (o1 is long && o2 is long)
#pragma warning restore IDE0038 // Use pattern matching
                    result.Result = (long)((long)o1 + (long)o2);
                else
                    throw new Exception(string.Format("Arguments must be integer, long, double, or string, arg1 is [{0}] and args2 is [{1}]", o1.GetType().ToString(), o2.GetType().ToString()));
            }
            catch (Exception e)
            {
                var message = string.Format("Cannot add ({0}) and ({1}): {2}", op1, op2, e.Message);
                throw new Exception(message, e);
            }

            return result;
        }

        [Macro]
        public static Value Substract(Symbol op1, Symbol op2)
        {
            var result = new Value();

            try
            {
                var o1 = op1.Value.Result;
                var o2 = op2.Value.Result;

                if (o1 is string || o2 is string)
                    throw new Exception("Strings are not supported by the substract method");
#pragma warning disable IDE0038 // Use pattern matching
                else if (o1 is double && o2 is double)
                    result.Result = (double)((double)o1 - (double)o2);
                else if (o1 is int && o2 is int)
                    result.Result = (int)((int)o1 - (int)o2);
                else if (o1 is long && o2 is long)
#pragma warning restore IDE0038 // Use pattern matching
                    result.Result = (long)((long)o1 - (long)o2);
                else
                    throw new Exception(string.Format("Arguments must be integer, long, or double; arg1 is [{0}] and arg2 is [{1}]", o1.GetType().ToString(), o2.GetType().ToString()));
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Cannot substract ({0}) and ({1})", op1.ToString(), op2.ToString()), e);
            }

            return result;
        }

        [Macro]
        public static Value Multiply(Symbol op1, Symbol op2)
        {
            var result = new Value();

            try
            {
                var o1 = op1.Value.Result;
                var o2 = op2.Value.Result;

                if (o1 is string || o2 is string)
                    throw new Exception("Strings are not supported by the multiply method");
#pragma warning disable IDE0038 // Use pattern matching
                else if (o1 is double && o2 is double)
                    result.Result = (double)((double)o1 * (double)o2);
                else if (o1 is int && o2 is int)
                    result.Result = (int)((int)o1 * (int)o2);
                else if (o1 is long && o2 is long)
#pragma warning restore IDE0038 // Use pattern matching
                    result.Result = (long)((long)o1 * (long)o2);
                else
                    throw new Exception(string.Format("Arguments must be integer, long, or double; arg1 is [{0}] and arg2 is [{1}]", o1.GetType().ToString(), o2.GetType().ToString()));
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Cannot multiply ({0}) and ({1})", op1.ToString(), op2.ToString()), e);
            }

            return result;
        }

        [Macro]
        public static Value Divide(Symbol op1, Symbol op2)
        {
            var result = new Value();

            try
            {
                var o1 = op1.Value.Result;
                var o2 = op2.Value.Result;

                if (o1 is string || o2 is string)
                    throw new Exception("Strings are not supported by the divide method");
#pragma warning disable IDE0038 // Use pattern matching
                else if (o1 is double && o2 is double)
                    result.Result = (double)((double)o1 / (double)o2);
                else if (o1 is int && o2 is int)
                    result.Result = (int)((int)o1 / (int)o2);
                else if (o1 is long && o2 is long)
#pragma warning restore IDE0038 // Use pattern matching
                    result.Result = (long)((long)o1 / (long)o2);
                else
                    throw new Exception(string.Format("Arguments must be integer, long, or double; arg1 is [{0}] and arg2 is [{1}]", o1.GetType().ToString(), o2.GetType().ToString()));
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Cannot divide ({0}) and ({1})", op1.ToString(), op2.ToString()), e);
            }

            return result;
        }
        #endregion

        #region Math utility functions

        [Flags]

        public enum Sizing
        {
            smallest,
            greatest
        }

        public enum Location
        {
            left = 1,
            right = 2,
            top = 4,
            bottom = 8,
            none = 16
        }


        public static T AddGenericValues<T>(T operand1, T operand2)
        {
            //use 'dynamic' to trick compiler so generics can be added with '+' operand
            dynamic o1 = operand1;
            dynamic o2 = operand2;

            return o1 + o2;
        }

        public static bool IsPrimeUsingSquareRootArray(int number, int sqrt)
        {
            if (number == 1)
                return false;
            if (number == 2)
                return true;
            if (number % 2 == 0) // number is odd
                return false;
            for (int m = 3; m <= sqrt; m += 2)
            {
                if (m % 2 != 0 && number % m == 0) //m is even and m is a divisor of number
                    return false;
            }

            return true;
        }

        public static int[] GetSquareRootArray(int number)
        {
            var sqrt = new List<int>();

            for (int i = 0; i <= number; i++)
            {
                //sqrt.Add((int)GetSqrtBabylonianMethod(i));
                sqrt.Add((int)Math.Sqrt(i));
            }

            return sqrt.ToArray();
        }

        public static bool IsComposite(long number)
        {
            long i = 2;
            var factors = new List<long>();

            while (number / i >= i)
            {
                if (number % i == 0 && !factors.Contains(number / i))
                {
                    if (IsPrime(i))
                        factors.Add(i);
                    number /= i;
                }
                i++;
            }

            return factors.Count > 0;
        }

        public static bool IsPrime(long number)
        {
            long i = 2;
            var factors = new List<long>();

            while (number / i >= i)
            {
                if (number % i == 0 && !factors.Contains(number / i))
                {
                    if (IsPrime(i))
                        factors.Add(i);
                    number /= i;
                }
                i++;
            }

            return factors.Count == 0;
        }


        public static Func<long, bool> IsPrimeUsingSquares = (number) =>
        {
            if (number <= 1)
                return false;

            if (number == 2)
                return true;

            if (number % 2 == 0)
                return false;

            var counter = 3;

            while ((counter * counter) <= number)
            {
                if (number % counter == 0)
                {
                    return false;
                }
                else
                {
                    counter += 2;
                }
            }

            return true;
        };

        public class Operands : IComparable<Operands>
        {
            public List<int> Values { get; private set; }
            public double Id { get; private set; }
            public int Count => Values.Count;
            public Operands()
            {
                Values = new List<int>();
                Id = 0;
            }

            public int CompareTo(Operands other)
            {
                var thisId = this.Id;
                var otherId = other.Id;

                if (thisId < otherId)
                {
                    return 1;
                }
                else if (thisId > otherId)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            }

            // Define the indexer to allow client code to use [] notation.
            public int this[int i]
            {
                get { return Values[i]; }
            }

            public void Add(int v)
            {
                Values.Add(v);
                Id = Convert.ToDouble(string.Join("", Values));
            }

            public double Sum => Values.Sum();
            public double Multiply => Values.Multiply();

        }
        /// <summary>
        /// Get list of list of 'k' operand permutations that represent the sum and the product of the same n positive 
        /// integers: a(n)=i(1)+i(2)+...+i(n)=i(1)*i(2)*...*i(n).
        /// <see cref="https://rosettacode.org/wiki/Permutations_with_repetitions#C"/> Initial version of source code
        /// <seealso cref="https://oeis.org/A104173"/>
        /// </summary>
        /// <param name="number">Mximum value of i(x) or operand for product-sum</param>
        /// <param name="k">nunmber of terms in operation</param>
        /// <returns></returns>
        public static List<Operands> GetProductSumPermutations(int number, int k, bool returnJustFirstPermutation = false, bool sortPermutationsByEachOperandsOrder = true)
        {
            var permutations = new List<Operands>();
            // var permutations = new SortedSet<Operands>(); sorting once tested faster than add in order, if that 
            // is better use SortedSet<> instead.

            var duplicateKeys = new List<SortedSet<int>>(); // set to ensure only one combination of operands is added
            int[] digits = Enumerable.Repeat(1, k).ToArray();
            int index;
            bool hasMatch = false;
 
            digits[k - 1] = 0;
            index = k - 1;
            while (true)
            {
                if (digits[index] == number)
                {
                    index--;
                    if (index < 0)
                        break;
                }
                else
                {
                    digits[index]++;
                    while (index < k - 1)
                    {
                        index++;
                        digits[index] = 1;
                    }

                    var operands = new Operands();
                    var key = new SortedSet<int>();
                    foreach (var d in digits)
                    {
                        if (d == 0) continue;

                        operands.Add(d);
                        key.Add(d);
                    }

                    if (operands.Sum == operands.Multiply)
                    {
                        // check if operands are new in permutations list, by checking its key

                        hasMatch = duplicateKeys.Any(op => op.Count == key.Count && key.All(y => op.Contains(y)));
                        if (!hasMatch)
                        {
                            duplicateKeys.Add(key);
                            permutations.Add(operands);
                            if (returnJustFirstPermutation && permutations.Count == 1)
                                break;
                        }
                    }
                }
            }
            if (sortPermutationsByEachOperandsOrder && permutations.Count > 0)
                return permutations.OrderBy(operands => operands.Id).ToList();
            else
                return permutations.ToList();
        }
        public static int[] GetMinimalProductSumPermutation(int number, int k)
        {
            string GenerateFrequencyKey(int value, int[] permutation, int threadCount = -1)
            {
                int[] frequencies = new int[value + 1];

                for(var  i=0;i<frequencies.Length;i++)
                {
                    frequencies[i]++;
                }

                return string.Join("", frequencies);
            };

            var duplicateKeys = new HashSet<string>(); // set to ensure only one combination of operands is added
            int[] digits = Enumerable.Repeat(1, k).ToArray();
            int index;
            double minimalOperandId = double.MaxValue;
            int[] minimalPermutation = new int[k];

            digits[k - 1] = 0;
            index = k - 1;
            while (true)
            {
                if (digits[index] == number)
                {
                    index--;
                    if (index < 0)
                        break;
                }
                else
                {
                    digits[index]++;
                    while (index < k - 1)
                    {
                        index++;
                        digits[index] = 1;
                    }

                    // check if operands are new in permutations list, by checking its key
                    var key = GenerateFrequencyKey(number, digits, 2);
                    var hasMatch = duplicateKeys.Contains(key);

                    if (!hasMatch)
                    {
                        var addition = digits.Sum();
                        var multiplication = digits.Multiply();

                        if (addition != multiplication)
                        {
                            continue;
                        }
                        else
                        {
                            if (!hasMatch)
                            {
                                duplicateKeys.Add(key);
                                var label = Convert.ToDouble(string.Join("", digits.LoopReverse()));
                                if (label <= minimalOperandId)
                                {
                                    minimalOperandId = label;
                                    Array.Copy(digits, minimalPermutation, k);
                                }
                            }
                            else
                                continue;
                        }
                    }
                    else
                        continue;
                }
            }

            return minimalPermutation;
        }

        public static int[] GetMinimalProductSumPermutation2(int number, int k)
        {
            string GenerateFrequencyKey(int value, int[] permutation, int threadCount = -1)
            {
                int[] frequencies = new int[value + 1];

                for (var i = 0; i < frequencies.Length; i++)
                {
                    frequencies[i]++;
                }

                return string.Join("", frequencies);
            };

            var duplicateKeys = new HashSet<string>(); // set to ensure only one combination of operands is added
            int[] digits = new int[k];
            int index;
            double minimalOperandId = double.MaxValue;
            int[] minimalPermutation = new int[k];
            var factors = UtilityMath.GetFactors(number, includeSelf: true).ToArray();
            int[] operands = Enumerable.Repeat(1, k).ToArray();
            // int[] operands = new int[k];
            Array.Sort(factors);

            index = 0;
            while (true)
            {
                if (index < 0) break;

                // calculate next digit
                if (digits[index] == k)
                {
                    index--;
                    if (index < 0)
                        break;
                }
                else
                {
                    digits[index]++;
                    int j = digits[index];
                    if (j == factors.Length)
                        j = 0;
                    operands[index] = (int)factors[j];
                    while (index < k - 1)
                    {
                        index++;
                        digits[index] = 0;
                    }
                    
                    // check if operands are new in permutations list, by checking its key
                    var key = GenerateFrequencyKey(number, operands, 2);
                    var hasMatch = duplicateKeys.Contains(key);

                    if (!hasMatch)
                    {
                        var addition = operands.Sum();
                        var multiplication = operands.Multiply();

                        if (addition != multiplication)
                        {
                            continue;
                        }
                        else
                        {
                            if (!hasMatch)
                            {
                                duplicateKeys.Add(key);
                                var label = Convert.ToDouble(string.Join("", digits.LoopReverse()));
                                if (label <= minimalOperandId)
                                {
                                    minimalOperandId = label;
                                    Array.Copy(operands, minimalPermutation, k);
                                }
                            }
                            else
                                continue;
                        }
                    }
                    else
                        continue;
                }
            }

            return minimalPermutation;
        }

        public static List<IEnumerable<int>> GetProductSums(int number, int k = 0)
        {
            var result = new List<IEnumerable<int>>();

            int upper;
            int lower;

            if (k == 0)
            {
                lower = 2;
                upper = number - 1;
            }
            else
            {
                lower = k;
                upper = lower;
            }

            for (var digits = upper; digits >= lower; digits--)
            {
                var combinations = (List<IEnumerable<int>>)GetCombinationsOfKRecursive(Enumerable.Range(1, number).ToArray(), digits).ToList();

                for (var i = combinations.Count - 1; i >= 0; i--)
                {
                    var operands = combinations[i];

                    if (operands.Sum() != number || operands.Multiply() != number)
                        combinations.RemoveAt(i);
                }

                result.AddRange(combinations);
            }

            return result;
        }

        private static bool NextCombination(IList<int> num, int n, int k)
        {
            bool finished;

            var changed = finished = false;

            if (k <= 0) return false;

            for (var i = k - 1; !finished && !changed; i--)
            {
                if (num[i] < n - 1 - (k - 1) + i)
                {
                    num[i]++;

                    if (i < k - 1)
                        for (var j = i + 1; j < k; j++)
                            num[j] = num[j - 1] + 1;
                    changed = true;
                }
                finished = i == 0;
            }

            return changed;
        }

        /// <summary>
        ///  Return all combinations of k number of elements within a finite set of n elements
        ///  <see cref="https://www.technical-recipes.com/2017/obtaining-combinations-of-k-elements-from-n-in-c/"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="elements"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<T>> GetCombinationsOfK<T>(IEnumerable<T> elements, int k)
        {
            var elem = elements.ToArray();
            var size = elem.Length;

            if (k > size) yield break;

            var numbers = new int[k];

            for (var i = 0; i < k; i++)
                numbers[i] = i;

            do
            {
                yield return numbers.Select(n => elem[n]).ToArray();
            } while (NextCombination(numbers, size, k));
        }

        public static IEnumerable<IEnumerable<T>> GetCombinationsOfKRecursive<T>(IEnumerable<T> list, int k)
        {
            if (k == 1) return list.Select(t => new T[] { t });

            return GetCombinationsOfKRecursive(list, k - 1)
                      .SelectMany(t => list, (t1, t2) => t1.Concat(new T[] { t2 }));
        }

        public static int[][] GetNumericPermutationsOfK(int number, int k, bool includeDuplicates = false)
        {
            void AddPermutation(int lenght, int[] collection, List<int[]> p)
            {
                var list = new List<int>();

                for (var i = 1; i <= lenght; i++)
                {
                    list.Add(collection[i]);
                }
                p.Add(list.ToArray());
            }

            if (k > number)
                throw new ArgumentException($"Sample size ({k}) cannot be greater than repetition set size ({number}).");

            var permutations = new List<int[]>();
            int[] a = new int[k + 1];
            int temp;

            for (var i = 1; i <= k; i++)
            {
                a[i] = 1;
            }
            a[k] = 0;
            temp = k;
            while (true)
            {
                if (a[temp] == number)
                {
                    temp--;
                    if (temp == 0)
                        break;
                }
                else
                {
                    a[temp]++;
                    while (temp < k)
                    {
                        temp++;
                        a[temp] = 1;
                    }
                    if (includeDuplicates || (!includeDuplicates && !a.HasDuplicates<int>()))
                        AddPermutation(k, a, permutations);
                }
            }
            return permutations.ToArray();
        }

        //public static IEnumerable<T[]> GeneratePermutations2<T>(T[] elements, int currentIndex=0, List<T> perm = null)
        //{
        //    if(perm == null)
        //    {
        //        perm = new List<T>();
        //    }
        //    double pass = (currentIndex == 0) ? 0 : pass + 1;
        //    for(int i=0;i<elements.Length;i++)
        //    {
        //        if (i == currentIndex)
        //            perm.Add(elements[i]);
        //        else
        //            perm[currentIndex] = elements[currentIndex + 1];
        //        foreach (var p in GeneratePermutations2<T>(elements, i, perm))
        //            yield return p;
        //    }
        //}

        public static IEnumerable<T[]> GeneratePermutations<T>(T[] values, int fromInd = 0)
        {
            if (fromInd + 1 == values.Length)
                yield return values;
            else
            {
                foreach (var v in GeneratePermutations(values, fromInd + 1))
                    yield return v;

                for (var i = fromInd + 1; i < values.Length; i++)
                {
                    SwapValues(values, fromInd, i);
                    foreach (var v in GeneratePermutations(values, fromInd + 1))
                        yield return v;
                    SwapValues(values, fromInd, i);
                }
            }
        }

        /// <summary>
        /// TODO: this is very innefficient, use zero-based index
        /// <typeparam name="T"></typeparam>
        /// <param name="actualCollection"></param>
        /// <param name="actualSetSize"></param>
        /// <returns></returns>
        public static IEnumerable<T[]> GeneratePermutationsOfK<T>(T[] actualCollection, int actualSetSize, bool includeDuplicates=false)
        {
            var permutationIndexes = UtilityMath.GetNumericPermutationsOfK(actualCollection.Length, actualSetSize, includeDuplicates);

            var result = permutationIndexes.Select(row => row.Select(i => actualCollection[i - 1]).ToArray()).ToArray();

            return result;
        }

        /// <summary>
        /// Generate a cyclycal set, in that the last two digits of each number is the first two digits of the next number (including the last number with the first), for example: the ordered set of three 4-digit numbers: 8128, 2882, 8281. 
        /// </summary>
        /// <param name="order"></param>
        /// <param name="polygonalNumbers"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static (string sequence, int[] set) GetNumericalCyclicalSet(string[] order, int[][] polygonalNumbers)
        {
            List<int> GetCyclicalMatches(int n, int[] numbers)
            {
                var tail = n % 100;

                var s = numbers.Where(item => item / 100 == tail);

                s = s.Where(item => item % 100 >= 10); // ignore numbers ending from 00 to 09

                return s.ToList();
            }

            // polygonal number sides count to index: polygonalNumbers[triangular, square, pentagonal, hexagonal, heptagonal, octagonal]
            var size = polygonalNumbers.Length;
            var set = new int[size];
            var subset = new List<int>[size + 1];
            var sequence = string.Empty;
            var found = false;
            var i = 0;

            while (i < order.Length)
            {
                sequence = order[i];
                int[] indexes = sequence.SplitIntoNumericParts(1).ToArray();

                subset[0] = polygonalNumbers[indexes[0]].Where(item => item % 100 >= 10).ToList(); // ignore numbers ending from 00 to 09
                foreach (var n in subset[0])
                {
                    subset[1] = GetCyclicalMatches(n, polygonalNumbers[indexes[1]]);
                    //if (subset[1] == null || subset[1].Count == 0) break;
                    if (subset[1] != null && subset[1].Count > 0)
                    {
                        set.Assign(0, n); //first

                        foreach (var n1 in subset[1])
                        {
                            subset[2] = GetCyclicalMatches(n1, polygonalNumbers[indexes[2]]);
                            if (subset[2] == null || subset[2].Count == 0) break;
                            if (subset[2] != null && subset[2].Count > 0)
                            {
                                set.Assign(1, n1); //second

                                foreach (var n2 in subset[2])
                                {
                                    subset[3] = GetCyclicalMatches(n2, polygonalNumbers[indexes[3]]);
                                    if (subset[3] == null || subset[3].Count == 0) break;
                                    if (subset[3] != null && subset[3].Count > 0)
                                    {
                                        set.Assign(2, n2); //third

                                        foreach (var n3 in subset[3])
                                        {

                                            subset[4] = GetCyclicalMatches(n3, polygonalNumbers[indexes[4]]);
                                            if (subset[4] == null || subset[4].Count == 0) break;
                                            if (subset[4] != null && subset[4].Count > 0)
                                            {
                                                set.Assign(3, n3); //fourth

                                                foreach (var n4 in subset[4])
                                                {
                                                    subset[5] = GetCyclicalMatches(n4, polygonalNumbers[indexes[5]]);
                                                    if (subset[5] == null || subset[5].Count == 0) break;
                                                    if (subset[5] != null && subset[5].Count > 0)
                                                    {
                                                        set.Assign(4, n4); //fifth

                                                        foreach (var n5 in subset[5])
                                                        {
                                                            subset[6] = GetCyclicalMatches(n5, polygonalNumbers[indexes[0]]);
                                                            if (subset[6] == null || subset[6].Count == 0) break;
                                                            if (subset[6] != null && subset[6].Count > 0)
                                                            {
                                                                if (n5 % 100 == n / 100) // ensure is cyclical
                                                                {
                                                                    set.Assign(5, n5); // sixth
                                                                    found = true;
                                                                    break;
                                                                }
                                                            }
                                                        }
                                                        if (found) break;
                                                    }
                                                }
                                                if (found) break;
                                            }
                                        }
                                        if (found) break;
                                    }
                                }
                                if (found) break;
                            }
                        }
                        if (found) break;
                    }
                    if (set.Count(item => item != 0) == size) break; // if set of six has been assigned we are done
                }
                if (set.Count(item => item == 0) > 0)
                {
                    Array.Clear(set, 0, size);
                    i++;
                }
                else
                    break;
            }

            return (sequence, set);
        }

        /// <summary>
        /// Two numbers without a common divisor are co-prime
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool AreCoprime(long x, long y)
        {
            return (GetCommonDivisor(x, y, Sizing.greatest) == 1);
        }

        public static bool IsTruncatablePrime(IEnumerable<string> primes)
        {
            foreach (var item in primes)
            {
                var number = Convert.ToInt64(item);
                if (number == 1) //if truncation ends wth 1 do not count it as a truncatable prime
                    return false;
                else if (!IsPrime(number))
                    return false;
                else if (item[0] == '0') //exclude  numbers with trailing zeroes
                    return false;
            }
            return true;

        }

        public static IEnumerable<string> GetNumberTruncations(long number, Location type)
        {
            return GetNumberTruncations(number.ToString(), type);
        }

        public static IEnumerable<string> GetNumberTruncations(string number, Location type)
        {
            var truncations = new List<string>();
            var length = number.Length;

            for (int first = 1; first < length; first++)
            {
                string value;
                if (type.HasFlag(Location.left))
                {
                    value = number.Substring(first);
                    if (!truncations.Contains(value)) truncations.Add(value);
                }
                if (type.HasFlag(Location.right))
                {
                    value = number.Substring(0, number.Length - first);
                    if (!truncations.Contains(value)) truncations.Add(value);
                }
            }
            return truncations;
        }

        public static Func<int, List<List<int>>> TwoDigitPartitions = number =>
        {
            var partitions = new List<List<int>>();

            if (number == 2)
            {
                partitions.Add(new List<int> { 1, 1 });
            }
            else if (number > 2)
            {
                for (int b = number; b >= (int)Math.Ceiling((double)number / 2); b--)
                {
                    var a = number - b;

                    if (a > 0)
                    {
                        var partition = new List<int> { a, b };
                        partitions.Add(partition);
                    }
                }
            }
            return partitions;
        };

        private static (List<int> end, List<int> start) SplitList(List<int> source, int index)
        {
            var length = source.Count;
            var countA = index;
            var countB = (length - 1) - index;

            List<int> a = null;
            List<int> b = null;

            if (index > 0 && countA > 0) a = source.GetRange(0, countA);
            if (index < length - 1 && countB > 0) b = source.GetRange(index + 1, countB);

            return (a, b);
        }

        /// <summary>
        /// Given a positive integer n, generate all possible unique ways to represent n as sum of positive integers.
        /// Code from GeeksForGeeks: https://www.geeksforgeeks.org/generate-unique-partitions-of-an-integer/
        /// This code is contributed by Sam007
        /// </summary>
        /// <param name="number"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static (int Count, List<string> Sequences) Partitions(int number, int size = 2)
        {
            var partitions = new List<string>();

            if (number < 2)
                return (1, partitions);

            // An array to store a partition 
            int[] p = new int[number];
            int count = 0;
            var calculateOnly = false;
            // Index of last element in a partition 
            int k = 0;

            if (number > 60) calculateOnly = true;

            // Initialize first partition as number itself 
            p[k] = number;

            // This loop first prints current partition, then generates next partition. The loop stops when the current partition has all 1s 
            while (true)
            {
                // print current partition 
                if (k + 1 >= size)
                {
                    count++;
                    if (!calculateOnly)
                    {
                        var current = string.Join("", p.Take(k + 1));

                        partitions.Add(current);
                    }
                }
                // Generate next partition 

                // Find the rightmost non-one value in p[]. Also, update the remainderValue so that we know how much value can be accommodated 
                int remainderValue = 0;

                while (k >= 0 && p[k] == 1)
                {
                    remainderValue += p[k];
                    k--;
                }

                // if k < 0, all the values are 1 so there are no more partitions 
                if (k < 0)
                    return (count, partitions);

                // Decrease the p[k] found above and adjust the remainderValue 
                p[k]--;
                remainderValue++;

                // If remainderValue is more, then the sorted  order is violated. Divide remainderValue in different values of size p[k] and copy these values at different positionsafter p[k] 
                while (remainderValue > p[k])
                {
                    p[k + 1] = p[k];
                    remainderValue -= p[k];
                    k++;
                }

                // Copy remainderValue to next position and increment position 
                p[k + 1] = remainderValue;
                k++;
            }
        }

        public static Func<int, List<List<int>>> PartitionsRecursive = number =>
        {
            var partitions = new List<List<int>>();

            if (number > 1)
            {
                var currentPartitions = TwoDigitPartitions.Memoize(); //try caching this

                foreach (var partition in currentPartitions(number))
                {
                    if (!partitions.ContantainsList(partition)) partitions.Add(partition);

                    var length = partition.Count;

                    // if partition is not all ones process it
                    if (length > 1 && partition.First() != 1 && partition.Distinct().Count() > 1) //(partition.All(x => x == partition.First()))
                    {
                        foreach (var (digit, i) in partition.Enumerate())
                        {
                            if (digit != 1)
                            {
                                var (end, start) = SplitList(partition, i);

                                foreach (var replacement in PartitionsRecursive(digit))
                                {
                                    List<int> p = new List<int>();

                                    if (start != null) p.AddRange(start);
                                    p.AddRange(replacement);
                                    if (end != null) p.AddRange(end);
                                    // add only if not already in list
                                    if (!partitions.ContantainsList(p)) partitions.Add(p);

                                    //TODO: Modify containslist to use hashset to speed-up

                                    //if (digit > 1)
                                    //{
                                    //    //partitions.AddRange(TwoDigitPartitions(number).Where(x => !partitions.ContantainsList(partition)));

                                    //    var newCollection = TwoDigitPartitions(number);
                                    //    var existingValues = new HashSet<List<int>>(from x in newCollection select x);
                                    //    var newItems = newCollection.Where(x => !existingValues.Contains(x.bar));

                                    //    foreach (var item in newItems)
                                    //    {
                                    //        partitions.Add(item);
                                    //    }
                                    //}
                                }
                            }
                        }
                    }
                }
            }
            return partitions;
        };

        public static IEnumerable<string> GetNumberRotations(string number)
        {
            var rotations = new List<string>();
            var length = number.Length;

            for (int first = 0; first < length; first++)
            {
                var rotation = new StringBuilder();
                var l = 0;
                while (l < length)
                {
                    var index = (first + l > (length - 1)) ? (first + l) % length : first + l;
                    rotation.Append(number[index].ToString());
                    l++;
                }
                rotations.Add(rotation.ToString());
            }
            return rotations;
        }

        public static IEnumerable<string> GetNumberRotations(int number)
        {
            return GetNumberRotations(number.ToString());
        }

        public static List<List<int>> GetPentagonalRing(List<int> digits, int sideLength, int ringSize)
        {
            const int first = 0;
            const int middle = 1;
            const int last = 2;

            var subs = new Dictionary<string, string>();
            var sides = new List<List<int>>();
            var ring = new List<List<int>>();
            var outerNodes = digits.Skip(ringSize);
            var digitPermuatations = UtilityMath.GeneratePermutations<int>(digits.ToArray<int>());
            var sum = CalculatePentagonalRingSideSum(digits, ringSize);

            // take all sideLength-character sub sequences from all combinations of digits
            // in ring ordered from outer to inner nodes that add to the "sum" of a side 
            // and add it to the possible perimeter
            foreach (var sequence in digitPermuatations)
            {
                var side = sequence.Take(sideLength).ToList();
                var hash = side[first].ToString() + side[middle] + side[last];
                if (side.Sum() == sum && outerNodes.Contains(side[first]) && !subs.ContainsKey(hash))
                {
                    subs.Add(hash, hash);
                    sides.Add(side);
                }
            }

            // now filter side sequences which do not join in ring
            // perimeter contains all possible value permutations for node values for each
            // side, first  separate them by outer node value, all perimeter are
            // alredy sorted by first digit
            var value = 0;
            var current = ringSize;

            var groupedSides = new List<List<int>>[ringSize];

            foreach (var s in sides)
            {
                if (s[first] != value)
                {
                    current--;
                    groupedSides[current] = new List<List<int>>();
                    value = s[first];
                }
                groupedSides[current].Add(s);
            }

            var a = digits.Max();
            var b = -1;
            var i = 0;
            var gi = 0;

            while (ring.Count < ringSize)
            {
                sides = groupedSides[gi];
                var side = sides[i];
                var count = sides.Count;

                if ((a == -1 || side[first] == a) && (b == -1 || side[middle] == b))
                {
                    ring.Add(side);
                    b = side[last];
                    a--;
                    gi++;
                    i = 0;
                }
                else
                    i++;
                if (i == count)
                    return null;
                if (gi == ringSize && ring.Count < ringSize)
                    return null;
            }
            return ring;
        }


        public static List<long> GetFactorList(long x, bool includeOne = true)
        {
            var factors = new List<long>();

            if (includeOne)
            {
                factors.Add(1);
                factors.Add(x);

            }
            var i = 2;
            while (i < x)
            {
                if (x % i == 0 && !factors.Contains(i))
                {
                    factors.Add(i);
                    factors.Add(x / i);
                    x /= i;
                }
                i++;

            }
            return factors;
        }

        public static int NumberOfDivisors(long number)
        {
            return GetFactors(number).Count();
        }

        public static long GetCommonDivisor(long a, long b, Sizing order)
        {
            var aDivisors = GetFactors(a, true, false).ToList();
            var bDivisors = GetFactors(b, true, false).ToList();

            aDivisors.Sort();
            bDivisors.Sort();

            if (order == Sizing.greatest)
                aDivisors.Reverse();

            foreach (long factor in aDivisors)
            {
                if (bDivisors.Contains(factor))
                    return factor;
            }

            return 1;
        }

        public static int GreatestCommonDivisorUsingLoop(double a, double b)
        {
            var gcd = 1;

            for (int i = 1; i < a && i <= b; ++i)
            {
                // Checks if i is factor of both integers
                if (a % i == 0 && b % i == 0)
                    gcd = i;
            }

            return gcd;
        }

        public static T EuclidianGCD<T>(T a, T b)
        {
            var methodType = typeof(T);

            if (!UtilityType.IsNumericType(methodType))
                throw new ArgumentException("Both arguments must be numeric.");
            dynamic da = a;
            dynamic db = b;

            while (da != 0)
            {
                T c = da;
                da = db % da;
                db = c;
            }
            return db;
        }
        public static T GreatestCommonDivisor<T>(T a, T b)
        {
            var methodType = typeof(T);

            if (!UtilityType.IsNumericType(methodType))
                throw new ArgumentException("Both arguments must be numeric.");
            dynamic da = a;
            dynamic db = b;

            if (db == 0)
                return da;
            else
                return GreatestCommonDivisor(db, da % db);
        }

        /// <summary>
        /// Sum of squares of each digit in number, which has to be positive.
        /// If  number is negative then return zero.
        /// <param name="n">Integer greater than zero</param>
        /// <returns></returns>
        /// </summary>
        public static Func<int, int> SquareDigits = n =>
        {
            int sum = 0;

            while (n > 0)
            {
                sum += (n % 10) * (n % 10);
                n /= 10;
            }

            return sum;
        };

        /// <summary>
        /// Introducing cached version of SquareDigits() function,
        /// so it can be memoized 
        /// </summary>
        public static int SquareDigitsCached(int n)
        {
            var squares = SquareDigits.Memoize();
            return squares(n);
        }

        /// <summary>
        /// Every starting number will eventually arrive at 1 or 89.   
        /// And chain ends when it gets to either of these.  Chains 
        /// ending in 1 have last  digit 1, otherwise  they end in 89.
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static (int terminator, HashSet<int> chain) SquareDigitsChain(int n)
        {
            var chain = new HashSet<int>();
            bool found;
            var value = n;
            do
            {
                // check existence beore adding it
                found = chain.Contains(value);
                chain.Add(value);
                value = SquareDigitsCached(value);

                // only end if chain alredy contains terminator
                if ((value == 1 || value == 89) && found)
                    break;
            } while (!found);

            var term = 89;
            if (chain.Last() == 1)
                term = 1;

            return (term, chain);
        }

        /// <summary>
        /// <see cref="https://stackoverflow.com/questions/3432412/calculate-square-root-of-a-biginteger-system-numerics-biginteger"/>
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static BigInteger SquareRootLargeNumbers(BigInteger number)
        {
            // TODO: fix rounding of large decimal parts to zero
            BigInteger square = (BigInteger )Math.Pow(Math.E,(double)BigInteger.Log(number) / 2);

            return square;
        }

        /// <summary>
        /// <see cref="https://www.csharp-console-examples.com/general/finding-the-square-root-of-a-number-in-c/"/>
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static BigInteger? BigNumberSquareRoot(BigInteger number)
        {
            if (number < 0) return null;

            BigInteger root = number / 3;

            int i;
            for (i = 0; i < 32; i++)
            {
                var x = number / root;
                x += root;
                root = x / 2;
            }

            root = (root + number / root) / 2;
            return root;
        }

        public static long LeastCommonMultiple(long a, long b)
        {
            if (a < 0 || b < 0)
                throw new ArgumentException("Both of the numbers compared must be greater than zero");

            return (a * b) / GetCommonDivisor(a, b, Sizing.greatest);
        }

        public enum Formula
        {
            Euclid,
            Pythagoras,
            BruteForce
        }

        /// <summary>
        /// For right triangles, using  euclids formula
        /// </summary>
        /// <param name="perimeter">Triangle Perimeter</param>
        /// <param name="source">todoAlgorithm to use to calculate triples</param>
        /// <returns></returns>
        public static List<Triple<long>> GetPythagoreanTriples(long perimeter, Formula source = Formula.Euclid, bool checkMultiples = true)
        {
            switch (source)
            {
                case Formula.Euclid:
                    {
                        return GetPythagoreanTriplesUsingEuclid(perimeter, checkMultiples);
                    }
                case Formula.Pythagoras:
                    {
                        return GetPythagoreanTriplesUsingPitagoras(perimeter, checkMultiples);
                    }
                case Formula.BruteForce:
                    {
                        return GetPythagoreanTriplesUsingBruteForce(perimeter);
                    }

                default:
                    throw new Exception("Unexpected Case");
            }

        }

        /// <summary>
        /// For right triangles, using  pithagorean formula
        /// </summary>
        /// <param name="p">Triangle perimeter</param>
        /// <returns></returns>
        public static List<Triple<long>> GetPythagoreanTriplesUsingPitagoras(long p, bool checkMultiples)
        {
            var triples = new List<Triple<long>>();

            for (double a = 3; a <= p / 2; a++)
            {
                for (double b = a; b <= p / 2; b++)
                {
                    var c = Math.Sqrt((a * a) + (b * b));
                    if (a + b + c == p)
                    {
                        var triple = new Triple<long>
                        {
                            Item1 = (long)a,
                            Item2 = (long)b,
                            Item3 = (long)c
                        };

                        if (checkMultiples)
                            AddMultiplesOfPythagoreanTriples(triple, p, triples);
                        else
                            triples.Add(triple);
                    }
                }
            }

            return triples;
        }

        /// <summary>
        /// For right triangles, using  euclids formula, implementation from https://www.mathblog.dk/pythagorean-triplets/
        /// </summary>
        /// <param name="p">Triangle perimeter</param>
        /// <returns></returns>
        public static List<Triple<long>> GetPythagoreanTriplesUsingEuclid(long p, bool checkMultiples)
        {
            var triples = new List<Triple<long>>();
            var mlimit = (long)Math.Sqrt(p / 2);
            long m;
            for (m = 2; m <= mlimit; m++)
            {
                if ((p / 2) % m == 0)
                { // m found
                    long k;
                    if (m % 2 == 0)
                    { // ensure that we find an odd number for k
                        k = m + 1;
                    }
                    else
                    {
                        k = m + 2;
                    }
                    while (k < 2 * m && k <= p / (2 * m))
                    {
                        if (p / (2 * m) % k == 0 && EuclidianGCD(k, m) == 1)
                        {
                            long d = p / 2 / (k * m);
                            long n = k - m;
                            long a = d * (m * m - n * n);
                            long b = 2 * d * n * m;
                            long c = d * (m * m + n * n);

                            triples.Add(new Triple<long>(a, b, c));

                            if (!checkMultiples && k == 1)
                                return triples;
                        }
                        k += 2;
                    }
                }
            }
            return triples;
        }



        /// <summary>
        /// Brute force solution described at https://www.mathblog.dk/pythagorean-triplets/
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static List<Triple<long>> GetPythagoreanTriplesUsingBruteForce(long p)
        {
            var triples = new List<Triple<long>>();

            long a;
            for (a = 1; a < p / 3; a++)
            {
                long b;
                for (b = a; b < p / 2; b++)
                {
                    long c = p - a - b;

                    if (a * a + b * b == c * c)
                    {
                        triples.Add(new Triple<long>(a, b, c));
                    }
                }
            }
            return triples;
        }
        public static List<Triple<long>> AddMultiplesOfPythagoreanTriples(Triple<long> t, long p, List<Triple<long>> triples)
        {
            var k = 1;
            long b = 1;
            long a = 1;
            long c = 1;

            while (c <= p / 2)
            {
                a = k * t.Item1;
                b = k * t.Item2;
                c = k * t.Item3;

                var triple = new Triple<long>
                {
                    Item1 = a,
                    Item2 = b,
                    Item3 = c
                };
                if (a + b + c == p)
                {
                    var containsItem = triples.Any(item => item.Item1 == triple.Item1 && item.Item2 == triple.Item2 && item.Item3 == triple.Item3);
                    //account a and b can be switched
                    containsItem = containsItem || triples.Any(item => item.Item1 == triple.Item2 && item.Item2 == triple.Item1 && item.Item3 == triple.Item3);

                    if (!containsItem)
                        triples.Add(triple);
                }
                k++;
            }

            return triples;
        }

        /// <summary>
        /// Calculate Champerowne's constant in base 10 for a certain seed integer
        /// </summary>
        /// <param name="size">length of the constant</param>
        /// <returns></returns>
        public static string GetChamperowneConstant(int size)
        {
            var count = 1;
            var temp = 0;
            var constant = new StringBuilder();

            constant.Append("0.");

            while (true)
            {
                var c = count.ToString();
                temp += c.Length;
                constant.Append(c);

                if (temp >= size)
                    break;
                count++;
            }

            return constant.ToString();
        }

        public static bool IsNaturalNumber(string number)
        {

            return (int.TryParse(number, out int n) && n > 0);
        }

        public static bool IsNaturalNumber<T>(T number)
        {
            var methodType = typeof(T);

            if (!UtilityType.IsNumericType(methodType))
                throw new InvalidOperationException(string.Format("{0} is invalid, IsNaturalNumber filter can only operate on numeric types.", methodType));


            return (int.TryParse(number.ToString(), out int n) && n > 0);
        }

        /// <summary>
        /// A number is triangular if 1+8*number is odd and a perfect square
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static bool IsTriangularNumber(long number)
        {
            var x = 8 * number + 1;
            var isMatch = (x % 2 == 1);

            isMatch = isMatch && IsPerferctSquare((double)x);

            return isMatch;
        }

        public static int[][] GetNumberTriangle(string sequence)
        {
            return sequence.Split(';').Select(row => row.Split(',').Select(value => Convert.ToInt32(value)).ToArray()).ToArray();
        }

        public static long IsPentagonalNumber(long number)
        {
            if (number == 0)
                return -1;

            var n = (1 + Math.Sqrt(24 * number + 1)) / 6;

            return ((n == (long)n) ? (long)n : -1);
        }

        public static long GetTriangularNumber(long n)
        {
            return (n * (n + 1) / 2);
        }


        public static IEnumerable<long> GetTriangularNumbers(long upperLimit, long lowerLimit = 1)
        {
            var i = lowerLimit;
            while (i <= upperLimit)
            {
                if (IsTriangularNumber(i))
                    yield return i;
                i++;
            }
        }


        public static long GetPentagonalNumber(long n)
        {
            return (n * (3 * n - 1) / 2);
        }

        public static IEnumerable<long> GetPentagonalNumbers(long limit, long minimum = long.MinValue)
        {
            long count = 1;

            while (count <= limit)
            {
                var n = (count * (3 * count - 1) / 2);
                if (n >= minimum)
                    yield return n;
                count++;
            }
        }

        public static long GetHexagonalNumber(long n)
        {
            return (n * (2 * n - 1));
        }

        public static long GetQuadrilateralNumber(long n)
        {
            return (n * n);
        }

        public static long GetHeptagonalNumber(long n)
        {
            return n * (5 * n - 3) / 2;
        }

        public static long GetOctagonalNumber(long n)
        {
            return n * (3 * n - 2);
        }

        public static long GetPolygonalNumber(int sides, long n)
        {
            long number;

            switch (sides)
            {

                case 3:
                    {
                        //Triangle numbers
                        number = GetTriangularNumber(n);
                        break;
                    }
                case 4:
                    {
                        // Square numbers
                        number = GetQuadrilateralNumber(n);
                        break;
                    }
                case 5:
                    {
                        // Pentagonal numbers
                        number = GetPentagonalNumber(n);
                        break;
                    }
                case 6:
                    {
                        //Hexagonal numbers
                        number = GetHexagonalNumber(n);
                        break;
                    }
                case 7:
                    {
                        //Heptagonal numbers
                        number = GetHeptagonalNumber(n);
                        break;
                    }
                case 8:
                    {
                        //Octagonal numbers
                        number = GetOctagonalNumber(n);
                        break;
                    }

                default:
                    throw new Exception("Unsupported nummber of perimeter to generate polygonal number.");
            }

            return number;
        }
        
        /// <summary>
        /// Calculate the area   and perimete of a triangle with two equal sides.
        /// <see cref="https://byjus.com/maths/area-of-isosceles-triangle/"/>
        /// <seealso cref=" https://www.omnicalculator.com/math/isosceles-triangle"/>
        /// </summary 
        /// <param name="b">base of the isosceles triangle</param>
        /// <param name="a">length of the two equal sides</param>
        /// <returns></returns>
        public static (BigDecimal area, BigDecimal perimeter)  GetIscocelesTriangleAreaAndPerimeterUsingSidesOnly(int b, int a, bool useIntegerCorrection=true)
        {
            BigDecimal A = new BigDecimal(a);
            BigDecimal B = new BigDecimal(b);
            BigDecimal x = B * B;
            x /= 4;
            x = (A * A) - x;

            BigDecimal area = x.Sqrt_Babylonian(true);

            if (useIntegerCorrection)
            {
                // sqrt  approximation is not accurate, use integer version
                if (!UtilityMath.IsPerferctSquare(x.GetWholePartAsBigInteger(), area.GetWholePartAsBigInteger()))
                {
                    area = x.Sqrt_BigInteger();
                }
            }
            area *= B;
            area /= 2;

            BigDecimal perimeter = B + (A * 2);
            
            return (area, perimeter);
        }

        public static IEnumerable<T> GetPolygonalNumbers<T>(int sides, int lowerBound, int upperBound)
        {
            var methodType = typeof(T);

            if (UtilityType.IsNumericType(methodType))
            {
                dynamic number = 0;
                var n = 1;

                while (number <= upperBound)
                {
                    number = GetPolygonalNumber(sides, n);
                    if (number > lowerBound && number <= upperBound)
                        yield return (T)number;
                    n++;
                }
            }
            else
                throw new InvalidOperationException(string.Format("Type {0} is invalid, GetPolygonalNumbers can only operate on numeric types.", methodType));
        }

        public static List<T[]> GetPolygonalNumbers<T>(int lbound, int ubound)
        {
            var numbers = new List<T[]>();

            for (int s = 3; s <= 8; s++)
            {
                var aux = GetPolygonalNumbers<T>(s, lbound, ubound).ToArray();
                numbers.Add(aux);
            }

            return numbers;
        }

        public static IEnumerable<long> GetHexagonalNumbers(long limit, long minimum = long.MinValue)
        {
            long count = 1;

            while (count <= limit)
            {
                var n = (count * (2 * count - 1));
                if (n >= minimum)
                    yield return n;
                count++;
            }
        }

        /// <summary>
        /// Use cahe which is to avoid recalculating factors.
        /// </summary> 
        /// <param name="number"></param>
        /// <param name="cache"></param>
        /// <returns></returns>
        public static bool IsAmicableNumber(long number, Dictionary<long, long> cache = null)
        {
            long sum = ProperDivisorSum(number, cache);
            long aux = ProperDivisorSum(sum, cache);

            return (aux == number && aux != sum);
        }

        public static List<long> GetAmicableNumbersChain(long number, long l, Dictionary<long, long> factorsSumCache)
        {
            var amicableNumbers = new List<long>();

            // create cictionary cache of factor sums, chain elements, as we go building chain
            do
            {
                if ((amicableNumbers.Count > 0 && number < amicableNumbers[0]) || number > l)
                    break;

                amicableNumbers.Add(number);
                if (factorsSumCache.TryGetValue(number, out long temp))
                {
                    number = temp;
                }
                else
                {
                    var sum = ProperDivisorSum(number, factorsSumCache);
                    number = sum;
                }

                if (amicableNumbers.Contains(number))
                    break;

            } while (number > 1 && number < l);

            return amicableNumbers;
        }

        private static long ProperDivisorSum(long n, Dictionary<long, long> cache = null)
        {
            long sum;
            if (cache == null || (cache != null && !cache.TryGetValue(n, out _)))
            {
                var divisors = GetFactors(n);
                sum = divisors.Sum();
                if(cache != null)  cache.Add(n, sum);
            }
            else
            {
                sum = -1;
            }

            return sum;
        }

        public static bool IsPandigital<T>(T number)
        {
            var methodType = typeof(T);

            if (!UtilityType.IsNumericType(methodType))
                throw new ArgumentException("Both arguments must be numeric.");

            return IsPandigital(number.ToString());
        }

        public static bool IsPandigital(string number)
        {
            var match = false;
            var digit = 0;

            if (UtilityString.ContainsDuplicates(number))
                return false;

            var n = number.Length;

            if (n > 10)
                return false;

            var l = (n == 10) ? n - 1 : n;

            if (l > 1)
            {

                for (int i = 0; i <= l; i++)
                {
                    if (number.Contains(i.ToString()) && ((i == 0 && n == 10) || (i > 0 && n <= 10)))
                    {
                        digit++;
                    }
                }

                if (digit == n)
                    match = true;
            }

            return match;
        }

        public static string[] GeneratePandigitalSet(int length, int begin = 1)
        {
            var end = (begin == 0) ? length + 1 : length;
            _ = (begin == 0) ? length : length - 1;
            var digits = Enumerable.Range(begin, end).ToArray<int>();

            return UtilityString.GeneratePermutations<int>(digits);
        }

        public static bool AreMatchingPermutations<T>(T a, T b)
        {
            return AreMatchingPermutations(a.ToString(), b.ToString()) && AreMatchingPermutations(b.ToString(), a.ToString());
        }

        private static bool AreMatchingPermutations(string a, string b)
        {
            return (a.IsAnagram(b));
        }


        public static bool IsAbundant(long number)
        {
            return (GetFactors(number).Sum() > number);
        }

          public static IEnumerable<long> GetFactors(long number, bool includeSelf = false, bool includeOne = true)
        {
//            if (number > 2 && !IsPrimeUsingSquares(number))
                long start = (includeSelf) ? 1 : 2;

                if (includeOne)
                    yield return 1;

                foreach (long n in GetFactors(number, start))
                    yield return n;
        }

        /// <summary>
        /// Get list of positive multiples of 'x'. This is faster than GetFactorList
        /// 
        /// </summary>
        /// <param name="number"></param>
        /// <param name="includeSelf"></param>
        /// <param name="includeOne"></param>
        /// <returns></returns>
        public static IEnumerable<long> GetFactors(long number, long start)
        {
            for (long i = start; i * i <= number; i++)
            {
                if ((number % i) == 0)
                {
                    yield return i;
                    if(number != i * i) yield return number / i;
                }
            }
        }

        public static IEnumerable<int> GetPrimeFactors(long number, bool includeSelf = false, bool includeOne = true)
        {
            var factors = GetFactors(number, includeSelf,includeOne).Distinct().ToList();

            foreach (int n in factors)
            {
                if (IsPrimeUsingSquares(n))
                    yield return n;
            }
        }

        public static List<long> GetPrimeFactors(long number, HashSet<long> primes)
        {
            List<long> factors = new List<long>();

            if (!primes.Contains(number))
            {
                int i = 0;
                long p = 0;

                while (p <= number)
                {
                    if (number % p == 0)
                    {
                        factors.Add(p);
                        number /= p;
                    }
                    else
                    {
                        i++;
                    }
                    p = primes.ElementAt(i);
                }
            }
            return factors;
        }

        /// <summary>
        /// All factors except self,
        /// <see cref="https://stackoverflow.com/questions/5793009/efficiently-finding-all-divisors-of-a-number"/>
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static IEnumerable<long> GetProperDivisors(long n)
        {
            return from a in CollectionExtensions.Range((long)1, n / 2)
                   where n % a == 0
                   select a;
        }

        public static int[] GetDigitReplacementArray(string[] mask)
        {
            var numbers = new List<int>();

            for (int d = 0; d <= 9; d++)
            {
                var number = Convert.ToInt32(string.Join("", mask.Select(c => ReplaceDigit(c, d))));
                numbers.Add(number);
            }

            return numbers.ToArray();
        }

        private static string ReplaceDigit(string element, int digit)
        {
            return (element == "*") ? digit.ToString() : element;
        }

        /// <summary>
        /// Concatenate as strings two numbers and return its numeric representation
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static T NumericConcat<T>(T a, T b)
        {
            dynamic A = a;
            dynamic B = b;
            dynamic C = b;
            while (C > 0)
            {
                A *= 10;
                C /= 10;
            }

            return A + B;
        }

        public static int DigitLength<T>(T number)
        {
            if (Utilities.UtilityType.IsNumericType(number.GetType()))
            {
                return number.ToString().Length;
            }
            else
                throw new InvalidOperationException(string.Format("{0} is invalid, DigitLength method can only be used numeric types.", number.GetType()));
        }

        public static int DigitCount<T>(T number)
        {
            if (Utilities.UtilityType.IsNumericType(number.GetType()))
            {
                dynamic n = number;
                return (IsNaturalNumber(n.ToString())) ? (int)Math.Floor(1 + Math.Log10((double)n)) : -1;
            }
            else
                throw new InvalidOperationException(string.Format("{0} is invalid, DigiatCount method can only be used numeric types.", number.GetType()));
        }


        public static IEnumerable<Fraction> GetReducedProperFractionsUsingYield(int d, Fraction key = null)
        {
            var fractions = RPF(d, key).ToList();
            var sorted = from fraction in fractions
                            orderby fraction.Value ascending
                            select fraction;
            return sorted;
        }

        public static IEnumerable<Fraction> RPF(int d, Fraction key = null)
        {
            var lowerBound = new Fraction(1, 2);

            if (key != null)
            {
                var y = (int)Math.Ceiling(d * .999997);
                var x = Math.Floor((double)(3 * y - 1) / 7);

                lowerBound = new Fraction(x, y);

                if (lowerBound.Denominator == d)
                    lowerBound = new Fraction(1, 2);
                else if (!key.Denominator.Between(lowerBound.Denominator, d)) // ((lowerBound.Denominator != d) && (key.Denominator < lowerBound.Denominator || key.Denominator > d))
                    yield return key;
                else
                    lowerBound = new Fraction(1, 2);
            }

            for (var denominator = lowerBound.Denominator; denominator <= d; denominator++)
            {
                for (var numerator = lowerBound.Numerator; numerator <= denominator; numerator++)
                {
                    if (GreatestCommonDivisorUsingLoop(numerator, denominator) == 1)
                    {
                        var fraction = new Fraction(numerator, denominator);

                        yield return fraction;
                    }
                }
            }
        }

        public static IEnumerable<Fraction> GetReducedProperFractions(int d, Fraction key = null, bool breakOnFirstLeftNeighbor = false)
        {
            var seq = new SortedSet<Fraction>();
            var lowerBound = new Fraction(1, 2);

            if (key != null)
            {
                var y = (int)Math.Ceiling(d * .999997);
                var x = Math.Floor((double)(3 * y - 1) / 7);

                lowerBound = new Fraction(x, y);

                if (lowerBound.Denominator == d)
                    lowerBound = new Fraction(1, 2);
                else if (!key.Denominator.Between(lowerBound.Denominator, d)) // ((lowerBound.Denominator != d) && (key.Denominator < lowerBound.Denominator || key.Denominator > d))
                    seq.Add(key);
                else
                    lowerBound = new Fraction(1, 2);
            }

            for (var denominator = lowerBound.Denominator; denominator <= d; denominator++)
            {
                for (var numerator = lowerBound.Numerator; numerator <= denominator; numerator++)
                {
                    if (GreatestCommonDivisorUsingLoop(numerator, denominator) == 1)
                    {
                        var fraction = new Fraction(numerator, denominator);

                        seq.Add(fraction);
                    }
                    if (breakOnFirstLeftNeighbor)
                    {
                        var index = seq.TakeWhile(fraction => !fraction.LiteralEqual(key)).Count();
                        if (index != 0 && seq.ElementAt(index - 1).Value < key.Value)
                        {
                            return new List<Fraction> { seq.ElementAt(index - 1) };
                        }
                    }
                }
            }

            return seq;
        }

        //public static IEnumerable<Fraction> GenerateFareySequenceSet(int i)
        //{
        //    var comparer = Comparer < (int n, int d)>.Create((a, b) => (a.n * b.d).CompareTo(a.d * b.n));
        //    var seq = new SortedSet<(int n, int d) > (comparer);
        //    for (int d = 1; d <= i; d++)
        //    {
        //        for (int n = 0; n <= d; n++)
        //        {
        //            seq.Add((n, d));
        //        }
        //    }
        //    return seq;
        //}

        public static Fraction GetFareySequenceNeighborOnly(int n, Fraction key, Location which = Location.left)
        {
            Fraction neighbor = null;

            if (key.Denominator > n)
                return null;

            //(left x/y,3/7):3y-7x=1.x=(3y-1)/7 (x<y,y<n,y>1)
            //(right x/y,3/7):3x-7y=1.x=(7y+1)/3

            // We know first two terms are 0/1 and 1/n 
            double x1 = 0, y1 = 1, x2 = 1, y2 = n;

            double distance = (double)key.Numerator / key.Denominator - (double)x1 / y1;

            if ((distance > 0 && which == Location.left) || //d>0 viene de izquierda
                (distance < 0 && which == Location.right))
                neighbor = new Fraction(x1, y1);

            distance = (double)key.Numerator / key.Denominator - (double)x2 / y2;

            if ((distance > 0 && which == Location.left) || //d>0 viene de izquierda
                (distance < 0 && which == Location.right))
                neighbor = new Fraction(x2, y2);

            //Console.Write("{0:F0}/{1:F0} {2:F0}/{3:F0}", x1, y1, x2, y2);

            double x, y = 0; // For next terms to be evaluated 
            while (y != 1.0)
            {
                // Using recurrence relation to find the next term 
                x = Math.Floor((y1 + n) / y2) * x2 - x1;
                y = Math.Floor((y1 + n) / y2) * y2 - y1;

                // Print next term  
                double d = (double)key.Numerator / key.Denominator - (double)x / y;

                if (d > 0 && which == Location.left && d < distance)
                {
                    distance = d;
                    neighbor = new Fraction(x, y);
                }
                else if (d < 0 && which == Location.right && d > distance)
                {
                    distance = d;
                    neighbor = new Fraction(x, y);
                }
                //Console.Write(" {0:F0}/{1:F0}", x, y);

                // Update x1, y1, x2 and y2 for next iteration 
                x1 = x2;
                x2 = x;
                y1 = y2;
                y2 = y;
            }

            return neighbor;
        }

        public static IEnumerable<Fraction> GenerateFareySequence(int n)
        {
            // We know first two terms are 0/1 and 1/n 
            double x1 = 0, y1 = 1, x2 = 1, y2 = n;

            //Console.Write("{0:F0}/{1:F0} {2:F0}/{3:F0}", x1, y1, x2, y2);
            yield return new Fraction(x1, y1);
            yield return new Fraction(x2, y2);

            double x, y = 0; // For next terms to be evaluated 
            while (y != 1.0)
            {
                // Using recurrence relation to find the next term 
                x = Math.Floor((y1 + n) / y2) * x2 - x1;
                y = Math.Floor((y1 + n) / y2) * y2 - y1;

                // Print next term 
                //Console.Write(" {0:F0}/{1:F0}", x, y);
                yield return new Fraction(x, y);

                // Update x1, y1, x2 and y2 for next iteration 
                x1 = x2;
                x2 = x;
                y1 = y2;
                y2 = y;
            }
        }

        public static int[] GetRepeatedDigitCounts(int number)
        {
            var counts = new int[10];
            var count = DigitCount(number);

            var n = number;
            for (int i = 1; i <= count; i++)
            {
                var d = n % 10;
                n /= 10;
                counts[d]++;
            }

            return counts;
        }

        public static int GetPowerOfTen(long number)
        {
            if (number <= 0)
                throw new ArgumentException($"Can only GetPowerOfTen({number}) of positive numbers greater than zero.");

            int power = 0;

            while(true)
            {
                if (number == 0)
                    break;

                number /= 10;
                power++;
            }

            return power;
        }

        public static bool IsPowerOfTen(long number)
        {

            if (number % 10 != 0 || number == 0)
            {
                return false;
            }

            if (number == 10)
            {
                return true;
            }

            return IsPowerOfTen(number / 10);
        }

        public static List<string> GetNumericPatterns(int number)
        {
            var masks = new List<string>();
            var template = number.ToString();
            var repeats = GetRepeatedDigitCounts(number);

            for (var i = 0; i < repeats.Length; i++)
            {
                if (repeats[i] == 0 || repeats[i] == 1) continue;
                
                var mask = string.Empty;
                var mark = i.ToString();
                //###(\d)#\1
                if (repeats[i] > 1)
                {
                    foreach (var c in template)
                    {
                        var C = Convert.ToString(c);
                        if (C != mark)
                            template = template.Replace(C, "\\d");
                    }
                    var position = template.IndexOf(mark);
                    StringBuilder maskBuilder = new StringBuilder(template);
                    maskBuilder.Remove(position, 1);
                    maskBuilder.Insert(position, "(\\d)");
                    mask = maskBuilder.ToString();
                    mask = mask.Replace(mark, "\\1");

                    template = number.ToString();
                }
                masks.Add(mask);
            }
            return masks;
        }
        /// <summary>
        /// Get list of all prime numbers less than or equal to limit(long)
        /// </summary>
        /// 
        /// <param name="upperBound"></param>
        /// <param name="includeOne">add nuomber one to the list of prime numbers</param>
        /// <returns></returns>
        public static List<long> GetPrimeNumbers(int upperBound, int lowerBound = 2, bool includeOne = true)
        {
            var primes = new List<long>();
            var start = (includeOne) ? 1 : lowerBound;

            var num = start;
            while (true)
            {
                if (IsPrime(num))
                    primes.Add(num);

                if (primes.Count > upperBound)
                    break;

                num++;
            }

            return primes;
        }

        /// <summary>
        /// Get list of all prime numbers less than or equal to limit(int), using no factor prime definition
        /// </summary>
        /// <param name="upperBound"></param>
        /// <returns></returns>
        public static IEnumerable<int> GetPrimeNumbersUsingSquares(int upperBound, int lowerBound = 2)
        {
            for (int num = lowerBound; num <= upperBound; num++)
            {
                if (IsPrimeUsingSquares(num))
                    yield return num;
            }
        }

        /// <summary>
        /// Get list of all prime numbers less than or equal to limit(int), using no factor prime definition
        /// </summary>
        /// <param name="upperBound"></param>
        /// <returns></returns>
        public static IEnumerable<int> GetPrimeNumbersUsingSquareRoot(int upperBound, int lowerBound = 2)
        {
            var squares = GetSquareRootArray(upperBound);

            for (int num = lowerBound; num <= upperBound; num++)
            {
                if (IsPrimeUsingSquareRootArray(num, squares[num]))
                    yield return num;
            }
        }
        /// <summary>
        /// Check both ascending or descending sequential positive integers
        /// in collection. And if additional item is specified compare it to
        /// last member of collection.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="newItem"></param>
        /// <returns></returns>
        public static bool ItemsArePositiveSequential(IEnumerable<int> source, int newItem = -1)
        {
//            if (!double.IsInfinity(result) && result == Math.Truncate(result) && result > 0) // is positive integer

                if (source == null)
                throw new ArgumentException("Collection  is null."); ;
            if (source.Count() < 1 && newItem == -1)
                return false;

            bool descending = true;
            int previous = int.MaxValue;

            foreach (var n in source)
            {
                if (n < 0)
                    return false;

                descending &= (n < previous);
                previous = n;
            }
            descending &= newItem == -1 || (newItem > ((source.Count() < 1) ? int.MinValue : source.Last()));


            bool ascending = true;
            previous = int.MinValue;

            foreach (var n in source)
            {
                ascending &= (n > previous);
                previous = n;
            }
            ascending &= newItem == -1 || (newItem > ((source.Count() < 1) ? int.MaxValue : source.Last()));

            return ascending || descending;
        }

        public static long[] ErathostenesSieve(int upperLimit, int lowerLimit=2, int threadCount=-1)
        {
            var sieveBound = (int)(upperLimit - 1) / 2;
            var upperSqrt = ((int)Math.Sqrt(upperLimit) - 1) / 2;

            var PrimeBits = new BitArray(sieveBound + 1, true);

            //            for (int i = 1; i <= upperSqrt; i++)
            var options = new ParallelOptions()
            {
                MaxDegreeOfParallelism = threadCount
            };
            Parallel.For(1, upperSqrt, options, i =>
            {
                if (PrimeBits.Get(i))
                {
                    int seed = i * 2 * (i + 1);
                    int delta = 2 * i + 1;

                    for (int j = seed; j <= sieveBound; j += delta)
                    {
                        PrimeBits.Set(j, false);
                    }
                }
            });

            var numbers = new List<long>((int)(upperLimit / (Math.Log(upperLimit) - 1.08366)));
            if (2 >= lowerLimit)
                numbers.Add(2);

            //for (int i = 1; i <= sieveBound; i++)
            Parallel.For(1, sieveBound, options, i =>
            {
                if (PrimeBits.Get(i))
                {
                    var n = 2 * i + 1;
                    if (n >= lowerLimit && UtilityMath.IsPrime(n))
                        numbers.Add(n);
                }
            });

            return numbers.ToArray();
        }

        /// <summary>
        /// Source code from <see cref="https://www.mathblog.dk/files/euler/Problem50.cs"/>
        /// </summary>
        /// <param name="lowerLimit"></param>
        /// <param name="upperLimit"></param>
        /// <returns></returns>
        public static long[] ESieve(int lowerLimit, int upperLimit)
        {

            int sieveBound = (int)(upperLimit - 1) / 2;
            int upperSqrt = ((int)Math.Sqrt(upperLimit) - 1) / 2;

            BitArray PrimeBits = new BitArray(sieveBound + 1, true);

            for (int i = 1; i <= upperSqrt; i++)
            {
                if (PrimeBits.Get(i))
                {
                    for (int j = i * 2 * (i + 1); j <= sieveBound; j += 2 * i + 1)
                    {
                        PrimeBits.Set(j, false);
                    }
                }
            }

            List<long> numbers = new List<long>((int)(upperLimit / (Math.Log(upperLimit) - 1.08366)));

            if (lowerLimit < 3)
            {
                numbers.Add(2);
                lowerLimit = 3;
            }

            for (int i = (lowerLimit - 1) / 2; i <= sieveBound; i++)
            {
                if (PrimeBits.Get(i))
                {
                    numbers.Add(2 * i + 1);
                }
            }

            return numbers.ToArray();
        }
        public static long[] CompositeErathostenesSieve(int upperLimit, int lowerlimit=4)
        {
            var lower = (lowerlimit < 4) ? 4 : lowerlimit;
            var primes = ErathostenesSieve(upperLimit, lowerlimit);
            List<long> numbers = (List<long>)Enumerable.Range(lower, upperLimit - 3);

            var composites = numbers.Except(primes);

            return composites.ToArray();
        }

        public static int[] GeneratePrimes(int n)
        {
            var primes = new List<int> { 2 };
            var nextPrime = 3;
            while (primes.Count < n)
            {
                var sqrt = (int)Math.Sqrt(nextPrime);
                var isPrime = true;
                for (int i = 0; (int)primes[i] <= sqrt; i++)
                {
                    if (nextPrime % primes[i] == 0)
                    {
                        isPrime = false;
                        break;
                    }
                }
                if (isPrime)
                {
                    primes.Add(nextPrime);
                }
                nextPrime += 2;
            }
            return primes.ToArray();
        }

        public static IEnumerable<int> GeneratePrimesUsingSquares(int upperLimit, int lowerLimit=2  )
        {
            for(var number=lowerLimit;number<upperLimit;number++)
            {
                if (IsPrimeUsingSquares(number))
                    yield return number;
            }
        }

        /// <summary>
        /// Returns first part of number.
        /// </summary>
        /// <param name="number">Initial number</param>
        /// <param name="N">Amount of digits required</param>
        /// <returns>First part of number</returns>
        public static int TakeNDigits(int number, int N)
        {
            // this is for handling negative numbers, we are only insterested in postitve number
            number = Math.Abs(number);
            // special case for 0 as Log of 0 would be infinity
            if (number == 0)
                return number;
            // getting number of digits on this input number
            int numberOfDigits = (int)Math.Floor(Math.Log10(number) + 1);
            // check if input number has more digits than the required get first N digits
            if (numberOfDigits >= N)
                return (int)Math.Truncate((number / Math.Pow(10, numberOfDigits - N)));
            else
                return number;
        }

        public static bool IsPalindrome(long number)
        {
            return number == number.Reverse();
        }

        public static bool IsPalindromeOverflow(BigInteger number)
        {
            return number == number.Reverse();
        }

        public static bool IsMultiple(int[] multiples, long number)
        {
            var length = 1;
            var matchCount = 0;

            if (multiples != null)
            {
                length = multiples.Length;

                foreach (int m in multiples)
                {
                    if (number % m == 0)
                        matchCount++;
                }
            }

            return matchCount == length;
        }

        public static string PowOverflow(int number, int exponent)
        {
            var result = number.ToString();

            for (int i = 1; i < exponent; i++)
                result = MultiplyLargeNumberBySingleDigit(result, number);

            return result;
        }

        /// <summary>
        /// Old Japanese method to calculate square root of integers using only subtraction, 
        /// no infinite decimals are involved at any step, which can cause loss of precision 
        /// due to rounding errors, and this method converges much more slowly than Newton’s
        /// method for finding square roots
        /// </summary>
        /// <see cref="http://www.afjarvis.staff.shef.ac.uk/maths/jarvisspec02.pdf"/>
        /// <param name="n"></param>
        /// <param name="digits"></param>
        /// <returns></returns>
        public static double GetSqrtBySubstraction(double n, int digits = 50)
        {
            double ShiftDecimal(double number, int position)
            {
                var s = number.ToString(FormatStrings.DoubleFixedPoint);
                var p = s.IndexOf(".");
                var point = string.Empty;

                if (position < 0)
                {
                    point = "0." + "0".Repeat(Math.Abs(position));
                    position = 0;
                }
                else if (position >= 0)
                {
                    point = ".";

                }

                if (p > 0) s = s.Remove(p); //remove current decimal
                s = s.Insert(position, point);

                return double.Parse(s);
            }

            if (n <= 0)
                throw new ArgumentException("Cannot calculate square root of zero or negative number.");

            if (n == 1)
                return 1;

            // calculate decimal position for future root value
            var originalNumber = n;
            int decimalPosition = 1;
            var shift = Location.left;
            if (n < 1)
                shift = Location.right;

            if (n <= 1 || n >= 100)
            {
                while (true)
                {
                    if (shift == Location.right)
                    {
                        n *= 100;
                        decimalPosition--;
                    }
                    else if (shift == Location.left)
                    {
                        n /= 100;
                        decimalPosition++;
                    }
                    if (n >= 1 && n <= 100)
                        break;
                }
            }

            // multiply original by 100 until is a natural
            n = originalNumber;
            while (!IsNaturalNumber(n))
            {
                n *= 100;
            }

            // calculate root
            var b = (double)SqrtDigitExpansion((int)n, digits);

            // add decimal
            if (!IsPerferctSquare((double)originalNumber))
            {
                // Decimal position indicate where to put the decimal in the square root
                b = ShiftDecimal(b, decimalPosition);
            }

            return b;
        }

        /// <summary>
        /// Same as <see cref="GetSqrtBySubstraction"/> but just expressing value as a string of numbers
        /// </summary>
        /// <param name="n"></param>
        /// <param name="digits"></param>
        /// <returns></returns>
        public static BigInteger SqrtDigitExpansion(double n, int digits)
        {
            int DigitCount(BigInteger number)
            {
                var s = number.ToString();

                var c = s.Length;
                if (s.Contains("."))
                    c--;

                return c;
            }

            BigInteger InsertZero(BigInteger number)
            {
                var s = number.ToString();

                s = s.Insert(s.Length - 1, "0");

                return BigInteger.Parse(s);
            }

            BigInteger RemoveLastZero(BigInteger number)
            {
                var s = number.ToString();
                var position = s.Length - 2;

                if (s[position] == '0')
                    s = s.Remove(position, 1);

                return BigInteger.Parse(s);
            }

            BigInteger RemoveLastDigit(BigInteger number)
            {
                var s = number.ToString();
                var position = s.Length - 1;

                s = s.Remove(position);

                return BigInteger.Parse(s);
            }

            ///  <seealso cref="https://www.mathwarehouse.com/arithmetic/numbers/what-is-a-perfect-square.php"/>
            bool isPerfectSquare = false;
            BigInteger a = 5 * (BigInteger)n;
            BigInteger b = 5;

            while (true)
            {
                if (a >= b)
                {
                    a -= b;
                    b += 10;
                }
                else
                {
                    a *= 100;
                    b = InsertZero(b);
                }
                if (a == 0)
                {
                    isPerfectSquare = true;
                    break;
                }

                if (DigitCount(b) > digits + 1)
                    break;
            }

            if (!isPerfectSquare)
                b = RemoveLastZero(b);

            return RemoveLastDigit(b);
        }

        /// <summary>
        /// Bared on Babylonian algorithm
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static float GetSqrtBabylonianMethod(float n, float epsilon = 0.00001F)
        {
            /*We are using n itself as initial approximation
             This can definitely be improved */
            var x = n;
            float y = 1;
            var e = epsilon; /* e decides the accuracy level*/
            while (x - y > e)
            {
                x = (x + y) / 2;
                y = n / x;
            }
            return x;
        }

        // x - a number, from which we need to calculate the square root
        // epsilon - an accuracy of calculation of the root from our number.
        // The result of the calculations will differ from an actual value
        // of the root on less than epslion.
        public static decimal GetSqrtUsingNewtonMethod(decimal x, decimal epsilon = 0.0M)
        {
            if (x < 0) throw new OverflowException("Cannot calculate square root from a negative number");

            decimal current = (decimal)Math.Sqrt((double)x), previous;
            do
            {
                previous = current;
                if (previous == 0.0M) return 0;
                current = (previous + x / previous) / 2;
            }
            while (Math.Abs(previous - current) > epsilon);
            return current;
        }

        public static decimal PowerRecursive(int x, int y)
        {
            if (y == 0)
                return (decimal)1;
            else if (y % 2 == 0)
                return PowerRecursive(x, y / 2) * PowerRecursive(x, y / 2);
            else
                return x * PowerRecursive(x, y / 2) * PowerRecursive(x, y / 2);
        }

        public static decimal PowerUsingLoop(int b, int e)
        {
            decimal value = 1;
            for (int i = 0; i < e; i++)
            {
                value *= b;
            }
            return value;
        }

        /// <summary>
        /// Get smaller integer which is base of given number if expressed as power of this number
        /// </summary>
        /// <param name="number"></param>
        /// <param name="maxBaseToCheck">Upper boundary for bases to try to check</param>
        /// <returns>Smallest integer base, zero if base not found</returns>
        public static int GetPowerBaseOf(int number, int maxBaseToCheck = -1)
        {
            var e = 0;
            var max = (maxBaseToCheck != -1) ? maxBaseToCheck : int.MaxValue;

            for (int i = 2; i < max; i++)
            {
                if (i >= number)
                    break;

                var x = number;
                e = 0;
                while (x % i == 0)
                {
                    e++;
                    x /= i;
                }
                if (e == maxBaseToCheck)
                {
                    e = -1;
                    break;
                }
                else if (e > 0)
                    break;
            }
            return e;
        }

        /// <summary>
        /// For a range of powers check if number can be expressed as power of its length
        /// </summary>
        /// <param name="number"></param>
        /// <param name="upperPowerBound">Upper bound for power of number, number of digits</param>
        /// <param name="upperBaseBound">todo: describe upperBaseBound parameter on GetCountOfLengthPower</param>
        /// <returns></returns>
        public static int GetCountOfLengthPower(int number, int upperBaseBound, int upperPowerBound)
        {
            var count = 0;
            var b = UtilityMath.GetPowerBaseOf(number, upperBaseBound);

            for (int n = 1; n <= upperPowerBound; n++)
            {
                var x = UtilityMath.PowerUsingLoop(b, n);
                if (("" + x).ToCharArray().Length == n)
                {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// Determine if a large  number  expressed as a biginter is a perfect square. 
        /// <see cref="https://www.quora.com/How-can-I-check-if-a-BigInteger-is-a-perfect-square-in-C"/>
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static bool IsLargeNumberPerferctSquare(BigInteger number)
        {
            switch ((int)(number % 10))
            {
                case 0:
                case 1:
                case 4:
                case 5:
                case 6:
                case 9:
                    {
                        switch (NumericDigitSum(number, true))
                        {
                            case 1:
                            case 4:
                            case 7:
                            case 9:
                                {
                                    return true;
                                }
                        }
                        return false;
                    }
            }
            
            return false;
        }

        public static bool IsPerferctSquare(double number)
        {
            for (int i = 1; i * i <= number; i++)
            {
                // if (i * i = n) 
                if ((number % i == 0) && (number / i == i))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsPerferctSquare(BigInteger number)
        {
            for (int i = 1; i * i <= number; i++)
            {
                if ((number % i == 0) && (number / i == i))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsPerferctSquare(BigInteger number,BigInteger actualSqtr)
        {
            var expectedSqrt = number * number;
            if (expectedSqrt == actualSqtr)
            {
                return true;
            }
            
            return false;
        }


        /// <summary>
        /// Implementing algorithm described in https://en.wikipedia.org/wiki/Methods_of_computing_square_roots#Continued_fraction_expansion
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static int GetContinuedFractionExpansionPeriodicLengthForNonPerfectSquare(int number)
        {
            var period = 0;

            var sqrt = (int)Math.Sqrt(number);

            var d = 1;
            var m = 0;
            var a = sqrt;

            do
            {
                m = d * a - m;
                d = (number - m * m) / d;
                a = (sqrt + m) / d;
                period++;
            } while (a != 2 * sqrt);

            return period;
        }

        public static bool IsOdd<T>(T number)
        {
            var methodType = typeof(T);

            if (!UtilityType.IsNumericType(methodType))
                throw new ArgumentException("The odd/even distribution can only be determined for numereric types.");

            return (dynamic)number % 2 != 0;
        }

        public static bool IsEven<T>(T number)
        {
            var methodType = typeof(T);

            if (!UtilityType.IsNumericType(methodType))
                throw new ArgumentException("The odd/even distribution can only be determined for numereric types.");

            return (dynamic)number % 2 == 0;
        }

        public static Func<int, BigInteger> FibonacciRecursive = number =>
        {
            if (number == 1)
                return 1;
            if (number < 1)
                return 0;

            return FibonacciRecursive(number - 1) + FibonacciRecursive(number - 2);
        };

        public static BigInteger FibonacciCached(int number)
        {
            var fibonacci = FibonacciRecursive.Memoize();

            return fibonacci(number);
        }

        public static string FibonacciString(string number)
        {
            if (number == "1")
                return "1";
            if (number == "2")
                return "1";

            return AddLargeNumbers(FibonacciString(SubstractLargeNumbers(number, "1")), FibonacciString(SubstractLargeNumbers(number, "2")));
        }

        public static BigInteger FibonacciLoop(int number)
        {
            BigInteger prev = 1;
            BigInteger next = 1;

            if (number > 2)
            {
                // In 'number' steps compute Fibonacci sequence iteratively.
                for (int i = 1; i < number; i++)
                {
                    BigInteger sum = prev + next;
                    prev = next;
                    next = sum;
                }
            }
            return prev;
        }

        public static IEnumerable<BigInteger> FibonacciSequence(int x)
        {
            yield return FibonacciLoop(x);
        }

        public static string NumberToLetters(long number)
        {
            return NumberToLetters(number.ToString());
        }

        public static string NumberToLetters(string number)
        {
            var letters = new StringBuilder();

            List<string> parts = SplitNumberByGroups(number, 3);
            var sequence = parts.Count;
            parts.Reverse();

            foreach (var part in parts)
            {
                var word = ThreeDigitSequenceToLetters(part.PadLeft(3, '0'));
                switch (sequence)
                {
                    case 1:
                        {
                            letters.Append(word);
                            break;
                        }
                    case 2:
                        {
                            letters.Append(word);
                            letters.Append(" thousand ");
                            break;
                        }

                    default:
                        throw new Exception("Unexpected Case");
                }
                sequence--;
            }
            return letters.ToString();
        }

        public static long DigitsSum(string number)
        {
            var digits = number.ToCharArray();
            long sum = 0;
            long d;

            for (int i = 0; i < digits.Length; i++)
            {
                d = long.Parse(digits[i].ToString());
                sum += d;
            }

            return sum;
        }

        /// <summary>
        /// Get digits  in numbeil them and/or those in its sum add up to a  single digit.
        /// E.g: 637 -> 6+3+7=1+6=7
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="number"></param>
        /// <returns></returns>
        public static int NumericDigitSum<T>(T number, bool addUpToSingleDigit = false)
        {
            var methodType = typeof(T);

            if (!UtilityType.IsNumericType(methodType))
                throw new ArgumentException("NumericDigitSum can only be used on numeric arguments.");

            long sum = 0;
            dynamic n = number;

            while (n > 0)
            {
                sum += (n % 10);
                n /= 10;
            }

            if(addUpToSingleDigit)
                while (sum > 10)
                    sum = NumericDigitSum(sum);

            return (int)sum;

        }
        public static List<Tuple<int, int>> TwoDigitSummation(List<int> digits, int fix, int target)
        {
            var sums = new List<Tuple<int, int>>();
            var trial = 1;

            while (true)
            {
                var a = digits[0];
                digits.RemoveAt(0);

                for (int i = 0; i < digits.Count; i++)
                {
                    var x = target - fix - a;

                    if (digits.Contains(x))
                    {
                        sums.Add(new Tuple<int, int>(a, x));
                        digits.Remove(x);
                    }

                }
                trial++;

                if (digits.Count < 2)
                    break;                           
                if (trial == digits.Count)
                    break;
            }

            return sums;
        }

        public static long AlphabeticPositionSum(string word)
        {
            var characters = word.ToArray();
            long sum = 0;

            for (int i = 0; i < characters.Length; i++)
            {
                sum += (characters[i] - 64);
            }

            return sum;
        }

        public static BigInteger BinomialCoefficients(int n, int r)
        {
            BigInteger ncr = 1;

            if (r < n)
            {
                var fn = FactorialLoop<BigInteger>(n);
                var fr = FactorialLoop<BigInteger>(r);
                var fnr = FactorialLoop<BigInteger>(n - r);
                ncr = fn / (fr * fnr);
            }

            return ncr;
        }

        /// <summary>
        /// Calcuate factorial on big numbers representing as string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="number">Faaactorial number must be of a numeric type</param>
        /// <returns></returns>
        public static string FactorialOverFlow(string number)
        {
            if (number == "0")
                return "1";
            if (number == "1")
                return "1";

            return MultiplyLargeNumbers(number, FactorialOverFlow(SubstractLargeNumbers(number,"1")));
        }

        public static T FactorialLoop<T>(int number)
        {
            var methodType = typeof(T);

            if (UtilityType.IsNumericType(methodType))
            {
                var x = default(T);
                var factorial = (dynamic)x + 1;
                
                for (int i = 2; i <= number; i++)
                {
                    factorial *= i;
                }
                return factorial; 
            }
            else
                throw new InvalidOperationException(string.Format("{0} is invalid, loop-based Factorial can only operate on numeric types.", methodType));
        }

        public static Func<int, BigInteger> FactorialRecursive = number =>
        {
            if (number == 0)
                return 1;
            if (number == 1)
                return 1;

            return number * FactorialRecursive(number - 1);
        };

        private static BigInteger FactorialCached(int number)
        {
            var factorial = FactorialRecursive.Memoize();

            return factorial(number);
        }

        public static double EulerNumber(int precision)
        {
            double e = 0;

            for (int i = 0; i < precision; i++)
            {
                var factorial = UtilityMath.FactorialLoop<double>(precision);
                e = (1.0 / factorial) + e;
            }
            return e;
        }

        public static long CollatzChain(long start) //837799,3811.1818ms
        {
            long length = 0;
            var begin = start;

            while (begin > 1)
            {
                if (begin % 2 == 0) //even
                    begin /= 2;
                else
                    begin = (begin * 3) + 1;
                length++;
            }

            return length;
        }

        public static bool IsSubstringDivisible(string number, int[] indices, int divisor)
        {
            var sub = new StringBuilder();

            foreach (var index in indices)
            {
                if (index > 1)
                    sub.Append(number[index - 1].ToString());
            }
            var dividend = Convert.ToInt32(sub.ToString());

            return (dividend % divisor == 0);
        }

        /// <summary>
        /// Get the priodical sequence of fractional part of rational number defined in digit list
        /// </summary>
        /// <param name="digits">todo: describe digits parameter on GetPeriodicalSequence</param>
        /// <returns></returns>
        public static string GetPeriodicalSequence(int[] digits)
        {
            int upperLimit = digits.Length;
            var value = string.Join("", digits);
            var sequence = string.Empty;

            for (int i = 0; i < upperLimit; i++)
            {
                sequence += value[i];

                var tail = (i == upperLimit) ? string.Empty : value.Substring(i + 1);
                var sequenceTail = sequence.Repeat(tail.Length / sequence.Length + 1);
                sequenceTail = sequenceTail.Substring(0, tail.Length);

                if (!tail.Contains(sequence))
                    sequence = string.Empty;

                if (sequenceTail  == tail)
                    return sequence;
                else
                    continue;
            }                                            
            return string.Empty;
        }

        public static IEnumerable<int> GetDecimalList(int numerator, int divisor, int upperLimit)
        {
            var dividend = 10 * numerator;
            var count = 0;
            while (count < upperLimit && dividend != 0)
            {
                Math.DivRem(dividend, divisor, out int remainder);
                var digit = (dividend - remainder) / divisor;
                yield return digit;
                dividend = 10 * remainder;
                count++;
            }
        }

        public static int[] Encrypt(int[] message, int[] key)
        {
            var encryptedMessage = new int[message.Length];

            for (int i = 0; i < message.Length; i++)
            {
                encryptedMessage[i] = message[i] ^ key[i % key.Length];
            }
            return encryptedMessage;
        }
        public static string AddLargeNumbers(string leftValue, string rightValue)
        {

            string addition;

            // supportRepetendSyntax in iputs == false
            leftValue = leftValue.RemoveParenthesis();
            rightValue = rightValue.RemoveParenthesis();

            if (leftValue[0] == '-' && rightValue[0] == '-')
            {
                // addition rule: when adding  same sign values, add the vallus ang keep  the sign
                leftValue = leftValue.Remove(0, 1);
                rightValue = rightValue.Remove(0, 1);

                addition = StringNumberAddition(leftValue, rightValue);
                addition = addition.Insert(0, "-");
            }
            else if (leftValue[0] != '-' && rightValue[0] == '-')
            {
                rightValue = rightValue.Remove(0, 1);
                addition = SubstractLargeNumbers(leftValue, rightValue);
            }
            else if (leftValue[0] == '-' && rightValue[0] != '-')
            {
                leftValue = leftValue.Remove(0, 1);

                addition = SubstractLargeNumbers(rightValue, leftValue);
            }
            else
                addition = StringNumberAddition(leftValue, rightValue);
         
            return addition;
        }

        public static string AddLargeNumbers(string[] numbers)
        {
            var result = "0";

            foreach (var number in numbers)
                result = AddLargeNumbers(result, number);

            return result;
        }

        public static string SubstractLargeNumbers(string leftValue, string rightValue)
        {            
            string substraction;

            // supportRepetendSyntax in iputs == false
            leftValue = leftValue.RemoveParenthesis();
            rightValue = rightValue.RemoveParenthesis();

            if (leftValue[0] == '-' && rightValue[0] == '-')
            {
                // addition rule: when  substracting  same sign values, substract the alues ang keep  the sign
                leftValue = leftValue.Remove(0, 1);
                rightValue = rightValue.Remove(0, 1);

                if (CompareLargeNumbers(leftValue, rightValue) == -1)
                {
                    substraction = StringNumberSubstraction(rightValue, leftValue);
                    substraction = (substraction != "0") ? substraction.Insert(0, "-") : substraction;
                }
                else
                    substraction = StringNumberSubstraction(leftValue, rightValue);
            }
            else if (leftValue[0] != '-' && rightValue[0] == '-')
            {
                rightValue = rightValue.Remove(0, 1);

                substraction = StringNumberAddition(leftValue, rightValue);
            }
            else if (leftValue[0] == '-' && rightValue[0] != '-')
            {
                leftValue = leftValue.Remove(0, 1);

                if (CompareLargeNumbers(leftValue, rightValue) == -1)
                    substraction = StringNumberAddition(rightValue, leftValue);
                else
                    substraction = StringNumberSubstraction(leftValue, rightValue);
                // if  left number is  negative  then substraction is negative
                substraction = (substraction != "0") ? substraction.Insert(0, "-") : substraction;
            }
            else
                if (CompareLargeNumbers(leftValue, rightValue) == -1)
                {
                    substraction = StringNumberSubstraction(rightValue, leftValue);
                    substraction = (substraction != "0") ? substraction.Insert(0, "-") : substraction;
                }
                else
                    substraction = StringNumberSubstraction(leftValue, rightValue);

            return substraction;
        }

        public static string MultiplyLargeNumbers(ulong leftValue, ulong rightValue)
        {
            return MultiplyLargeNumbers(leftValue.ToString(), rightValue.ToString());
        }

        /// <summary>
        /// Multiple one-by-one digits and then insert decimal place.
        /// <see cref="https://www.mathsisfun.com/multiplying-decimals.html"/>
        /// </summary>
        /// <param name="rightValue"></param>
        /// <param name="leftValue"></param>
        /// <returns></returns>
        public static string MultiplyLargeNumbers(string leftValue, string rightValue, bool supportRepetendSyntax=true)
        {
            if (leftValue == "0" || rightValue == "0")
                return "0";

            // supportRepetendSyntax in iputs == false
            leftValue = leftValue.RemoveParenthesis();
            rightValue = rightValue.RemoveParenthesis();

            // process negative signs first
            bool isNegative = false;
            if (rightValue[0] == '-')
            {
                rightValue = rightValue.Remove(0, 1);
                isNegative = true;
            }
            if (leftValue[0] == '-')
            {
                leftValue = leftValue.Remove(0, 1);
                isNegative = !isNegative;
            }

            rightValue = rightValue.TrimStart('0');
            leftValue = leftValue.TrimStart('0');

            var rightDecimalPlace = rightValue.IndexOf('.');
            var leftDecimalPlace = leftValue.IndexOf('.');

            // remove decimal places on both sides
            // note: indexof was needed to remove the '.'s but the decimal place counts from the end
            if (rightDecimalPlace != -1)
            {
                rightValue = rightValue.Remove(rightDecimalPlace, 1);
                rightDecimalPlace = rightValue.Length - rightDecimalPlace;
            }
            else
            {
                rightDecimalPlace++;
            }

            if (leftDecimalPlace != -1)
            {
                leftValue = leftValue.Remove(leftDecimalPlace, 1);
                leftDecimalPlace = leftValue.Length - leftDecimalPlace;
            }
            else
            {
                leftDecimalPlace++;
            }

            //  calculate poduct decimal place
            var productDecimalPlace = rightDecimalPlace + leftDecimalPlace;

            var multiplication = ProcessAsMatrixToMultiplyLargeNumbers(leftValue, rightValue);

            // add multiplication decimal place
            if (productDecimalPlace > multiplication.Length)
                multiplication = multiplication.PadLeft(productDecimalPlace, '0');

            if (productDecimalPlace > 0)
            {
                multiplication = multiplication.Insert(multiplication.Length - productDecimalPlace, ".");
                multiplication = multiplication.TrimEnd('0');
            }

            int productDecimalIndex = ConvertLengthToIndex(productDecimalPlace, multiplication);
            multiplication = FormatArithmeticStringValue(multiplication, productDecimalIndex, isNegative);
            return multiplication;
        }

        private static int ConvertLengthToIndex(int decimalPlace, string number)
        {
            return ((number != null) ? number.Length : 0) - decimalPlace;
        }

        /// <summary>
        /// Perform dision of two big  string numbers using long division
        /// <see cref="https://www.dummies.com/education/math/basic-math/how-to-divide-big-numbers-with-long-division/"/>
        /// <seealso cref="https://www.geeksforgeeks.org/divide-large-number-represented-string/"/>
        /// <seealso cref="https://www.dummies.com/education/math/pre-algebra/how-to-divide-decimals/"/>
        /// TODO: Address issue when for cycle-analyisis quotioent length is less than round default
        /// </summary>
        /// <param name="rightValue"></param>
        /// <param name="leftValue"></param>
        /// <param name=" round">Some divisions have a great number of  decimals so truncate the
        /// response if it is above this number, by default do not round</param>
        /// <returns></returns> 
        public static string DivideLargeNumbers(string leftValue, string rightValue, out string remainder, int round=40, bool processRemainder=true, bool supportRepetendSyntax=true)
        {
            // only output a remainder as below when processReminder=false
            remainder = string.Empty;

            //supportRepetendSyntax in iputs == false
            leftValue = leftValue.RemoveParenthesis();
            rightValue = rightValue.RemoveParenthesis();

            if (rightValue == "0")
                throw new ArithmeticException("Cannot divide by zero.");
            if (leftValue == "0")
                return "0";

            // process decimal
            int divisorDecimalIndex = rightValue.IndexOf('.');
            int decimalShift = rightValue.DecimalNumberPart(divisorDecimalIndex).Length; 

            // move divisor decimal to the right
             int dividendDecimalIndex = leftValue.IndexOf('.');

            // if no dividend decimal add it, only if needed when divisor has decimal
            if (dividendDecimalIndex == -1 && divisorDecimalIndex  != -1)
            {
                leftValue += ".0";
                dividendDecimalIndex = leftValue.WholeNumberPart(dividendDecimalIndex).Length;
            }
            int divisionDecimalIndex = dividendDecimalIndex + decimalShift;

            // do not conider decimal positions
            if (dividendDecimalIndex > 0)
            {
                leftValue = leftValue.Remove(dividendDecimalIndex, 1);
                leftValue = leftValue.TrimStart('0');
            }
            if (divisorDecimalIndex > 0)
            {
                rightValue = rightValue.Remove(divisorDecimalIndex, 1);
                rightValue = rightValue.TrimStart('0');
            }

            // process negative signs first
            bool isNegative = false;
            if (rightValue[0] == '-')
            {
                rightValue = rightValue.Remove(0, 1);
                isNegative = true;
            }
            if (leftValue[0] == '-')
            {
                leftValue = leftValue.Remove(0, 1);
                isNegative = !isNegative;
            }

            var quotient = string.Empty;
            int i = 1;
            string dividend = leftValue.Substring(0, i);

            //  find prefix of leftValue that is larger than rightValue (divisor).
            // if dividend has not enough decimal placess to address shift then add zeroes
            while (CompareLargeNumbers(dividend, rightValue) == -1)
            {
                if (i >= leftValue.Length)
                {
                    leftValue += "0";
                    decimalShift++;
                }

                i++;
                dividend = leftValue.Substring(0, i);
            }

            // for every position skipped for devidend insert left side zero
            quotient += "0".Repeat(i - 1);

            // repeatedly divide dividend with ightValue. After
            // every division, update dividend to include one
            // more digit from leftValue.
            while (leftValue.Length > i)
            {
                (string q, string r) = IntegerDivision(dividend, rightValue);
                // keep result in answer i.e. dividend / rightValue
                quotient += q;

                 // take remainder and add next digit of number
                dividend = (r == "0") ? string.Empty : r;
                dividend += leftValue[i];

                i++;
            }
            (string quot, string rem) = IntegerDivision(dividend, rightValue);
            quotient += quot;

            // process decimal quotient
             string sequence = string.Empty;
            if (processRemainder)
            {
                if (new BigDecimal(rem) > BigDecimal.Zero)
                {

                    if (dividendDecimalIndex == -1)
                        divisionDecimalIndex = quotient.Length - decimalShift;

                    string remainderDigits = string.Empty; //track cyclic decimals
                    bool isCyclic = false;
                    do
                    {
                        remainderDigits += quot;
                        if (supportRepetendSyntax)
                            (isCyclic, sequence) = DetermineNumericCyclicalSequence(remainderDigits, quot);

                        // add zeroes until rightValue fits in dividend
                        dividend = rem + "0";

                        while (UtilityMath.CompareLargeNumbers(dividend, rightValue) == -1)
                        {
                            dividend += "0";
                            quotient += "0";
                            remainderDigits += "0";
                        }

                        (quot, rem) = IntegerDivision(dividend, rightValue);
                        quotient += quot;

                        if (round > 0 && remainderDigits.Length >= round)
                            break;
                    }
                    while (rem != "0" && !isCyclic);
                }
            }
            else
                remainder = rem;
   
            //if repetend  syntax is not supported, make sure there is no sequence evaluated
            if (!supportRepetendSyntax)
                sequence = string.Empty;

            string division = FormatArithmeticStringValue(quotient, divisionDecimalIndex, isNegative, sequence);
            return division;
        }

        public static string DivideLargeNumbers(string leftValue, string rightValue, int round = 40, bool processRemainder = true, bool supportRepetendSyntax = true)
        {
            return DivideLargeNumbers(leftValue, rightValue, out string r, round, processRemainder, supportRepetendSyntax);
        }

            /// </summary>
            /// A repeating decimal or recurring decimal is decimal representation of a number whose digits are periodic 
            /// (repeating its values at regular intervals) and the infinitely repeated portion is not zero, this method tells
            /// if a string of digits contains a sequence like  this and returns it too. 
            /// Start searching after sequence has been aappended at  least three times.
            /// </summary>
            /// <param name="digits"></param>
            /// <param name="quot"></param>
            /// <returns></returns>
            public static (bool IsCyclic, string Sequence) DetermineNumericCyclicalSequence(string digits, string quot)
        {
            bool match = false;
            char digit = quot[0];
            string sequence = string.Empty;

            // to determine a sequence asssume repeating group already starts three times
            if (digits.Count(c => c == digit) > 2)
            {
                var mark1 = digits.IndexOf(digit);
                var start1 = mark1 + 1;
                var mark2 = (mark1 != -1) ? digits.Substring(start1).IndexOf(digit) : -1;
                var start2 = start1 + mark2 + 1;
                var mark3 = (mark2 != -1) ? digits.Substring(start2).IndexOf(digit) : -1;

                int i = mark1;
                int j = start1 + mark2;
                var k = start2 + mark3;

                sequence = digits.Substring(i, j - i);
                var repeat = digits.Substring(j, k - j);

                if (repeat == sequence)
                    match = true;
                else
                    sequence = string.Empty;
            }
            return (match, sequence);
        }

        /// <summary>
        /// Calculate how many times, using *, number represented in rightValue fits in leftValue,
        /// and number that remains of leftValue minus this 'n' times. == leftValue / rightValue and
        /// leftValue % rightValue.
        /// </summary>
        /// <param name="leftValue"></param>
        /// <param name="rightValue"></param>
        /// <returns></returns>
        public static (string quotient, string remainder) IntegerDivision(string leftValue, string rightValue)
        {
            if (rightValue == "0")
                throw new ArithmeticException("Cannot divide by zero.");
            if (leftValue == "0")
                return ("0", "0");

            leftValue = leftValue.TrimStart('0');
            rightValue = rightValue.TrimStart('0');

            int i = 2;
            string aux = rightValue;
            string lastRemainder;
            do
            {
                 lastRemainder = (aux == "0") ? "0" : SubstractLargeNumbers(leftValue, aux);

                if (CompareLargeNumbers(lastRemainder, "0") == -1)
                {
                    lastRemainder = lastRemainder.Remove(0, 1);
                    return ("0", lastRemainder);
                }

                aux = MultiplyLargeNumberBySingleDigit(rightValue, i);
                i++;
            }
            while (CompareLargeNumbers(aux, leftValue)  != 1);
            i -= 2;

            return (i.ToString(), lastRemainder);
        }

        
        /// <summary>
        /// Return from numerical value comparison: 
        ///         0 if rightValue = LeftValue
        ///         1 if rightValue < LeftValue
        ///         -1 if rightValue > leftValue.
        /// <see cref="https://docs.microsoft.com/en-us/dotnet/api/system.icomparable.compareto?view=net-5.0"/>
        /// </summary>
        /// <param name="rightValue"></param>
        /// <param name="leftValue"></param>
        /// <returns></returns>
        public static int CompareLargeNumbers(string leftValue, string rightValue)
        {
            if (!leftValue.IsNumeric() || !rightValue.IsNumeric())
                throw new ArithmeticException($"Both values been compared: {rightValue}, {leftValue} must be numeric.");

            AlignValuesOfBinaryOpertation(ref leftValue, ref rightValue, false);

            int comparison = 0;

            if (rightValue[0] == '-' && leftValue[0] != '-')
            {
                comparison = 1;
                goto complete;
            }
            else if (rightValue[0] != '-' && leftValue[0] == '-')
            {
                comparison = -1;
                goto complete;
            }
            else if (rightValue[0] == '-' && leftValue[0] == '-')
            {
                // if both are negative, number line has opposite direction, so flip values
                string temp = leftValue;
                leftValue = rightValue.Remove(0, 1);
                rightValue = temp.Remove(0, 1);
            }

            leftValue = FormatArithmeticStringValue(leftValue);
            rightValue = FormatArithmeticStringValue(rightValue);

            var rightDecimalPlace = rightValue.IndexOf('.');
            var leftDecimalPlace = leftValue.IndexOf('.');

            var rightNumericPart = (rightDecimalPlace == -1) ? rightValue : rightValue.Substring(0, rightDecimalPlace);
            var leftNumericPart = (leftDecimalPlace == -1) ? leftValue : leftValue.Substring(0, leftDecimalPlace);

            var l = leftNumericPart.Length;
            var r = rightNumericPart.Length;

            if (l > r)
            {
                comparison = 1;
                goto complete;
            }
            else if (l < r)
            {
                comparison = -1;
                goto complete;
            }
            else
            {
                for (int i = 0; i < r; i++)
                {
                    var leftDigit = leftNumericPart[i];
                    var rightDigit = rightNumericPart[i];

                    if (leftDigit > rightDigit)
                    {
                        comparison = 1;
                        goto complete;
                    }
                    else if (leftDigit < rightDigit)
                    {
                        comparison = -1;
                        goto complete;
                    }
                }
                // at this point numric part are identical if they had decimals then compare those
                if (rightDecimalPlace > 0 || leftDecimalPlace > 0)
                {
                    var rightDecimalPart = (rightDecimalPlace == -1) ? rightValue : rightValue.Substring(rightDecimalPlace + 1);
                    var leftDecimalPart = (leftDecimalPlace == -1) ? leftValue : leftValue.Substring(leftDecimalPlace + 1);

                    l = leftDecimalPart.Length;
                    r = rightDecimalPart.Length;
                    // since decimal vaues are aligned then lengths are equal
                    for (int i = 0; i < r; i++)
                    {
                        var leftDigit = leftDecimalPart[i];
                        var rightDigit = rightDecimalPart[i];

                        if (leftDigit < rightDigit)
                        {
                            comparison = -1;
                            goto complete;
                        }
                        else if (leftDigit > rightDigit)
                        {
                            comparison = 1;
                            goto complete;
                        }
                    }
                    comparison = 0;
                    goto complete;
                }
            }
        complete:
            return comparison;
        }
        public static string ProcessAsMatrixToMultiplyLargeNumbers(string leftValue, string rightValue)
        {
            var operands = string.Empty;
            var stringOperands = new StringBuilder();
            stringOperands.Append(operands);


            for (int i = leftValue.Length - 1; i >= 0; i--)
            {
                if (leftValue[i] == '.')
                    continue;

                var digit = int.Parse(leftValue[i].ToString());
                var number = MultiplyLargeNumberBySingleDigit(rightValue, digit);
                
                //pad with zeroes for multiplication for partials of 'left' digits    
                var padRight = number.Length + (leftValue.Length - 1 - i);
                var paddedNumber = number.PadRight(padRight, '0');

                if (operands.Length > 0)
                    stringOperands.Append(",");

                stringOperands.Append(paddedNumber);

                operands = stringOperands.ToString();
            } 

            var numbers = operands.Split(',').ToArray<string>();

            var numericMatrix = LoadAsMatrix(numbers);

            //return AddMatrix(numericMatrix);
            string[] m = null;
            if (!numericMatrix.Any(row => row == null))
                m = numericMatrix.Select(r => string.Join("", r.ToArray())).ToArray();

            return AddLargeNumbers(m.ToArray());
        }

        /// <summary>
        /// The left-shift operator causes the bits in shift-expression to be shifted to the left by the number of positions specified by
        /// additive-expression. The bit positions that have been vacated by the shift operation are zero-filled. A left shift is a logical
        /// shift (the bits that are shifted off the end are discarded, including the sign bit)
        /// <see cref="https://docs.microsoft.com/en-us/cpp/cpp/left-shift-and-right-shift-operators-input-and-output?view=msvc-160"/>
        /// </summary>
        /// <param name="shiftExpression"></param>
        /// <param name="additiveExpressdion"></param>
        /// <param name="length"> minimal length of bits of original number</param>
        /// <returns></returns>
        public static BigDecimal BitLeftShift(BigDecimal  shiftExpression , int additiveExpressdion, int length=1000)
        {
            var binary = shiftExpression.ToBinary();
            BigDecimal result;

            binary = binary.PadLeft(length, '0');

            binary = binary.Remove(0, additiveExpressdion) + "0".Repeat(additiveExpressdion);
            result = BigDecimal.ConvertFromBinary(binary);

            return result;
        }

        /// <summary>
        /// When shifting right with a right shift, the least-significant bit is lost and  with 1s for negative values, else zeros inserted on the other end.
        /// <see cref="https://www.interviewcake.com/concept/java/bit-shift"/>
        /// </summary>
        /// <param name="shiftExpression"></param>
        /// <param name="additiveExpressdion"></param>
        /// <param name="type"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static BigDecimal BitRightShift(BigDecimal shiftExpression, int additiveExpressdion, int length = 1000)
        {
            var binary = shiftExpression.ToBinary();
            BigDecimal result;

            binary = binary.PadLeft(length, '0');
            
            string prefix;
            if (shiftExpression.IsNegative)
                prefix = "1".Repeat(additiveExpressdion);
            else
                prefix = "0".Repeat(additiveExpressdion);
            binary = prefix + binary.Remove(binary.Length - 1 - additiveExpressdion);
            
            result = BigDecimal.ConvertFromBinary(binary);

            return result;
        }

        /// <summary>
        /// Handle binary arithmetic operations with -,+,*/ and any precedence
        /// ordering arrangement using parenthesis, as:
        ///         a o b o c o d
        ///         a o  (b o c)  o  d 
        ///         a o b o  (c o d)
        ///         a o  (b o c o d)
        ///         (a o b)  o  (c o d)
        ///         And operands a,b,c,d can be negative.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static double EvaluateSimpleArithmeticOerations(string expression, int NumberOfDecimalsAllowedForArithmeticOperationParsing=4)
        {
            if(!CheckForValidArithmeticSyntax(expression, NumberOfDecimalsAllowedForArithmeticOperationParsing))
                throw new ArgumentException($"{expression} is not a valid expression.");

            double result = double.PositiveInfinity;

            //string pattern = @"(-?\d+)(\+|\/|\-|\*)(-?\d+)";
            int startCount = (NumberOfDecimalsAllowedForArithmeticOperationParsing == 0) ? 0 : 1;
            string pattern = $@"(-?\d+(\.\d{{{startCount},{NumberOfDecimalsAllowedForArithmeticOperationParsing}}})?)(\+|\/|\-|\*)(-?\d+(\.\d{{{startCount},{NumberOfDecimalsAllowedForArithmeticOperationParsing}}})?)";
            string originalExpression = "";
            var regexExpression = new Regex(pattern);
            bool found = false, matched = false;
            var match = regexExpression.Match(expression);

            if (match.Success)
            {
                if (match.Index > 0)
                {
                    found = true;
                    originalExpression = expression;
                    expression = match.Value;
                }
                if (match.Index == 0)
                    found = true;
            }

            if (found)
            {
                matched = true;
                (double a, double b, char op) = ParseOperation(match, NumberOfDecimalsAllowedForArithmeticOperationParsing);
                switch (op)
                {
                    case '+':
                        result = a + b;
                        break;
                    case '*':
                        result = a * b;
                        break;
                    case '-':
                        result = a - b;
                        break;
                    case '/':
                        {
                            if (b == 0)
                                throw new DivideByZeroException($"Division by zero in {expression}.");
                            else
                                result = a / b;
                                if (NumberOfDecimalsAllowedForArithmeticOperationParsing == 0 && result < 1) result = 0;
                            break;
                        }
                    default:
                        throw new ArgumentException($"{expression} contains a not recognized operator {op}.");
                }
                result = Math.Round(result, NumberOfDecimalsAllowedForArithmeticOperationParsing);

            }
            else if (!match.Success)
                throw new ArgumentException($"{expression} is not a valid expression.");

            if (matched && originalExpression == "")
                return Math.Round(Convert.ToDouble(result), NumberOfDecimalsAllowedForArithmeticOperationParsing);
            else
            {
                if (NumberOfDecimalsAllowedForArithmeticOperationParsing == 0 && result < 1)
                    return 0;
                pattern = "(" + expression + ")";
                if (!originalExpression.Contains(pattern))
                    throw new ArgumentException($"{pattern} is not a valid expression in {originalExpression}.");

                return EvaluateSimpleArithmeticOerations(originalExpression.Replace(pattern, result.ToString()), NumberOfDecimalsAllowedForArithmeticOperationParsing);
            }
        }

        /// <summary>
        /// Get produt of numeric characters in string number
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static long GetProduct(string number)
        {
            long p = 1;

            for (int i = 0; i < number.Length; i++)
            {
                if ((number[1] - 48) == 0)
                {
                    p = 0;
                    break;
                }
                p *= (number[i] - 48);
            }

            return p;
        }

        public static long GetSum(string number)
        {
            long s = 0;

            for (int i = 0; i < number.Length; i++)
            {
                var n = (number[1] - 48);

                if (n != 0)
                {
                    s += n;
                }
            }

            return s;
        }

        public static int GetAdjacentMaxproduct(int i, int j, int adjacent, int size, string[][] grid, CellList UsedCellBlocks)
        {
            var key = string.Empty;
            var maxProduct = int.MinValue;

            var CellBlock = new CellList(); //contain only adjacent cells for this coordinate


            //determine valid directions for adjacent cells
            var direction = SpiralMatrix.Direction.none;

            if (i >= 0 && i < size - adjacent)
            {
                direction |= SpiralMatrix.Direction.east;

                if (j >= 0 && j < size - adjacent)
                {
                    if ((direction & SpiralMatrix.Direction.east) == SpiralMatrix.Direction.east)
                        direction |= SpiralMatrix.Direction.southeast;

                    direction |= SpiralMatrix.Direction.south;
                }
                else
                {
                    if ((direction & SpiralMatrix.Direction.east) == SpiralMatrix.Direction.east)
                        direction |= SpiralMatrix.Direction.northeast;

                    direction |= SpiralMatrix.Direction.north;
                }
            }

            if (i < size && i > adjacent)
            {
                direction |= SpiralMatrix.Direction.west;

                if (j < size && j > adjacent)
                {
                    if ((direction & SpiralMatrix.Direction.west) == SpiralMatrix.Direction.west)
                        direction |= SpiralMatrix.Direction.northwest;

                    direction |= SpiralMatrix.Direction.north;
                }
                else
                {
                    if ((direction & SpiralMatrix.Direction.west) == SpiralMatrix.Direction.west)
                        direction |= SpiralMatrix.Direction.southwest;

                    direction |= SpiralMatrix.Direction.south;

                }
            }

            //build array with adjacent cells and add it to list for product calculation
            var directions = Enum.GetValues(typeof(SpiralMatrix.Direction));
            foreach (SpiralMatrix.Direction value in directions)
            {
                if ((direction & value) == value)
                {
                    Cells block;
                    var cells = new string[adjacent];

                    switch (value)
                    {
                        case SpiralMatrix.Direction.north:
                            {
                                for (int k = 0; k < adjacent; k++)
                                {
                                    key = key + i.ToString() + (j - k).ToString();
                                    cells[k] = grid[i][j - k];
                                }

                                break;
                            }
                        case SpiralMatrix.Direction.northeast:
                            {
                                for (int k = 0; k < adjacent; k++)
                                {
                                    key = key + (i + k).ToString() + (j - k).ToString();
                                    cells[k] = grid[i + k][j - k];
                                }
                                break;
                            }
                        case SpiralMatrix.Direction.northwest:
                            {
                                for (int k = 0; k < adjacent; k++)
                                {
                                    key = key + (i - k).ToString() + (j - k).ToString();
                                    cells[k] = grid[i - k][j - k];
                                }
                                break;
                            }
                        case SpiralMatrix.Direction.south:
                            {
                                for (int k = 0; k < adjacent; k++)
                                {
                                    key = key + i.ToString() + (j + k).ToString();
                                    cells[k] = grid[i][j + k];
                                }
                                break;
                            }
                        case SpiralMatrix.Direction.southeast:
                            {
                                for (int k = 0; k < adjacent; k++)
                                {
                                    key = key + (i + k).ToString() + (j + k).ToString();
                                    cells[k] = grid[i + k][j + k];
                                }
                                break;
                            }
                        case SpiralMatrix.Direction.southwest:
                            {
                                for (int k = 0; k < adjacent; k++)
                                {
                                    key = key + (i - k).ToString() + (j + k).ToString();
                                    cells[k] = grid[i - k][j + k];
                                }
                                break;
                            }
                        case SpiralMatrix.Direction.west:
                            {
                                for (int k = adjacent - 1; k >= 0; k--)
                                {
                                    key = key + (i - k).ToString() + j.ToString();
                                    cells[k] = grid[i - k][j];
                                }
                                break;
                            }
                        case SpiralMatrix.Direction.east:
                            {
                                for (int k = 0; k < adjacent; k++)
                                {
                                    key = key + (i + k).ToString() + j.ToString();
                                    cells[k] = grid[i + k][j];
                                }
                                break;
                            }
                        case SpiralMatrix.Direction.none:
                            {
                                break;
                            }
                        default:
                            {
                                throw new Exception("Unexpected Case");
                            }
                    }
                    block = new Cells(key, cells);
                    CalculateProduct(block, UsedCellBlocks);

                    CellBlock.Add(key, block);
                    key = string.Empty;
                }
            }

            //calculate maximum adjacell prodct and get the maximum
            foreach (Cells block in CellBlock.Values)
            {
                var p = block.Product;

                if (p > maxProduct) maxProduct = p;

                //add blocks to global list of calculate blocks
                if (!UsedCellBlocks.ContainsKey(block.Id))
                    UsedCellBlocks.Add(block.Id, block);
            }

            return maxProduct;
        }

        #endregion

        #region Private Methods
        private static string StringNumberAddition(string leftValue, string rightValue)
        {
            // check for empty or zero operands
            double.TryParse(rightValue, out double r);
            double.TryParse(leftValue, out double l);

            if (l == 0)
                return rightValue;
            if (r == 0)
                return leftValue;

            var result = new StringBuilder();
            var carryOn = 0;

            int sumDecimalPoint = AlignValuesOfBinaryOpertation(ref leftValue, ref rightValue);

            int max = Math.Max(rightValue.Length, leftValue.Length);
            for (int i = max - 1; i >= 0; i--)
            {
                if (rightValue[i] == '.')
                    continue;

                var digitRight = (i < rightValue.Length) ? rightValue[i] - 48 : 0;
                var digitLeft = (i < leftValue.Length) ? leftValue[i] - 48 : 0;
                var sum = (digitRight + digitLeft + carryOn).ToString();
                string digit;

                if (sum.Length == 1)
                {
                    digit = sum;
                    carryOn = 0;
                }
                else
                {
                    digit = sum[sum.Length - 1].ToString();
                    carryOn = Convert.ToInt32(sum.Substring(0, sum.Length - 1));
                }
                if (i == 0)
                {
                    result.Insert(0, digit);
                    if (carryOn != 0)
                    {
                        result.Insert(0, carryOn);
                        if (sumDecimalPoint != -1)
                            sumDecimalPoint++;
                    }
                }
                else
                    result.Insert(0, digit);
            }

            return FormatArithmeticStringValue(result.ToString(), sumDecimalPoint); ;
        }
        private static string StringNumberSubstraction(string leftValue, string rightValue)
        {
            if (leftValue == string.Empty)
                return rightValue;
            if (rightValue == string.Empty)
                return leftValue;

            var result = new StringBuilder();
            var carryOn = 0;
            int subsDecimalPoint = AlignValuesOfBinaryOpertation(ref leftValue, ref rightValue);

            var max = (rightValue.Length > leftValue.Length) ? rightValue.Length : leftValue.Length;
            for (int i = max - 1; i >= 0; i--)
            {
                if (rightValue[i] == '.')
                    continue;

                var digitLeft = leftValue[i] - 48;
                var digitRight = rightValue[i] - 48;

                // process carry-on
                var originalDigitLeft = digitLeft;
                digitLeft = (digitLeft == 0 && carryOn == 1) ? 10 : digitLeft;
                digitLeft -= carryOn;

                int dif;
                string digit;
                if (digitLeft == digitRight)
                {
                    digit = "0";
                    carryOn = 0;
                }
                else if (digitLeft > digitRight)
                {
                    dif = digitLeft - digitRight;
                    digit = dif.ToString();
                    carryOn = (originalDigitLeft == 0) ? 1 : 0;
                }
                else
                {
                    dif = 10 + (digitLeft - digitRight);
                    digit = dif.ToString();
                    carryOn = 1;
                }
                //if (i == 0 && carryOn == 1)
                //    throw new ArithmeticException($"{leftValue}-{rightValue}: generates a negative result.");

                result.Insert(0, digit);
            }

            return FormatArithmeticStringValue(result.ToString(), subsDecimalPoint);
        }

        private static T[][] InitializeMatrix<T>(int x, int y, T initialValue)
        {
            var nums = new T[x][];
            for (int i = 0; i < x; i++)
            {
                nums[i] = new T[y];
                for (int j = 0; j < y; j++)
                    nums[i][j] = initialValue;
            }
            return nums;
        }

        /// <summary>
        /// Insert numbers into matrix  of single digits, top to bottom, using the fact that 
        /// maytis pre-initialized
        /// </summary>
        /// <param name="numbers"></param>
        /// <returns></returns>
        public static int[][] LoadAsMatrix(string[] numbers)
        {
            // get matrix widtth
            int width = 0;
            foreach (var n in numbers)
            {
                int w = n.ToString().Length;
                if (w > width)
                    width = w;
            }
            var height = numbers.Length;

            var matrix = InitializeMatrix<int>(height, width, 0);
            // by default matrix is initialized with zeros therfore 
            // addressing trail number in matrix with zeros to align values of different magnitudes,
            // then load numbers right to left
            for (int i = 0; i < height; i++)
            {
                var number = numbers[i];

                int index = width - 1;

                for (int j = number.Length - 1; j >= 0; j--)
                {
                    if (j <= index)
                    {
                        matrix[i][index] = number[j] - 48;
                        index--;
                    }
                }
            }

            return matrix;
        }

        public static string AddMatrix(int[][] matrix)
        {
            var height = matrix.Length - 1;
            var width = matrix[height].Length - 1;
            var value = string.Empty;
            var carryon = 0;

            for (int j = width; j >= 0; j--)
            {
                int column = 0;

                for (int i = 0; i <= height; i++)
                {
                    column += matrix[i][j];
                }

                var number = (column + carryon).ToString();
                //column value
                value = value.Insert(0, number[number.Length - 1].ToString());
                //carry-on
                number = number.Remove(number.Length - 1, 1); // last
                carryon = Convert.ToInt32(number);
            }
            //if is the last column include the carry-on as part of the value
            value = carryon.ToString()  + value;

            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="valueDecimalPointIndex"></param>
        /// <param name="value"></param>
        /// <param name="repetend">A repeating decimal or recurring decimal is decimal representation of a number whose digits are periodic (repeating its values at regular intervals) and the infinitely repeated portion is not zero.
        /// Normallyonly a division may pass this.</param>
        /// <returns></returns>
        private static string FormatArithmeticStringValue(string value, int valueDecimalPointIndex = -1, bool isNegativeResult = false, string repetend = "")
        {
            if (value.Contains("-"))
                isNegativeResult = true;

            if (valueDecimalPointIndex != -1)
            {
                if (!value.Contains("."))
                    value = value.Insert(valueDecimalPointIndex, ".");

                var noDecimal = new Regex(@"\d+\.0+$");
                var match = noDecimal.Match(value);

                if (match.Success)
                {
                    value = value.TrimEnd('0');
                    value = value.Remove(value.Length - 1);
                }
            }

            var allLeadingZeroes = new Regex(@"^0{1,}.+$");
            if (allLeadingZeroes.IsMatch(value))
                value = value.TrimStart('0');

            if (value == "" || value == ".")
                value = "0";
            else if (value[0] == '.')
                value = value.Insert(0, "0");
            else if (value[value.Length - 1] == '.')
                value = value.TrimEnd('.');

            if (isNegativeResult && !value.Contains("-"))
                value = value.Insert(0, "-");

            // if a repetend is specified show in number using parennthesis notation
            if (repetend != string.Empty)
            {
                var i = value.IndexOf(repetend);
                if (i > -1)
                {
                    value = value.Remove(i);
                    value = $"{value}({repetend})";
                }
            }

            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rightValue"></param>
        /// <param name="leftValue"></param>
        /// <returns></returns>
        private static int AlignValuesOfBinaryOpertation(ref string rightValue, ref string leftValue, bool alignNumericParts=true)
        {
            var rightDecimalPointIndex = AlignStringArithmeticValues(ref rightValue, leftValue, alignNumericParts);
            var leftDecimalPointIndex = AlignStringArithmeticValues(ref leftValue, rightValue, alignNumericParts);

            // if both parts have no decimal neither the +/- result
            int decimalPointIndex = (rightDecimalPointIndex == -1 && leftDecimalPointIndex == -1) ? -1 : Math.Max(rightDecimalPointIndex, leftDecimalPointIndex);

            return decimalPointIndex;
        }

        private static int AlignStringArithmeticValues(ref string value, string secondaryValue, bool alignNumericParts = true)
        {
            // first process negative sign, since this affects string lengths
            if (secondaryValue[0] == '-')
                secondaryValue = secondaryValue.Remove(0, 1);
            bool isNegative = false;
            if (value[0] == '-')
            {
                isNegative = true;
                value = value.Remove(0, 1);
            }

            int i = secondaryValue.IndexOf('.');
            int secondaryNumeriPartValueLength = (i != -1) ? secondaryValue.Substring(0, i).Length : secondaryValue.Length;
            int secondaryDecimalPartValueLength = (i != -1) ? secondaryValue.Substring(i + 1).Length : 0;


            // First align real and decimal parts
            var valueDecimalPointIndex = value.IndexOf('.');

            if (valueDecimalPointIndex == -1)
            {
                value += ".0";
                valueDecimalPointIndex = value.IndexOf('.');
            }

            int max;
            string decimalPart = string.Empty;

            if (valueDecimalPointIndex != -1)
            {
                //  align decimal parts: pad numbers with zeroes to make them of equal length
                decimalPart = value.Substring(valueDecimalPointIndex + 1);
                max = Math.Max(decimalPart.Length, secondaryDecimalPartValueLength);
                decimalPart = decimalPart.PadRight(max, '0');
            }
            // align real parts
            var numericPart = (valueDecimalPointIndex == -1) ? value : value.Substring(0, valueDecimalPointIndex);
            if (alignNumericParts)
            {
                max = Math.Max(numericPart.Length, secondaryNumeriPartValueLength);
                numericPart = numericPart.PadLeft(max, '0');
            }

            value = $"{numericPart}.{decimalPart}";
            if (isNegative)
                value = value.Insert(0, "-");

            return valueDecimalPointIndex;
        }

        /// <summary>
        /// Check if expression, using parentheses to specify  precedence
        /// of operations: +, -, *, /
        /// <see cref="https://stackoverflow.com/questions/1631820/regular-expression-to-match-digits-and-basic-math-operators"/>
        /// <seealso cref="http://javaonlineguide.net/2013/03/how-to-check-parentheses-in-string-exp.html"/>
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        private static bool CheckForValidArithmeticSyntax(string expression, int NumberOfDecimalsAllowed)
        {
            // check opeeeeerands and operations without parentheses
            string pattern = $@"[-?(\d +)(\.\d\{{1,{NumberOfDecimalsAllowed}}})?)\-*\/\(\)]*$";
            var regexExpression = new Regex(pattern);
            var match = regexExpression.Match(expression);

            // check parentheses are baanced
            var stack = new DataStructures.Stack<char>();

            foreach (var c in expression)
            {
                switch (c)
                {
                    case '(': stack.Push(')'); break;
                    case ')':
                        {
                            if (stack.IsEmpty())
                                return false;

                            else if (stack.Peek() == c)
                            {
                                stack.Pop();
                            }
                            break;
                        }
                    default:
                        break;
                }
            }
            return match.Success && stack.IsEmpty();
        }

        private static (double a, double b, char op) ParseOperation(Match matches, int NumberOfDecimals)
        {
            // groups 2 and 5 are the decimal parts, 1 and 4 have the complete numbers
            double x = Math.Round(Convert.ToDouble(matches.Groups[1].Value), NumberOfDecimals);
            double y = Math.Round(Convert.ToDouble(matches.Groups[4].Value), NumberOfDecimals);
            char o = Convert.ToChar(matches.Groups[3].Value);

            return (x, y, o);
        }

        private static void CalculateProduct(Cells block, CellList blocks)
        {
            var p = 1;

            if (blocks.ContainsKey(block.Id))
            {
                var aux = blocks[block.Id];
                p = aux.Product;
            }
            else
            {
                foreach (string cell in block.Values)
                {
                    p *= Convert.ToInt32(cell);
                }
            }

            block.Product = p;
        }

        private static string ThreeDigitSequenceToLetters(string part)
        {
            var ones = string.Empty; //position=1
            var tens = string.Empty; //position=2
            var hundreds = string.Empty; //position=3
            var position = 3;

            foreach (char c in part)
            {
                if (position == 2)
                {
                    switch (c)
                    {
                        case '0':
                            {
                                tens = "";
                                break;
                            }
                        case '1':
                            {
                                tens = "ten";
                                break;
                            }
                        case '2':
                            {
                                tens = "twenty";
                                break;
                            }
                        case '3':
                            {
                                tens = "thirty";
                                break;
                            }
                        case '4':
                            {
                                tens = "forty";
                                break;
                            }
                        case '5':
                            {
                                tens = "fifty";
                                break;
                            }
                        case '6':
                            {
                                tens = "sixty";
                                break;
                            }
                        case '7':
                            {
                                tens = "seventy";
                                break;
                            }
                        case '8':
                            {
                                tens = "eighty";
                                break;
                            }
                        case '9':
                            {
                                tens = "ninety";
                                break;
                            }

                        default:
                            throw new Exception(string.Format("Invalid number '{0}' for tens position.", c));
                    }
                }
                else
                {
                    switch (c)
                    {
                        case '0':
                            {
                                if (position == 1) ones = "zero";
                                break;
                            }
                        case '1':
                            {
                                if (position == 1) ones = "one";
                                if (position == 3) hundreds = "one";
                                break;
                            }
                        case '2':
                            {
                                if (position == 1) ones = "two";
                                if (position == 3) hundreds = "two";
                                break;
                            }
                        case '3':
                            {
                                if (position == 1) ones = "three";
                                if (position == 3) hundreds = "three";
                                break;
                            }
                        case '4':
                            {
                                if (position == 1) ones = "four";
                                if (position == 3) hundreds = "four";
                                break;
                            }
                        case '5':
                            {
                                if (position == 1) ones = "five";
                                if (position == 3) hundreds = "five";
                                break;
                            }
                        case '6':
                            {
                                if (position == 1) ones = "six";
                                if (position == 3) hundreds = "six";
                                break;
                            }
                        case '7':
                            {
                                if (position == 1) ones = "seven";
                                if (position == 3) hundreds = "seven";
                                break;
                            }
                        case '8':
                            {
                                if (position == 1) ones = "eight";
                                if (position == 3) hundreds = "eight";
                                break;
                            }
                        case '9':
                            {
                                if (position == 1) ones = "nine";
                                if (position == 3) hundreds = "nine";
                                break;
                            }

                        default:
                            throw new Exception(string.Format("Invalid number '{0}' for ones or hundreds position.", c));
                    }
                }
                if (tens == "ten")
                {
                    switch (ones)
                    {
                        case "":
                            {
                                break;
                            }
                        case "zero":
                            {
                                tens = "ten";
                                break;
                            }
                        case "one":
                            {
                                tens = "eleven";
                                break;
                            }
                        case "two":
                            {
                                tens = "twelve";
                                break;
                            }
                        case "three":
                            {
                                tens = "thirteen";
                                break;
                            }
                        case "four":
                            {
                                tens = "fourteen";
                                break;
                            }
                        case "five":
                            {
                                tens = "fifteen";
                                break;
                            }
                        case "six":
                            {
                                tens = "sixteen";
                                break;
                            }
                        case "seven":
                            {
                                tens = "seventeen";
                                break;
                            }
                        case "eight":
                            {
                                tens = "eighteen";
                                break;
                            }
                        case "nine":
                            {
                                tens = "nineteen";
                                break;
                            }

                        default:
                            throw new Exception(string.Format("Invalid expression '{0}' for combination with tens position.", ones));
                    }
                    ones = string.Empty;
                }
                else
                {
                    if (ones == "zero") ones = string.Empty;
                }
                position--;
            }
            var letters = new StringBuilder();

            letters.AppendFormat("{0}{1}{2}", hundreds, (hundreds != string.Empty) ? " hundred " : string.Empty, (hundreds != string.Empty && (tens != string.Empty || ones != string.Empty)) ? " and " : string.Empty);
            letters.AppendFormat("{0}{1}", (tens != string.Empty) ? tens : string.Empty, (ones != string.Empty && tens != string.Empty) ? "-" : string.Empty);
            letters.Append((ones != string.Empty) ? ones : string.Empty);

            return letters.ToString();
        }

        private static List<string> SplitNumberByGroups(string number, int partLength)
        {
            var parts = new List<string>();
            var part = string.Empty;
            var count = 1;

            for (int i = number.Length - 1; i >= 0; i--)
            {
                var d = number[i].ToString();

                part = d + part;

                if (count % partLength == 0 || i == 0)
                {
                    parts.Add(part);
                    part = string.Empty;
                    count = 0;
                }
                count++;
            }
            return parts;
        }

        /// <summary>
        /// Multiyply a sttrimng of numbers using its carryover between single digit multiplications
        /// with 'd', while adding them to calculate total
        /// </summary>
        /// <param name="n"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        private static string MultiplyLargeNumberBySingleDigit(string n, int d)
        {
            var c = 0; //carry over
            var result = string.Empty;

            if (d == 0)
                return "0";
            if (d == 1)
                return new string(n.ToCharArray());

            for (int i = n.Length - 1; i >= 0; i--)
            {
                if (n[i] == '.')
                    continue;

                int digit = int.Parse(n[i].ToString());
                int m = (digit * d) + c;
                string s;

                if (i == 0)
                    s = m.ToString();
                else
                {
                    if (m < 10)
                    {
                        c = 0;
                        s = m.ToString();
                    }
                    else
                    {
                        c = m / 10;
                        s = (m % 10).ToString();
                    }
                }
                result = s + result;
            }
            return result;
        }

        private static int CalculatePentagonalRingSideSum(List<int> digits, int ringSize)
        {
            var outer = digits.Skip(ringSize).Sum();
            var inner = digits.Take(ringSize).Sum();

            return (2 * inner + outer) / ringSize;
        }

        private static int FindListSingleMatch(List<int> list1, List<int> list2)
        {
            var x = -1;

            try
            {
                x = (from v1 in list1
                     join v2 in list2
                     on v1 equals v2
                     select v1).DefaultIfEmpty(-1).SingleOrDefault();
            }
            catch (ArgumentNullException)
            {
                x = -1;
            }
            catch (InvalidOperationException)
            {
                x = -1;
            }

            return x;
        }


        private static void SwapValues<T>(T[] values, int pos1, int pos2)
        {
            if (pos1 != pos2)
            {
                var tmp = values[pos1];
                values[pos1] = values[pos2];
                values[pos2] = tmp;
            }
        }
        #endregion
    }
}
