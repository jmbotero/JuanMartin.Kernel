using System.Collections.Generic;

namespace JuanMartin.Kernel.RuleEngine
{
    public class Actions
    {
        private List<IAction> _actions;
        private bool _executeIf;
        private Rule _parent;

        public Actions(Rule Parent, bool ExecuteIf)
        {
            _parent = Parent;
            _actions = new List<IAction>();
            _executeIf = ExecuteIf;
        }

        public bool ExecuteIf
        {
            get { return _executeIf; }
            set { _executeIf = value; }
        }

        public void AddRedirect(string Expression, string FactName)
        {
            Evaluate eval = new Evaluate(_parent, Expression, FactName);

            _actions.Add(eval);
        }

        public void AddEvaluate(string Expression, string FactName)
        {
            Evaluate eval = new Evaluate(_parent, Expression, FactName);

            _actions.Add(eval);
        }

        public void AddEvaluate(string Expression)
        {
            AddEvaluate(Expression, string.Empty);
        }


        public void AddAction(IAction Action)
        {
            if (!_actions.Contains(Action))
                _actions.Add(Action);
        }

        public void Execute()
        {
            //first execute all the actions then the rulescope child
            foreach (IAction action in _actions)
                action.Execute();
        }
    }
}
