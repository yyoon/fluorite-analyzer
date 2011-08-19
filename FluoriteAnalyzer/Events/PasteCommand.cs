using System.Xml;

namespace FluoriteAnalyzer.Events
{
    internal class PasteCommand : Command
    {
        public PasteCommand(XmlElement element)
            : base(element)
        {
        }
    }
}