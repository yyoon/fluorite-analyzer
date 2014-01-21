using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace FluoriteAnalyzer.Events
{
    [Serializable]
    internal class HistorySearchCommand : Command
    {
        public HistorySearchCommand(XmlElement element)
            : base(element)
        {
            IsCaseSensitive = bool.Parse(GetPropertyValueFromDict("caseSensitive"));
            IsSelectedCode = bool.Parse(GetPropertyValueFromDict("selectedCode"));
            IsCurrentSession = bool.Parse(GetPropertyValueFromDict("currentSession"));

            SearchText = GetPropertyValueFromDict("searchText", false);
        }

        public bool IsCaseSensitive { get; private set; }
        public bool IsSelectedCode { get; private set; }
        public bool IsCurrentSession { get; private set; }

        public string SearchText { get; private set; }
    }
}
