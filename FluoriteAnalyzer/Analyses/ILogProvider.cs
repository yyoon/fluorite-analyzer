using System.Collections.Generic;
using FluoriteAnalyzer.Events;

namespace FluoriteAnalyzer.Analyses
{
    internal interface ILogProvider
    {
        string LogPath { get; }
        IEnumerable<Event> LoggedEvents { get; }

        long? TimeDiff { get; }

        string GetVideoTime(Event anEvent);
        string GetVideoTime(long timestamp);
    }
}