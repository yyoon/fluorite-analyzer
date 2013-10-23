using FluoriteAnalyzer.Commons;
using FluoriteAnalyzer.Events;
using FluoriteAnalyzer.PatternDetectors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace FluoriteAnalyzer.Pipelines
{
    public class RemoveTyposFilter : BasePipelineFilter<FileInfo, FileInfo>
    {
        public RemoveTyposFilter()
        {
            _settings = new RemoveTyposFilterSettings();
        }

        public RemoveTyposFilter(string prefix, string postfix)
        {
            _settings = new RemoveTyposFilterSettings();
            _settings.Prefix = prefix;
            _settings.Postfix = postfix;
        }

        protected class RemoveTyposFilterSettings
        {
            public RemoveTyposFilterSettings()
            {
                Prefix = "";
                Postfix = "_TyposRemoved";
            }

            public string Prefix { get; set; }
            public string Postfix { get; set; }
        }

        private RemoveTyposFilterSettings _settings;
        public override object FilterSettings
        {
            get { return _settings; }
        }

        public override FileInfo Compute(FileInfo input)
        {
            try
            {
                return RemoveTyposFromFile(input);
            }
            catch (Exception e)
            {
                throw new Exception("RemoveTyposFilter: Exception thrown while processing \"" + input.FullName + "\"", e);
            }
        }

        private FileInfo RemoveTyposFromFile(FileInfo fileInfo)
        {
            AppendResult(fileInfo.DirectoryName, fileInfo.Name,
                "=========================================" + Environment.NewLine + 
                "Remove Typos Start: " + DateTime.Now.ToString());

            LogProvider provider = new LogProvider();
            provider.OpenLog(fileInfo.FullName);

            TypoCorrectionDetector typoDetector = TypoCorrectionDetector.GetInstance();
            var patterns = typoDetector.DetectAsPatternInstances(provider);

            // Save the results to a file.
            DetectionResult result = new DetectionResult(provider.LogPath, patterns);
            result.SaveToFile(GetSaveFileName(fileInfo.DirectoryName, fileInfo.Name));
            result.ExportToCSV(GetSaveFileName(fileInfo.DirectoryName, fileInfo.Name, "csv"));

            var xmlDoc = new XmlDocument();
            xmlDoc.Load(fileInfo.FullName);

            var documentChanges = provider.LoggedEvents.OfType<DocumentChange>().ToList();

            // This should be done in reverse order, to process consecutive typo corrections correctly.
            foreach (PatternInstance pattern in patterns.Reverse())
            {
                // Determine the type
                int startIndex = documentChanges.IndexOf(pattern.PrimaryEvent as DocumentChange);

                // Type 1: Insert -> Delete -> Insert
                if (documentChanges[startIndex + 1] is Delete)
                {
                    ProcessType1(xmlDoc, documentChanges, startIndex);
                }
                // Type 2: Insert -> Replace
                else if (documentChanges[startIndex + 1] is Replace)
                {
                    ProcessType2(xmlDoc, documentChanges, startIndex);
                }
            }

            string newPath = Path.Combine(fileInfo.DirectoryName,
                _settings.Prefix + Path.GetFileNameWithoutExtension(fileInfo.Name) + _settings.Postfix + fileInfo.Extension);

            xmlDoc.Save(newPath);

            AppendResult(fileInfo.DirectoryName, fileInfo.Name,
                string.Format("{0} typo corrections have been removed" + Environment.NewLine, patterns.Count()));

            return new FileInfo(newPath);
        }

        private static void ProcessType1(XmlDocument xmlDoc, List<DocumentChange> documentChanges, int startIndex)
        {
            Insert i1 = documentChanges[startIndex + 0] as Insert;
            Delete d1 = documentChanges[startIndex + 1] as Delete;
            Insert i2 = documentChanges[startIndex + 2] as Insert;

            // Calculate new values of i1
            StringBuilder resultingText = new StringBuilder(i1.Text);
            resultingText.Remove(d1.Offset - i1.Offset, d1.Length);
            resultingText.Insert(i2.Offset - i1.Offset, i2.Text);

            int resultingLength = i1.Length - d1.Length + i2.Length;

            long endingTimestamp = i2.Timestamp2.HasValue ? i2.Timestamp2.Value : i2.Timestamp;

            // Apply these in the XmlElement corresponding with i1.
            ApplyChanges(xmlDoc, i1, resultingText, resultingLength, endingTimestamp);

            // Remove XmlElements corresponding with d1 & i2.
            xmlDoc.DocumentElement.RemoveChild(Event.FindCorrespondingXmlElementFromXmlDocument(xmlDoc, d1));
            xmlDoc.DocumentElement.RemoveChild(Event.FindCorrespondingXmlElementFromXmlDocument(xmlDoc, i2));
        }

        private static void ProcessType2(XmlDocument xmlDoc, List<DocumentChange> documentChanges, int startIndex)
        {
            Insert i1 = documentChanges[startIndex + 0] as Insert;
            Replace r1 = documentChanges[startIndex + 1] as Replace;

            // Calculate new values of i1
            StringBuilder resultingText = new StringBuilder(i1.Text);
            resultingText.Remove(r1.Offset - i1.Offset, r1.Length);
            resultingText.Insert(r1.Offset - i1.Offset, r1.InsertedText);

            int resultingLength = i1.Length - r1.Length + r1.InsertionLength;

            long endingTimestamp = r1.Timestamp2.HasValue ? r1.Timestamp2.Value : r1.Timestamp;

            // Apply these in the XmlElement corresponding with i1.
            ApplyChanges(xmlDoc, i1, resultingText, resultingLength, endingTimestamp);

            // Remove XmlElements corresponding with d1 & i2.
            xmlDoc.DocumentElement.RemoveChild(Event.FindCorrespondingXmlElementFromXmlDocument(xmlDoc, r1));
        }

        private static void ApplyChanges(XmlDocument xmlDoc, Insert insert, StringBuilder resultingText, int resultingLength, long endingTimestamp)
        {
            insert.Text = resultingText.ToString();
            insert.Length = resultingLength;
            insert.Timestamp2 = endingTimestamp;

            XmlElement insertElement = Event.FindCorrespondingXmlElementFromXmlDocument(xmlDoc, insert);   // should be in there!
            insertElement.FirstChild.FirstChild.Value = resultingText.ToString();
            insertElement.Attributes["length"].Value = resultingLength.ToString();
            XmlAttribute timestamp2Attr = insertElement.Attributes["timestamp2"];
            if (timestamp2Attr == null) { timestamp2Attr = xmlDoc.CreateAttribute("timestamp2"); }
            timestamp2Attr.Value = endingTimestamp.ToString();
        }
    }
}
