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
    public class StringExtensionsTests
    {
        [Test()]
        public static void ShouldBeFlaggedAsNumericIntegerValueTest()
        {
            string actualNumber = "123";

            Assert.IsTrue(actualNumber.IsNumeric());
        }

        [Test()]
        public static void ShouldNotBeFlaggedAsNumericAlphabeticalCharacterTest()
        {
            string actualNumber = "a";

            Assert.IsFalse(actualNumber.IsNumeric(),"Single letter.");

            actualNumber = "1a3";

            Assert.IsFalse(actualNumber.IsNumeric(),"Letter in number.");
        }

        [Test()]
        public static void ShouldNotBeFlaggedAsNumericSingleDecimalPointCharacterTest()
        {
            string actualNumber = ".";

            Assert.IsFalse(actualNumber.IsNumeric());
        }

        [Test()]
        public static void ShouldBeFlaggedAsNumericNegativeIntegerValueTest()
        {
            string actualNumber = "-123";

            Assert.IsTrue(actualNumber.IsNumeric());

            actualNumber = "1-23";

            Assert.IsFalse(actualNumber.IsNumeric());
        }

        [Test()]
        public static void ShouldBeFlaggedAsNumericDecimalValueTest()
        {
            string actualNumber = "456.123";

            Assert.IsTrue(actualNumber.IsNumeric(),"Multiple digit decimal number.");

            actualNumber = ".123";

            Assert.IsFalse(actualNumber.IsNumeric(), "Value starting with decimal point.");

            actualNumber = "45.612.3";

            Assert.IsFalse(actualNumber.IsNumeric(), "Value with multiple decimal points.");
        }
    }
}