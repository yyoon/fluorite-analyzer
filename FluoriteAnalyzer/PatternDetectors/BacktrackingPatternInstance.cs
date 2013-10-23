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

        protected abstract Event Event1 { get; }
        protected  abstract Event Event2 { get; }

        public override string CSVLine
        {
            get
            {
                long t1 = Event1.Timestamp2 ?? Event1.Timestamp;
                long t2 = Event2.Timestamp2 ?? Event2.Timestamp;

                return string.Join(", ",
                    new object[]
                    {
                        Type.ToString(),
                        Event1.ID,
                        Event2.ID,
                        Event2.ID - Event1.ID,
                        t1,
                        t2,
                        t2 - t1,
                    });
            }
        }
    }
}
