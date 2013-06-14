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
    public class DetectMovesFilter : BasePipelineFilter<FileInfo, FileInfo>
    {
        public DetectMovesFilter()
        {
            _settings = new DetectMovesFilterSettings();
        }

        public DetectMovesFilter(string prefix, string postfix)
        {
            _settings = new DetectMovesFilterSettings();
            _settings.Prefix = prefix;
            _settings.Postfix = postfix;
        }

        protected class DetectMovesFilterSettings
        {
            public DetectMovesFilterSettings()
            {
                Prefix = "";
                Postfix = "_MovesDetected";
            }

            public string Prefix { get; set; }
            public string Postfix { get; set; }
        }

        private DetectMovesFilterSettings _settings;
        public override object FilterSettings
        {
            get { return _settings; }
        }

        public override FileInfo Compute(FileInfo input)
        {
            try
            {

                return DetectMovesFromFile(input);
            }
            catch (Exception e)
            {
                throw new Exception("DetectMovesFilter: Exception thrown while processing \"" + input.FullName + "\"", e);
            }
        }

        private FileInfo DetectMovesFromFile(FileInfo fileInfo)
        {
            LogProvider provider = new LogProvider();
            provider.OpenLog(fileInfo.FullName);

            MoveDetector moveDetector = MoveDetector.GetInstance();
            var patterns = moveDetector.DetectAsPatternInstances(provider);

            var xmlDoc = new XmlDocument();
            xmlDoc.Load(fileInfo.FullName);

            var documentChanges = provider.LoggedEvents.OfType<DocumentChange>().ToList();

            foreach (MovePatternInstance pattern in patterns)
            {
                int startIndex = documentChanges.IndexOf(pattern.PrimaryEvent as DocumentChange);

                Delete delete = documentChanges[startIndex + 0] as Delete;
                Insert insert = documentChanges[startIndex + 1] as Insert;

                // Same file, same place
                if (pattern.FromFile == pattern.ToFile && delete.Offset == insert.Offset)
                {
                    // Just cancel them out.
                    xmlDoc.DocumentElement.RemoveChild(Event.FindCorrespondingXmlElementFromXmlDocument(xmlDoc, delete));
                    xmlDoc.DocumentElement.RemoveChild(Event.FindCorrespondingXmlElementFromXmlDocument(xmlDoc, insert));
                }
                else
                {
                    // Transform Insert -> Move and then remove Delete.
                    SetMoveElement(xmlDoc, pattern, delete, insert);
                    xmlDoc.DocumentElement.RemoveChild(Event.FindCorrespondingXmlElementFromXmlDocument(xmlDoc, delete));
                }
            }

            string newPath = Path.Combine(fileInfo.DirectoryName,
                _settings.Prefix + Path.GetFileNameWithoutExtension(fileInfo.Name) + _settings.Postfix + fileInfo.Extension);

            xmlDoc.Save(newPath);

            return new FileInfo(newPath);
        }

        private static void SetMoveElement(XmlDocument xmlDoc, MovePatternInstance pattern, Delete delete, Insert insert)
        {
            XmlElement deleteElement = Event.FindCorrespondingXmlElementFromXmlDocument(xmlDoc, delete);
            XmlElement insertElement = Event.FindCorrespondingXmlElementFromXmlDocument(xmlDoc, insert);
            XmlElement child;
            XmlCDataSection cdata;
            XmlAttribute attr;

            XmlElement moveElement = insertElement;
            XmlElement textElement = moveElement.FirstChild as XmlElement;
            moveElement.Attributes["_type"].Value = "Move";
            moveElement.Attributes["offset"].Value = delete.Offset.ToString();
            moveElement.Attributes["length"].Value = delete.Length.ToString();

            attr = moveElement.Attributes["repeat"];
            if (attr == null)
            {
                attr = xmlDoc.CreateAttribute("repeat");
            }
            attr.Value = (delete.RepeatCount + insert.RepeatCount).ToString();

            moveElement.Attributes["timestamp"].Value = delete.Timestamp.ToString();

            if (insert.Timestamp2.HasValue == false)
            {
                attr = xmlDoc.CreateAttribute("timestamp2");
                attr.Value = insert.Timestamp.ToString();
                moveElement.Attributes.Append(attr);
            }

            attr = xmlDoc.CreateAttribute("insertionOffset");
            attr.Value = insert.Offset.ToString();
            moveElement.Attributes.Append(attr);

            attr = xmlDoc.CreateAttribute("insertionLength");
            attr.Value = insert.Length.ToString();
            moveElement.Attributes.Append(attr);

            attr = xmlDoc.CreateAttribute("originalDeleteID");
            attr.Value = delete.ID.ToString();
            moveElement.Attributes.Append(attr);

            // startLine & endLine
            attr = xmlDoc.CreateAttribute("startLine");
            attr.Value = delete.StartLine.ToString();
            moveElement.Attributes.Append(attr);

            attr = xmlDoc.CreateAttribute("endLine");
            attr.Value = delete.EndLine.ToString();
            moveElement.Attributes.Append(attr);

            // take out the first child
            moveElement.RemoveChild(textElement);

            // deletedFrom & insertedTo
            child = xmlDoc.CreateElement("deletedFrom");
            cdata = xmlDoc.CreateCDataSection(pattern.FromFile);
            child.AppendChild(cdata);
            moveElement.AppendChild(child);

            child = xmlDoc.CreateElement("insertedTo");
            cdata = xmlDoc.CreateCDataSection(pattern.ToFile);
            child.AppendChild(cdata);
            moveElement.AppendChild(child);

            // deletedText & insertedText
            child = xmlDoc.CreateElement("deletedText");
            cdata = xmlDoc.CreateCDataSection(delete.Text);
            child.AppendChild(cdata);
            moveElement.AppendChild(child);

            child = xmlDoc.CreateElement("insertedText");
            cdata = textElement.FirstChild as XmlCDataSection;
            textElement.RemoveChild(cdata);
            child.AppendChild(cdata);
            moveElement.AppendChild(child);
        }
    }
}
