using NUnit.Framework;

namespace JuanMartin.Kernel.RuleEngine.Tests
{
    [TestFixture]
    public class RuleEngineTests
    {
        [Test]
        public static void SingleXmlRuleTest()
        {
            var actualEngine = new RuleEngine("SingleXmlRuleTest");
            var actualFactName = "foo2";
            var expectedFactValue = 4;

            actualEngine.Load(@"C:\GitRepositories\JuanMartin.Kernel\JuanMartin.Kernel.Test\data\ut-single-rule.xml");
            actualEngine.Execute();

            var actualFact = actualEngine.FactLookup(actualFactName); // after rule executes fact = 3+1

            Assert.AreEqual(expectedFactValue, actualFact.Value.Result);
        }
    }
}
