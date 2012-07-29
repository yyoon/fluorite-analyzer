using System.Xml;
using System;

namespace FluoriteAnalyzer.Events
{
    [Serializable]
    internal class FileOpenCommand : Command
    {
        public FileOpenCommand(XmlElement element)
            : base(element)
        {
            ProjectName = GetPropertyValueFromDict("projectName");
            FilePath = GetPropertyValueFromDict("filePath");

            IsUnknownFile = ProjectName == "" || FilePath == "null";

            DocumentLength = int.Parse(GetPropertyValueFromDict("docLength", false, "-1"));
            ActiveCodeLength = int.Parse(GetPropertyValueFromDict("docActiveCodeLength", false, "-1"));
            ExpressionCount = int.Parse(GetPropertyValueFromDict("docExpressionCount", false, "-1"));
            ASTNodeCount = int.Parse(GetPropertyValueFromDict("docASTNodeCount", false, "-1"));

            Snapshot = GetPropertyValueFromDict("snapshot", false, null);
        }

        public bool IsUnknownFile { get; private set; }

        public string ProjectName { get; private set; }
        public string FilePath { get; private set; }

        public int DocumentLength { get; private set; }
        public int ActiveCodeLength { get; private set; }
        public int ExpressionCount { get; private set; }
        public int ASTNodeCount { get; private set; }

        public string Snapshot { get; private set; }
    }
}