using System.Collections.Generic;
using FluoriteAnalyzer.Events;

namespace FluoriteAnalyzer.Commons
{
    public interface ILogProvider
    {
        string LogPath { get; }
        IEnumerable<Event> LoggedEvents { get; }

        long? TimeDiff { get; }

        string GetVideoTime(Event anEvent);
        string GetVideoTime(long timestamp);

        // TODO: Generalize
        bool CausedByPaste(Event dc);
        bool CausedByAssist(DocumentChange dc);
        bool CausedByAutoIndent(DocumentChange dc);
        bool CausedByInsertString(DocumentChange dc);
        bool CausedByAutoFormatting(Event dc);
    }
}