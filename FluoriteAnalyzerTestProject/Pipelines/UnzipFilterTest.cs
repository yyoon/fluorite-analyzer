using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace FluoriteAnalyzer.Pipelines
{
    [TestClass]
    public class UnzipFilterTest : BaseFilterTest
    {
        [TestMethod]
        public void UnzipFilterBasicTest()
        {
            string path = GetDataPath();
            string dirname = "test";

            UnzipFilter filter = new UnzipFilter();

            var result = filter.Compute(new DirectoryInfo(Path.Combine(path, dirname)));

            var dinfo = new DirectoryInfo(Path.Combine(path, dirname));

            // It should return the exact same dinfo object
            Assert.AreEqual(dinfo.FullName, result.FullName);

            // All the files should be there
            Assert.AreEqual(5, dinfo.GetFiles("*.xml").Length);

            // No *.lck files
            Assert.AreEqual(0, dinfo.GetFiles("*.lck").Length);

            // No Subdirs
            Assert.AreEqual(0, dinfo.GetDirectories().Length);

            // Later archive overwrites the earlier ones
            var filename = @"Log2013-04-08-11-39-42-502.xml";
            Assert.AreEqual(865, new FileInfo(Path.Combine(path, dirname, filename)).Length);
        }
    }
}
