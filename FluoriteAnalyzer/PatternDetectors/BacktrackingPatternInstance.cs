using FluoriteAnalyzer.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluoriteAnalyzer.PatternDetectors
{
    [Serializable]
    abstract class BacktrackingPatternInstance : PatternInstance
    {
        public BacktrackingPatternInstance(Event primaryEvent, int patternLength, string description)
            : base(primaryEvent, patternLength, description)
        {
        }

        public enum BacktrackingType : int
        {
            TYPE1,  // Insert -> Delete
            TYPE2,  // Delete -> Insert
        }

        public abstract BacktrackingType Type { get; }
    }
}
