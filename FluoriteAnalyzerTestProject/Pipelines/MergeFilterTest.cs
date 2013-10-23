using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace FluoriteAnalyzer.Pipelines
{
    [TestClass]
    public class MergeFilterTest : BaseFilterTest
    {
        [TestMethod]
        [DeploymentItem(@"Data\MergeFilterTest\MergeFilterBasicTest", @"Data\MergeFilterTest\MergeFilterBasicTest")]
        public void MergeFilterBasicTest()
        {
            string path = GetDataPath();
            string dirname = "test";
            string postfix = "_Merged";

            MergeFilter filter = new MergeFilter("", postfix, "");

            filter.Compute(new DirectoryInfo(Path.Combine(path, dirname)));

            string filepath = Path.Combine(path, dirname + postfix + ".xml");
            Assert.IsTrue(new FileInfo(filepath).Exists);

            using (StreamReader reader1 = new StreamReader(filepath))
            {
                string expectedFilePath = Path.Combine(path, "expected_output.xml");
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
