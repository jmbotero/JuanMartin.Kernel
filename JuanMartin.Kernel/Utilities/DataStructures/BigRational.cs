using System;
using System.Numerics;

namespace JuanMartin.Kernel.Utilities.DataStructures
{
    public struct BigRational : IComparable<BigRational>
    {
        private readonly string _display;

        public BigInteger Numerator { get; set; }
        public BigInteger Denominator { get; set; }

        public BigRational(BigInteger fractionNumerator, BigInteger fractionDenominator) : this()
        {
            Numerator = fractionNumerator;
            Denominator = fractionDenominator;

            if (fractionDenominator == 0)
                throw new ArgumentOutOfRangeException("A fraction cannot be defined with a denominator of zero.");

            _display = String.Format("{0}/{1}", fractionNumerator, fractionDenominator);
        }

        private decimal GetValue()
        {
            BigInteger quotient;

            quotient = BigInteger.DivRem(Numerator, Denominator, out BigInteger remainder);

            return (decimal)quotient + (decimal)remainder / (decimal)Denominator;
        }

        public decimal Value
        {
            get { return GetValue(); }
        }

        public bool LiteralEqual(BigRational other)
        {
            return Numerator == other.Numerator && Denominator == other.Denominator;
        }

        public override string ToString()
        {
            return _display;
        }

        public override bool Equals(object obj)
        {
            return Value == ((BigRational)obj).Value;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public int CompareTo(BigRational other) => Value.CompareTo(other.Value);

        public static bool operator ==(BigRational leftSide, BigRational rightSide)
        {
            if ((object)rightSide == null && (object)leftSide == null)
                return true;
            else if ((object)rightSide == null && (object)leftSide != null || (object)rightSide != null && (object)leftSide == null)
                return false;

            return rightSide.Value == leftSide.Value;
        }

        public static bool operator !=(BigRational leftSide, BigRational rightSide)
        {
            if ((object)rightSide == null)
                return (object)leftSide != null;

            return rightSide.Value != leftSide.Value;
        }

    }
}
