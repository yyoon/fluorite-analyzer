using System;
namespace FluoriteAnalyzer.Events
{
    [Serializable]
    internal class DummyEvent : Event
    {
        public DummyEvent(int timestamp)
            : base(timestamp)
        {
        }

        public override EventType EventType
        {
            get { return EventType.Dummy; }
        }
    }
}