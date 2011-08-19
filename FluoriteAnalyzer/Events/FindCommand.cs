using System.Xml;

namespace FluoriteAnalyzer.Events
{
    internal class FindCommand : Command
    {
        public FindCommand(XmlElement element)
            : base(element)
        {
            IsForward = bool.Parse(GetPropertyValueFromDict("forward"));
            IsCaseSensitive = bool.Parse(GetPropertyValueFromDict("caseSensitive"));
            IsRegExp = bool.Parse(GetPropertyValueFromDict("regexp"));
            IsMatchWord = bool.Parse(GetPropertyValueFromDict("matchWord"));
            IsReplaceAll = bool.Parse(GetPropertyValueFromDict("replaceAll"));
            IsSelectionScope = bool.Parse(GetPropertyValueFromDict("selectionScope"));
            IsWrapSearch = bool.Parse(GetPropertyValueFromDict("wrapSearch"));

            // These two strings can be null.
            SearchString = GetPropertyValueFromDict("searchString", false);
            ReplaceString = GetPropertyValueFromDict("replaceString", false);
        }

        public bool IsForward { get; private set; }
        public bool IsCaseSensitive { get; private set; }
        public bool IsRegExp { get; private set; }
        public bool IsMatchWord { get; private set; }
        public bool IsReplaceAll { get; private set; }
        public bool IsSelectionScope { get; private set; }
        public bool IsWrapSearch { get; private set; }

        public string SearchString { get; private set; }
        public string ReplaceString { get; private set; }
    }
}