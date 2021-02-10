using System;

namespace JuanMartin.Kernel.Utilities
{
    public static class UtilityArray
    {
        /// <summary>
        /// Returns index of item if it is present in arr[l..r], else return -1 
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static int BinarySearch<T>(T[] arr, T item, int left = 0,int right = -1) where T : IComparable
        {
            if (right == -1) right = arr.Length - 1;

            if (right >= left)
            {
                int mid = left + (right - left) / 2;

                // If the element is present at the middle itself 
                if (arr[mid].Equals(item))
                    return mid;

                // If element is smaller than mid, then it can only be present in left subarray 
                if (arr[mid].CompareTo(item) > 0)
                    return BinarySearch(arr, item, left, mid - 1);

                // Else the element can only be present in right subarray 
                return BinarySearch(arr, item, mid + 1, right);
            }

            // We reach here when element is not present in array 
            return -1;
        }
    }
}
