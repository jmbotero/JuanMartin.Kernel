using JuanMartin.Kernel.Utilities;
using NUnit.Framework;
using System;

namespace JuanMartin.Kernel.Utilities.Tests
{
    [TestFixture]
    class UtilityFileTests
    {
        [Test]
        public void ShouldFindAllFilesUnderDirectoryTree()
        {
            var actualFiles = UtilityFile.GetAllFiles(@"C:\GitHub\JuanMartin.Kernel\JuanMartin.Kernel.Test\data", false);
            var expectedFileCount = 4;

            Assert.AreEqual(expectedFileCount, actualFiles.Count);
        }

        [Test]
        public void ShoulProcessAsZipAndContainAContentXmlFileIfAnOdtFileIsSelected()
        {
            var files = UtilityFile.ListZipFileContents(@"C:\GitHub\JuanMartin.Kernel\JuanMartin.Kernel.Test\data\dvd-labels-template.odt");
            Assert.IsTrue(files.Contains("content.xml"));
        }
    }
}
