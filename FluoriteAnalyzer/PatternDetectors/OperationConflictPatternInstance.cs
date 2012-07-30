using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluoriteAnalyzer.Events;

namespace FluoriteAnalyzer.PatternDetectors
{
    class OperationConflictPatternInstance : PatternInstance
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
    }
}
