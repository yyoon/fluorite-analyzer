using System.Xml;

namespace FluoriteAnalyzer.Events
{
    internal class MoveCaretCommand : Command
    {
        public MoveCaretCommand(XmlElement element)
            : base(element)
        {
            CaretOffset = int.Parse(GetPropertyValueFromDict("caretOffset"));
            DocOffset = int.Parse(GetPropertyValueFromDict("docOffset"));
        }

        public int CaretOffset { get; private set; }
        public int DocOffset { get; private set; }
    }
}