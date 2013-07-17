using FluoriteAnalyzer.Commons;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace FluoriteAnalyzer.Pipelines
{
    /// <summary>
    /// Takes a DirectoryInfo (e.g., p101) and find all the not-closed logs
    /// and fixes the problem.
    /// </summary>
    public class FixClosingFilter : BasePipelineFilter<DirectoryInfo, DirectoryInfo>
    {
        public FixClosingFilter()
        {
            _settings = new FixClosingFilterSettings();
        }

        // Settings class for this filter.
        protected class FixClosingFilterSettings
        {
            public FixClosingFilterSettings()
            {
            }
        }

        private FixClosingFilterSettings _settings;
        public override object FilterSettings
        {
            get { return _settings; }
        }

        public override DirectoryInfo Compute(DirectoryInfo input)
        {
            var files = input.GetFiles("*.xml");

            foreach (var file in files)
            {
                LogProvider logProvider = new LogProvider();

                bool exceptionThrown = false;

                try
                {
                    logProvider.OpenLog(file.FullName);
                }
                catch (XmlException)
                {
                    exceptionThrown = true;
                }

                if (!exceptionThrown) { continue; }

                // Do the fix.
                string fileContent = null;
                using (StreamReader reader = new StreamReader(file.FullName, Encoding.Default))
                {
                    fileContent = reader.ReadToEnd();
                }

                if (fileContent != null && fileContent.Contains("<Events"))
                {
                    using (StreamWriter writer = new StreamWriter(file.FullName, false, Encoding.Default))
                    {
                        writer.Write(fileContent);
                        writer.WriteLine("</Events>");
                    }
                }
            }

            return input;
        }
    }
}
