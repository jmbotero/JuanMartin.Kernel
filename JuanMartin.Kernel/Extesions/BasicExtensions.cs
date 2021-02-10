using System;
using System.IO;

namespace JuanMartin.Kernel.Extesions
{
    public static class BasicExtensions
    {
        public static bool Between<T>(this T source, T low, T high,bool inclusive=true) where T : IComparable
        {
            if(inclusive)
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
            if (Utilities.UtilityType.IsNumericType(number.GetType()))
            {
                dynamic ReverseNumber = 0;
                dynamic n = number;
                while (n > 0)
                {
                    ReverseNumber = (ReverseNumber * 10) + (n % 10);
                    n /= 10;
                }
                return ReverseNumber;
            }
            else
                throw new InvalidOperationException(string.Format("{0} is invalid, Reverse extension only applies to numeric types.", number.GetType()));
        }

    }
}
