using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using FluoriteAnalyzer.Commons;
using FluoriteAnalyzer.Events;
using FluoriteAnalyzer.PatternDetectors;

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

            RenamingDetector detector = RenamingDetector.GetInstance();
            var patterns = detector.DetectAsPatternInstances(provider);

            // Save the results to a file.
            DetectionResult result = new DetectionResult(provider.LogPath, patterns);
            result.SaveToFile(GetSaveFileName(fileInfo.DirectoryName, fileInfo.Name));
            result.ExportToCSV(GetSaveFileName(fileInfo.DirectoryName, fileInfo.Name, "csv"));

            var xmlDoc = new XmlDocument();
            xmlDoc.Load(fileInfo.FullName);

            List<Event> renameEvents = provider.LoggedEvents
                .Where(x => x is DocumentChange || RenamingDetector.IsRenameCommand(x)).ToList();

            foreach (var pattern in patterns)
            {
                int renameIndex = renameEvents.IndexOf(pattern.PrimaryEvent);
                for (int i = 1; i <= pattern.PatternLength; ++i)
                {
                    XmlElement elem = Event.FindCorrespondingXmlElementFromXmlDocument(xmlDoc, renameEvents[renameIndex + i]);
                    xmlDoc.DocumentElement.RemoveChild(elem);
                }
            }

            string newPath = Path.Combine(fileInfo.DirectoryName,
                _settings.Prefix + Path.GetFileNameWithoutExtension(fileInfo.Name) + _settings.Postfix + fileInfo.Extension);

            xmlDoc.Save(newPath);

            AppendResult(fileInfo.DirectoryName, fileInfo.Name,
                string.Format("{0} renamings have been fixed" + Environment.NewLine, patterns.Count()));

            return new FileInfo(newPath);
        }
    }
}
