using System.Xml;

namespace FluoriteAnalyzer.Events
{
    internal class Insert : DocumentChange
    {
        public Insert(XmlElement element)
            : base(element)
        {
            Text = GetPropertyValueFromDict("text");
        }

        public string Text { get; set; }
    }
}