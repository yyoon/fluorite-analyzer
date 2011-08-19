using System.Xml;

namespace FluoriteAnalyzer.Events
{
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
        public int InsertionLength { get; private set; }

        public string DeletedText { get; private set; }
        public string InsertedText { get; private set; }
    }
}