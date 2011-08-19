using System.Xml;

namespace FluoriteAnalyzer.Events
{
    internal abstract class Command : Event
    {
        public Command(XmlElement element)
            : base(element)
        {
        }

        public override EventType EventType
        {
            get { return EventType.Command; }
        }
    }
}