using DiffMatchPatch;
using FluoriteAnalyzer.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace FluoriteAnalyzer.PatternDetectors
{
    class BacktrackingDetector : AbstractPatternDetector
    {
        private static BacktrackingDetector _instance = null;
        internal static BacktrackingDetector GetInstance()
        {
            return _instance ?? (_instance = new BacktrackingDetector());
        }

        private string CurrentFile { get; set; }
        private Dictionary<string, string> Snapshots { get; set; }
        private List<BacktrackingPatternInstance> Patterns { get; set; }

        // For Type I Backtracking
        private class InsertSegment
        {
            public InsertSegment(Event anEvent, int offset, int length, string text)
            {
                Event = anEvent;
                Offset = offset;
                Length = length;
                Text = text;
            }

            public Event Event { get; set; }
            public int Offset { get; set; }
            public int Length { get; set; }
            public int EndOffset { get { return Offset + Length; } }
            public string Text { get; set; }

            public InsertSegment Split(int offset)
            {
                Debug.Assert(Offset < offset && offset < Offset + Length);

                var result = new InsertSegment(
                    Event,
                    offset,
                    Offset + Length - offset,
                    Text
                );

                // Adjust my own length.
                Length = offset - Offset;

                return result;
            }
        }

        private Dictionary<string, List<InsertSegment>> InsertSegments { get; set; }

        public override IEnumerable<PatternInstance> DetectAsPatternInstances(Commons.ILogProvider logProvider)
        {
            List<Event> completeList = logProvider.LoggedEvents.ToList();
            List<Event> dcList = logProvider.LoggedEvents.Where(x => x is DocumentChange || x is FileOpenCommand).ToList();

            Patterns = new List<BacktrackingPatternInstance>();

            // Initialize
            CurrentFile = null;
            Snapshots = new Dictionary<string, string>();
            InsertSegments = new Dictionary<string, List<InsertSegment>>();

            foreach (int i in Enumerable.Range(0, dcList.Count))
            {
                if (dcList[i] is FileOpenCommand)
                {
                    ProcessFileOpenCommand(dcList, i);
                }
                else if (dcList[i] is Insert)
                {
                    Insert insert = (Insert)dcList[i];
                    ProcessInsert(insert, insert.Offset, insert.Text);
                }
                else if (dcList[i] is Delete)
                {
                    Delete delete = (Delete)dcList[i];
                    ProcessDelete(delete, delete.Offset, delete.Length);
                }
                else if (dcList[i] is Replace)
                {
                    // Process as if this was a two separate Delete / Insert commands.
                    Replace replace = (Replace)dcList[i];
                    ProcessDelete(replace, replace.Offset, replace.Length);
                    ProcessInsert(replace, replace.Offset, replace.InsertedText);
                }
            }

            return Patterns;
        }

        private void ProcessFileOpenCommand(List<Event> dcList, int i)
        {
            FileOpenCommand foc = (FileOpenCommand)dcList[i];
            CurrentFile = foc.FilePath;

            if (foc.Snapshot != null)
            {
                if (!Snapshots.ContainsKey(CurrentFile))
                {
                    Snapshots.Add(CurrentFile, foc.Snapshot);
                    InsertSegments.Add(CurrentFile, new List<InsertSegment>());
                }
                else
                {
                    if (foc.Snapshot == Snapshots[CurrentFile])
                    {
                        // Do nothing.
                    }
                    else
                    {
                        // Extracts the diffs
                        string before = Snapshots[CurrentFile] == null ? string.Empty : Snapshots[CurrentFile];
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
                                    ProcessInsert(foc, curOffset, diff.text);

                                    curOffset += diff.text.Length;
                                    curLength += diff.text.Length;
                                    break;

                                case Operation.DELETE:
                                    ProcessDelete(foc, curOffset, diff.text.Length);

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
                if (!Snapshots.ContainsKey(CurrentFile))
                {
                    Snapshots.Add(CurrentFile, foc.Snapshot);
                    InsertSegments.Add(CurrentFile, new List<InsertSegment>());
                }
            }
        }

        private void ProcessInsert(Event anEvent, int offset, string insertedText)
        {
            // Iterate the segments list
            List<InsertSegment> list = InsertSegments[CurrentFile];
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
                    Debug.Assert(false);
                }
            }

            if (!added)
            {
                list.Add(segmentToBeAdded);
                added = true;
            }

            // Update the snapshot.
            string snapshot = Snapshots[CurrentFile];
            if (snapshot != null)
            {
                snapshot = snapshot.Substring(0, offset) + insertedText + snapshot.Substring(offset);
                Snapshots[CurrentFile] = snapshot;
            }
        }

        private void ProcessDelete(Event anEvent, int offset, int length)
        {
            int endOffset = offset + length;

            // Iterate the segments list
            var list = InsertSegments[CurrentFile];

            var detectedInserts = new List<InsertSegment>();

            for (int i = 0; i < list.Count; ++i)
            {
                if (endOffset <= list[i].Offset)
                {
                    list[i].Offset -= length;
                }
                else if (offset <= list[i].Offset && list[i].Offset < endOffset && endOffset < list[i].EndOffset)
                {
                    detectedInserts.Add(list[i]);

                    list.Insert(i + 1, list[i].Split(endOffset));
                    list.RemoveAt(i);
                    --i;
                }
                else if (offset <= list[i].Offset && list[i].EndOffset <= endOffset)
                {
                    detectedInserts.Add(list[i]);

                    list.RemoveAt(i);
                    --i;
                }
                else if (list[i].Offset < offset && offset < list[i].EndOffset)
                {
                    detectedInserts.Add(list[i]);

                    list.Insert(i + 1, list[i].Split(offset));
                }
                else if (list[i].EndOffset <= offset)
                {
                    // Do nothing.
                }
                else
                {
                    Debug.Assert(false);
                }
            }

            // Current snapshot.
            string snapshot = Snapshots[CurrentFile];

            // Deleted text
            string deletedText = snapshot == null ? "null" : snapshot.Substring(offset, length);

            // Add Type1 backtracking instances.
            Patterns.AddRange(
                detectedInserts
                .GroupBy(x => x.Event.ID)
                .OrderBy(x => x.Key)
                .Select(grp => grp.First())
                .Select(x => new Type1BacktrackingPatternInstance(
                    x.Event,
                    anEvent,
                    string.Format("Type1 [{2} -> {3}]: \"{0}\" - \"{1}\"",
                        x.Text,
                        deletedText,
                        x.Event.ID,
                        anEvent.ID
                    )
                ))
            );

            // Update the snapshot.
            if (snapshot != null)
            {
                snapshot = snapshot.Substring(0, offset) + snapshot.Substring(offset + length);
                Snapshots[CurrentFile] = snapshot;
            }
        }
    }
}
