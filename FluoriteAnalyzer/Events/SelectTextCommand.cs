using System.Xml;

namespace FluoriteAnalyzer.Events
{
    internal class SelectTextCommand : Command
    {
        public SelectTextCommand(XmlElement element)
            : base(element)
        {
            Start = int.Parse(GetPropertyValueFromDict("start"));
            End = int.Parse(GetPropertyValueFromDict("end"));
            CaretOffset = int.Parse(GetPropertyValueFromDict("caretOffset"));
        }

        public int Start { get; private set; }
        public int End { get; private set; }
        public int CaretOffset { get; private set; }
    }
}