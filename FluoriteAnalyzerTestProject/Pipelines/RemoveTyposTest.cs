using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace FluoriteAnalyzer.Pipelines
{
    [TestClass]
    public class RemoveTyposTest
    {
        [TestMethod]
        public void BasicTest()
        {
            string path = @"Data\RemoveTyposFilterTest\BasicTest\";
            string postfix = "_TyposRemoved";

            RemoveTyposFilter filter = new RemoveTyposFilter("", postfix);

            filter.Compute(new FileInfo(path + "test.xml"));

            string filepath = path + "test" + postfix + ".xml";
            Assert.IsTrue(new FileInfo(filepath).Exists);

            using (StreamReader reader1 = new StreamReader(filepath))
            {
                string expectedFilePath = Path.Combine(path, "test_expected.xml");
                using (StreamReader reader2 = new StreamReader(expectedFilePath))
                {
                    Assert.AreEqual(reader1.ReadToEnd(), reader2.ReadToEnd());
                }
            }
        }
    }
}
