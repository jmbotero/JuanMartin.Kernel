using System;
using System.Linq;
using JuanMartin.Kernel.Utilities;

namespace JuanMartin.Kernel.Utilities.DataStructures
{
    public struct BigDecimal
    {
        public string Number { get; set; }
        public string Decimal { get; private set; }
        public int DecimalPoint { get; private set; }


        public BigDecimal(string n) : this()
        {
            if (n.Count(c => c == '.') > 1)
                throw new ArgumentException("Inalid number contains multiple decimal points.");

            Number = n;
            DecimalPoint = 0;
            Decimal = "";

            if(n.Contains('.'))
            {
                DecimalPoint = n.IndexOf('.');
                if (DecimalPoint == n.Length - 1 )
                {
                    n = n.Substring(0, DecimalPoint);
                    DecimalPoint = 0;
                }
                else
                    Decimal = n.Substring(DecimalPoint + 1);
            }
        }

        public BigDecimal(int n) : this(n.ToString())
        { }

        public BigDecimal(decimal n) : this(n.ToString())
        { }

        public static BigDecimal operator +(BigDecimal a, BigDecimal b) => new BigDecimal(UtilityMath.AddLargeNumbers(a.Number, b.Number));
        public static BigDecimal operator -(BigDecimal a, BigDecimal b) => new BigDecimal(UtilityMath.SubstractLargeNumbers(a.Number, b.Number));

        public override string ToString()
        {
            return Number;
        }
    }

}
