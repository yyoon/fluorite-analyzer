namespace FluoriteAnalyzer.Commons
{
    using System.Collections.Generic;
    using FluoriteAnalyzer.Events;

    /// <summary>
    /// Interface for LogProvider
    /// </summary>
    public interface ILogProvider
    {
        /// <summary>
        /// Gets the log path.
        /// </summary>
        /// <value>
        /// The log path.
        /// </value>
        string LogPath { get; }

        /// <summary>
        /// Gets the logged events.
        /// </summary>
        /// <value>
        /// The logged events.
        /// </value>
        IEnumerable<Event> LoggedEvents { get; }

        /// <summary>
        /// Gets the time difference.
        /// </summary>
        /// <value>
        /// The time difference.
        /// </value>
        long? TimeDiff { get; }

        /// <summary>
        /// Gets the video time.
        /// </summary>
        /// <param name="anEvent">An event.</param>
        /// <returns>the video time calculated by the TimeDiff value</returns>
        string GetVideoTime(Event anEvent);

        /// <summary>
        /// Gets the video time.
        /// </summary>
        /// <param name="timestamp">The timestamp.</param>
        /// <returns>the video time calculated by the TimeDiff value</returns>
        string GetVideoTime(long timestamp);

        // TODO: Generalize the following methods.

        /// <summary>
        /// Determines whether the given event is caused by a paste command.
        /// </summary>
        /// <param name="dc">The dc.</param>
        /// <returns>true if the dc is caused by a paste command; false otherwise</returns>
        bool CausedByPaste(Event dc);

        /// <summary>
        /// Determines whether the given event is caused by an assist command.
        /// </summary>
        /// <param name="dc">The dc.</param>
        /// <returns>true if the dc is caused by an assist command; false otherwise</returns>
        bool CausedByAssist(DocumentChange dc);

        /// <summary>
        /// Determines whether the given event is caused by an auto indent.
        /// </summary>
        /// <param name="dc">The dc.</param>
        /// <returns>true if the dc is caused by an auto indent; false otherwise</returns>
        bool CausedByAutoIndent(DocumentChange dc);

        /// <summary>
        /// Determines whether the given event is caused by typing.
        /// </summary>
        /// <param name="dc">The dc.</param>
        /// <returns>true if the dc is caused by typing; false otherwise</returns>
        bool CausedByInsertString(DocumentChange dc);

        /// <summary>
        /// Determines whether the given event is caused by an auto-formatting command.
        /// </summary>
        /// <param name="dc">The dc.</param>
        /// <returns>
        /// true if the dc is caused by an auto-formatting command; false otherwise
        /// </returns>
        bool CausedByAutoFormatting(Event dc);
    }
}