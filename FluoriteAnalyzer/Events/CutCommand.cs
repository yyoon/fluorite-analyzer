using System.Xml;

namespace FluoriteAnalyzer.Events
{
    internal class CutCommand : Command
    {
        public CutCommand(XmlElement element)
            : base(element)
        {
        }
    }
}