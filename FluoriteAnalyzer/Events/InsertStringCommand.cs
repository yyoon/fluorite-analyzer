using System.Xml;

namespace FluoriteAnalyzer.Events
{
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