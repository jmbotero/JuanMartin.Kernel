using System.Collections.Generic;
using System.Linq;

namespace JuanMartin.Kernel.Utilities
{
    public class UtilityList
    {
        public enum ItemRelativePosition
        {
            first = 0,
            last = 1
        };
        
        public static List<T> ListRepeatedElements<T>(T value, int count)
        {
            var ret = new List<T>(count);
            ret.AddRange(Enumerable.Repeat(value, count));
            return ret;
        }
    }
}
