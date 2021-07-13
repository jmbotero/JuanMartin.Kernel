using JuanMartin.Kernel.RuleEngine;
using NUnit.Framework;
using System.Collections.Generic;

namespace JuanMartin.Kernel.RuleEngine.Tests
{
    [TestFixture]
    class ExpressionEvaluatorTests
    {
        [Test]
        public void ShouldEvaluateCorrectlyLogicalBinaryMathematicalOperation()
        {
            ExpressionEvaluator actualEvaluator = new ExpressionEvaluator();
            var actualOperation = "3 > 4";

            actualEvaluator.Parse(actualOperation);
            Symbol actualOperationSymbol = actualEvaluator.Evaluate(new Dictionary<string, Symbol>());
            var expectedOperationResult = false;

            Assert.AreEqual(expectedOperationResult, actualOperationSymbol.Value.Result);
        }

        [Test]
        public void ShouldEvaluateAliasIsNullMacrsDefinedAsUtilityMethod()
        {
            Dictionary<string, Symbol> actualAliases = new Dictionary<string, Symbol>();
            ExpressionEvaluator actualEvaluator = new ExpressionEvaluator(actualAliases);

            Symbol actualTypeAlias = new Symbol("alias::Type", "JuanMartin.Kernel.Utilities.UtilityType", Symbol.TokenType.Alias);
            actualAliases.Add("Type", actualTypeAlias);

            var actualMacroCall = "Type.IsNull('a')";
            actualEvaluator.Parse(actualMacroCall);
            Symbol actualMacroResult = actualEvaluator.Evaluate();
            //since 'a' is not null returns false
            var expectedMacroResult = false;

            Assert.AreEqual(expectedMacroResult, actualMacroResult.Value.Result);
        }

        [Test]
        public void ShouldResolveCorrectlySimpleArithmenticOperation()
        {
            ExpressionEvaluator actualEvaluator = new ExpressionEvaluator();
            var actualOperation = "(4-3)*2";

            actualEvaluator.Parse(actualOperation);
            Symbol actualOperationSymbol = actualEvaluator.Evaluate(new Dictionary<string, Symbol>());
            var expectedOperationResult = 2;

            Assert.AreEqual(expectedOperationResult, actualOperationSymbol.Value.Result);
        }

        [Test]
        public void ShouldResolveCorrectlyNonIntegerArithmenticOperation()
        {
            ExpressionEvaluator actualEvaluator = new ExpressionEvaluator();
            var actualOperation = "((1 / 2) + 5) * 8";

            actualEvaluator.Parse(actualOperation);
            Symbol actualOperationSymbol = actualEvaluator.Evaluate(new Dictionary<string, Symbol>());
            var expectedOperationResult = 44;

            Assert.AreEqual(expectedOperationResult, actualOperationSymbol.Value.Result);
        }

        [Test]
        public void ShouldEvaluateCorrectlySimpleArithmenticOperatorPrecedenceInOperation()
        {
            ExpressionEvaluator actualEvaluator = new ExpressionEvaluator();

            var actualPrecedenceOperation = "2*3-4";
            actualEvaluator.Parse(actualPrecedenceOperation);
            Symbol actualPrecedenceOperationSymbol = actualEvaluator.Evaluate(new Dictionary<string, Symbol>());

            actualEvaluator.ClearStack();
            var actualOperation = "(2*3)-4";
            actualEvaluator.Parse(actualOperation);
            Symbol actualOperationSymbol = actualEvaluator.Evaluate(new Dictionary<string, Symbol>());

            Assert.AreEqual(actualPrecedenceOperationSymbol.Value.Result, actualOperationSymbol.Value.Result);
        }
    }
}
