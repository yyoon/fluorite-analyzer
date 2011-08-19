using System.Xml;

namespace FluoriteAnalyzer.Events
{
    internal class Annotation : Event
    {
        #region AnnotationDirection enum

        public enum AnnotationDirection
        {
            Forward,
            Backward
        }

        #endregion

        public Annotation(XmlElement element)
            : base(element)
        {
            Direction = GetPropertyValueFromDict("direction") == "FORWARD"
                            ? AnnotationDirection.Forward
                            : AnnotationDirection.Backward;
            Comment = GetPropertyValueFromDict("comment");
        }

        public override EventType EventType
        {
            get { return EventType.Annotation; }
        }

        public AnnotationDirection Direction { get; private set; }
        public string Comment { get; private set; }
    }
}