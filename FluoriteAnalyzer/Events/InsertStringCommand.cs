using System.Xml;
using System;

namespace FluoriteAnalyzer.Events
{
    [Serializable]
    internal class InsertStringCommand : Command
    {
        public InsertStringCommand(XmlElement element)
            : base(element)
        {
            Data = GetPropertyValueFromDict("data");
        }

        public string Data { get; private set; }
    }
}