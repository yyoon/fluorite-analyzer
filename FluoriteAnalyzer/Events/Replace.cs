using System.Xml;
using System;

namespace FluoriteAnalyzer.Events
{
    [Serializable]
    internal class Replace : DocumentChange
    {
        public Replace(XmlElement element)
            : base(element)
        {
            StartLine = int.Parse(GetPropertyValueFromDict("startLine"));
            EndLine = int.Parse(GetPropertyValueFromDict("endLine"));

            DeletedText = GetPropertyValueFromDict("deletedText", false);
            InsertedText = GetPropertyValueFromDict("insertedText", false);

            // For backward compatibility
            InsertionLength = int.Parse(GetPropertyValueFromDict("insertionLength", false, "-1"));
            if (InsertionLength == -1)
            {
                if (InsertedText != null)
                {
                    InsertionLength = InsertedText.Length;
                }
            }
        }

        public int StartLine { get; private set; }
        public int EndLine { get; private set; }
        public int InsertionLength { get; internal set; }

        public string DeletedText { get; internal set; }
        public string InsertedText { get; internal set; }

        public int LengthDiff { get { return InsertionLength - Length; } }
    }
}