using System.Xml;
using System;

namespace FluoriteAnalyzer.Events
{
    [Serializable]
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