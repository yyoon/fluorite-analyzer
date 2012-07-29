using System.Xml;
using System;

namespace FluoriteAnalyzer.Events
{
    [Serializable]
    internal class RedoCommand : Command
    {
        public RedoCommand(XmlElement element)
            : base(element)
        {
        }
    }
}