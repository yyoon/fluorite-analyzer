using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluoriteAnalyzer.Events;

namespace FluoriteAnalyzer.PatternDetectors
{
    class PatternInstance
    {
        public PatternInstance(Event primaryEvent, int patternLength, string description)
        {
            PrimaryEvent = primaryEvent;
            PatternLength = patternLength;
            Description = description;
        }

        public Event PrimaryEvent { get; private set; }
        public int PatternLength { get; private set; }
        public string Description { get; private set; }

        public override string ToString()
        {
            return Description;
        }
    }
}
