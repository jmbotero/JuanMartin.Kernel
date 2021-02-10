using JuanMartin.Kernel.RuleEngine;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace JuanMartin.Kernel.Utilities
{
    public class UtilityType
    {
        #region System.Type return methods
        public static bool IsNull(object Token)
        {
            return (Token == null);
        }

        public static bool IsBoolean(string Token)
        {
            if (Token != null && (Token.ToLower() == "true" || Token.ToLower() == "false"))
                return true;
            else
                return false;
        }

        public static bool IsNumber(string Token)
        {
            if (Token == null)
                return false;

            //numbers must have all digits as numbers 
            bool result = true;
            foreach (char c in Token.ToCharArray())
            {
                if (Char.IsNumber(c))
                    continue;

                result = false;
                break;
            }

            return result;
        }

        public static bool IsString(string Token)
        {
            if (Token == null)
                return false;

            bool result = false;
            if (Token.StartsWith(@"""") && Token.EndsWith(@""""))
                result = true;
            else if (Token.StartsWith("'") && Token.EndsWith("'"))
                result = true;

            return result;
        }
        #endregion

        #region RuleEngine macro methods

        [Macro]
        public static Value IsNull(Symbol Operand)
        {
            //If the attribute passed is not defined do not evaluate the function
            if (Operand == null)
                return null;

            return new Value(IsNull(Operand.Value.Result) || Operand.Type == Symbol.TokenType.Invalid);
        }

        [Macro]
        public static Value IsNotNull(Symbol Operand)
        {
            //If the attribute passed is not defined do not evaluate the function
            if (Operand == null)
                return null;

            return new Value(!IsNull(Operand.Value.Result) || Operand.Type == Symbol.TokenType.Invalid);
        }
        #endregion

        #region Utilitype Methods

        private static readonly HashSet<Type> NumericTypes = new HashSet<Type>
        {
        typeof(int),
        typeof(Int32),
        typeof(Int64),
        typeof(uint),
        typeof(double),
        typeof(decimal),
        typeof(float),
        typeof(long),
        typeof(ulong),
        typeof(BigInteger)
        };

        public static bool IsNumericType(Type type)
        {
            return NumericTypes.Contains(type) ||
                   NumericTypes.Contains(Nullable.GetUnderlyingType(type));
        }

        public static Type ParseType(string type_name)
        {
            Type t = null;
            try
            {
                t = Type.GetType(type_name);
            }
            catch (TypeLoadException e)
            {
                Console.WriteLine("{0}: Unable to load type {1}", e.GetType().Name, type_name);
            }
            return t;
        }
        #endregion
    }
}
