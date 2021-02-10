using System;
using System.Collections.Generic;
using System.Xml;

namespace JuanMartin.Kernel.RuleEngine
{
    public class RuleEngine : IRuleContainer
    {
        private string _name;
        private Dictionary<string, RuleScope> _scopes;
        private Dictionary<string, Symbol> _aliases;
        private XmlDocument _kb;

        public RuleEngine(string Name, bool LoadUtilityAliases)
        {
            _name = Name;

            _aliases = new Dictionary<string, Symbol>();
            _scopes = new Dictionary<string, RuleScope>();
            _kb = new XmlDocument();

            if (LoadUtilityAliases)
                AddDefaultAliases();
        }

        public RuleEngine(string Name)
            : this(Name, true)
        {
        }

        public string Name
        {
            get { return _name; }
        }

        public Dictionary<string, Symbol> Aliases
        {
            get { return _aliases; }
        }

        public void AddScope(RuleScope Scope)
        {
            if (!_scopes.ContainsKey(Scope.Name))
                _scopes.Add(Scope.Name, Scope);
        }

        public void AddAlias(Symbol Alias)
        {
            if (!_aliases.ContainsValue(Alias))
                _aliases.Add(Alias.Name, Alias);
        }

        public void AddAlias(string Name, string ReferenceType)
        {
            Symbol alias = new Symbol(Name, ReferenceType, Symbol.TokenType.Value);

            AddAlias(alias);
        }
        #region Load Xml Kb
        public void Load(string KbFilename)
        {
            _kb.Load(KbFilename);

            //load kb aliases
            XmlNodeList aliases = _kb.SelectNodes("RuleEngine/Aliases/Alias");

            foreach (XmlNode aliasNode in aliases)
            {
                string name = aliasNode.Attributes["name"].Value;
                string type = aliasNode.Attributes["type"].Value;

                Symbol alias = new Symbol(name, type, Symbol.TokenType.Alias);
                _aliases.Add(name, alias);
            }

            //load first rulescope
            XmlNodeList scopes = _kb.SelectNodes("RuleEngine//RuleScope");

            foreach (XmlNode scopeNode in scopes)
            {
                LoadRuleScope((RuleEngine)this, scopeNode);
            }
        }

        private void LoadRuleScope(IRuleContainer Container, XmlNode ScopeXml)
        {
            LoadRuleScope(Container, ScopeXml, null);
        }

        private void LoadRuleScope(IRuleContainer Container, XmlNode ScopeXml, RuleScope Parent)
        {
            string name = ScopeXml.Attributes["name"].Value;
            bool isDependent;
            try
            {
                isDependent = Boolean.Parse(ScopeXml.Attributes["isDependent"].Value);
            }
            catch
            {
                isDependent = false;
            }

            //add scope to engine
            RuleScope scope = new RuleScope(this, name, Parent, isDependent);

            //add associated facts
            XmlNodeList facts = ScopeXml.SelectNodes("Memory//Fact");

            foreach (XmlNode factNode in facts)
            {
                string factName = factNode.Attributes["name"].Value;
                string factType = factNode.Attributes["type"].Value;
                string factValue = factNode.InnerText;

                try
                {
                    Type type = Type.GetType(factType);
                    object value = Convert.ChangeType(factValue, type);

                    scope.AddFact(factName, value);
                }
                catch (Exception e)
                {
                    throw new Exception(string.Format("Error changing type of {0} to {1}.", factName, factType), e);
                }
            }

            //add the scopes in this scope
            XmlNodeList scopes = ScopeXml.SelectNodes("RuleScope");

            foreach (XmlNode scopeNode in scopes)
            {
                LoadRuleScope((RuleScope)scope, scopeNode, Parent);
            }

            //add the rules in this scope
            XmlNodeList rules = ScopeXml.SelectNodes("Rule");

            foreach (XmlNode ruleNode in rules)
            {
                LoadRule(scope, ruleNode);
            }

            //add scope to engine
            Container.AddScope(scope);
        }
        private void LoadRule(RuleScope Scope, XmlNode RuleXml)
        {
            string expression = RuleXml["Condition"].InnerText;
            string name = RuleXml.Attributes["name"].Value;
            bool isDependent;
            try
            {
                isDependent = Boolean.Parse(RuleXml.Attributes["isDependent"].Value);
            }
            catch
            {
                isDependent = false;
            }

            //create new rule instance 
            Rule rule = new Rule(Scope, name, expression, isDependent);

            //add action sets
            XmlNodeList sets = RuleXml.SelectNodes("Actions");

            foreach (XmlNode actionsNode in sets)
            {
                try
                {
                    bool executeIf = Boolean.Parse(actionsNode.Attributes["evaluate"].Value);
                    string equation = actionsNode.InnerText;

                    IAction action = null;
                    Actions set = new Actions(rule, executeIf);
                    string xPath = string.Format("//Actions[@evaluate='{0}']/*", executeIf.ToString());

                    XmlNodeList actions = RuleXml.SelectNodes(xPath);

                    foreach (XmlNode actionNode in actions)
                    {
                        switch (actionNode.Name)
                        {
                            case "Evaluate":
                                string factName = actionNode.Attributes["factId"].Value;
                                action = new Evaluate(rule, equation, factName);
                                break;
                            case "Redirect":
                                string ruleName = actionNode.Attributes["ruleId"].Value;
                                action = new Redirect(rule, ruleName);
                                break;
                        }

                        if (action != null)
                            set.AddAction(action);
                    }

                    rule.AddActionSet(set);
                }
                catch
                {
                    throw new Exception(string.Format("Actions node for rule '{0}' must have a boolean evaluate type defined.", name));
                }
            }

            Scope.AddRule(rule);
        }

        #endregion

        public void Execute()
        {
            foreach (RuleScope scope in _scopes.Values)
                if (!scope.IsDependent)
                    scope.Execute();
        }

        public Symbol FactLookup(string Name)
        {
            return FactLookup(Name, _scopes);
        }

        public Symbol FactLookup(string Name, Dictionary<string, RuleScope> Scopes)
        {
            foreach (RuleScope scope in Scopes.Values)
            {
                foreach (Symbol fact in scope.Facts.Values)
                {
                    if (fact.Name == Name)
                    {
                        return fact;
                    }
                }

                return FactLookup(Name, scope.Scopes);
            }

            return null;
        }

        public void AddObjectReference(string Name, object Object)
        {
            Symbol reference = new Symbol(Name, Object, Symbol.TokenType.Instance);

            _aliases.Add(reference.Name, reference);
        }

        private void AddDefaultAliases()
        {
            this.AddAlias("Date", "JuanMartin.Kernel.Utilities.UtilityDate");
            this.AddAlias("DaysOfWeek", "JuanMartin.Kernel.Utilities.DaysOfWeek");
            this.AddAlias("Type", "JuanMartin.Kernel.Utilities.UtilityType");
            this.AddAlias("Logic", "JuanMartin.Kernel.Utilities.UtilityLogic");
            this.AddAlias("Math", "JuanMartin.Kernel.Utilities.UtilityMath");
            this.AddAlias("String", "JuanMartin.Kernel.Utilities.UtilityString");
        }
    }
}
