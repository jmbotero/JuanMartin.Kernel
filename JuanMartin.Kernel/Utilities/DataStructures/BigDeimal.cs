using System;
using System.Linq;
using JuanMartin.Kernel.Utilities;

namespace JuanMartin.Kernel.Utilities.DataStructures
{
    public struct BigDecimal
    {
        public string Number { get; set; }
        public string Decimal { get; private set; }
        public int DecimalPlaces
        { 
            get
            {
                var i = Number.IndexOf('.');

                // note: indexof was needed to remove the '.'s but the decimal plce counts from the end
                if (i != -1)
                    i = Number.Length - i;
                else
                    i++;

                return i;
                }
            private set { } 
        }
        
        public BigDecimal(string n) : this()
        {
            if (n.Count(c => c == '.') > 1)
                throw new ArgumentException("Inalid number contains multiple decimal points.");

            Number = n;
            DecimalPlaces = 0;
            Decimal = "";

            if(n.Contains('.'))
            {
                    Decimal = n.Substring(n.IndexOf('.') + 1);
            }
        }

        public BigDecimal(int n) : this(n.ToString())
        { }

        public BigDecimal(decimal n) : this(n.ToString())
        { }

        public static BigDecimal operator +(BigDecimal a, BigDecimal b) => new BigDecimal(UtilityMath.AddLargeNumbers(a.Number, b.Number));
        public static BigDecimal operator -(BigDecimal a, BigDecimal b) => new BigDecimal(UtilityMath.SubstractLargeNumbers(a.Number, b.Number));
        public static BigDecimal operator *(BigDecimal a, BigDecimal b) => new BigDecimal(UtilityMath.MultiplyLargeNumbers(a.Number, b.Number));

        public static bool operator ==(BigDecimal a, BigDecimal b) => UtilityMath.CompareLargeNumbers(a.Number, b.Number) == 0;
        public static bool operator !=(BigDecimal a, BigDecimal b) => UtilityMath.CompareLargeNumbers(a.Number, b.Number) != 0;
        public static bool operator <(BigDecimal a, BigDecimal b) => UtilityMath.CompareLargeNumbers(a.Number, b.Number) == 1;
        public static bool operator >(BigDecimal a, BigDecimal b) => UtilityMath.CompareLargeNumbers(a.Number, b.Number) == -1;

        public override bool Equals(object obj)
        {
            return UtilityMath.CompareLargeNumbers(this.Number, ((BigDecimal)obj).Number) == 0;
        }

        public override int GetHashCode()
        {
            return this.Number.GetHashCode();
        }

        public override string ToString()
        {
            return Number;
        }
    }

}
