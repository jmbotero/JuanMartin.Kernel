﻿using JuanMartin.Kernel.Utilities;
using System;

namespace JuanMartin.Kernel
{
    public class Value : ICloneable
    {
        private object _value;
        private Type _type;

        public Value()
        {
            _value = null;
            _type = null;
        }

        public Value(object Value, Type Type)
        {
            _value = Value;
            _type = Type;
        }

        public Value(string Token)
        {
            object value = Parse(Token);

            _value = value;
            _type = value.GetType();
        }

        public Value(object Value)
        {
            if (Value is Value)
            {
                if (Value == null)
                {
                    _value = null;
                    _type = typeof(object);
                }
                else
                    this.Result = ((Value)Value).Result;
            }
            else
            {
                _value = Value;

                if (_value == null)
                    _type = typeof(object);
                else
                    _type = Value.GetType();
            }
        }

        public object Result
        {
            get { return _value; }
            set
            {
                _value = value;
                if (_value == null)
                    _type = null;
                else
                    _type = value.GetType();
            }
        }

        public Type Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public object Clone()
        {
            return new Value((Value)this);
        }                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           

        public static object Parse(string token)
        {
            object value;

            if (UtilityType.IsBoolean(token))
            {
                value = Boolean.Parse(token);
            }
            else if (UtilityType.IsNumber(token))
            {
                value = Double.Parse(token);
            }
            else if (UtilityType.IsString(token))
            {
                int length = token.Length - 2;
                value = ((length) > 0) ? token.Substring(1, length) : token;
            }
            else
            {
                value = token; //TO DO: Use serialization
            }

            return value;
        }

        public override string ToString()
        {
            return string.Format("[{0}, {1}]", _value.ToString(), _type.ToString());
        }
    }
}
