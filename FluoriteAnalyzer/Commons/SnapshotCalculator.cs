using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluoriteAnalyzer.Events;
using FluoriteAnalyzer.Utils;

namespace FluoriteAnalyzer.Commons
{
    class SnapshotCalculator
    {
        public SnapshotCalculator(ILogProvider logProvider)
        {
            LogProvider = logProvider;
        }

        public ILogProvider LogProvider { get; private set; }

        public EntireSnapshot CalculateSnapshotAtEvent(Event anEvent)
        {
            EntireSnapshot result = new EntireSnapshot();

            if (!LogProvider.LoggedEvents.Contains(anEvent))
            {
                throw new ArgumentException("The passed event is not originated from the LogProvider of this snapshot calculator");
            }

            var docChanges = LogProvider.LoggedEvents
                .TakeUntil(x => x == anEvent)
                .Where(x => IsValidFileOpenCommand(x) || x is DocumentChange);

            string currentFile = null;
            var lastDocChanges = new Dictionary<string, DocumentChange>();

            Dictionary<string, StringBuilder> files =
                new Dictionary<string,StringBuilder>();

            // calculation loop.
            foreach (Event docChange in docChanges)
            {
                currentFile = ProcessDocumentChange(result, currentFile, lastDocChanges, files, docChange);
            }

            // set the result
            foreach (string filePath in result.FilePaths)
            {
                FileSnapshot fileSnapshot = new FileSnapshot();
                fileSnapshot.Content = files[filePath] != null ? files[filePath].ToString() : null;
                fileSnapshot.LastChange = lastDocChanges[filePath];

                result.FileSnapshots.Add(filePath, fileSnapshot);
            }

            return result;
        }

        private string ProcessDocumentChange(EntireSnapshot result, string currentFile, Dictionary<string, DocumentChange> lastDocChanges, Dictionary<string, StringBuilder> files, Event docChange)
        {
            if (docChange is FileOpenCommand)
            {
                currentFile = ProcessFileOpenCommand((FileOpenCommand)docChange, files, lastDocChanges, result);
            }
            else if (docChange is DocumentChange)
            {
                if (currentFile == null)
                {
                    // Ignore document change events until a FileOpenCommand appears.
                    //continue;
                }

                ApplyDocumentChange((DocumentChange)docChange, files, currentFile, lastDocChanges);
            }
            return currentFile;
        }

        private void ApplyDocumentChange(DocumentChange docChange, Dictionary<string, StringBuilder> files, string currentFile, Dictionary<string, DocumentChange> lastDocChanges)
        {
            StringBuilder builder = files[currentFile];
            if (builder == null) { return; }

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

        private static void UpdateLastDocumentChange(DocumentChange docChange, string currentFile, Dictionary<string, DocumentChange> lastDocChanges)
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

        private static string ProcessFileOpenCommand(FileOpenCommand foc, Dictionary<string, StringBuilder> files, Dictionary<string, DocumentChange> lastDocChanges, EntireSnapshot result)
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

        private bool IsValidFileOpenCommand(Event anEvent)
        {
            FileOpenCommand foc = anEvent as FileOpenCommand;
            if (foc == null) { return false; }
            if (string.IsNullOrEmpty(foc.FilePath)) { return false; }
            if (foc.FilePath == "null") { return false; }

            return true;
        }
    }
}
