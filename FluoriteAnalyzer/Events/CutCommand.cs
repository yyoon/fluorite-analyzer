using System.Xml;
using System;

namespace FluoriteAnalyzer.Events
{
    [Serializable]
    internal class CutCommand : Command
    {
        public CutCommand(XmlElement element)
            : base(element)
        {
        }
    }
}