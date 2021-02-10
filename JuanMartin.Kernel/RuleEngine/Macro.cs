using System;

namespace JuanMartin.Kernel.RuleEngine
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class Macro : System.Attribute
    {
        public Macro()
        {
        }
    }
}
