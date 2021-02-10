using System;
using System.Diagnostics;

namespace JuanMartin.Kernel.RuleEngine
{
    public class Condition
    {
        private Rule _parent;
        private string _expression;
        private Symbol _value = new Symbol();

        public Condition(Rule Parent, string Expression)
        {
            _parent = Parent;
            _expression = Expression;
        }

        public Rule Parent
        {
            get { return _parent; }
        }

        public string Expression
        {
            get { return _expression; }
            set { _expression = value; }
        }

        public Symbol Result
        {
            get { return _value; }
        }

        public bool Evaluate()
        {
            ExpressionEvaluator eval = new ExpressionEvaluator(_parent.Scope.Engine.Aliases);

            try
            {
                eval.Parse(Expression);
                _value = eval.Evaluate(_parent.Scope.Facts);
                Debug.WriteLine(string.Format("{0} -> {1}", Expression, _value.Value.Result));

                if (_value.Value.Type != typeof(bool))
                    throw new Exception(string.Format("{0} not a valid booolean expression", Expression));
            }
            catch
            {
                _value.Name = @"condition::" + _parent.Name;
                _value.Value = new Value((Value)null);
                _value.Type = Symbol.TokenType.Invalid;
            }

            return (bool)_value.Value.Result;
        }
    }
}
