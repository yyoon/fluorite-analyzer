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

        public string DeletedFrom { get; internal set; }
        public string InsertedTo { get; internal set; }

        // normal "offset" and "length" are used for deletion info.
        public int InsertionOffset { get; internal set; }
        public int InsertionLength { get; internal set; }

        public int DeletionOffset { get { return base.Offset; } internal set { base.Offset = value; } }
        public int DeletionLength { get { return base.Length; } internal set { base.Length = value; } }

        public int OriginalDeleteID { get; private set; }

        public int EffectiveDeletionOffset
        {
            get
            {
                if (DeletedFrom == InsertedTo && InsertionOffset < DeletionOffset)
                    return DeletionOffset + InsertionLength;
                else
                    return DeletionOffset;
            }
        }
    }
}
