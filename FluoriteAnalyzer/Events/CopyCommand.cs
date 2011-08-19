using System.Xml;

namespace FluoriteAnalyzer.Events
{
    internal class CopyCommand : Command
    {
        public CopyCommand(XmlElement element)
            : base(element)
        {
        }
    }
}