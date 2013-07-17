using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluoriteAnalyzer.Events;
using System.Windows.Forms;
using System.IO;

namespace FluoriteAnalyzer.PatternDetectors
{
    [Serializable]
    class OperationConflictPatternInstance : PatternInstance, IPreviewablePatternInstance
    {
        public OperationConflictPatternInstance(Event primaryEvent, int patternLength, string description,
            DocumentChange before, DocumentChange after, string conflictType)
            : base(primaryEvent, patternLength, description)
        {
            Before = before;
            After = after;

            ConflictType = conflictType;
        }

        public DocumentChange Before { get; private set; }
        public DocumentChange After { get; private set; }

        public string ConflictType { get; private set; }

        public override void CopyToClipboard()
        {
            base.CopyToClipboard();

            StringBuilder builder = new StringBuilder();

            string pID = Path.GetFileName(Before.LogFilePath).Substring(1, 3);

            builder.AppendLine(string.Format("[{0}, cmdID: {1}]", pID, Before.ID));
            builder.AppendLine(string.Format("[{0}, cmdID: {1}]", pID, After.ID));

            Clipboard.SetText(builder.ToString());
        }

        public int FirstID
        {
            get { return Before.ID; }
        }

        public int SecondID
        {
            get { return After.ID; }
        }
    }
}
