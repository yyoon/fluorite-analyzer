using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace FluoriteAnalyzer.Pipelines
{
    [TestClass]
    public class RemoveTyposFilterTest : BaseFilterTest
    {
        [TestMethod]
        public void RemoveTyposFilterBasicTest()
        {
            string path = GetDataPath();
            string postfix = "_TyposRemoved";

            RemoveTyposFilter filter = new RemoveTyposFilter("", postfix);

            filter.Compute(new FileInfo(Path.Combine(path, "test.xml")));

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
