using System;

namespace JuanMartin.Kernel.RuleEngine
{
    public class Symbol : ICloneable
    {
        private string _name;
        private Value _value;
        private TokenType _type;

        public enum TokenType
        {
            Alias,
            Instance,
            Fact,
            Value,
            Operator,
            Function,
            Result,
            OpenBracket,
            CloseBracket,
            Invalid //states the comparison could not be made and is invalid
        }

        public Symbol(Symbol other)
        {
            this.Name = other.Name;
            this.Value = (Value)other.Value.Clone();
            this.Type = other.Type;
        }

        //overwirte the generic constructor that passes an object, to avoid creating a new Value instance if Value is passed
        public Symbol(string Name, Value SymbolValue, TokenType SystemType)
        {
            _name = Name;
            _value = SymbolValue;
            _type = SystemType;
        }

        public Symbol(string Name, object SymbolValue, TokenType SystemType)
        {
            _name = Name;
            _value = new Value(SymbolValue);
            _type = SystemType;
        }

        public Symbol()
            : this(string.Empty, null, Symbol.TokenType.Invalid)
        {
        }

        public Symbol(string Name)
            : this(Name, null, Symbol.TokenType.Invalid)
        {
        }

        public Value Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public TokenType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public object Clone()
        {
            return new Symbol(this);
        }

        public override string ToString()
        {
            return string.Format("[{0}, {1}, {2}]", _name, _value.ToString(), _type.ToString());
        }
    }
}
