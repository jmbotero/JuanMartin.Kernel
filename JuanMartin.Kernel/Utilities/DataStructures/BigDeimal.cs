using System;
using System.Linq;
using JuanMartin.Kernel.Utilities;
using JuanMartin.Kernel.Extesions;
using System.Numerics;

namespace JuanMartin.Kernel.Utilities.DataStructures
{
    /// <summary>
    /// Represents an arbitrarily large signed decimal
    /// not usign the E notation
    /// </summary>
    public struct BigDecimal : IComparable<BigDecimal>
    {
        public string Whole { get; set; }
        public string Decimal { get; private set; }
        public int DecimalPlaces
        {
            get
            {
                var i = Whole.IndexOf('.');

                // note: indexof was needed to remove the '.'s but the decimal place counts from the end
                if (i != -1)
                    i = Whole.Length - i;
                else
                    i++;

                return i;
            }

            private set { }
        }

        public bool SupportDecimalRepetendSyntax { get; set; }
        public static BigDecimal Zero => new BigDecimal("0"); 
        public bool IsNegative { get; private set; }

        public BigDecimal(string n) : this()
        {
            if (!n.IsNumeric())
                throw new ArgumentException($"String {n} does not represent a number.");

            DecimalPlaces = 0;
            Decimal = "";
            IsNegative = false;
            SupportDecimalRepetendSyntax = true;
            Parse(n);
        }

        public BigDecimal(int n) : this(n.ToString())
        { }

        public BigDecimal(long n) : this(n.ToString())
        { }

        public BigDecimal(decimal n) : this(n.ToString())
        { }

        public static BigDecimal operator +(BigDecimal a, BigDecimal b) => new BigDecimal(UtilityMath.AddLargeNumbers(a.Whole, b.Whole));
        public static BigDecimal operator -(BigDecimal a, BigDecimal b) => new BigDecimal(UtilityMath.SubstractLargeNumbers(a.Whole, b.Whole));
        public static BigDecimal operator *(BigDecimal a, BigDecimal b) => new BigDecimal(UtilityMath.MultiplyLargeNumbers(a.Whole, b.Whole));
        public static BigDecimal operator /(BigDecimal a, BigDecimal b) => new BigDecimal(UtilityMath.DivideLargeNumbers(a.Whole, b.Whole,supportRepetendSyntax: false));

        // operate with integers
        public static BigDecimal operator +(BigDecimal a, int b) => new BigDecimal(UtilityMath.AddLargeNumbers(a.Whole, b.ToString()));
        public static BigDecimal operator -(BigDecimal a, int b) => new BigDecimal(UtilityMath.SubstractLargeNumbers(a.Whole, b.ToString()));
        public static BigDecimal operator *(BigDecimal a, int b) => new BigDecimal(UtilityMath.MultiplyLargeNumbers(a.Whole, b.ToString()));
        public static BigDecimal operator /(BigDecimal a, int b) => new BigDecimal(UtilityMath.DivideLargeNumbers(a.Whole, b.ToString(),supportRepetendSyntax:false));

        public static bool operator ==(BigDecimal a, BigDecimal b) => UtilityMath.CompareLargeNumbers(a.Whole, b.Whole) == 0;
        public static bool operator !=(BigDecimal a, BigDecimal b) => UtilityMath.CompareLargeNumbers(a.Whole, b.Whole) != 0;
        public static bool operator <(BigDecimal a, BigDecimal b) => UtilityMath.CompareLargeNumbers(a.Whole, b.Whole) == -1;
        public static bool operator >(BigDecimal a, BigDecimal b) => UtilityMath.CompareLargeNumbers(a.Whole, b.Whole) == 1;

        // boolean comparisons with integers
        public static bool operator ==(BigDecimal a, int b) => UtilityMath.CompareLargeNumbers(a.Whole, b.ToString()) == 0;
        public static bool operator !=(BigDecimal a, int b) => UtilityMath.CompareLargeNumbers(a.Whole, b.ToString()) != 0;
        public static bool operator <(BigDecimal a, int b) => UtilityMath.CompareLargeNumbers(a.Whole, b.ToString()) == -1;
        public static bool operator >(BigDecimal a, int b) => UtilityMath.CompareLargeNumbers(a.Whole, b.ToString()) == 1;

        public BigDecimal Divide(BigDecimal divisor)
        {
            return new BigDecimal(UtilityMath.DivideLargeNumbers(this.Whole, divisor.Whole));
        }
        public BigInteger GetWholePartAsBigInteger()
        {
            return BigInteger.Parse(Whole);
        }

        public BigDecimal Parse(string value)
        {
            Whole = value;

            if (value.Contains('.'))
            {
                Decimal = value.Substring(value.IndexOf('.') + 1);
            }
            if (value.Contains('-'))
                IsNegative = true;

            return this;
        }
        public override bool Equals(object obj)
        {
            return UtilityMath.CompareLargeNumbers(this.Whole, ((BigDecimal)obj).Whole) == 0;
        }

        public override int GetHashCode()
        {
            return this.Whole.GetHashCode();
        }

        public override string ToString()
        {
            return Whole;
        }

        public int CompareTo(BigDecimal other)
        {
            return UtilityMath.CompareLargeNumbers(other.Whole, this.Whole);
        }
    }

}
