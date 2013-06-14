using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace FluoriteAnalyzer.Pipelines
{
    public class MergeFilter : BasePipelineFilter<DirectoryInfo, FileInfo>
    {
        public MergeFilter()
        {
            _settings = new MergeFilterSettings();
        }

        public MergeFilter(string prefix, string postfix, string outputFolder)
        {
            _settings = new MergeFilterSettings();
            _settings.Prefix = prefix;
            _settings.Postfix = postfix;
            _settings.OutputFolder = outputFolder;
        }

        // Settings class for this filter.
        protected class MergeFilterSettings
        {
            public MergeFilterSettings()
            {
                Prefix = "";
                Postfix = "_Merged";
                OutputFolder = "";
            }

            public string Prefix { get; set; }
            public string Postfix { get; set; }
            public string OutputFolder { get; set; }
        }

        private MergeFilterSettings _settings;
        public override object FilterSettings
        {
            get { return _settings; }
        }

        public override FileInfo Compute(DirectoryInfo input)
        {
            try
            {
                // Get all the file infos.
                List<FileInfo> fileInfos = input.EnumerateFiles("*.xml").ToList();

                // Next, determine the mergedFilePath.
                string outputFolder = string.IsNullOrWhiteSpace(_settings.OutputFolder)
                    ? Path.Combine(input.FullName, "..")
                    : _settings.OutputFolder;

                string mergedFilePath = Path.Combine(outputFolder, _settings.Prefix + input.Name + _settings.Postfix + ".xml");

                // Call the merge method.
                Merge(fileInfos, mergedFilePath);

                return new FileInfo(mergedFilePath);
            }
            catch (Exception e)
            {
                throw new Exception("MergeFilter: Exception thrown while processing \"" + input.FullName + "\"", e);
            }
        }

        private void Merge(List<FileInfo> fileInfos, string mergedFilePath)
        {
            var mergedLog = new XmlDocument();
            mergedLog.Load(fileInfos[0].FullName);

            XmlNode root = mergedLog.DocumentElement;
            long baseTimestamp = long.Parse(root.Attributes["startTimestamp"].Value);

            // last id + 1
            long id = long.Parse(root.LastChild.Attributes["__id"].Value) + 1;
            List<XmlComment> comments = new List<XmlComment>();
            comments.Add(GenerateCommentForFile(mergedLog, fileInfos[0], 0, id));

            foreach (FileInfo fileInfo in fileInfos.Skip(1))
            {
                var subsequentLog = new XmlDocument();
                subsequentLog.Load(fileInfo.FullName);

                long startTimestamp = long.Parse(subsequentLog.DocumentElement.Attributes["startTimestamp"].Value);
                long delta = startTimestamp - baseTimestamp;
                long startID = id;

                foreach (XmlNode node in subsequentLog.DocumentElement.ChildNodes)
                {
                    XmlNode copiedNode = mergedLog.ImportNode(node, true);
                    foreach (XmlAttribute attr in copiedNode.Attributes)
                    {
                        if (attr.Name.StartsWith("timestamp"))
                        {
                            attr.Value = (long.Parse(attr.Value) + delta).ToString();
                        }
                        else if (attr.Name == "__id")
                        {
                            attr.Value = id.ToString();
                        }
                    }

                    ++id;
                    root.AppendChild(copiedNode);
                }

                comments.Add(GenerateCommentForFile(mergedLog, fileInfo, startID, id));
            }

            var refChild = mergedLog.FirstChild;
            foreach (XmlComment comment in comments)
            {
                mergedLog.InsertBefore(comment, refChild);
            }

            mergedLog.Save(mergedFilePath);
        }

        private XmlComment GenerateCommentForFile(XmlDocument doc, FileInfo file, long startID, long endID)
        {
            string content = string.Format(
                "Original Log File: {0}, [{1}, {2})",
                file.Name, startID, endID);

            return doc.CreateComment(content);
        }
    }
}
