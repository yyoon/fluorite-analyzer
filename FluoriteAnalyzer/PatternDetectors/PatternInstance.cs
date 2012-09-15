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

            _involvingEvents = new List<KeyValuePair<string, int>>();
        }

        public Event PrimaryEvent { get; private set; }
        public int PatternLength { get; private set; }
        public string Description { get; private set; }

        private List<KeyValuePair<string, int>> _involvingEvents;

        public override string ToString()
        {
            return Description;
        }

        public void AddInvolvingEvent(string description, int id)
        {
            _involvingEvents.Add(new KeyValuePair<string, int>(description, id));
        }

        public IEnumerable<KeyValuePair<string, int>> GetInvolvingEvents()
        {
            if (_involvingEvents.Count == 0)
            {
                if (PrimaryEvent != null)
                {
                    return new KeyValuePair<string, int>[] { new KeyValuePair<string, int>("Primary Event", PrimaryEvent.ID) };
                }
                else
                {
                    return Enumerable.Empty<KeyValuePair<string, int>>();
                }
            }
            else
            {
                return _involvingEvents;
            }
        }

        public virtual void CopyToClipboard()
        {
            // Do nothing here.
        }
    }
}
