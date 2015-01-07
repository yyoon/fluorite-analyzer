namespace FluoriteAnalyzer.Events
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml;

    [Serializable]
    internal class JUnitCommand : Command
    {
        public JUnitCommand(XmlElement element)
            : base(element)
        {
            // TODO Read the TestSession data.
        }
    }
}
