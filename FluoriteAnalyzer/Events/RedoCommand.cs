using System.Xml;

namespace FluoriteAnalyzer.Events
{
    internal class RedoCommand : Command
    {
        public RedoCommand(XmlElement element)
            : base(element)
        {
        }
    }
}