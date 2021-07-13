using JuanMartin.Kernel.Utilities.DataStructures;
using JuanMartin.Kernel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Collections.Specialized;

namespace JuanMartin.Kernel.Extesions
{
    public static class CollectionExtensions
    {
        #region OrderedDictionary
        public static T GetKey<T>(this OrderedDictionary dictionary, int index)
        {
            if (dictionary == null)
            {
                return default;
            }

            if (index < 0 || index > dictionary.Count)
                throw new IndexOutOfRangeException($"Specified index is out  of dictionaary item list bounds 0..{dictionary.Count} ->[{index}].");

            return (T)dictionary.Cast<DictionaryEntry>().ElementAt(index).Key;
        }

        public static T GetKeyOnValue<T, U>(this OrderedDictionary dictionary, U value)
        {
            if (dictionary == null)
            {
                return default;
            }

            try
            {
                var duplicateCount = dictionary.Cast<DictionaryEntry>().AsQueryable().Count(item => item.Value.Equals(value));

                if (duplicateCount > 1)
                    throw new InvalidOperationException($"Value {value} has {duplicateCount}duplicates use GetKey operation to narrow search by index.");

                return (T)dictionary.Cast<DictionaryEntry>().AsQueryable().Single(item

                    => ((U)item.Value).Equals(value)).Key;
            }
            catch (Exception)
            {
                return default;
            }
        }

        public static U GetValue<T, U>(this OrderedDictionary dictionary, T key)
        {
            if (dictionary == null)
            {
                return default;
            }

            try
            {
                return (U)dictionary.Cast<DictionaryEntry>().AsQueryable().Single(item

                    => ((T)item.Key).Equals(key)).Value;
            }
            catch (Exception)
            {
                return default;
            }
        }
        #endregion
        #region IEnumerable<T>, IEnumerable<List<T>>
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source, IEqualityComparer<T> comparer = null)
        {
            return new HashSet<T>(source, comparer);
        }

        public static bool HasDuplicates<T>(this IEnumerable<T> source)
        {
            var hashset = new HashSet<T>();
            foreach (var item in source)
            {
                if (!hashset.Add(item))
                {
                    return true;
                }
            }

            return false;
        }
        public static IEnumerable<(T item, int index)> Enumerate<T>(this IEnumerable<T> self)
             => self?.Select((item, index) => (item, index)) ?? new List<(T, int)>();

        public static List<T> Clone<T>(this IEnumerable<T> listToClone) where T : ICloneable
        {
            return listToClone.Select(item => (T)item.Clone()).ToList();
        }

        public static double Multiply<T>(this IEnumerable<T> source, Func<T, int> predicate = null)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source", "The sequence is null and contains no elements.");
            }

            // how to use predicate here???
            if (predicate == null)
            {
                var methodType = typeof(T);

                if (!UtilityType.IsNumericType(methodType))
                    throw new ArgumentException("Extension can only be used in numeric collections.");
            }
            double result = 1;

            if (source.Count() < 1)
                result = 0;
            else
            {
                foreach (var d in source)
                    result *= (predicate == null) ? (dynamic)d : (dynamic)predicate(d);
            }

            return result;
        }

        public static int InUse<T>(this IEnumerable<T> self)
        {
            var c = self.Where(x => (dynamic)x != default(T))
                        .Select(x => x)
                        .Count();

            return c;
        }

        public static bool ContantainsList<T>(this IEnumerable<List<T>> source, List<T> value, bool checkSortedValues = true) where T : IComparable
        {
            if (value != null)
            {
                if (checkSortedValues) value.Sort((a, b) => b.CompareTo(a)); // descending sort, .Sort((a, b) => a.CompareTo(b)); // ascending sort

                foreach (var item in source)
                {
                    if (item.Count == value.Count && item.SequenceEqual(value))
                        return true;
                }
            }
            return false;
        }
        #endregion

        #region Array<T>
        /// <summary>
        /// Assign value to array at index and set all elements after index to generic type default
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool Assign<T>(this T[] self, int index, T value)
        {
            var success = true;

            try
            {
                if (index <= self.Length - 1)
                {
                    self[index] = value;

                    if (index < self.Length - 1)
                    {
                        for (int i = self.Length - 1; i >= index + 1; i--)
                        {
                            self[i] = default;
                        }
                    }
                }
                else
                    throw new IndexOutOfRangeException($"Index ({index}) is greater than last position in array ({self.Length - 1}).");
            }
            catch (Exception)
            {
                success = false;
            }

            return success;
        }
        public static T[] LoopReverse<T>(this T[] source)
        {
            int len = source.Length;
            T[] destination = new T[len];
            Array.Copy(source, destination, len);
            for (var i = 0; i < len / 2; i++)
            {
                var aux = destination[i];
                destination[i] = destination[len - 1 - i];
                destination[len - 1 - i] = aux;
            }

            return destination;
        }
        public static T[] RemoveAt<T>(this T[] source, int index)
        {
            if (index < 0) return source;

            T[] destination = new T[source.Length - 1];
            if (index > 0)
                Array.Copy(source, 0, destination, 0, index);

            if (index < source.Length - 1)
                Array.Copy(source, index + 1, destination, index, source.Length - index - 1);

            return destination;
        }

        public static T[] Remove<T>(this T[] source, T element)
        {
            int index = Array.IndexOf<T>(source, element);

            return RemoveAt<T>(source, index);
        }

        public static T[] Concatenate<T>(this T[] first, T[] second)
        {
            if (first == null)
            {
                return second;
            }
            if (second == null)
            {
                return first;
            }

            return first.Concat(second).ToArray();
        }

        #endregion

        #region IList<T>, List<Vertex<T>>
        public static bool IsComplete<T>(this List<Vertex<T>> path)
        {
            var result = false;

            if (path != null)
            {
                var end = path.Last();
                if (end != null && end.Edges.Count == 0)
                    result = true;
            }
            return result;
        }

        public static bool IsSingle<T>(this List<Vertex<T>> path)
        {
            var result = false;

            if (path != null)
                result = path.All(v => v.OutgoingNeighbors().Count == 1);

            return result;
        }

        /// <summary>
        /// If value on index exists in current list exists replace it with given value i not add itt and remove all other values to the end of the list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool Replace<T>(this IList<T> self, T value, int index = -1)
        {
            var success = true;

            try
            {
                if (self.Count > 0 && index <= self.Count - 1)
                {
                    self[index] = value;
                    if (index < self.Count - 1)
                    {
                        for (int i = self.Count - 1; i >= index + 1; i--)
                        {
                            self.RemoveAt(i);
                        }
                    }
                }
                else
                    self.Add(value);
            }
            catch (Exception)
            {
                success = false;
            }

            return success;
        }
        #endregion
    }
}
