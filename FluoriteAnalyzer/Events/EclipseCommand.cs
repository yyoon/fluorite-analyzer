using System.Xml;

namespace FluoriteAnalyzer.Events
{
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