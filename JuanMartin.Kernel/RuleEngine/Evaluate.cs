using System.Diagnostics;

namespace JuanMartin.Kernel.RuleEngine
{
    public class Evaluate : IAction
    {
        private string _expression;
        private string _factName;
        private Rule _parent;

        public Evaluate(Rule Parent, string Expression, string FactName)
        {
            _expression = Expression;
            _factName = FactName;
            _parent = Parent;
        }

        public Evaluate(Rule Parent, string Expression)
            : this(Parent, Expression, string.Empty)
        {
        }

        public Symbol Execute()
        {
            Symbol value = new Symbol();
            ExpressionEvaluator eval = new ExpressionEvaluator(_parent.Scope.Engine.Aliases);

            try
            {
                eval.Parse(_expression);
                value = eval.Evaluate(_parent.Scope.GetInheritedFacts());

                //add result as a fact in the containing rule scope facts. If no fact name is associated the evaluate 
                //add it as a generic temp result per scope (using scope name as fact name)
                if (_factName == string.Empty) _factName = _parent.Name;

                _parent.Scope.UpdateFactValue(_factName, value.Value.Result);

                Debug.WriteLine(string.Format("{0} -> {1}", _expression, value.Value.Result));
            }
            catch
            {
                value.Name = _parent.Name;
                value.Value = new Value((Value)null);
                value.Type = Symbol.TokenType.Invalid;
            }

            return value;
        }
    }
}
