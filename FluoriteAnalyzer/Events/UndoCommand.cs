using System.Xml;
using System;

namespace FluoriteAnalyzer.Events
{
    [Serializable]
    internal class UndoCommand : Command
    {
        public UndoCommand(XmlElement element)
            : base(element)
        {
        }
    }
}