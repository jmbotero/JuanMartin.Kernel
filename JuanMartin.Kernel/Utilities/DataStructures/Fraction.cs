using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JuanMartin.Kernel.Utilities.DataStructures
{
    public class Fraction : IComparable
    {
        private string _display;
        private decimal _value;

        public double Numerator { get; set; }
        public double Denominator { get; set; }

        public Fraction(double fractionNumerator, double fractionDenominator)
        {
            Numerator = fractionNumerator;
            Denominator = fractionDenominator;

            if (fractionDenominator == 0)
                throw new ArgumentOutOfRangeException("A fraction cannot be defined with a denominnnator of zero.");

            Refresh();
        }

        private void Refresh()
        {
            _value = GetValue();

            _display = String.Format("{0}/{1}", Numerator, Denominator);
        }

        private decimal GetValue()
        {
            //var quotient= Math.Floor(Numerator / (double)Denominator);
            //var remainder=Numerator % Denominator;

            //quotient = Math.DivRem(Numerator, Denominator, out remainder);

            return (decimal)((double)Numerator / Denominator); //(decimal)quotient + (decimal)remainder / (decimal)Denominator;
        }

        public decimal Value
        {
            get { return GetValue(); }
        }

        public bool LiteralEqual(Fraction other)
        {
            return Numerator == other.Numerator && Denominator == other.Denominator;
        }

        public override string ToString()
        {
            return _display;
        }

        public override bool Equals(object obj)
        {
            return Value == ((Fraction)obj).Value;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public int CompareTo(object other) => Value.CompareTo(((Fraction)other).Value);

        public static bool operator ==(Fraction leftSide, Fraction rightSide)
        {
            if ((object)rightSide == null && (object)leftSide == null)
                return true;
            else if ((object)rightSide == null && (object)leftSide != null || (object)rightSide != null && (object)leftSide == null)
                return false;

            return rightSide.Value == leftSide.Value;
        }

        public static bool operator !=(Fraction leftSide, Fraction rightSide)
        {
            if ((object)rightSide == null)
                return (object)leftSide != null;

            return rightSide.Value != leftSide.Value;
        }

        public static bool operator <(Fraction leftSide, Fraction rightSide)
        {
            if ((object)rightSide == null)
                throw new ArgumentNullException("Right hand-side operator in comparison should not be null.");
            else if ((object)leftSide == null)
                throw new ArgumentNullException("Left hand-side operator in comparison should not be null.");

            return leftSide.Value < rightSide.Value;
        }

        public static bool operator >(Fraction leftSide, Fraction rightSide)
        {
            if ((object)rightSide == null)
                throw new ArgumentNullException("Right hand-side operator in comparison should not be null.");
            else if ((object)leftSide == null)
                throw new ArgumentNullException("Left hand-side operator in comparison should not be null.");

            return leftSide.Value > rightSide.Value;
        }

    }
}
