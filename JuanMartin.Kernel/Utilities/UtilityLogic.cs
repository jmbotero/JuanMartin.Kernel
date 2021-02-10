using JuanMartin.Kernel.RuleEngine;
using System;

namespace JuanMartin.Kernel.Utilities
{
    public class UtilityLogic
    {
        [Macro]
        public static Value Equals(Symbol Operand1, Symbol Operand2)
        {
            Value result = new Value();

            try
            {
                object o1 = Operand1.Value.Result;
                object o2 = Operand2.Value.Result;

                result.Result = o1.Equals(o2);
            }
            catch
            {
            }

            return result;
        }

        [Macro]
        public static Value NotEquals(Symbol Operand1, Symbol Operand2)
        {
            Value result = Equals(Operand1, Operand2);

            bool equals = (bool)result.Result;

            result.Result = !(equals);

            return result;
        }

        [Macro]
        public static Value And(Symbol Operand1, Symbol Operand2)
        {
            Value result = new Value();

            //see if the facts are not equal
            try
            {
                bool b1 = (bool)Operand1.Value.Result;
                bool b2 = (bool)Operand2.Value.Result;

                result.Result = (b1 && b2);
            }
            catch
            {
            }

            return result;
        }

        [Macro]
        public static Value Not(Symbol Operand1)
        {
            Value result = new Value();

            //see if the facts are not equal
            try
            {
                bool b1 = (bool)Operand1.Value.Result;

                result.Result = !(b1);
            }
            catch
            {
            }

            return result;
        }

        [Macro]
        public static Value Or(Symbol Operand1, Symbol Operand2)
        {
            Value result = new Value();

            //see if the facts are not equal
            try
            {
                bool b1 = (bool)Operand1.Value.Result;
                bool b2 = (bool)Operand2.Value.Result;

                result.Result = (b1 || b2);
            }
            catch
            {
            }

            return result;
        }

        [Macro]
        private static int ArithmeticComparison(Symbol Operand1, Symbol Operand2)
        {
            int result;

            try
            {
                IComparable o1 = (IComparable)Operand1.Value.Result;
                IComparable o2 = (IComparable)Operand2.Value.Result;

                result = o1.CompareTo(o2);
            }
            catch
            {
                return int.MinValue;
            }

            return result;
        }

        [Macro]
        public static Value GreaterThan(Symbol Operand1, Symbol Operand2)
        {
            Value result = new Value();

            int compare = ArithmeticComparison(Operand1, Operand2);

            if (compare != int.MinValue)
                result.Result = (compare == 1);

            return result;
        }

        [Macro]
        public static Value LessThan(Symbol Operand1, Symbol Operand2)
        {
            Value result = new Value();

            int compare = ArithmeticComparison(Operand1, Operand2);

            if (compare != int.MinValue)
                result.Result = (compare == -1);

            return result;
        }

        [Macro]
        public static Value GreaterThanEqual(Symbol Operand1, Symbol Operand2)
        {
            Value result = new Value();

            int compare = ArithmeticComparison(Operand1, Operand2);

            if (compare != int.MinValue)
                result.Result = (compare == 1 || compare == 0);

            return result;
        }

        [Macro]
        public static Value LessThanEqual(Symbol Operand1, Symbol Operand2)
        {
            Value result = new Value();

            int compare = ArithmeticComparison(Operand1, Operand2);

            if (compare != int.MinValue)
                result.Result = (compare == -1 || compare == 0);

            return result;
        }

    }
}
