using System;
using System.Collections.Generic;
using System.IO;
using JuanMartin.Kernel.Utilities;
using Microsoft.Extensions.Configuration;

namespace JuanMartin.Kernel.Extesions
{
    public static class BasicExtensions
    {
        public static Dictionary<string, object> RemoveNullEntries(this Dictionary<string, object?> dictionary)
        {
            Dictionary < string, object> tmp = dictionary;

            foreach (var entry in tmp)
            {
                if(entry.Value==null)
                    tmp.Remove(entry.Key);
            }
            return tmp;
        }

        public static string GetStringConfigurationValue(this IConfiguration configuration, string settingName, string defaultValue, string sectionName = "")
        {
            string? aux;
            if (string.IsNullOrEmpty(sectionName))
                aux = configuration[sectionName];
            else
            {
                aux = null;
                var section = configuration.GetSection(sectionName);
                if (section != null)
                    aux = section[settingName];
            }
            return aux ?? defaultValue;
        }
        public static bool GetBooleanConfigurationValue(this IConfiguration configuration, string settingName, bool defaultValue, string sectionName = "")
        {
            string? aux;

            if (string.IsNullOrEmpty(sectionName))
                aux = configuration[sectionName];
            else
            {
                aux = null;
                var section = configuration.GetSection(sectionName);
                if (section != null)
                    aux = section[settingName];
            }

            if (aux != null)
                return Convert.ToBoolean(aux);

            return defaultValue;
        }
        public static int GetIntegerConfigurationValue(this IConfiguration configuration, string settingName, int defaultValue, string sectionName = "")
        {
            string? aux;

            if (string.IsNullOrEmpty(sectionName))
                aux = configuration[sectionName];
            else
            {
                aux = null;
                var section = configuration.GetSection(sectionName);
                if (section != null)
                    aux = section[settingName];
            }

            if (aux != null)
                return Convert.ToInt32(aux);

            return defaultValue;
        }
        /// <summary>
        /// Act like an implicit cast from a SQL data type to a C#/.NET data type.
        /// <see cref="https://stackoverflow.com/questions/870697/unable-to-cast-object-of-type-system-dbnull-to-type-system-string"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T ConvertFromSqlDataType<T>(object obj)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return default; // returns the default value for the type
            }
            else
            {
                return (T)obj;
            }
        }
        public static bool HasDuplicates<T>(this T number, int length=10)
        {
            if (!Utilities.UtilityType.IsNumericType(number.GetType()))
                throw new InvalidOperationException($"{number} is {number.GetType()}, and HasDuplicates extension only applies to numeric types.");

            dynamic n = number;

            // Flag to indicate digit has been used, all fase to start.
            int l = (length != 10) ? Utilities.UtilityMath.DigitCount(number) : length;
            var used = new bool[l];

            // Process each digit in number.
            while (n != 0)
            {
                var i = n % 10;

                // If duplicate, return true as soon as found.
                if (used[i]) return true;

                // Otherwise, mark used, go to next digit.
                used[i] = true;
                n /= 10;
            }

            // No duplicates after checking all digits, return false.
            return false;
        }

        private static int IsGrowthNumber(int number, UtilityMath.Growth growth)
        {
            if (number <= 0)
            {
                throw new InvalidOperationException("To decide if number is increasing or decreasing, it must be positive");
            }
            if (number < 100)
            {
                throw new InvalidOperationException("Clearly there cannot be any bouncy numbers below one-hundred");
            }

            int right = number % 10;
            number /= 10;
            int left = number % 10;
            number /= 10;

            while (true)
            {
                if (right != left)
                {
                    if (growth == UtilityMath.Growth.increase && left > right)
                        return 0;
                    if (growth == UtilityMath.Growth.decrease && left < right)
                        return 0;
                }
                if (number == 0)
                    break;

                right = left;
                left = number % 10;
                number /= 10;
            }
            return 1;
        }

        public static bool IsIncreasingNumber(this int number)
        {
            return IsGrowthNumber(number, UtilityMath.Growth.increase) == 1;
        }

        public static bool IsDecreasingNumber(this int number)
        {
            return IsGrowthNumber(number, UtilityMath.Growth.decrease) == 1;
        }

        public static int Sign<T>(this T number)
        {
            var methodType = typeof(T);

            if (!Utilities.UtilityType.IsNumericType(methodType))
                throw new InvalidOperationException(string.Format("{0} is invalid, Sign extension only applies to numeric types.", methodType));

            dynamic n = number;

            return (n >= 0) ?   1 : -1;
        }

        public static bool Between<T>(this T source, T low, T high,bool inclusive=true) where T : IComparable
        {
            var methodType = typeof(T);

            if (!Utilities.UtilityType.IsNumericType(methodType))
                throw new InvalidOperationException(string.Format("{0} is invalid, Between extension only applies to numeric types.", methodType));

            if (inclusive)
                return source.CompareTo(low) >= 0 && source.CompareTo(high) <= 0;
            else
                return source.CompareTo(low) > 0 && source.CompareTo(high) < 0;
        }

        /// <summary>
        ///     Returns the file name of the specified @this string without the extension.
        /// </summary>
        /// <param name="this">The @this of the file.</param>
        /// <returns>
        ///     The string returned by <see cref="M:System.IO.Path.GetFileName(System.String)" />, minus the last period (.)
        ///     and all characters following it.
        /// </returns>
        /// ###
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="this" /> contains one or more of the invalid
        ///     characters defined in
        ///     <see
        ///         cref="M:System.IO.Path.GetInvalidPathChars" />
        ///     .
        /// </exception>
        public static string GetFileNameWithoutExtension(this FileInfo @this)
        {
            return Path.GetFileNameWithoutExtension(@this.FullName);
        }

        /// <summary>
        /// Reverse digits in a number.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="number"></param>
        /// <returns></returns>
        public static T Reverse<T>(this T number)
        {
            var methodType = typeof(T);

            if (!Utilities.UtilityType.IsNumericType(methodType))
                throw new InvalidOperationException(string.Format("{0} is invalid, Reverse extension only applies to numeric types.", methodType));

            dynamic ReverseNumber = 0;
            dynamic n = number;
            while (n > 0)
            {
                ReverseNumber = (ReverseNumber * 10) + (n % 10);
                n /= 10;
            }
            return ReverseNumber;
        }

    }
}
