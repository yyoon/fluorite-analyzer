using System.Xml;

namespace FluoriteAnalyzer.Events
{
    internal class RunCommand : Command
    {
        public RunCommand(XmlElement element)
            : base(element)
        {
            IsDebug = GetPropertyValueFromDict("type") == "Debug";
            IsTerminate = GetPropertyValueFromDict("kind") == "Terminate";
            ProjectName = GetPropertyValueFromDict("projectName");
        }

        public bool IsDebug { get; private set; }
        public bool IsTerminate { get; private set; }
        public string ProjectName { get; private set; }
    }
}