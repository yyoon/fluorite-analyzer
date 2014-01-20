using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FluoriteAnalyzer.Pipelines
{
    [TestClass]
    public class FixRenamingsFilterTest : BaseFilterTest
    {
        [TestMethod]
        [DeploymentItem(@"Data\FixRenamingsFilterTest\FixRenamingsFilterBasicTest", @"Data\FixRenamingsFilterTest\FixRenamingsFilterBasicTest")]
        public void FixRenamingsFilterBasicTest()
        {
            string path = GetDataPath();
            string postfix = "_RenamingsFixed";

            FixRenamingsFilter filter = new FixRenamingsFilter("", postfix);

            filter.Compute(new System.IO.FileInfo(Path.Combine(path, "test.xml")));

            string filepath = Path.Combine(path, "test" + postfix + ".xml");
            Assert.IsTrue(new FileInfo(filepath).Exists);

            using (StreamReader reader1 = new StreamReader(filepath))
            {
                string expectedFilePath = Path.Combine(path, "test_expected.xml");
                using (StreamReader reader2 = new StreamReader(expectedFilePath))
                {
                    Assert.AreEqual(reader1.ReadToEnd(), reader2.ReadToEnd());
                }
            }

            // Delete the output file.
            new FileInfo(filepath).Delete();
            Assert.IsFalse(new FileInfo(filepath).Exists);
        }
    }
}
