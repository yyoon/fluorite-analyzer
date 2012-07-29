using System.Xml;
using System;

namespace FluoriteAnalyzer.Events
{
    [Serializable]
    internal class PasteCommand : Command
    {
        public PasteCommand(XmlElement element)
            : base(element)
        {
        }
    }
}