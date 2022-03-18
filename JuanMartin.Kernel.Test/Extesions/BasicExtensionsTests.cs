using NUnit.Framework;
using JuanMartin.Kernel.Extesions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JuanMartin.Kernel.Extesions.Tests
{
    [TestFixture()]
    public class BasicExtensionsTests
    {
        [Test()]
        public void ShouldIndicateIfNumberHasAnyRepeateDigits()
        {
            int actualIntNumber;
            long actualLongNumber;

            actualIntNumber = 2147483647;

            Assert.IsTrue(actualIntNumber.HasDuplicates(), "Int number has duplicates.");

            actualIntNumber = 1234567890;

            Assert.IsFalse(actualIntNumber.HasDuplicates(), "Int number has no duplicates.");

            actualLongNumber = 9223372036854775807;

            Assert.IsTrue(actualLongNumber.HasDuplicates(), "Int number has duplicates.");
        }
    }
}