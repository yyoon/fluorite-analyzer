namespace FluoriteAnalyzer.Commons
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using FluoriteAnalyzer.Events;
    using FluoriteAnalyzer.Utils;

    /// <summary>
    /// Snapshot calculator class that produces snapshots of source code at any
    /// time, given a log provider.
    /// </summary>
    internal class SnapshotCalculator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SnapshotCalculator"/> class.
        /// </summary>
        /// <param name="logProvider">The log provider.</param>
        public SnapshotCalculator(ILogProvider logProvider)
        {
            this.LogProvider = logProvider;
        }

        /// <summary>
        /// Gets the log provider.
        /// </summary>
        /// <value>
        /// The log provider.
        /// </value>
        public ILogProvider LogProvider { get; private set; }

        /// <summary>
        /// Calculates the snapshot at the given event ID.
        /// </summary>
        /// <param name="id">The unique identifier of an event.</param>
        /// <returns>The entire snapshot</returns>
        public EntireSnapshot CalculateSnapshotAtID(int id)
        {
            // Make sure that the given id is valid.
            Event anEvent = this.LogProvider.LoggedEvents
                .FirstOrDefault(x => x.ID == id);

            if (anEvent != null)
            {
                return this.CalculateSnapshotAtEvent(anEvent);
            }

            return new EntireSnapshot();
        }

        /// <summary>
        /// Calculates the snapshot at the given event.
        /// </summary>
        /// <param name="anEvent">The event.</param>
        /// <returns>
        /// The entire snapshot
        /// </returns>
        public EntireSnapshot CalculateSnapshotAtEvent(Event anEvent)
        {
            return CalculateSnapshotAtEvent(null, null, anEvent);
        }

        /// <summary>
        /// Calculates the snapshot at the given event.
        /// </summary>
        /// <param name="lastEvent">The last event associated with the last known snapshot.</param>
        /// <param name="lastKnownSnapshot">The last known snapshot.</param>
        /// <param name="anEvent">The event.</param>
        /// <returns>
        /// The entire snapshot
        /// </returns>
        public EntireSnapshot CalculateSnapshotAtEvent(
            Event lastEvent,
            EntireSnapshot lastKnownSnapshot,
            Event anEvent)
        {
            if (!this.LogProvider.LoggedEvents.Contains(anEvent) ||
                (lastEvent != null && !this.LogProvider.LoggedEvents.Contains(lastEvent)))
            {
                throw new ArgumentException(
                    "The passed event is not originated from the " +
                    "LogProvider of this snapshot calculator");
            }

            if (lastEvent != null && lastKnownSnapshot == null ||
                lastEvent == null && lastKnownSnapshot != null)
            {
                throw new ArgumentException(
                    "lastEvent and lastKnownSnapshot must be either both null or both non-null.");
            }

            EntireSnapshot result = lastKnownSnapshot != null
                ? new EntireSnapshot(lastKnownSnapshot)
                : new EntireSnapshot();

            var docChanges = this.LogProvider.LoggedEvents;
            if (lastEvent != null)
            {
                docChanges = docChanges.SkipUntil(x => x == lastEvent);
            }

            docChanges = docChanges
                .TakeUntil(x => x == anEvent)
                .Where(x => this.IsValidFileOpenCommand(x) || x is DocumentChange);

            string currentFile = result.CurrentFile;

            var lastDocChanges = new Dictionary<string, DocumentChange>();
            Dictionary<string, StringBuilder> files = new Dictionary<string, StringBuilder>();

            foreach (var fileSnapshot in result.FileSnapshots.Values)
            {
                lastDocChanges.Add(fileSnapshot.FilePath, fileSnapshot.LastChange);
                files.Add(fileSnapshot.FilePath, new StringBuilder(fileSnapshot.Content));
            }

            // calculation loop.
            foreach (Event docChange in docChanges)
            {
                currentFile = this.ProcessDocumentChange(
                    result, currentFile, lastDocChanges, files, docChange);
            }

            // set the result
            foreach (string filePath in result.FilePaths)
            {
                FileSnapshot fileSnapshot = new FileSnapshot();
                fileSnapshot.FilePath = filePath;
                fileSnapshot.Content = files[filePath] != null ? files[filePath].ToString() : null;
                fileSnapshot.LastChange = lastDocChanges[filePath];

                result.FileSnapshots[filePath] = fileSnapshot;
            }

            return result;
        }

        /// <summary>
        /// Updates the last document change information, which is used for highlighting the last
        /// change made in a specific file.
        /// </summary>
        /// <param name="docChange">The document change.</param>
        /// <param name="currentFile">The current file.</param>
        /// <param name="lastDocChanges">The last document changes.</param>
        private static void UpdateLastDocumentChange(
            DocumentChange docChange,
            string currentFile,
            Dictionary<string, DocumentChange> lastDocChanges)
        {
            if (docChange is Move)
            {
                Move move = (Move)docChange;
                lastDocChanges[move.DeletedFrom] = docChange;
                lastDocChanges[move.InsertedTo] = docChange;
            }
            else
            {
                lastDocChanges[currentFile] = docChange;
            }
        }

        /// <summary>
        /// Processes the file open command.
        /// </summary>
        /// <param name="foc">The file open command.</param>
        /// <param name="files">The files.</param>
        /// <param name="lastDocChanges">The last document changes.</param>
        /// <param name="result">The result.</param>
        /// <returns>The new current file path.</returns>
        private static string ProcessFileOpenCommand(
            FileOpenCommand foc,
            Dictionary<string, StringBuilder> files,
            Dictionary<string, DocumentChange> lastDocChanges,
            EntireSnapshot result)
        {
            string currentFile = foc.FilePath;

            if (!result.FilePaths.Contains(currentFile))
            {
                result.FilePaths.Insert(0, currentFile);
                lastDocChanges.Add(currentFile, null);

                if (foc.Snapshot == null)
                {
                    files.Add(currentFile, null);
                }
                else
                {
                    files.Add(currentFile, new StringBuilder(foc.Snapshot));
                }
            }
            else
            {
                // Bring the current file to the front
                result.FilePaths.Remove(currentFile);
                result.FilePaths.Insert(0, currentFile);

                // If this command has a new snapshot, replace it.
                if (foc.Snapshot != null)
                {
                    files[currentFile] = new StringBuilder(foc.Snapshot);
                }
            }

            return currentFile;
        }

        /// <summary>
        /// Processes a document change event and updates a given snapshot based on the doc change.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="currentFile">The current file.</param>
        /// <param name="lastDocChanges">The last document changes.</param>
        /// <param name="files">The files.</param>
        /// <param name="docChange">The document change.</param>
        /// <returns>The path of the current file.</returns>
        private string ProcessDocumentChange(
            EntireSnapshot result,
            string currentFile,
            Dictionary<string, DocumentChange> lastDocChanges,
            Dictionary<string, StringBuilder> files,
            Event docChange)
        {
            if (docChange is FileOpenCommand)
            {
                currentFile = ProcessFileOpenCommand(
                    (FileOpenCommand)docChange,
                    files,
                    lastDocChanges,
                    result);
            }
            else if (docChange is DocumentChange)
            {
                if (currentFile == null)
                {
                    // Ignore document change events until a FileOpenCommand appears.
                    // continue;
                }

                this.ApplyDocumentChange(
                    (DocumentChange)docChange, files, currentFile, lastDocChanges);
            }

            return currentFile;
        }

        /// <summary>
        /// Applies the given document change to the result.
        /// </summary>
        /// <param name="docChange">The document change.</param>
        /// <param name="files">The files.</param>
        /// <param name="currentFile">The current file.</param>
        /// <param name="lastDocChanges">The last document changes.</param>
        private void ApplyDocumentChange(
            DocumentChange docChange,
            Dictionary<string, StringBuilder> files,
            string currentFile,
            Dictionary<string, DocumentChange> lastDocChanges)
        {
            StringBuilder builder = files[currentFile];
            if (builder == null)
            {
                return;
            }

            if (docChange is Delete)
            {
                Delete delete = (Delete)docChange;
                builder.Remove(delete.Offset, delete.Length);
            }
            else if (docChange is Insert)
            {
                Insert insert = (Insert)docChange;
                builder.Insert(insert.Offset, insert.Text);
            }
            else if (docChange is Replace)
            {
                Replace replace = (Replace)docChange;
                builder.Remove(replace.Offset, replace.Length);
                builder.Insert(replace.Offset, replace.InsertedText);
            }
            else if (docChange is Move)
            {
                Move move = (Move)docChange;

                if (files.ContainsKey(move.DeletedFrom) && files[move.DeletedFrom] != null)
                {
                    files[move.DeletedFrom].Remove(move.DeletionOffset, move.DeletionLength);
                }

                if (files.ContainsKey(move.InsertedTo) && files[move.InsertedTo] != null)
                {
                    files[move.InsertedTo].Insert(move.InsertionOffset, move.InsertedText);
                }
            }

            // Update the last document change.
            UpdateLastDocumentChange(docChange, currentFile, lastDocChanges);
        }

        /// <summary>
        /// Determines whether the given event is a file open command with a valid file path.
        /// </summary>
        /// <param name="anEvent">An event.</param>
        /// <returns>true if the given event is a valid file open command, false otherwise</returns>
        private bool IsValidFileOpenCommand(Event anEvent)
        {
            FileOpenCommand foc = anEvent as FileOpenCommand;
            if (foc == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(foc.FilePath))
            {
                return false;
            }

            if (foc.FilePath == "null")
            {
                return false;
            }

            return true;
        }
    }
}
