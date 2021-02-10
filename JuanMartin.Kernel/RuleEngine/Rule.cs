using System;
using System.Collections.Generic;

namespace JuanMartin.Kernel.RuleEngine
{
    public class Rule
    {
        private string _name;
        private bool _isDependent;

        private RuleScope _scope;

        private Condition _condition;
        private Dictionary<object, Actions> _actionSets;

        public Rule(RuleScope Scope, string Name, string Expression, bool IsDependent)
        {
            if (Name.Length == 0)
                throw new Exception(string.Format("Rule name is required, containing scope '{0}'.", Scope.Name));

            if (Expression.Length == 0)
                throw new Exception(string.Format("Rule condition expression is required, for rule '{0}'.", Name));

            _name = Name;
            _isDependent = IsDependent;
            _scope = Scope;
            _actionSets = new Dictionary<object, Actions>();
            _condition = new Condition(this, Expression);
        }

        public Rule(RuleScope Scope, string Name, string Expression)
            : this(Scope, Name, Expression, false)
        {
        }

        public Dictionary<string, Symbol> Facts
        {
            get { return null; }
        }

        public RuleScope Scope
        {
            get { return _scope; }
            set { _scope = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Expression
        {
            set { _condition = new Condition(this, value); }
            get { return _condition.Expression; }
        }

        public bool IsDependent
        {
            get { return _isDependent; }
        }

        public void AddActionSet(Actions set)
        {
            if (!_actionSets.ContainsKey(set.ExecuteIf))
                _actionSets.Add(set.ExecuteIf, set);
        }

        public void AddActionSet(bool ExecuteIf)
        {
            Actions set = new Actions(this, ExecuteIf);

            AddActionSet(set);
        }

        public void Execute()
        {
            bool setFound = false;
            bool result = _condition.Evaluate();

            foreach (Actions set in _actionSets.Values)
            {
                if (set.ExecuteIf.Equals(result))
                {
                    set.Execute();
                    setFound = true;
                    break;
                }
            }

            if (!setFound)
                throw new Exception(string.Format("Action set for the conditon evaluating as {0} was not found in rule '{1}'.", result.ToString(), _name));
        }
    }
}
