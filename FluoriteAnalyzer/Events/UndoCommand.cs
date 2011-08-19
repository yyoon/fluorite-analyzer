using System.Xml;

namespace FluoriteAnalyzer.Events
{
    internal class UndoCommand : Command
    {
        public UndoCommand(XmlElement element)
            : base(element)
        {
        }
    }
}