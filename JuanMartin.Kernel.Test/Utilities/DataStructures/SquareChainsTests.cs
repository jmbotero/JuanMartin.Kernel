using NUnit.Framework;
using JuanMartin.Kernel.Utilities.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JuanMartin.Kernel.Utilities.DataStructures.Tests
{
    [TestFixture()]
    public class SquareChainsTests
    {
        [Test()]
        public void SquareDigitChainForTenShouldHaveTerminatorOne()
        {
            var sd = new SquareChains(10);
            var actual_chain = sd.GetChain(10);
            var expected_terminator = 1;
            if (actual_chain != null && actual_chain.Count > 0)
                Assert.AreEqual(expected_terminator, actual_chain.Last());
            else
                Assert.Fail();
        }

        [Test()]
        public void SquareDigitChainForFourSouldHaveTerminatorEightyNine()
        {
            var sd = new SquareChains(4);
            var actual_chain = sd.GetChain(4);
            var expected_terminator = 89;
            if (actual_chain != null && actual_chain.Count > 0)
                Assert.AreEqual(expected_terminator, actual_chain.Last());
            else
                Assert.Fail();
        }

        [Test()]
        public void SquareDigitChainForEightyFiveSouldHaveTwoEightyNines()
        {
            var expected_chain = new List<int> { 85, 89, 145, 42, 20, 4, 16, 37, 58, 89 };
            var sd = new SquareChains(85);
            var actual_chain = sd.GetChain(85);
            if (actual_chain != null && actual_chain.Count > 0)
                Assert.AreEqual(expected_chain, actual_chain);
            else
                Assert.Fail();
        }

        [Test()]
        public void SquareDigitChainForfortyFourSouldHaveTwoOnes()
        {
            var expected_chain = new List<int> { 44, 32, 13, 10, 1, 1 };
            var sd = new SquareChains(44);
            var actual_chain = sd.GetChain(44);
            if (actual_chain != null && actual_chain.Count > 0)
                Assert.AreEqual(expected_chain, actual_chain);
            else
                Assert.Fail();
        }
    }
}