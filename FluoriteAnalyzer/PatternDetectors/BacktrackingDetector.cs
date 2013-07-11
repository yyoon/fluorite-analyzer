using DiffMatchPatch;
using FluoriteAnalyzer.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluoriteAnalyzer.PatternDetectors
{
    class BacktrackingDetector : AbstractPatternDetector
    {
        private string CurrentFile { get; set; }
        private Dictionary<string, string> Snapshots { get; set; }
        private List<BacktrackingPatternInstance> Patterns { get; set; }

        public override IEnumerable<PatternInstance> DetectAsPatternInstances(Commons.ILogProvider logProvider)
        {
            List<Event> completeList = logProvider.LoggedEvents.ToList();
            List<Event> dcList = logProvider.LoggedEvents.Where(x => x is DocumentChange || x is FileOpenCommand).ToList();

            Patterns = new List<BacktrackingPatternInstance>();

            // Initialize
            CurrentFile = null;
            Snapshots = new Dictionary<string, string>();

            foreach (int i in Enumerable.Range(0, dcList.Count))
            {
                if (dcList[i] is FileOpenCommand)
                {
                    ProcessFileOpenCommand(dcList, i);
                }
                else if (dcList[i] is Insert)
                {
                    Insert insert = (Insert)dcList[i];
                    ProcessInsert(insert.Offset, insert.Text, insert.ID);
                }
                else if (dcList[i] is Delete)
                {
                    Delete delete = (Delete)dcList[i];
                    ProcessDelete(delete.Offset, delete.Length, delete.ID);
                }
                else if (dcList[i] is Replace)
                {
                    // Process as if this was a two separate Delete / Insert commands.
                    Replace replace = (Replace)dcList[i];
                    ProcessDelete(replace.Offset, replace.Length, replace.ID);
                    ProcessInsert(replace.Offset, replace.InsertedText, replace.ID);
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
                                    ProcessInsert(curOffset, diff.text, foc.ID);

                                    curOffset += diff.text.Length;
                                    curLength += diff.text.Length;
                                    break;

                                case Operation.DELETE:
                                    ProcessDelete(curOffset, diff.text.Length, foc.ID);

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
        }

        private void ProcessInsert(int offset, string insertedText, int eventID)
        {
            throw new NotImplementedException();
        }

        private void ProcessDelete(int offset, int length, int eventID)
        {
            throw new NotImplementedException();
        }
    }
}
