using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JuanMartin.Kernel.Utilities.DataStructures
{
    public class Triple<T>
    {
        public T Item1 { get; set; }
        public T Item2 { get; set; }
        public T Item3 { get; set; }

        public Triple()
        {
            var methodType = typeof(T);

            if (!UtilityType.IsNumericType(methodType))
                throw new ArgumentException("Class only supports numeric tyepes.");
        }

        public Triple(T side_a, T side_b, T side_c)
            : this()
        {
            Item1 = side_a;
            Item2 = side_b;
            Item3 = side_c;
        }

        public T Sum()
        {
            //triple class will be used with basic numeric types, so instead of overloading the + binary operator, we can use dynamic to resolve it at runtime
            return (dynamic)Item1 + (dynamic)Item2 + (dynamic)Item3;
        }
    }
}
