using FluoriteAnalyzer.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluoriteAnalyzer.PatternDetectors
{
    [Serializable]
    class Type2BacktrackingPatternInstance : BacktrackingPatternInstance, IPreviewablePatternInstance
    {
        public Type2BacktrackingPatternInstance(Event delete, Event insert, string description)
            : base(delete, 2, description)
        {
            this.Delete = delete;
            this.Insert = insert;

            AddInvolvingEvent("Delete", delete.ID);
            AddInvolvingEvent("Insert", insert.ID);
        }

        public Event Delete { get; set; }
        public Event Insert { get; set; }

        protected override Event Event1
        {
            get { return Delete; }
        }

        protected override Event Event2
        {
            get { return Insert; }
        }

        public override BacktrackingPatternInstance.BacktrackingType Type
        {
            get { return BacktrackingType.TYPE2; }
        }

        public int FirstID
        {
            get { return Delete.ID; }
        }

        public int SecondID
        {
            get { return Insert.ID; }
        }
    }
}
