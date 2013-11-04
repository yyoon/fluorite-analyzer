namespace FluoriteAnalyzer.PatternDetectors
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using DiffMatchPatch;
    using FluoriteAnalyzer.Commons;
    using FluoriteAnalyzer.Events;

    /// <summary>
    /// Detect some types of backtracking.
    /// Heuristics methods are used to filter out false positives.
    /// </summary>
    internal class BacktrackingDetector : AbstractPatternDetector
    {
        /// <summary>
        /// The minimum size threshold for Type I backtracking
        /// </summary>
        private static readonly int Type1SizeThreshold = 30;

        /// <summary>
        /// The minimum size threshold for Type II backtracking
        /// </summary>
        private static readonly int Type2SizeThreshold = 30;

        /// <summary>
        /// The static instance of this detector.
        /// </summary>
        private static BacktrackingDetector instance = null;

        /// <summary>
        /// Gets or sets the current file's full path.
        /// </summary>
        /// <value>
        /// The current file.
        /// </value>
        private string CurrentFile { get; set; }

        /// <summary>
        /// Gets or sets the snapshots.
        /// </summary>
        /// <value>
        /// The snapshots.
        /// </value>
        private Dictionary<string, string> Snapshots { get; set; }

        /// <summary>
        /// Gets or sets the patterns.
        /// </summary>
        /// <value>
        /// The patterns.
        /// </value>
        private List<BacktrackingPatternInstance> Patterns { get; set; }

        /// <summary>
        /// Gets or sets the insert segments.
        /// </summary>
        /// <value>
        /// The insert segments.
        /// </value>
        private Dictionary<string, List<InsertSegment>> InsertSegments { get; set; }

        /// <summary>
        /// Gets or sets the delete segments.
        /// </summary>
        /// <value>
        /// The delete segments.
        /// </value>
        private List<DeleteSegment> DeleteSegments { get; set; }

        /// <summary>
        /// Detects backtracking patterns and return a list of detected instances.
        /// </summary>
        /// <param name="logProvider">The log provider.</param>
        /// <returns>
        /// Detected instances of BacktrackingPatternInstance.
        /// </returns>
        public override IEnumerable<PatternInstance> DetectAsPatternInstances(ILogProvider logProvider)
        {
            List<Event> completeList = logProvider.LoggedEvents.ToList();
            List<Event> dcList = logProvider.LoggedEvents.Where(x => x is DocumentChange || x is FileOpenCommand).ToList();

            this.Patterns = new List<BacktrackingPatternInstance>();

            // Initialize
            this.CurrentFile = null;
            this.Snapshots = new Dictionary<string, string>();
            this.InsertSegments = new Dictionary<string, List<InsertSegment>>();
            this.DeleteSegments = new List<DeleteSegment>();

            foreach (int i in Enumerable.Range(0, dcList.Count))
            {
                if (dcList[i] is FileOpenCommand)
                {
                    this.ProcessFileOpenCommand(dcList, i, logProvider);
                }
                else if (dcList[i] is Insert)
                {
                    Insert insert = (Insert)dcList[i];
                    this.ProcessInsert(insert, insert.Offset, insert.Text, logProvider);
                }
                else if (dcList[i] is Delete)
                {
                    Delete delete = (Delete)dcList[i];
                    this.ProcessDelete(delete, delete.Offset, delete.Length, logProvider);
                }
                else if (dcList[i] is Replace)
                {
                    // Process as if this was a two separate Delete / Insert commands.
                    Replace replace = (Replace)dcList[i];
                    if (replace.InsertedText.StartsWith(replace.DeletedText))
                    {
                        if (replace.InsertedText.Length > replace.DeletedText.Length)
                        {
                            this.ProcessInsert(replace, replace.Offset + replace.Length, replace.InsertedText.Substring(replace.Length), logProvider);
                        }
                    }
                    else
                    {
                        this.ProcessDelete(replace, replace.Offset, replace.Length, logProvider);
                        this.ProcessInsert(replace, replace.Offset, replace.InsertedText, logProvider);
                    }
                }
                else if (dcList[i] is Move)
                {
                    // Process the snapshot, but don't count this as backtracking...? How??
                    Move move = (Move)dcList[i];
                }
            }

            return this.Patterns;
        }

        /// <summary>
        /// Gets the instance of this detector.
        /// </summary>
        /// <returns>
        /// The static instance of backtracking detector.
        /// </returns>
        internal static BacktrackingDetector GetInstance()
        {
            return instance ?? (instance = new BacktrackingDetector());
        }

        /// <summary>
        /// Processes the file open command.
        /// </summary>
        /// <param name="dcList">The dc list.</param>
        /// <param name="i">The i.</param>
        /// <param name="logProvider">The log provider.</param>
        private void ProcessFileOpenCommand(List<Event> dcList, int i, ILogProvider logProvider)
        {
            FileOpenCommand foc = (FileOpenCommand)dcList[i];
            this.CurrentFile = foc.FilePath;

            if (foc.Snapshot != null)
            {
                if (!this.Snapshots.ContainsKey(this.CurrentFile))
                {
                    this.Snapshots.Add(this.CurrentFile, foc.Snapshot);
                    this.InsertSegments.Add(this.CurrentFile, new List<InsertSegment>());
                }
                else
                {
                    if (foc.Snapshot == this.Snapshots[this.CurrentFile])
                    {
                        // Do nothing.
                    }
                    else if (this.Snapshots[this.CurrentFile] == null)
                    {
                        this.Snapshots[this.CurrentFile] = foc.Snapshot;
                        this.InsertSegments[this.CurrentFile] = new List<InsertSegment>();
                    }
                    else
                    {
                        // Extracts the diffs
                        string before = this.Snapshots[this.CurrentFile];
                        string after = foc.Snapshot;

                        diff_match_patch dmp = new diff_match_patch();
                        List<Diff> diffs = dmp.diff_main(before, after);
                        int curOffset = 0;
                        int curLength = before.Length;

                        foreach (Diff diff in diffs)
                        {
                            switch (diff.operation)
                            {
                                case Operation.INSERT:
                                    this.ProcessInsert(foc, curOffset, diff.text, logProvider);

                                    curOffset += diff.text.Length;
                                    curLength += diff.text.Length;
                                    break;

                                case Operation.DELETE:
                                    this.ProcessDelete(foc, curOffset, diff.text.Length, logProvider);

                                    curLength -= diff.text.Length;
                                    break;

                                case Operation.EQUAL:
                                    curOffset += diff.text.Length;
                                    break;
                            }
                        }
                    }
                }
            }
            else
            {
                if (!this.Snapshots.ContainsKey(this.CurrentFile))
                {
                    this.Snapshots.Add(this.CurrentFile, null);
                    this.InsertSegments.Add(this.CurrentFile, new List<InsertSegment>());
                }
            }
        }

        /// <summary>
        /// Processes the insert change.
        /// </summary>
        /// <param name="anEvent">An event.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="insertedText">The inserted text.</param>
        /// <param name="logProvider">The log provider.</param>
        private void ProcessInsert(Event anEvent, int offset, string insertedText, ILogProvider logProvider)
        {
            if (this.Snapshots[this.CurrentFile] == null)
            {
                return;
            }

            // Check if the offset and length are in valid range.
            if (this.Snapshots[this.CurrentFile] != null)
            {
                if (offset < 0 || offset > this.Snapshots[this.CurrentFile].Length)
                {
                    this.ResetCurrentFile();
                    return;
                }
            }

            this.ProcessInsertType1(anEvent, offset, insertedText);
            this.ProcessInsertType2(anEvent, offset, insertedText, logProvider);

            // Update the snapshot.
            string snapshot = this.Snapshots[this.CurrentFile];
            if (snapshot != null)
            {
                snapshot = snapshot.Substring(0, offset) + insertedText + snapshot.Substring(offset);
                this.Snapshots[this.CurrentFile] = snapshot;
            }
        }

        /// <summary>
        /// Processes the insert change for Type I backtracking detection.
        /// </summary>
        /// <param name="anEvent">An event.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="insertedText">The inserted text.</param>
        private void ProcessInsertType1(Event anEvent, int offset, string insertedText)
        {
            // Iterate the segments list
            List<InsertSegment> list = this.InsertSegments[this.CurrentFile];
            InsertSegment segmentToBeAdded = new InsertSegment(anEvent, offset, insertedText.Length, insertedText);
            bool added = false;

            for (int i = 0; i < list.Count; ++i)
            {
                if (list[i].EndOffset <= offset)
                {
                    continue;
                }
                else if (list[i].Offset < offset && offset < list[i].EndOffset)
                {
                    list.Insert(i + 1, list[i].Split(offset));
                    list.Insert(i + 1, segmentToBeAdded);
                    added = true;
                    ++i;
                }
                else if (offset <= list[i].Offset)
                {
                    if (!added)
                    {
                        list.Insert(i, segmentToBeAdded);
                        added = true;
                        ++i;
                    }

                    list[i].Offset += insertedText.Length;
                }
                else
                {
                    Debug.Assert(false, "Invalid control flow");
                }
            }

            if (!added)
            {
                list.Add(segmentToBeAdded);
                added = true;
            }
        }

        /// <summary>
        /// Processes the insert change for Type II backtracking detection.
        /// </summary>
        /// <param name="anEvent">An event.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="insertedText">The inserted text.</param>
        /// <param name="logProvider">The log provider.</param>
        private void ProcessInsertType2(Event anEvent, int offset, string insertedText, ILogProvider logProvider)
        {
            // Do nothing if the insertion happened with a paste command.
            if (logProvider.CausedByPaste(anEvent))
            {
                return;
            }

            if (logProvider.CausedByAutoFormatting(anEvent))
            {
                return;
            }

            // See if there is are any previous deletions that contains this insertedText.
            foreach (var delete in this.DeleteSegments)
            {
                if (delete.Text.Contains(insertedText.Trim()))
                {
                    if (this.FilterType2Backtracking(delete, insertedText))
                    {
                        var desc = string.Format(
                            "Type2 [{2} -> {3}]: \"{0}\" - \"{1}\"",
                            delete.Text,
                            insertedText,
                            delete.Event.ID,
                            anEvent.ID);

                        var instance = new Type2BacktrackingPatternInstance(
                            delete.Event,
                            anEvent,
                            desc);

                        this.Patterns.Add(instance);
                    }
                }

                // TODO Maybe consider very similar piece of code?
            }
        }

        /// <summary>
        /// Processes the delete.
        /// </summary>
        /// <param name="anEvent">An event.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="length">The length.</param>
        /// <param name="logProvider">The log provider.</param>
        private void ProcessDelete(Event anEvent, int offset, int length, ILogProvider logProvider)
        {
            // Current snapshot.
            string snapshot = this.Snapshots[this.CurrentFile];

            if (snapshot == null)
            {
                return;
            }

            // Check if the offset and length are in valid range.
            if (snapshot != null)
            {
                if (offset < 0 || offset + length >= snapshot.Length)
                {
                    this.ResetCurrentFile();
                    return;
                }
            }

            // Deleted text
            string deletedText = snapshot == null ? null : snapshot.Substring(offset, length);

            this.ProcessDeleteType1(anEvent, offset, length, deletedText, logProvider);
            this.ProcessDeleteType2(anEvent, offset, length, deletedText);

            // Update the snapshot.
            if (snapshot != null)
            {
                snapshot = snapshot.Substring(0, offset) + snapshot.Substring(offset + length);
                this.Snapshots[this.CurrentFile] = snapshot;
            }
        }

        /// <summary>
        /// Resets the current file's snapshot status.
        /// </summary>
        private void ResetCurrentFile()
        {
            this.Snapshots[this.CurrentFile] = null;
            this.InsertSegments[this.CurrentFile].Clear();
        }

        /// <summary>
        /// Processes the delete change for Type I backtracking detection.
        /// </summary>
        /// <param name="anEvent">An event.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="length">The length.</param>
        /// <param name="deletedText">The deleted text.</param>
        /// <param name="logProvider">The log provider.</param>
        private void ProcessDeleteType1(Event anEvent, int offset, int length, string deletedText, ILogProvider logProvider)
        {
            if (logProvider.CausedByAutoFormatting(anEvent))
            {
                return;
            }

            int endOffset = offset + length;

            // Iterate the segments list
            var list = this.InsertSegments[this.CurrentFile];

            var detectedInserts = new List<Tuple<InsertSegment, string>>();

            for (int i = 0; i < list.Count; ++i)
            {
                if (endOffset <= list[i].Offset)
                {
                    list[i].Offset -= length;
                }
                else if (offset <= list[i].Offset && list[i].Offset < endOffset && endOffset < list[i].EndOffset)
                {
                    detectedInserts.Add(
                        Tuple.Create(
                            list[i],
                            list[i].Text.Substring(0, endOffset - list[i].Offset)));

                    list.Insert(i + 1, list[i].Split(endOffset));
                    list.RemoveAt(i);
                    --i;
                }
                else if (offset <= list[i].Offset && list[i].EndOffset <= endOffset)
                {
                    detectedInserts.Add(
                        Tuple.Create(
                            list[i],
                            list[i].Text));

                    list.RemoveAt(i);
                    --i;
                }
                else if (list[i].Offset < offset && offset < list[i].EndOffset)
                {
                    list.Insert(i + 1, list[i].Split(offset));
                }
                else if (list[i].EndOffset <= offset)
                {
                    // Do nothing.
                }
                else
                {
                    Debug.Assert(false, "Invalid control flow");
                }
            }

            // Add Type1 backtracking instances.
            this.Patterns.AddRange(
                detectedInserts
                .OrderByDescending(x => x.Item2.Length)
                .GroupBy(x => x.Item1.Event.ID)
                .OrderBy(x => x.Key)
                .Select(grp => grp.First())
                .Where(x => this.FilterType1Backtracking(x.Item1, x.Item2))
                .Select(x => new Type1BacktrackingPatternInstance(
                    x.Item1.Event,
                    anEvent,
                    string.Format("Type1 [{2} -> {3}]: \"{0}\" - \"{1}\"", x.Item1.Text, x.Item2, x.Item1.Event.ID, anEvent.ID))));
        }

        /// <summary>
        /// Processes the delete change for Type II backtracking detection.
        /// </summary>
        /// <param name="anEvent">An event.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="length">The length.</param>
        /// <param name="deletedText">The deleted text.</param>
        private void ProcessDeleteType2(Event anEvent, int offset, int length, string deletedText)
        {
            // Add to the deleted list
            if (deletedText != null)
            {
                this.DeleteSegments.Add(new DeleteSegment(anEvent, this.CurrentFile, deletedText));
            }
        }

        /// <summary>
        /// Filters the given type1 backtracking instance with various heuristics.
        /// </summary>
        /// <param name="insertSegment">The insert segment.</param>
        /// <param name="deletedText">The deleted text.</param>
        /// <returns>
        /// true if this instance should be detected, false otherwise.
        /// </returns>
        private bool FilterType1Backtracking(InsertSegment insertSegment, string deletedText)
        {
            if (deletedText == null)
            {
                return false;
            }

            // Size heuristic.
            if (deletedText.Trim().Length < Type1SizeThreshold)
            {
                return false;
            }

            // Exclude "import" statements
            if (deletedText.Trim().StartsWith("import"))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Filters the given type2 backtracking instance with various heuristics.
        /// </summary>
        /// <param name="deleteSegment">The delete segment.</param>
        /// <param name="insertedText">The inserted text.</param>
        /// <returns>
        /// true if this instance should be detected, false otherwise.
        /// </returns>
        private bool FilterType2Backtracking(DeleteSegment deleteSegment, string insertedText)
        {
            if (insertedText == null)
            {
                return false;
            }

            // Size heuristic.
            if (insertedText.Trim().Length < Type2SizeThreshold)
            {
                return false;
            }

            if (insertedText.Contains("TODO Auto-generated"))
            {
                return false;
            }

            if (insertedText.Trim().StartsWith("import"))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Represents an insert segment. Used for Type I backtracking detection.
        /// </summary>
        private class InsertSegment
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="InsertSegment"/> class.
            /// </summary>
            /// <param name="anEvent">An event.</param>
            /// <param name="offset">The offset.</param>
            /// <param name="length">The length.</param>
            /// <param name="text">The text.</param>
            public InsertSegment(Event anEvent, int offset, int length, string text)
            {
                this.Event = anEvent;
                this.Offset = offset;
                this.Length = length;
                this.Text = text;
            }

            /// <summary>
            /// Gets or sets the event.
            /// </summary>
            /// <value>
            /// The event.
            /// </value>
            public Event Event { get; set; }

            /// <summary>
            /// Gets or sets the offset.
            /// </summary>
            /// <value>
            /// The offset.
            /// </value>
            public int Offset { get; set; }

            /// <summary>
            /// Gets or sets the length.
            /// </summary>
            /// <value>
            /// The length.
            /// </value>
            public int Length { get; set; }

            /// <summary>
            /// Gets the end offset.
            /// </summary>
            /// <value>
            /// The end offset.
            /// </value>
            public int EndOffset
            {
                get
                {
                    return this.Offset + this.Length;
                }
            }

            /// <summary>
            /// Gets or sets the text.
            /// </summary>
            /// <value>
            /// The text.
            /// </value>
            public string Text { get; set; }

            /// <summary>
            /// Splits with the specified offset.
            /// </summary>
            /// <param name="offset">The offset.</param>
            /// <returns>
            /// The latter InsertSegment resulted from this split.
            /// </returns>
            public InsertSegment Split(int offset)
            {
                if (this.Offset >= offset || offset >= this.Offset + this.Length)
                {
                    throw new ArgumentOutOfRangeException();
                }

                var result = new InsertSegment(
                    this.Event,
                    offset,
                    this.Offset + this.Length - offset,
                    this.Text.Substring(offset - this.Offset));

                // Adjust my own length.
                this.Length = offset - this.Offset;
                this.Text = this.Text.Substring(0, offset - this.Offset);

                return result;
            }
        }

        /// <summary>
        /// Represents an insert segment. Used for Type II backtracking detection.
        /// </summary>
        private class DeleteSegment
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="DeleteSegment"/> class.
            /// </summary>
            /// <param name="anEvent">An event.</param>
            /// <param name="file">The file.</param>
            /// <param name="text">The text.</param>
            public DeleteSegment(Event anEvent, string file, string text)
            {
                this.Event = anEvent;
                this.File = file;
                this.Text = text;
            }

            /// <summary>
            /// Gets or sets the event.
            /// </summary>
            /// <value>
            /// The event.
            /// </value>
            public Event Event { get; set; }

            /// <summary>
            /// Gets or sets the file.
            /// </summary>
            /// <value>
            /// The file.
            /// </value>
            public string File { get; set; }

            /// <summary>
            /// Gets or sets the text.
            /// </summary>
            /// <value>
            /// The text.
            /// </value>
            public string Text { get; set; }
        }
    }
}
