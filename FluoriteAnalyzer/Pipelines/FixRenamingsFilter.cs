using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FluoriteAnalyzer.Commons;

namespace FluoriteAnalyzer.Pipelines
{
    public class FixRenamingsFilter : BasePipelineFilter<FileInfo, FileInfo>
    {
        public FixRenamingsFilter()
        {
            _settings = new FixRenamingsFilterSettings();
        }

        public FixRenamingsFilter(string prefix, string postfix)
        {
            _settings = new FixRenamingsFilterSettings();
            _settings.Prefix = prefix;
            _settings.Postfix = postfix;
        }

        protected class FixRenamingsFilterSettings
        {
            public FixRenamingsFilterSettings()
            {
                Prefix = string.Empty;
                Postfix = "_RenamingsFixed";
            }

            public string Prefix { get; set; }
            public string Postfix { get; set; }
        }

        private FixRenamingsFilterSettings _settings;
        public override object FilterSettings
        {
            get { return _settings; }
        }

        public override FileInfo Compute(FileInfo input)
        {
            try
            {
                return FixRenamingsFromFile(input);
            }
            catch (Exception e)
            {
                throw new Exception("FixRenamingsFilter: Exception thrown while processing \"" + input.FullName + "\"", e);
            }
        }

        private FileInfo FixRenamingsFromFile(FileInfo fileInfo)
        {
            AppendResult(fileInfo.DirectoryName, fileInfo.Name,
                "=========================================" + Environment.NewLine +
                "Fix Renamings Start: " + DateTime.Now.ToString());

            LogProvider provider = new LogProvider();
            provider.OpenLog(fileInfo.FullName);

            // TODO implement this!!

            throw new NotImplementedException();
        }
    }
}
