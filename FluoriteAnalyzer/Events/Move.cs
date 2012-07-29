using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace FluoriteAnalyzer.Events
{
    [Serializable]
    internal class Move : DocumentChange
    {
        public Move(XmlElement element)
            : base(element)
        {
            StartLine = int.Parse(GetPropertyValueFromDict("startLine"));
            EndLine = int.Parse(GetPropertyValueFromDict("endLine"));

            DeletedText = GetPropertyValueFromDict("deletedText", false);
            InsertedText = GetPropertyValueFromDict("insertedText", false);

            DeletedFrom = GetPropertyValueFromDict("deletedFrom", false);
            InsertedTo = GetPropertyValueFromDict("insertedTo", false);

            InsertionOffset = int.Parse(GetPropertyValueFromDict("insertionOffset"));
            InsertionLength = int.Parse(GetPropertyValueFromDict("insertionLength"));

            OriginalDeleteID = int.Parse(GetPropertyValueFromDict("originalDeleteID"));
        }

        public int StartLine { get; private set; }
        public int EndLine { get; private set; }

        public string DeletedText { get; private set; }
        public string InsertedText { get; private set; }

        public string DeletedFrom { get; private set; }
        public string InsertedTo { get; private set; }

        // normal "offset" and "length" are used for deletion info.
        public int InsertionOffset { get; private set; }
        public int InsertionLength { get; private set; }

        public int OriginalDeleteID { get; private set; }
    }
}
