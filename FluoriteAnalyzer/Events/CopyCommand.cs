using System.Xml;
using System;

namespace FluoriteAnalyzer.Events
{
    [Serializable]
    internal class CopyCommand : Command
    {
        public CopyCommand(XmlElement element)
            : base(element)
        {
        }
    }
}