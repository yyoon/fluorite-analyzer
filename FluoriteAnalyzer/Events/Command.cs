using System.Xml;
using System;

namespace FluoriteAnalyzer.Events
{
    [Serializable]
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