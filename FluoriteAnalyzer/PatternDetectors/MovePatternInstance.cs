using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluoriteAnalyzer.Events;

namespace FluoriteAnalyzer.PatternDetectors
{
    [Serializable]
    class MovePatternInstance : PatternInstance
    {
        public MovePatternInstance(Event deleteEvent, Event insertEvent, int patternLength, string description, string fromFile, string toFile)
            : base(deleteEvent, patternLength, description)
        {
            InsertEvent = insertEvent;
            FromFile = fromFile;
            ToFile = toFile;
        }

        public Event InsertEvent { get; private set; }

        public string FromFile { get; private set; }
        public string ToFile { get; private set; }
    }
}
