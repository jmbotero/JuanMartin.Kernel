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
            var actualChain = sd.GetChain(10);
            var expectedTerminator = 1;
            if (actualChain != null && actualChain.Count > 0)
                Assert.AreEqual(expectedTerminator, actualChain.Last());
            else
                Assert.Fail();
        }

        [Test()]
        public void SquareDigitChainForFourSouldHaveTerminatorEightyNine()
        {
            var sd = new SquareChains(4);
            var actualChain  = sd.GetChain(4);
            var vexpectedTerminator = 89;
            if ( actualChain  != null && actualChain .Count > 0)
                Assert.AreEqual(vexpectedTerminator, actualChain .Last());
            else
                Assert.Fail();
        }

        [Test()]
        public void SquareDigitChainForEightyFiveSouldHaveTwoEightyNines()
        {
            var expectedChain = new List<int> { 85, 89, 145, 42, 20, 4, 16, 37, 58, 89 };
            var sd = new SquareChains(85);
            var actualChain  = sd.GetChain(85);
            if (actualChain  != null && actualChain .Count > 0)
                Assert.AreEqual(expectedChain, actualChain );
            else
                Assert.Fail();
        }

        [Test()]
        public void SquareDigitChainForfortyFourSouldHaveTwoOnes()
        {
            var expectedChain = new List<int> { 44, 32, 13, 10, 1, 1 };
            var sd = new SquareChains(44);
            var actualChain  = sd.GetChain(44);
            if (actualChain  != null && actualChain .Count > 0)
                Assert.AreEqual(expectedChain, actualChain );
            else
                Assert.Fail();
        }
    }
}