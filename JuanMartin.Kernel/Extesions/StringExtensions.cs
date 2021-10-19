using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JuanMartin.Kernel.Extesions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Using regular expressions is slower than a decimal.TryParse but  can operate on very large
        /// numbers. Note this is limited to use off '.' so check string locale settings
        /// <see cref="https://stackoverflow.com/questions/11431945/regular-expression-for-decimal-value"/>
        /// </summary>
        public static bool IsNumeric(this string value)
        {
            bool match;

            var numeric = new Regex(@"^-?[\d]+([.][\d]+)?$");
            match = numeric.IsMatch(value);
            match &= (value.Count(c => c == '.') <= 1);
            return match;
        }

        public static bool IsNullOrEmptyOrZero(this string source)
        {
            bool match = false;
            
            if(source!=null  && source != string.Empty)
            {
                var zeroes = new Regex(@"^0+$");
                match = zeroes.IsMatch(source);
            }

            return match;
        }
        public static string WholeNumberPart(this string source, int index = -1)
        {
            if (index == -1)
                index = source.IndexOf('.');

            return (index != -1) ? source.Substring(0, index) : source;
        }

        public static string DecimalNumberPart(this string source, int index = -1)
        {
            if (index == -1)
                index = source.IndexOf('.');

            return (index != -1) ? source.Substring(index + 1) : string.Empty;
        }
        public static char[] To(this char start, char end)
        {
            return Enumerable.Range(start, (end - start)).ToArray().Select(x => Convert.ToChar(x)).ToArray();
        }

        public static T ToEnum<T>(this string enumValue) where T : struct, IConvertible
        {
            return (T)Enum.Parse(typeof(T), enumValue);
        }
        public static IOrderedEnumerable<KeyValuePair<char, double>> Frequency(this string message)
        {
            var characters = message.ToCharArray();
            var myDict = characters.GroupBy(x => x).ToDictionary(g => g.Key, g => (double)g.Count() / characters.Count());
            var sortedDict = from entry in myDict orderby entry.Value descending select entry;

            return sortedDict;
        }
        public static string GetSmallestPermutation(this long number)
        {
            return SmallestPermutation(number);
        }

        public static string[] GetPermutations(this long number)
        {
            var digits = Array.ConvertAll(number.ToString("0").ToCharArray(), ch => ch.ToString()); // - '0');

            return Permutations(digits);
        }

        private static string[] Permutations(string[] digits)
        {
            var numbers = new List<string>();
            for (int i = 0; i < digits.Length; i++)
            {
                var digit = digits[i];
                var temp = new List<string>(digits);
                if (temp.Count == 1)
                {
                    numbers.Add(digit);
                }
                else if (temp.Count > 1)
                {
                    temp.RemoveAt(i);
                    var result = Permutations(temp.ToArray());
                    foreach (var r in result)
                    {
                        var value = r + digit;
                        numbers.Add(value);
                    }
                }
            }
            return numbers.ToArray();
        }

        private static string SmallestPermutation(long number)
        {
            var k = number;
            var digits = new int[10];
            long retVal = 0;

            while (k > 0)
            {
                digits[k % 10]++;
                k /= 10;
            }

            for (int i = 9; i >= 0; i--)
            {
                for (int j = 0; j < digits[i]; j++)
                {
                    retVal = retVal * 10 + i;
                }
            }

            return retVal.ToString().PadLeft(digits.Length, '0');
            //var min = long.MaxValue;
            //for (int i = 0; i < digits.Length; i++)
            //{
            //    var digit = digits[i];
            //    var temp = new LinkedList<string>(digits);
            //    if (temp.Count == 1)
            //    {
            //        var d = long.Parse(digit);
            //        if (d<min)
            //            min = d;
            //    }
            //    else if (temp.Count > 1)
            //    {
            //        temp.RemoveAt(i);
            //        var r = GeneratePermutations(temp.ToArray());
            //        var v = long.Parse(value);
            //        var value = r + digit;
            //        if (v<min)
            //            min = v;
            //    }
            //}
            //return min.ToString().PadLeft(digits.Length,'0');
        }

        public static string Repeat(this string value, int count)
        {
            return new StringBuilder(value.Length * count).Insert(0, value, count).ToString();
        }
        //public static string Repeat(this string input, int count)
        //{
        //    if (!string.IsNullOrEmpty(input))
        //    {
        //        var builder = new StringBuilder(input.Length * count);

        //        for (int i = 0; i < count; i++) builder.Append(input);

        //        return builder.ToString();
        //    }

        //    return string.Empty;
        //}

        public static string Tail(this string source, int tailLength)
        {
            if (tailLength >= source.Length)
                return source;
            return source.Substring(source.Length - tailLength);
        }

        public static bool IsAnagram(this string source, string target)
        {
            var contains = 0;
            int length;
            if (source.Length != target.Length)
            {
                return false;
            }
            else
                length = source.Length;

            var s = source.ToCharArray();
            var t = target.ToCharArray();

            Array.Sort(s);
            Array.Sort(t);

            for (int i = 0; i < length; i++)
            {
                var cs = s[i];
                var ct = t[i];
                if (cs == ct)
                {
                    contains++;
                }
            }

            return (contains == length);
        }

        public static IEnumerable<int> SplitIntoNumericParts(this String s, Int32 partLength)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));
            if (partLength <= 0)
                throw new ArgumentException(string.Format("Part length has to be positive ({0}).", partLength));

            for (var i = 0; i < s.Length; i += partLength)
            {
                yield return Convert.ToInt32(s.Substring(i, Math.Min(partLength, s.Length - i)));
            }
        }

        public static string RemoveParenthesis(this string s)
        {
            char[] sResultChars = new char[s.Length];
            int sResultCharsIndex = 0;

            if (s.IndexOfAny(new char[] { '(', ')' }) != -1)
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (s[i] != '(' && s[i] != ')')
                    {
                        sResultChars[sResultCharsIndex] = s[i];
                        sResultCharsIndex++;
                    }
                }

                return new string(sResultChars, 0, sResultCharsIndex);
            }
            else
                return s;
        } 
     }
}
