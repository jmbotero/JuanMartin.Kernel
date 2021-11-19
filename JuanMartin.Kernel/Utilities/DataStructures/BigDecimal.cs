using System;
using System.Linq;
using JuanMartin.Kernel.Utilities;
using JuanMartin.Kernel.Extesions;
using System.Numerics;
using System.Collections.Generic;

namespace JuanMartin.Kernel.Utilities.DataStructures
{
    /// <summary>
    /// Represents an arbitrarily large signed decimal
    /// not usign the E notation
    /// </summary>
    public struct BigDecimal : IComparable<BigDecimal>
    {
        private string _whole;
        private string _decimal;

        public string GetWhole()
        {
            return _whole;
        }

        private void SetWhole(string value)
        {
            _whole = value;
        }

        public string GetDecimal()
        {
            return (_decimal == string.Empty) ? "0" : _decimal;
        }

        private void SetDecimal(string value)
        {
            _decimal = value;
        }

        private string GetNumber()
        {
            string d = GetDecimal();
            string w = GetWhole();
            string number = (d == string.Empty) ? w : $"{w}.{d}";

            if (IsNegative)
                number = "-" + number;

            return number;
        }
        public int DecimalPlaces => (_decimal.IsNullOrEmptyOrZero()) ? 0 : GetDecimal().Length;

        public bool SupportDecimalRepetendSyntax { get; set; }
        public static BigDecimal Zero => new BigDecimal("0");
        public static BigDecimal One => new BigDecimal("1");
        public static BigDecimal Two => new BigDecimal("2");
        public static BigDecimal Epsilon => new BigDecimal("0.000000001");
        public bool IsNegative { get; private set; }

        public BigDecimal(string n) : this()
        {
            if (!n.IsNumeric())
                throw new ArgumentException($"String {n} does not represent a number.");

            SetDecimal("");
            IsNegative = false;
            SupportDecimalRepetendSyntax = true;
            ConvertFromString(n);
        }

        public BigDecimal(int n) : this(n.ToString())
        { }

        public BigDecimal(long n) : this(n.ToString())
        { }

        public BigDecimal(decimal n) : this(n.ToString())
        { }

        public BigDecimal(double n) : this(n.ToString())
        { }

        public static BigDecimal operator +(BigDecimal a, BigDecimal b) => new BigDecimal(UtilityMath.AddLargeNumbers(a.ToString(), b.ToString()));
        public static BigDecimal operator -(BigDecimal a, BigDecimal b) => new BigDecimal(UtilityMath.SubstractLargeNumbers(a.ToString(), b.ToString()));
        public static BigDecimal operator *(BigDecimal a, BigDecimal b) => new BigDecimal(UtilityMath.MultiplyLargeNumbers(a.ToString(), b.ToString()));
        public static BigDecimal operator /(BigDecimal a, BigDecimal b) => new BigDecimal(UtilityMath.DivideLargeNumbers(a.ToString(), b.ToString(), supportRepetendSyntax: false));

        // operate with doubles
        public static BigDecimal operator +(BigDecimal a, double b) => new BigDecimal(UtilityMath.AddLargeNumbers(a.ToString(), b.ToString()));
        public static BigDecimal operator -(BigDecimal a, double b) => new BigDecimal(UtilityMath.SubstractLargeNumbers(a.ToString(), b.ToString()));
        public static BigDecimal operator *(BigDecimal a, double b) => new BigDecimal(UtilityMath.MultiplyLargeNumbers(a.ToString(), b.ToString()));
        public static BigDecimal operator /(BigDecimal a, double b) => new BigDecimal(UtilityMath.DivideLargeNumbers(a.ToString(), b.ToString(), supportRepetendSyntax: false));

        // operate with integers
        public static BigDecimal operator <<(BigDecimal a, int b) => UtilityMath.BitLeftShift(a, b);
        public static BigDecimal operator >>(BigDecimal a, int b) => UtilityMath.BitRightShift(a, b);
        public static BigDecimal operator +(BigDecimal a, int b) => new BigDecimal(UtilityMath.AddLargeNumbers(a.ToString(), b.ToString()));
        public static BigDecimal operator +(int a, BigDecimal b) => new BigDecimal(UtilityMath.AddLargeNumbers(a.ToString(), b.ToString()));
        public static BigDecimal operator -(BigDecimal a, int b) => new BigDecimal(UtilityMath.SubstractLargeNumbers(a.ToString(), b.ToString()));
        public static BigDecimal operator -(int a, BigDecimal b) => new BigDecimal(UtilityMath.SubstractLargeNumbers(a.ToString(), b.ToString()));
        public static BigDecimal operator *(BigDecimal a, int b) => new BigDecimal(UtilityMath.MultiplyLargeNumbers(a.ToString(), b.ToString()));
        public static BigDecimal operator *(int a, BigDecimal b) => new BigDecimal(UtilityMath.MultiplyLargeNumbers(a.ToString(), b.ToString()));
        public static BigDecimal operator /(BigDecimal a, int b) => new BigDecimal(UtilityMath.DivideLargeNumbers(a.ToString(), b.ToString(), supportRepetendSyntax: false));
        public static BigDecimal operator /(int a, BigDecimal b) => new BigDecimal(UtilityMath.DivideLargeNumbers(a.ToString(), b.ToString(), supportRepetendSyntax: false));
        public static (string quotient, string remainder) operator %(BigDecimal a, int b) => UtilityMath.IntegerDivision(a.ToString(), b.ToString());
        public static (string quotient, string remainder) operator %(BigDecimal a, long b) => UtilityMath.IntegerDivision(a.ToString(), b.ToString());
        public static bool operator ==(BigDecimal a, BigDecimal b) => UtilityMath.CompareLargeNumbers(a.ToString(), b.ToString()) == 0;
        public static bool operator !=(BigDecimal a, BigDecimal b) => UtilityMath.CompareLargeNumbers(a.ToString(), b.ToString()) != 0;
        public static bool operator <(BigDecimal a, BigDecimal b) => UtilityMath.CompareLargeNumbers(a.ToString(), b.ToString()) == -1;
        public static bool operator >(BigDecimal a, BigDecimal b) => UtilityMath.CompareLargeNumbers(a.ToString(), b.ToString()) == 1;
        public static bool operator <=(BigDecimal a, BigDecimal b) => UtilityMath.CompareLargeNumbers(a.ToString(), b.ToString()) == 0 || UtilityMath.CompareLargeNumbers(a.ToString(), b.ToString()) == -1;
        public static bool operator >=(BigDecimal a, BigDecimal b) => UtilityMath.CompareLargeNumbers(a.ToString(), b.ToString()) == 0 || UtilityMath.CompareLargeNumbers(a.ToString(), b.ToString()) == 1;

        // boolean comparisons with integers
        public static bool operator ==(BigDecimal a, int b) => UtilityMath.CompareLargeNumbers(a.ToString(), b.ToString()) == 0;
        public static bool operator !=(BigDecimal a, int b) => UtilityMath.CompareLargeNumbers(a.ToString(), b.ToString()) != 0;
        public static bool operator <(BigDecimal a, int b) => UtilityMath.CompareLargeNumbers(a.ToString(), b.ToString()) == -1;
        public static bool operator >(BigDecimal a, int b) => UtilityMath.CompareLargeNumbers(a.ToString(), b.ToString()) == 1;
        public static bool operator <=(BigDecimal a, int b) => UtilityMath.CompareLargeNumbers(a.ToString(), b.ToString()) == 0 || UtilityMath.CompareLargeNumbers(a.ToString(), b.ToString()) == -1;
        public static bool operator >=(BigDecimal a, int b) => UtilityMath.CompareLargeNumbers(a.ToString(), b.ToString()) == 0 || UtilityMath.CompareLargeNumbers(a.ToString(), b.ToString()) == 1;

        public BigDecimal Divide(BigDecimal divisor)
        {
            return new BigDecimal(UtilityMath.DivideLargeNumbers(this.ToString(), divisor.ToString()));
        }
        public BigInteger GetWholePartAsBigInteger()
        {
            return BigInteger.Parse(GetWhole());
        }

        /// <summary>
        /// Returns the smallest integral value greater than or equal to the specified number.
        /// </summary>
        /// <returns></returns>
        public BigDecimal Cieling()
        {
            var n = new BigDecimal(GetWhole());

            if (GetDecimal().Length > 0)
                n += 1;

            n.IsNegative = this.IsNegative;

            return n;
        }

        /// <summary>
        ///  Returns the smallest integral value less than or equal to the specified number.
        /// </summary>
        /// <returns></returns>
        public BigDecimal Floor()
        {
            var n = new BigDecimal(GetWhole());

            n.IsNegative = this.IsNegative;

            return n;
        }


        /// <summary>
        /// If no digits are specified this method rounds a double-precision 
        /// floating-point value to the nearest integer value.
        /// </summary>
        /// <returns></returns>
        public BigDecimal Round(BigDecimal? value = null)
        {
            string w, d, next;

            if (value == null)
            {
                w = GetWhole();
                d = GetDecimal();
            }
            else
            {
                var n = value.ToString();
                w = n.WholeNumberPart();
                d = n.DecimalNumberPart();
            }
            string midpoint = "5";
            midpoint = midpoint.PadRight(d.Length, '0');

            if(UtilityMath.CompareLargeNumbers(d,midpoint) == 1)
                next = UtilityMath.AddLargeNumbers(w, "1");
            else
                next = w;

            if (value == null)
            {
                SetDecimal(string.Empty);
                SetWhole(next);

                return this;
            }
            else
                return new BigDecimal(next);
        }

        /// <summary>
        /// Rounds a decimal value to a specified number of fractional digits, and rounds midpoint values to the nearest even number.
        /// </summary>
        /// <param name="digits"></param>
        /// <returns></returns>
        public BigDecimal Round(int  digits, BigDecimal? value=null)
        {
            string w, d;

            if(value==null)
            {
                w = GetWhole();
                d =  GetDecimal();
            }
            else
            {
                var n = value.ToString();
                w = n.WholeNumberPart();
                d = n.DecimalNumberPart();
            }
            var l = d.Length;

            if (digits < l)
            {
                var digit = Convert.ToInt32(d[digits] + "");
                var number = (digits < l + 1) ? d.Substring(0, digits) : "0";

                if (digit >= 5)
                {
                    var roundPart = UtilityMath.AddLargeNumbers(number, "1");

                    // if round overflows the decimal, eliminate it and increase whole part
                    if (roundPart.Length > number.Length)
                    {
                        var next = UtilityMath.AddLargeNumbers(w, "1");
                        if (value == null)
                        {
                            SetDecimal(string.Empty);
                            SetWhole(next);
                        }
                        else
                            return new BigDecimal(next);
                    }
                    else
                    {
                        if (value == null)
                        {
                            roundPart = roundPart.TrimEnd('0');
                            SetDecimal(roundPart);
                        }
                        else
                            return new BigDecimal((roundPart != "0") ? $"{w}.{roundPart}" : w);
                    }
                }
                else
                {
                    d = d.Remove(digits);
                    if (value == null)
                    {
                        d = d.TrimEnd('0');
                        SetDecimal(d);
                    }
                    else
                        return new BigDecimal((d != "0") ? $"{w}.{d}" : w);
                }
            }

             return (value==null)?this:(BigDecimal)value;
        }

        /// <summary>
        /// Reduce decimal size without rounding value, just delete digits.     
        /// </summary>
        /// <param name="digits"></param>
        /// <returns></returns>
        public string Truncate(int digits = 2)
        {
            string d = GetDecimal();

            if (d.Length > digits)
            {
                SetDecimal(d.Remove(digits));
                SetDecimal(d.TrimEnd('0'));
            }
             return GetNumber();
        }

        public static BigDecimal ConvertFromBinary(string binary)
        {
            BigDecimal result = BigDecimal.Zero;
            BigInteger pow;

            for (int i = binary.Length-1; i >= 0; i--)
            {
                int digit = binary[i] - 48;
                pow = BigInteger.Pow(2, (binary.Length - 1) - i);
                pow *= digit;
                result += new BigDecimal(pow.ToString());
            }

            return result;
        }

        public BigDecimal ConvertFromString(string value)
        {
            if (value.Contains('.'))
            {
                var i = value.IndexOf('.');
                SetDecimal(value.Substring(i + 1));
                SetWhole(value.Substring(0, i));
            }
            else
                SetWhole(value);

            string w = GetWhole();
            if (w.Contains('-'))
            {
                SetWhole(w.Remove(0, 1));
                IsNegative = true;
            }

            return this;
        }
        public override bool Equals(object obj)
        {
            return UtilityMath.CompareLargeNumbers(this.ToString(), ((BigDecimal)obj).ToString()) == 0;
        }

        public override int GetHashCode()
        {
            return GetNumber().GetHashCode();
        }

        public override string ToString()
        {
            return GetNumber();
        }

        /// <summary>
        /// <see cref="https://www.javatpoint.com/csharp-program-to-convert-decimal-to-binary"/>
        /// <seealso cref="https://stackoverflow.com/questions/5372279/how-can-i-convert-very-large-decimal-numbers-to-binary-in-java"/>
        /// </summary>
        /// <returns></returns>
        public string ToBinary()
        {
            int[] ReverseDigits(string s)
            {
                char[] a = s.ToCharArray();
                int[] b = new int[s.Length];
                for (int i = 0; i < a.Length; i++)
                {
                    b[a.Length - 1 - i] = a[i] - 48;
                }
                return b;
            };

            // adds two binary numbers represented as strings
            string BinaryAdd(string s1, string s2)
            {
                string r = "", aux;
                int[] a1, a2;
                int digit, mark = 0;

                // a1 should be the longer one
                a1 = ReverseDigits((s1.Length > s2.Length ? s1 : s2));
                a2 = ReverseDigits((s1.Length < s2.Length ? s1 : s2));

                for (int i = 0; i < a1.Length; i++)
                {
                    digit = a1[i] + (i < a2.Length ? a2[i] : 0) + mark;

                    switch (digit)
                    {
                        default:
                        case 0:
                            aux = "0";
                            mark = 0;
                            break;
                        case 1:
                            aux = "1";
                            mark = 0;
                            break;
                        case 2:
                            aux = "0";
                            mark = 1;
                            break;
                        case 3:
                            aux = "1";
                            mark = 1;
                            break;
                    }

                    r = aux + r;   
                }

                if (mark > 0) { r = "1" + r; }

                return r;
            };

            var result = string.Empty;
            //var number = this;
            //string q, r;

            //for (int i = 0; number > 0; i++)
            //{
            //    (q, r) = number % 2;

            //    number = BigDecimal.Parse(q);
            //    result = r + result;
            //}
            string number = GetWhole();

            for (int i = 0; i < number.Length; i++)
            {
                result = BinaryAdd(result + "0", result + "000");
                var digit = number[i] - 48;
                result = BinaryAdd(result, Convert.ToString(digit, 2));
            }

            return result.TrimStart('0');
        }

        public void ToDecimal(string binary)
        {
            this = ConvertFromBinary(binary);
        }

        public int CompareTo(BigDecimal other)
        {
            return UtilityMath.CompareLargeNumbers(other.ToString(), this.ToString());
        }
        /// <summary>
        /// If caaled as a statement  then return this if not then convert cccurrent value to positive
        /// </summary>
        /// <returns></returns>
        public BigDecimal Abs()
        {
            if (this.IsNegative)
            {
                this.IsNegative = false;
            }

            return this;
        }
        public BigDecimal Pow(int power)
        {
            if (power == 0)
                return BigDecimal.Zero;
            if (power < 0)
                throw new ArithmeticException("Power must greater or equal to Zero. And method only supports integer values.");

            BigDecimal result = this;

            for (int i = 1; i < power; i++)
                result *= this;

            return result;
        }

        /// <summary> 0? 
        /// Using newton- raphson aproximation algorithm or
        /// <see cref="https://stackoverflow.com/questions/1695592/math-sqrt-vs-newton-raphson-method-for-finding-roots-in-c-sharp"/>
        /// </summary>
        /// <param name="truncateValue"></param>
        /// <returns></returns>
        public BigDecimal Sqrt_Newton_Raphson(bool truncateValue = false)
        {
            BigDecimal number = this;
            if (truncateValue) number.Truncate();

            if (number < 0) return BigDecimal.Zero;

            BigDecimal root = BigDecimal.Zero;
            BigDecimal FIRST_APPROX = BigDecimal.Two;

            // pick 2 as first approximation
            BigDecimal xNPlus1 = FIRST_APPROX;
            do
            {
                root = xNPlus1;
                xNPlus1 = root - ((root * root - number) / (FIRST_APPROX * root));

            } while ((xNPlus1 - root).Abs() > BigDecimal.Epsilon);
            return root;
        }


        /// <summary>
        /// Using babylonian aproximation algorithm or
        /// <see cref="https://www.csharp-console-examples.com/general/finding-the-square-root-of-a-number-in-c/"/>
        /// </summary>
        /// <param name="truncateValue"></param>
        /// <returns></returns>
        public BigDecimal Sqrt_Babylonian(bool truncateValue = false)
        {
            BigDecimal number = this;
            if (truncateValue) number.Truncate();

            if (number < 0) return BigDecimal.Zero;

            // if (truncateValue) number.Truncate();
            //BigDecimal root = number / 3;

            //for (int i = 0; i < 32; i++)
            //{
            //    root = (root + number / root) / 2;
            //    if (truncateValue) root.Truncate();
            //}
            //using babylonian method
            BigDecimal y = BigDecimal.One;
            BigDecimal e = Epsilon; /* e decides the accuracy level*/
            BigDecimal root = number;

            while (root - y > e)
            {
                root = (root + y) / 2;
                //if (truncateValue) root.Truncate();
                y = this / root;
                //if (truncateValue) y.Truncate();
            }

            return root;
        }

        public BigDecimal Sqrt(bool truncateValue = false)
        {
            BigDecimal number = this;
            if (truncateValue) number.Truncate();

            if (number < 0) return BigDecimal.Zero;

            if (truncateValue) number.Truncate();
            BigDecimal root = number / 3;

            for (int i = 0; i < 32; i++)
            {
                root = (root + number / root) / 2;
                if (truncateValue) root.Truncate();
            }

            return root;
        }

        public BigDecimal Sqrt_BigInteger()
        {
            BigInteger number = this.GetWholePartAsBigInteger();

            BigInteger root = (number >> 1) + 1;
            BigInteger n = (root + (number / root)) >> 1;
            while (n < root)
            {
                root = n;
                n = (root + (number / root)) >> 1;
            }

            return new BigDecimal(root.ToString());
        }

        /// <summary>
        /// <see cref="https://stackoverflow.com/questions/10866119/what-is-the-fastest-way-to-find-integer-square-root-using-bit-shifts"/>
        /// </summary>
        /// <returns></returns>
        public BigDecimal BitSqrt(bool truncateValue = false)
        {
            BigDecimal number = this;
            if (truncateValue) number.Truncate();

            if (number < 0) return BigDecimal.Zero;

            BigDecimal root = BigDecimal.Zero;
            BigDecimal bit = BigDecimal.One << 30; // The second-to-top bit is set: 1L<<30 for long
                                                   // "bit" starts at the highest power of four <= the argument.
            while (bit > number)
                bit >>= 2;
            while (bit != 0)
            {
                if (number >= root + bit)
                {
                    number -= root + bit;
                    root = (root >> 1) + bit;
                }
                else
                    root >>= 1;
                bit >>= 2;
            }

            return root;
        }

        /// <summary>
        /// <see cref="https://stackoverflow.com/questions/3581528/how-is-the-square-root-function-implemented"/>
        /// </summary>
        /// <returns></returns>
        public BigDecimal NoFloatingPointSqrt()
        {
            BigDecimal i = BigDecimal.Zero;
            BigDecimal x = this;
            
            if (x >= 65536)
            {
                if ((i + 32768) * (i + 32768) <= x) i += 32768;
                if ((i + 16384) * (i + 16384) <= x) i += 16384;
                if ((i + 8192) * (i + 8192) <= x) i += 8192;
                if ((i + 4096) * (i + 4096) <= x) i += 4096;
                if ((i + 2048) * (i + 2048) <= x) i += 2048;
                if ((i + 1024) * (i + 1024) <= x) i += 1024;
                if ((i + 512) * (i + 512) <= x) i += 512;
                if ((i + 256) * (i + 256) <= x) i += 256;
            }
            if ((i + 128) * (i + 128) <= x) i += 128;
            if ((i + 64) * (i + 64) <= x) i += 64;
            if ((i + 32) * (i + 32) <= x) i += 32;
            if ((i + 16) * (i + 16) <= x) i += 16;
            if ((i + 8) * (i + 8) <= x) i += 8;
            if ((i + 4) * (i + 4) <= x) i += 4;
            if ((i + 2) * (i + 2) <= x) i += 2;
            if ((i + 1) * (i + 1) <= x) i += 1;

            return i;
        }

        public static BigDecimal  Parse(string value)
        {
            var number = new BigDecimal();

            return number.ConvertFromString(value);
        }
        public static BigDecimal Min(BigDecimal a, BigDecimal b)
        {
            int compare = UtilityMath.CompareLargeNumbers(a.ToString(), b.ToString());
            if (compare <= 0)
                return a;
            else
                return b;
        }

        public static BigDecimal Max(BigDecimal a, BigDecimal b)
        {
            int compare = UtilityMath.CompareLargeNumbers(a.ToString(), b.ToString());
            if (compare >= 0)
                return a;
            else
                return b;
        }
    }
}
                                    