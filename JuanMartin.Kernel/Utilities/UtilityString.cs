using JuanMartin.Kernel.Extesions;
using JuanMartin.Kernel.RuleEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace JuanMartin.Kernel.Utilities
{
    public class UtilityString
    {
        [Macro]
        public static Value Append(params Symbol[] Operands)
        {
            var result = new Value();
            var text = new StringBuilder();

            foreach (Symbol operand in Operands)
                text.Append(operand.Value.Result.ToString());

            result.Result = text.ToString();

            return result;
        }

        [Macro]
        public static Value Format(Symbol Format, params Symbol[] Operands)
        {
            var result = new Value();

            var args = new object[Operands.Count()];
            var format = (string)Format.Value.Result;
            var i = 0;

            foreach (Symbol operand in Operands)
            {
                args[i] = operand.Value.Result;
                i++;
            }

            result.Result = string.Format(format, args);

            return result;
        }

        public static string ReverseString(string s)
        {
            var arr = s.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }

        public static string ReverseString(string s, char delimiter)
        {
            var arr = s.Split(delimiter);
            Array.Reverse(arr);

            return string.Join(delimiter.ToString(), arr);
        }
        public static int GetAlphabeticValue(string word)
        {
            var letters = word.ToCharArray();
            var total = 0;

            for (int i = 0; i < letters.Length; i++)
            {
                var letter = letters[i] - 64;

                total += letter;
            }
            return total;
        }

        /// <summary>
        /// Returns string with the frequency of each character in the original word.
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static string GetCharacterFrequencyMap(string word)
        {
            var map = new StringBuilder();

            foreach (var c in word)
            {
                var count = word.Count(n => n == c);
                map.Append(count);
            }
            return map.ToString();
        }
        /// <summary>
        /// Convert zero-based index to letter based name
        /// <see cref="https://stackoverflow.com/questions/181596/how-to-convert-a-column-number-e-g-127-into-an-excel-column-e-g-aa"/>
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string GetColumnName(int index)
        {
            const byte AlphabetLength = 'Z' - 'A' + 1;
            string name = String.Empty;

            do
            {
                name = Convert.ToChar('A' + index % AlphabetLength) + name;
                index = index / AlphabetLength - 1;
            } while (index >= 0);

            return name;
        }

        public static bool ContainsDuplicates(string key)
        {
            var table = string.Empty;
            var result = false;
            var builder = new StringBuilder();
            builder.Append(table);
            foreach (char value in key)
            {
                if (table.IndexOf(value) == -1)
                {
                    builder.Append(value);
                    table = builder.ToString();
                }
                else
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// Check if guid in string representation has a valid form.
        /// </summary>
        /// <param name="guidString"></param>
        /// <returns></returns>
        public static bool IsGuid(string guidString)
        {
            bool isValid = false;
            if (!string.IsNullOrEmpty(guidString))
            {
                Regex isGuid =
                    new Regex(@"^({){0,1}[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}(}){0,1}$"
                        , RegexOptions.Compiled);

                if (isGuid.IsMatch(guidString))
                {
                    isValid = true;
                }
            }
            return isValid;
        }

        public static bool IsPalindrome(string sequence)
        {
            var isPalindrome = true;
            var length = sequence.Length;
            var midpoint = length / 2;

            sequence = sequence.ToLower();
            if (length > 1) //only check in strings with more than 1 character
            {
                if (length % 2 == 1) //if string length is odd increse midpoint in 1
                    midpoint++;

                for (int i = 0; i < midpoint; i++)
                {
                    if (sequence[i] != sequence[(length - 1) - i])
                    {
                        isPalindrome = false;
                        break;
                    }
                }
            }

            return isPalindrome;
        }

        public static string[] GeneratePermutations<T>(T[] digits, int limit = -1)
        {
            var set = new List<string>();
            var number = new StringBuilder();

            foreach (var d0 in digits)
            {
                var digits1 = digits.Remove(d0);
                if (digits1.Length == 0)
                {
                    number.Append(d0);

                    if (set.Count <= limit || limit == -1)
                        set.Add(number.ToString());
                    number.Clear();
                }
                else
                {
                    foreach (var d1 in digits1)
                    {
                        var digits2 = digits1.Remove(d1);
                        if (digits2.Length == 0)
                        {
                            number.Append(d0);
                            number.Append(d1);

                            if (set.Count <= limit || limit == -1)
                                set.Add(number.ToString());
                            number.Clear();
                        }
                        else
                        {
                            foreach (var d2 in digits2)
                            {
                                var digits3 = digits2.Remove(d2);
                                if (digits3.Length == 0)
                                {
                                    number.Append(d0);
                                    number.Append(d1);
                                    number.Append(d2);

                                    if (set.Count <= limit || limit == -1)
                                        set.Add(number.ToString());
                                    number.Clear();

                                }
                                else
                                {
                                    foreach (var d3 in digits3)
                                    {
                                        var digits4 = digits3.Remove(d3);
                                        if (digits4.Length == 0)
                                        {
                                            number.Append(d0);
                                            number.Append(d1);
                                            number.Append(d2);
                                            number.Append(d3);

                                            if (set.Count <= limit || limit == -1)
                                                set.Add(number.ToString());
                                            number.Clear();
                                        }
                                        else
                                        {
                                            foreach (var d4 in digits4)
                                            {
                                                var digits5 = digits4.Remove(d4);
                                                if (digits5.Length == 0)
                                                {
                                                    number.Append(d0);
                                                    number.Append(d1);
                                                    number.Append(d2);
                                                    number.Append(d3);
                                                    number.Append(d4);

                                                    if (set.Count <= limit || limit == -1)
                                                        set.Add(number.ToString());
                                                    number.Clear();
                                                }

                                                else
                                                {
                                                    foreach (var d5 in digits5)
                                                    {
                                                        var digits6 = digits5.Remove(d5);
                                                        if (digits6.Length == 0)
                                                        {
                                                            number.Append(d0);
                                                            number.Append(d1);
                                                            number.Append(d2);
                                                            number.Append(d3);
                                                            number.Append(d4);
                                                            number.Append(d5);

                                                            if (set.Count <= limit || limit == -1)
                                                                set.Add(number.ToString());
                                                            number.Clear();
                                                        }
                                                        else
                                                        {
                                                            foreach (var d6 in digits6)
                                                            {
                                                                var digits7 = digits6.Remove(d6);
                                                                if (digits7.Length == 0)
                                                                {
                                                                    number.Append(d0);
                                                                    number.Append(d1);
                                                                    number.Append(d2);
                                                                    number.Append(d3);
                                                                    number.Append(d4);
                                                                    number.Append(d5);
                                                                    number.Append(d6);

                                                                    if (set.Count <= limit || limit == -1)
                                                                        set.Add(number.ToString());
                                                                    number.Clear();
                                                                }
                                                                else
                                                                {
                                                                    foreach (var d7 in digits7)
                                                                    {
                                                                        var digits8 = digits7.Remove(d7);
                                                                        if (digits8.Length == 0)
                                                                        {
                                                                            number.Append(d0);
                                                                            number.Append(d1);
                                                                            number.Append(d2);
                                                                            number.Append(d3);
                                                                            number.Append(d4);
                                                                            number.Append(d5);
                                                                            number.Append(d6);
                                                                            number.Append(d7);

                                                                            if (set.Count <= limit || limit == -1)
                                                                                set.Add(number.ToString());
                                                                            number.Clear();
                                                                        }
                                                                        else
                                                                        {
                                                                            foreach (var d8 in digits8)
                                                                            {
                                                                                var digits9 = digits8.Remove(d8);
                                                                                if (digits9.Length == 0)
                                                                                {
                                                                                    number.Append(d0);
                                                                                    number.Append(d1);
                                                                                    number.Append(d2);
                                                                                    number.Append(d3);
                                                                                    number.Append(d4);
                                                                                    number.Append(d5);
                                                                                    number.Append(d6);
                                                                                    number.Append(d7);
                                                                                    number.Append(d8);

                                                                                    if (set.Count <= limit || limit == -1)
                                                                                        set.Add(number.ToString());
                                                                                    number.Clear();
                                                                                }
                                                                                else
                                                                                {
                                                                                    foreach (var d9 in digits9)
                                                                                    {
                                                                                        var digits0 = digits9.Remove(d9);
                                                                                        if (digits0.Length == 0)
                                                                                        {
                                                                                            number.Append(d0);
                                                                                            number.Append(d1);
                                                                                            number.Append(d2);
                                                                                            number.Append(d3);
                                                                                            number.Append(d4);
                                                                                            number.Append(d5);
                                                                                            number.Append(d6);
                                                                                            number.Append(d7);
                                                                                            number.Append(d8);
                                                                                            number.Append(d9);

                                                                                            if (set.Count <= limit || limit == -1)
                                                                                                set.Add(number.ToString());
                                                                                            number.Clear();
                                                                                        }

                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return set.ToArray();
        }
    }
}