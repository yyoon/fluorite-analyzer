using System;
using System.IO;
using System.Text.RegularExpressions;
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
                    String s1 = reader1.ReadToEnd();
                    String s2 = reader2.ReadToEnd();

                    s1 = Regex.Replace(s1, @"\r\n|\n\r|\n|\r", "\r\n").Trim();
                    s2 = Regex.Replace(s2, @"\r\n|\n\r|\n|\r", "\r\n").Trim();
                    Assert.AreEqual(s1, s2);
                }
            }

            // Delete the output file.
            new FileInfo(filepath).Delete();
            Assert.IsFalse(new FileInfo(filepath).Exists);
        }
    }
}
