using System.Xml;

namespace FluoriteAnalyzer.Events
{
    internal class Delete : DocumentChange
    {
        public Delete(XmlElement element)
            : base(element)
        {
            StartLine = int.Parse(GetPropertyValueFromDict("startLine"));
            EndLine = int.Parse(GetPropertyValueFromDict("endLine"));

            Text = GetPropertyValueFromDict("text");
        }

        public int StartLine { get; private set; }
        public int EndLine { get; private set; }

        public string Text { get; private set; }
    }
}