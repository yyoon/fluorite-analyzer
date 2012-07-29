using System.Xml;
using System;

namespace FluoriteAnalyzer.Events
{
    [Serializable]
    internal class EclipseCommand : Command
    {
        public EclipseCommand(XmlElement element)
            : base(element)
        {
            CommandID = GetPropertyValueFromDict("commandID");
        }

        public string CommandID { get; private set; }

        public override string TypeOrCommandString
        {
            get { return CommandID; }
        }
    }
}