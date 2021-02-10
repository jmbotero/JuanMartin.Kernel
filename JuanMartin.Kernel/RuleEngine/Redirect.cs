using System;

namespace JuanMartin.Kernel.RuleEngine
{
    public class Redirect : IAction
    {
        private Rule _parent;
        private string _targetName;

        public Redirect(Rule Parent, string TargetName)
        {
            _parent = Parent;
            _targetName = TargetName;
        }

        public Symbol Execute()
        {
            Rule target = _parent.Scope.FindRule(_targetName);

            if (target != null)
                target.Execute();
            else
                throw new Exception(string.Format("Could not find rule '{0}' in the current scope '{1}' or a preceding one.", _targetName, _parent.Scope.Name));

            return null;
        }

    }
}
