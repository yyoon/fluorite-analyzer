namespace FluoriteAnalyzer.Events
{
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