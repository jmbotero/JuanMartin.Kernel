using System;
using System.Collections.Generic;

namespace JuanMartin.Kernel.RuleEngine
{
    public class RuleScope : IRuleContainer
    {
        private RuleEngine _engine;
        private RuleScope _parent;

        private Dictionary<string, RuleScope> _scopes; //sub-scopes
        private Dictionary<string, Rule> _rules;

        private string _name;
        private bool _isDependent;

        private Dictionary<string, Symbol> _facts;

        public RuleScope(RuleEngine Engine, string Name, RuleScope Parent, bool IsDependent)
        {
            if (Name.Length == 0 && Parent != null)
                throw new Exception(string.Format("Scope name is required, containing scope '{0}'.", Parent.Name));

            if (Name.Length == 0)
                throw new Exception(string.Format("Scope name is required, no parent scope defined."));

            _name = Name;
            _isDependent = IsDependent;
            _engine = Engine;
            _parent = Parent;
            _facts = new Dictionary<string, Symbol>();
            _rules = new Dictionary<string, Rule>();
            _scopes = new Dictionary<string, RuleScope>();
        }

        public RuleScope(RuleEngine Engine, string Name, bool IsDependent)
            : this(Engine, Name, null, IsDependent)
        {
        }

        public RuleScope(RuleEngine Engine, string Name)
            : this(Engine, Name, null, false)
        {
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public Dictionary<string, Symbol> Facts
        {
            get { return _facts; }
        }

        public Dictionary<string, RuleScope> Scopes
        {
            get { return _scopes; }
        }

        public Dictionary<string, Rule> Rules
        {
            get { return _rules; }
        }

        public RuleEngine Engine
        {
            get { return _engine; }
        }

        public RuleScope Parent
        {
            get { return _parent; }
        }

        public bool IsDependent
        {
            get { return _isDependent; }
        }


        public void AddRule(Rule Rule)
        {
            if (!_rules.ContainsKey(Rule.Name))
                _rules.Add(Rule.Name, Rule);
        }

        public void AddRule(string Name, string Expression)
        {
            Rule rule = new Rule(this, Name, Expression);

            AddRule(rule);
        }

        public void AddScope(RuleScope Scope)
        {
            if (!_scopes.ContainsKey(Scope.Name))
                _scopes.Add(Scope.Name, Scope);
        }

        public void AddFact(Symbol Fact)
        {
            if (!_facts.ContainsKey(Fact.Name))
                _facts.Add(Fact.Name, Fact);
        }

        public void AddFact(string Name, object Value)
        {
            Symbol fact = new Symbol(Name, Value, Symbol.TokenType.Fact);

            AddFact(fact);
        }

        public void UpdateFactValue(string Name, object Value)
        {
            Symbol fact;

            try
            {
                fact = _facts[Name];

                if (fact != null)
                    fact.Value = new Value(Value);
            }
            catch
            {
                AddFact(Name, Value);
            }
        }

        public Dictionary<string, Symbol> GetInheritedFacts()
        {
            //first add the facts from this scope then from all parent scopes usinf recursion
            Dictionary<string, Symbol> facts = new Dictionary<string, Symbol>();

            foreach (Symbol fact in _facts.Values)
                facts.Add(fact.Name, fact);

            if (_parent != null)
                AddScopeFacts(_parent, facts);

            return facts;
        }

        private void AddScopeFacts(RuleScope Scope, Dictionary<string, Symbol> Facts)
        {
            //first add fcts in current scope
            foreach (Symbol fact in Scope.Facts.Values)
            {
                if (!Facts.ContainsKey(fact.Name))
                    Facts.Add(fact.Name, fact);
            }

            //second add facts recursively on scope's parent
            if (Scope.Parent != null)
                AddScopeFacts(Scope.Parent, Facts);
        }

        public void Execute()
        {
            //first execute rulescopes then rules
            foreach (RuleScope scope in _scopes.Values)
                if (!scope.IsDependent)
                    scope.Execute();

            foreach (Rule rule in _rules.Values)
                if (!rule.IsDependent)
                    rule.Execute();
        }

        public Rule FindRule(string Name, bool RecursiveSearch)
        {
            foreach (Rule rule in _rules.Values)
            {
                if (rule.Name == Name)
                    return rule;
            }

            if (RecursiveSearch)
            {
                foreach (RuleScope scope in _scopes.Values)
                    return FindRule(Name, RecursiveSearch);
            }

            return null;
        }

        public Rule FindRule(string Name)
        {
            return FindRule(Name, true);
        }

    }
}
