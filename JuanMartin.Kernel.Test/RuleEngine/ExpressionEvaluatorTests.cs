using JuanMartin.Kernel.RuleEngine;
using NUnit.Framework;
using System.Collections.Generic;

namespace JuanMartin.Kernel.RuleEngine.Tests
{
    [TestFixture]
    class ExpressionEvaluatorTests
    {
        [Test]
        public void TestEvaluateGreaterThan()
        {
            ExpressionEvaluator _evaluator = new ExpressionEvaluator();

            _evaluator.Parse("3 > 4");
            Symbol symbol = _evaluator.Evaluate(new Dictionary<string, Symbol>());

            Assert.AreEqual(symbol.Value.Result, false);
        }

        [Test]
        public void TestEvaluateGreaterThanMacro()
        {
            ExpressionEvaluator _evaluator = new ExpressionEvaluator();

            _evaluator.Parse("3 > 4");
            Symbol symbol = _evaluator.Evaluate(new Dictionary<string, Symbol>());

            Assert.AreEqual(symbol.Value.Result, false);
        }

        [Test]
        public void TestEvaluateIsNullMacro()
        {
            Dictionary<string, Symbol> aliases = new Dictionary<string, Symbol>();
            ExpressionEvaluator _eval = new ExpressionEvaluator(aliases);

            Symbol alias = new Symbol("alias::Type", "JuanMartin.Kernel.Utilities.UtilityType", Symbol.TokenType.Alias);
            aliases.Add("Type", alias);

            _eval.Parse("Type.IsNull('a')");
            Symbol result = _eval.Evaluate();

            //since 'a' is not null returns false
            Assert.AreEqual(result.Value.Result, false);
        }

        [Test]
        public void TestArithmenticFixedPrecedence()
        {
            ExpressionEvaluator _evaluator = new ExpressionEvaluator();

            _evaluator.Parse("(4-3)*2");
            Symbol symbol = _evaluator.Evaluate(new Dictionary<string, Symbol>());

            Assert.AreEqual(symbol.Value.Result, 2);
        }

        [Test]
        public void TestArithmenticPrecedence()
        {
            ExpressionEvaluator _evaluator = new ExpressionEvaluator();

            _evaluator.Parse("2*3-4");
            Symbol symbol1 = _evaluator.Evaluate(new Dictionary<string, Symbol>());

            _evaluator.ClearStack();
            _evaluator.Parse("(2*3)-4");
            Symbol symbol2 = _evaluator.Evaluate(new Dictionary<string, Symbol>());

            Assert.AreEqual(symbol1.Value.Result, symbol2.Value.Result);
        }
    }
}
