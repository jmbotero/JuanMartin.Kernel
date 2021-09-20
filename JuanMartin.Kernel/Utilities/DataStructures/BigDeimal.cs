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
        public string Number { get; private set; }
        public string Whole { get; private set; }
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

        public static BigDecimal operator +(BigDecimal a, BigDecimal b) => new BigDecimal(UtilityMath.AddLargeNumbers(a.Number, b.Number));
        public static BigDecimal operator -(BigDecimal a, BigDecimal b) => new BigDecimal(UtilityMath.SubstractLargeNumbers(a.Number, b.Number));
        public static BigDecimal operator *(BigDecimal a, BigDecimal b) => new BigDecimal(UtilityMath.MultiplyLargeNumbers(a.Number, b.Number));
        public static BigDecimal operator /(BigDecimal a, BigDecimal b) => new BigDecimal(UtilityMath.DivideLargeNumbers(a.Number, b.Number,supportRepetendSyntax: false));

        // operate with integers
        public static BigDecimal operator +(BigDecimal a, int b) => new BigDecimal(UtilityMath.AddLargeNumbers(a.Number, b.ToString()));
        public static BigDecimal operator -(BigDecimal a, int b) => new BigDecimal(UtilityMath.SubstractLargeNumbers(a.Number, b.ToString()));
        public static BigDecimal operator *(BigDecimal a, int b) => new BigDecimal(UtilityMath.MultiplyLargeNumbers(a.Number, b.ToString()));
        public static BigDecimal operator /(BigDecimal a, int b) => new BigDecimal(UtilityMath.DivideLargeNumbers(a.Number, b.ToString(),supportRepetendSyntax:false));

        public static bool operator ==(BigDecimal a, BigDecimal b) => UtilityMath.CompareLargeNumbers(a.Number, b.Number) == 0;
        public static bool operator !=(BigDecimal a, BigDecimal b) => UtilityMath.CompareLargeNumbers(a.Number, b.Number) != 0;
        public static bool operator <(BigDecimal a, BigDecimal b) => UtilityMath.CompareLargeNumbers(a.Number, b.Number) == -1;
        public static bool operator >(BigDecimal a, BigDecimal b) => UtilityMath.CompareLargeNumbers(a.Number, b.Number) == 1;

        // boolean comparisons with integers
        public static bool operator ==(BigDecimal a, int b) => UtilityMath.CompareLargeNumbers(a.Number, b.ToString()) == 0;
        public static bool operator !=(BigDecimal a, int b) => UtilityMath.CompareLargeNumbers(a.Number, b.ToString()) != 0;
        public static bool operator <(BigDecimal a, int b) => UtilityMath.CompareLargeNumbers(a.Number, b.ToString()) == -1;
        public static bool operator >(BigDecimal a, int b) => UtilityMath.CompareLargeNumbers(a.Number, b.ToString()) == 1;

        public BigDecimal Divide(BigDecimal divisor)
        {
            return new BigDecimal(UtilityMath.DivideLargeNumbers(this.Number, divisor.Number));
        }
        public BigInteger GetWholePartAsBigInteger()
        {
            return BigInteger.Parse(Whole);
        }

        /// <summary>
        /// Rounds a decimal value to a specified number of fractional digits, and rounds midpoint values to the nearest even number.
        /// </summary>
        /// <param name="digits">Default is five digits</param>
        /// <returns></returns>
        public string Round(int  digits = 5)
        {
            if (Decimal.Length > digits)
            {
                var value = Convert.ToInt32(Decimal[digits] + "");

                Decimal = Decimal.Remove(digits);
                DecimalPlaces = Decimal.Length;

                if (value > 5)
                {
                    Decimal = UtilityMath.AddLargeNumbers(Decimal, "1");
                    Decimal = Decimal.TrimEnd('0');

                    // if round overflows the decimal, eliminate it and increase whole part
                    if(Decimal == "1")
                    {
                        Decimal = string.Empty;
                        Whole = UtilityMath.AddLargeNumbers(Whole, "1");
                    }
                }
                Decimal = Decimal.TrimEnd('0');
            }
            Number = (Decimal == string.Empty) ? Whole : $"{Whole}.{Decimal}";

             return Number;
        }

        public BigDecimal Parse(string value)
        {
            Number = value;

            if (value.Contains('.'))
            {
                var i = value.IndexOf('.');
                Decimal = value.Substring(i + 1);
                Whole = value.Substring(0, i);
            }
            else
                Whole = value;

            if (Whole.Contains('-'))
            {
                Whole = Whole.Remove(0, 1);
                IsNegative = true;
            }

            return this;
        }
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
            return  Number;
        }

        public int CompareTo(BigDecimal other)
        {
            return UtilityMath.CompareLargeNumbers(other.Number, this.Number);
        }

        public BigDecimal Sqrt()
        {
            BigDecimal number = this;

            if (number < 0) return BigDecimal.Zero;

            BigDecimal root = number / 3;
            root.Round();

            int i;
            for (i = 0; i < 32; i++)
            {
                var x = number / root;
                x.Round();
                x += root;
                root = x / 2;
            }
            root = (root + number / root) / 2;

            return root;
        }
    }

}
