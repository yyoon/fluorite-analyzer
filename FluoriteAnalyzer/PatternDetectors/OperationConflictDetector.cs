using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluoriteAnalyzer.Events;
using FluoriteAnalyzer.Utils;
using System.Diagnostics;
using System.Windows.Forms;

namespace FluoriteAnalyzer.PatternDetectors
{
    class OperationConflictDetector : AbstractPatternDetector
    {
        private List<OperationConflictPatternInstance> DetectedPatterns { get; set; }
        private Dictionary<string, List<DocumentChange>> ProcessedChangesDict { get; set; }
        private List<DocumentChange> ProcessedChanges { get; set; }
        private string CurrentFile { get; set; }

        // Remembers all the "first" document changes involved in any of the conflicting cases.
        private HashSet<DocumentChange> ConflictedChanges { get; set; }

        private static OperationConflictDetector _instance = null;
        internal static OperationConflictDetector GetInstance()
        {
            return _instance ?? (_instance = new OperationConflictDetector());
        }

        public override IEnumerable<ListViewItem> DetectAsListViewItems(Analyses.ILogProvider logProvider)
        {
            return DetectAsPatternInstances(logProvider)
                .Cast<OperationConflictPatternInstance>()
                .OrderBy(x => x.ConflictType)
                .Select(x => new ListViewItem(new string[] {
                    x.PrimaryEvent.ID.ToString(),
                    x.PatternLength.ToString(),
                    logProvider.GetVideoTime(x.PrimaryEvent),
                    x.Description
                }) { Tag = x });
        }

        public override IEnumerable<PatternInstance> DetectAsPatternInstances(Analyses.ILogProvider logProvider)
        {
            DetectedPatterns = new List<OperationConflictPatternInstance>();
            ProcessedChangesDict = new Dictionary<string, List<DocumentChange>>();
            ProcessedChanges = null;
            CurrentFile = null;
            ConflictedChanges = new HashSet<DocumentChange>();

            // should consider "FileOpenCommand"s and all "DocumentChange"s
            foreach (Event anEvent in logProvider.LoggedEvents)
            {
                if (anEvent is FileOpenCommand)
                {
                    FileOpenCommand fileOpenCommand = (FileOpenCommand)anEvent;
                    CurrentFile = fileOpenCommand.FilePath;

                    if (ProcessedChangesDict.ContainsKey(CurrentFile) == false)
                        ProcessedChangesDict.Add(CurrentFile, new List<DocumentChange>());

                    ProcessedChanges = ProcessedChangesDict[CurrentFile];

                    if (fileOpenCommand.Snapshot != null)
                    {
                        // At the moment, just cancel all the processed things.
                        // TODO: Use "Diff" and apply patch.
                        // Apply the same thing to the logger part, too.
                        ProcessedChanges.Clear();
                    }
                }
                else if (anEvent is DocumentChange)
                {
                    if (ProcessedChanges == null) { continue; }  // This should never happen, but just in case.

                    DocumentChange newChange = (DocumentChange)anEvent;

                    if (newChange is Insert) ProcessInsert((Insert)newChange);
                    else if (newChange is Delete) ProcessDelete((Delete)newChange);
                    else if (newChange is Replace) ProcessReplace((Replace)newChange);
                    else if (newChange is Move) ProcessMove((Move)newChange);

                    ProcessedChanges.Add(ObjectCopier.Clone(newChange));
                }
            }

            return DetectedPatterns;
        }

        private void ProcessInsert(Insert newChange)
        {
            foreach (DocumentChange oldChange in ProcessedChanges.ToArray())
            {
                if (ConflictedChanges.Contains(oldChange)) { continue; }

                if (oldChange is Insert) { ProcessInsertBeforeInsert((Insert)oldChange, newChange); }
                else if (oldChange is Delete) { ProcessDeleteBeforeInsert((Delete)oldChange, newChange); }
                else if (oldChange is Replace) { ProcessReplaceBeforeInsert((Replace)oldChange, newChange); }
                else if (oldChange is Move) { ProcessMoveBeforeInsert((Move)oldChange, newChange); }
            }
        }

        #region <Some operation> -> Insert

        private void ProcessInsertBeforeInsert(Insert oldChange, Insert newChange)
        {
            int NS = newChange.Offset;

            int IS = oldChange.Offset;
            int IE = oldChange.Offset + oldChange.Length;

            // Trivial case
            if (NS <= IS)
            {
                oldChange.Offset += newChange.Length;
            }
            // CONFLICTING CASE!!
            else if (IS < NS && NS < IE)
            {
                AddPatternInstance(oldChange, newChange, "II-01");
            }
            // Trivial case
            else if (IE <= NS)
            {
                // Do nothing.
            }
            else
            {
                Debug.Assert(false);
            }
        }

        private void ProcessDeleteBeforeInsert(Delete oldChange, Insert newChange)
        {
            int NS = newChange.Offset;

            int DO = oldChange.Offset;

            // Trivial case
            if (NS <= DO)
            {
                oldChange.Offset += newChange.Length;
            }
            // Trivial case
            else if (DO < NS)
            {
                // Do nothing.
            }
            else
            {
                Debug.Assert(false);
            }
        }

        private void ProcessReplaceBeforeInsert(Replace oldChange, Insert newChange)
        {
            int NS = newChange.Offset;

            int RS = oldChange.Offset;
            int RE = oldChange.Offset + oldChange.InsertionLength;

            // Trivial case
            if (NS <= RS)
            {
                oldChange.Offset += newChange.Length;
            }
            // CONFLICTING CASE!!
            else if (RS < NS && NS < RE)
            {
                AddPatternInstance(oldChange, newChange, "RI-01");
            }
            // Trivial case
            else if (RE <= NS)
            {
                // Do nothing.
            }
            else
            {
                Debug.Assert(false);
            }
        }

        private void ProcessMoveBeforeInsert(Move oldChange, Insert newChange)
        {
            // Case #1: some code was moved from the current file to another file.
            // Treat the move as a deletion
            if (oldChange.DeletedFrom == CurrentFile && oldChange.InsertedTo != CurrentFile)
            {
                int NS = newChange.Offset;

                int DO = oldChange.DeletionOffset;

                // Trivial case
                if (NS < DO)
                {
                    oldChange.DeletionOffset += newChange.Length;
                }
                // Trivial case
                else if (DO <= NS)
                {
                    // Do nothing.
                }
                else
                {
                    Debug.Assert(false);
                }
            }
            // Case #2: some code was moved from another file to the current file.
            // Treat the move as an insertion
            else if (oldChange.DeletedFrom != CurrentFile && oldChange.InsertedTo == CurrentFile)
            {
                int NS = newChange.Offset;

                int IS = oldChange.InsertionOffset;
                int IE = oldChange.InsertionOffset + oldChange.InsertionLength;

                // Trivial case
                if (NS <= IS)
                {
                    oldChange.InsertionOffset += newChange.Length;
                }
                // Trivial case
                else if (IS < NS && NS < IE)
                {
                    //oldChange.InsertionLength += newChange.Length;
                    AddPatternInstance(oldChange, newChange, "MI-01");
                }
                // Trivial case
                else if (IE <= NS)
                {
                    // Do nothing.
                }
                else
                {
                    Debug.Assert(false);
                }
            }
            // Case #3: some code was moved within the current file.
            else if (oldChange.DeletedFrom == CurrentFile && oldChange.InsertedTo == CurrentFile)
            {
                if (oldChange.DeletionOffset <= oldChange.InsertionOffset)
                {
                    int NS = newChange.Offset;

                    int DO = oldChange.DeletionOffset;

                    int IS = oldChange.InsertionOffset;
                    int IE = oldChange.InsertionOffset + oldChange.InsertionLength;

                    // Trivial case
                    if (NS <= DO)
                    {
                        oldChange.DeletionOffset += newChange.Length;
                        oldChange.InsertionOffset += newChange.Length;
                    }
                    // Trivial case
                    else if (DO < NS && NS <= IS)
                    {
                        oldChange.InsertionOffset += newChange.Length;
                    }
                    // Trivial case
                    else if (IS < NS && NS < IE)
                    {
                        //oldChange.InsertionLength += newChange.Length;
                        AddPatternInstance(oldChange, newChange, "MI-02");
                    }
                    // Trivial case
                    else if (IE <= NS)
                    {
                        // Do nothing.
                    }
                    else
                    {
                        Debug.Assert(false);
                    }
                }
                else if (oldChange.InsertionOffset < oldChange.DeletionOffset)
                {
                    int NS = newChange.Offset;

                    int DO = oldChange.DeletionOffset + oldChange.InsertionLength;

                    int IS = oldChange.InsertionOffset;
                    int IE = oldChange.InsertionOffset + oldChange.InsertionLength;

                    // Trivial case
                    if (NS <= IS)
                    {
                        oldChange.InsertionOffset += newChange.Length;
                        oldChange.DeletionOffset += newChange.Length;
                    }
                    // Trivial case
                    else if (IS < NS && NS < IE)
                    {
                        //oldChange.InsertionLength += newChange.Length;
                        AddPatternInstance(oldChange, newChange, "MI-03");
                    }
                    // Trivial case
                    else if (IE <= NS && NS < DO)
                    {
                        oldChange.DeletionOffset += newChange.Length;
                    }
                    // Trivial case
                    else if (DO <= NS)
                    {
                        // Do nothing.
                    }
                    else
                    {
                        Debug.Assert(false);
                    }
                }
            }
        }

        #endregion

        private void ProcessDelete(Delete newChange)
        {
            foreach (DocumentChange oldChange in ProcessedChanges.ToArray())
            {
                if (ConflictedChanges.Contains(oldChange)) { continue; }

                if (oldChange is Insert) { ProcessInsertBeforeDelete((Insert)oldChange, newChange); }
                else if (oldChange is Delete) { ProcessDeleteBeforeDelete((Delete)oldChange, newChange); }
                else if (oldChange is Replace) { ProcessReplaceBeforeDelete((Replace)oldChange, newChange); }
                else if (oldChange is Move) { ProcessMoveBeforeDelete((Move)oldChange, newChange); }
            }
        }

        #region <Some operation> -> Delete

        private void ProcessInsertBeforeDelete(Insert oldChange, Delete newChange)
        {
            int NS = newChange.Offset;
            int NE = newChange.Offset + newChange.Length;

            int IS = oldChange.Offset;
            int IE = oldChange.Offset + oldChange.Length;

            // Trivial case
            if (NE <= IS)
            {
                oldChange.Offset -= newChange.Length;
            }
            // CONFLICTING CASE!!
            else if (NS <= IS && IS < NE && NE < IE)
            {
                AddPatternInstance(oldChange, newChange, "ID-01");
            }
            // CONFLICTING CASE!!
            else if (NS <= IS && IE <= NE)
            {
                AddPatternInstance(oldChange, newChange, "ID-02");
            }
            // Trivial case
            //else if (IS < NS && NE < IE)
            //{
            //    oldChange.Length -= newChange.Length;
            //}
            // CONFLICTING CASE!!
            else if (IS < NS && NE < IE)
            {
                AddPatternInstance(oldChange, newChange, "ID-025");
            }
            // CONFLICTING CASE!!
            else if (IS < NS && NS < IE && IE <= NE)
            {
                AddPatternInstance(oldChange, newChange, "ID-03");
            }
            // Trivial case
            else if (IE <= NS)
            {
                // Do noting.
            }
            else
            {
                Debug.Assert(false);
            }
        }

        private void ProcessDeleteBeforeDelete(Delete oldChange, Delete newChange)
        {
            int NS = newChange.Offset;
            int NE = newChange.Offset + newChange.Length;

            int DO = oldChange.Offset;

            // Trivial case
            if (NE <= DO)
            {
                oldChange.Offset -= newChange.Length;
            }
            // CONFLICTING CASE!!
            else if (NS < DO && DO < NE)
            {
                AddPatternInstance(oldChange, newChange, "DD-01");
            }
            else if (DO <= NS)
            {
                // Do nothing.
            }
            else
            {
                Debug.Assert(false);
            }
        }

        private void ProcessReplaceBeforeDelete(Replace oldChange, Delete newChange)
        {
            int NS = newChange.Offset;
            int NE = newChange.Offset + newChange.Length;

            int RS = oldChange.Offset;
            int RE = oldChange.Offset + oldChange.InsertionLength;

            // Trivial case
            if (NE <= RS)
            {
                oldChange.Offset -= newChange.Length;
            }
            // CONFLICTING CASE!!
            else if (NS <= RS && RS < NE && NE < RE)
            {
                AddPatternInstance(oldChange, newChange, "RD-01");
            }
            // CONFLICTING CASE!!
            else if (NS <= RS && RE <= NE)
            {
                AddPatternInstance(oldChange, newChange, "RD-02");
            }
            // CONFLICTING CASE!!
            else if (RS < NS && NE < RE)
            {
                AddPatternInstance(oldChange, newChange, "RD-03");
            }
            // CONFLICTING CASE!!
            else if (RS < NS && NS < RE && RE <= NE)
            {
                AddPatternInstance(oldChange, newChange, "RD-04");
            }
            // Trivial case
            else if (RE <= NS)
            {
                // Do nothing.
            }
            else
            {
                Debug.Assert(false);
            }
        }

        private void ProcessMoveBeforeDelete(Move oldChange, Delete newChange)
        {
            // Case #1: some code was moved from the current file to another file.
            // Treat the move as a deletion
            if (oldChange.DeletedFrom == CurrentFile && oldChange.InsertedTo != CurrentFile)
            {
                int NS = newChange.Offset;
                int NE = newChange.Offset + newChange.Length;

                int DO = oldChange.DeletionOffset;

                // Trivial case
                if (NE <= DO)
                {
                    oldChange.DeletionOffset -= newChange.Length;
                }
                // CONFLICTING CASE!!
                else if (NS < DO && DO < NE)
                {
                    AddPatternInstance(oldChange, newChange, "MD-01");
                }
                // Trivial case
                else if (DO <= NS)
                {
                    // Do nothing.
                }
                else
                {
                    Debug.Assert(false);
                }
            }
            // Case #2: some code was moved from another file to the current file.
            // Treat the move as an insertion
            else if (oldChange.DeletedFrom != CurrentFile && oldChange.InsertedTo == CurrentFile)
            {
                int NS = newChange.Offset;
                int NE = newChange.Offset + newChange.Length;

                int IS = oldChange.InsertionOffset;
                int IE = oldChange.InsertionOffset + oldChange.InsertionLength;

                // Trivial case
                if (NE <= IS)
                {
                    oldChange.InsertionOffset -= newChange.Length;
                }
                // CONFLICTING CASE!!
                else if (NS <= IS && IS < NE && NE < IE)
                {
                    AddPatternInstance(oldChange, newChange, "MD-02");
                }
                // CONFLICTING CASE!!
                else if (NS <= IS && IE <= NE)
                {
                    AddPatternInstance(oldChange, newChange, "MD-03");
                }
                // Trivial case
                // CONFLICTING CASE!!
                else if (IS < NS && NE < IE)
                {
                    //oldChange.InsertionLength -= newChange.Length;
                    AddPatternInstance(oldChange, newChange, "MD-035");
                }
                // CONFLICTING CASE!!
                else if (IS < NS && NS < IE && IE <= NE)
                {
                    AddPatternInstance(oldChange, newChange, "MD-04");
                }
                // Trivial case
                else if (IE <= NS)
                {
                    // Do nothing.
                }
                else
                {
                    Debug.Assert(false);
                }
            }
            // Case #3: some code was moved within the current file.
            else if (oldChange.DeletedFrom == CurrentFile && oldChange.InsertedTo == CurrentFile)
            {
                if (oldChange.DeletionOffset <= oldChange.InsertionOffset)
                {
                    int NS = newChange.Offset;
                    int NE = newChange.Offset + newChange.Length;

                    int DO = oldChange.DeletionOffset;

                    int IS = oldChange.InsertionOffset;
                    int IE = oldChange.InsertionOffset + oldChange.InsertionLength;

                    // Trivial case
                    if (NE <= DO)
                    {
                        oldChange.DeletionOffset -= newChange.Length;
                        oldChange.InsertionOffset -= newChange.Length;
                    }
                    // CONFLICTING CASE!!
                    else if (NS <= DO && DO < NE && NE <= IS)
                    {
                        AddPatternInstance(oldChange, newChange, "MD-05");
                    }
                    // CONFLICTING CASE!!
                    else if (NS <= DO && IS < NE && NE < IE)
                    {
                        AddPatternInstance(oldChange, newChange, "MD-06");
                    }
                    // CONFLICTING CASE!!
                    else if (NS <= DO && IE <= NE)
                    {
                        AddPatternInstance(oldChange, newChange, "MD-07");
                    }
                    // Trivial case
                    else if (DO < NS && NE <= IS)
                    {
                        oldChange.InsertionOffset -= newChange.Length;
                    }
                    // CONFLICTING CASE!!
                    else if (DO < NS && NS <= IS && IS < NE && NE < IE)
                    {
                        AddPatternInstance(oldChange, newChange, "MD-08");
                    }
                    // CONFLICTING CASE!!
                    else if (DO < NS && NS <= IS && IE <= NE)
                    {
                        AddPatternInstance(oldChange, newChange, "MD-09");
                    }
                    // Trivial case
                    // CONFLICTING CASE!!
                    else if (IS < NS && NE < IE)
                    {
                        //oldChange.InsertionLength -= newChange.Length;
                        AddPatternInstance(oldChange, newChange, "MD-095");
                    }
                    else if (IS < NS && NS < IE && IE <= NE)
                    {
                        AddPatternInstance(oldChange, newChange, "MD-10");
                    }
                    // Trivial case
                    else if (IE <= NS)
                    {
                        // Do nothing.
                    }
                    else
                    {
                        Debug.Assert(false);
                    }
                }
                else if (oldChange.InsertionOffset < oldChange.DeletionOffset)
                {
                    int NS = newChange.Offset;
                    int NE = newChange.Offset + newChange.Length;

                    int DO = oldChange.DeletionOffset + oldChange.InsertionLength;

                    int IS = oldChange.InsertionOffset;
                    int IE = oldChange.InsertionOffset + oldChange.InsertionLength;

                    // Trivial case
                    if (NE <= IS)
                    {
                        oldChange.InsertionOffset -= newChange.Length;
                        oldChange.DeletionOffset -= newChange.Length;
                    }
                    // CONFLICTING CASE!!
                    else if (NS <= IS && IS < NE && NE < IE)
                    {
                        AddPatternInstance(oldChange, newChange, "MD-11");
                    }
                    // CONFLICTING CASE!!
                    else if (NS <= IS && IE <= NE && NE < DO)
                    {
                        AddPatternInstance(oldChange, newChange, "MD-12");
                    }
                    // CONFLICTING CASE!!
                    else if (NS <= IS && DO <= NE)
                    {
                        AddPatternInstance(oldChange, newChange, "MD-13");
                    }
                    // Trivial case
                    else if (IS < NS && NE < IE)
                    {
                        //oldChange.InsertionLength -= newChange.Length;
                        AddPatternInstance(oldChange, newChange, "MD-135");
                    }
                    // CONFLICTING CASE!!
                    else if (IS < NS && NS < IE && IE <= NE && NE < DO)
                    {
                        AddPatternInstance(oldChange, newChange, "MD-14");
                    }
                    // CONFLICTING CASE!!
                    else if (IS < NS && NS < IE && DO <= NE)
                    {
                        AddPatternInstance(oldChange, newChange, "MD-15");
                    }
                    // Trivial case
                    else if (IE <= NS && NE < DO)
                    {
                        oldChange.DeletionOffset -= newChange.Length;
                    }
                    // CONFLICTING CASE!!
                    else if (IE <= NS && NS < DO && DO >= NE)
                    {
                        AddPatternInstance(oldChange, newChange, "MD-16");
                    }
                    // Trivial case
                    else if (DO <= NS)
                    {
                        // Do nothing.
                    }
                    else
                    {
                        Debug.Assert(false);
                    }
                }
            }
        }

        #endregion

        private void ProcessReplace(Replace newChange)
        {
            foreach (DocumentChange oldChange in ProcessedChanges.ToArray())
            {
                if (ConflictedChanges.Contains(oldChange)) { continue; }

                if (oldChange is Insert) { ProcessInsertBeforeReplace((Insert)oldChange, newChange); }
                else if (oldChange is Delete) { ProcessDeleteBeforeReplace((Delete)oldChange, newChange); }
                else if (oldChange is Replace) { ProcessReplaceBeforeReplace((Replace)oldChange, newChange); }
                else if (oldChange is Move) { ProcessMoveBeforeReplace((Move)oldChange, newChange); }
            }
        }

        #region <Some operation> -> Replace

        private void ProcessInsertBeforeReplace(Insert oldChange, Replace newChange)
        {
            int NS = newChange.Offset;
            int NE = newChange.Offset + newChange.Length;

            int IS = oldChange.Offset;
            int IE = oldChange.Offset + oldChange.Length;

            // Trivial case
            if (NE <= IS)
            {
                oldChange.Offset += newChange.LengthDiff;
            }
            // CONFLICTING CASE!!
            else if (NS <= IS && IS < NE && NE <= IE)
            {
                AddPatternInstance(oldChange, newChange, "IR-01");
            }
            // CONFLICTING CASE!!
            else if (NS <= IS && IE <= NE)
            {
                AddPatternInstance(oldChange, newChange, "IR-02");
            }
            // Trivial case
            else if (IS < NS && NE < IE)
            {
                //oldChange.Length += newChange.LengthDiff;
                AddPatternInstance(oldChange, newChange, "IR-025");
            }
            // CONFLICTING CASE!!
            else if (IS < NS && NS < IE && IE <= NE)
            {
                AddPatternInstance(oldChange, newChange, "IR-03");
            }
            // Trivial case
            else if (IE <= NS)
            {
                // Do nothing.
            }
            else
            {
                Debug.Assert(false);
            }
        }

        private void ProcessDeleteBeforeReplace(Delete oldChange, Replace newChange)
        {
            int NS = newChange.Offset;
            int NE = newChange.Offset + newChange.Length;

            int DO = oldChange.Offset;

            // Trivial case
            if (NE <= DO)
            {
                oldChange.Offset += newChange.LengthDiff;
            }
            // CONFLICTING CASE!!
            else if (NS < DO && DO < NE)
            {
                AddPatternInstance(oldChange, newChange, "DR-01");
            }
            // Trivial case
            else if (DO <= NS)
            {
                // Do nothing.
            }
            else
            {
                Debug.Assert(false);
            }
        }

        private void ProcessReplaceBeforeReplace(Replace oldChange, Replace newChange)
        {
            int NS = newChange.Offset;
            int NE = newChange.Offset + newChange.Length;

            int RS = oldChange.Offset;
            int RE = oldChange.Offset + oldChange.InsertionLength;

            // Trivial case
            if (NE <= RS)
            {
                oldChange.Offset += newChange.LengthDiff;
            }
            // CONFLICTING CASE!!
            else if (NS <= RS && RS < NE && NE < RE)
            {
                AddPatternInstance(oldChange, newChange, "RR-01");
            }
            // CONFLICTING CASE!!
            else if (NS <= RS && RE <= NE)
            {
                AddPatternInstance(oldChange, newChange, "RR-02");
            }
            // CONFLICTING CASE!!
            else if (RS < NS && NE < RE)
            {
                AddPatternInstance(oldChange, newChange, "RR-03");
            }
            // CONFLICTING CASE!!
            else if (RS < NS && NS < RE && RE <= NE)
            {
                AddPatternInstance(oldChange, newChange, "RR-04");
            }
            // Trivial case
            else if (RE <= NS)
            {
                // Do nothing.
            }
            else
            {
                Debug.Assert(false);
            }
        }

        private void ProcessMoveBeforeReplace(Move oldChange, Replace newChange)
        {
            // Case #1: some code was moved from the current file to another file.
            // Treat the move as a deletion
            if (oldChange.DeletedFrom == CurrentFile && oldChange.InsertedTo != CurrentFile)
            {
                int NS = newChange.Offset;
                int NE = newChange.Offset + newChange.Length;

                int DO = oldChange.DeletionOffset;

                // Trivial case
                if (NE <= DO)
                {
                    oldChange.DeletionOffset += newChange.LengthDiff;
                }
                // CONFLICTING CASE!!
                else if (NS < DO && DO < NE)
                {
                    AddPatternInstance(oldChange, newChange, "MR-01");
                }
                // Trivial case
                else if (DO <= NS)
                {
                    // Do nothing.
                }
                else
                {
                    Debug.Assert(false);
                }
            }
            // Case #2: some code was moved from another file to the current file.
            // Treat the move as an insertion
            else if (oldChange.DeletedFrom != CurrentFile && oldChange.InsertedTo == CurrentFile)
            {
                int NS = newChange.Offset;
                int NE = newChange.Offset + newChange.Length;

                int IS = oldChange.InsertionOffset;
                int IE = oldChange.InsertionOffset + oldChange.InsertionLength;

                // Trivial case
                if (NE <= IS)
                {
                    oldChange.InsertionOffset += newChange.LengthDiff;
                }
                // CONFLICTING CASE!!
                else if (NS <= IS && IS < NE && NE < IE)
                {
                    AddPatternInstance(oldChange, newChange, "MR-02");
                }
                // CONFLITING CASE!!
                else if (NS <= IS && IE <= NE)
                {
                    AddPatternInstance(oldChange, newChange, "MR-03");
                }
                // Trivial case
                else if (IS < NS && NE < IE)
                {
                    //oldChange.InsertionLength += newChange.LengthDiff;
                    AddPatternInstance(oldChange, newChange, "MR-035");
                }
                // CONFLICTING CASE!!
                else if (IS < NS && NS < IE && IE <= NE)
                {
                    AddPatternInstance(oldChange, newChange, "MR-04");
                }
                // Trivial case
                else if (IE <= NS)
                {
                    // Do nothing.
                }
                else
                {
                    Debug.Assert(false);
                }
            }
            // Case #3: some code was moved within the current file.
            else if (oldChange.DeletedFrom == CurrentFile && oldChange.InsertedTo == CurrentFile)
            {
                if (oldChange.DeletionOffset <= oldChange.InsertionOffset)
                {
                    int NS = newChange.Offset;
                    int NE = newChange.Offset + newChange.Length;

                    int DO = oldChange.DeletionOffset;

                    int IS = oldChange.InsertionOffset;
                    int IE = oldChange.InsertionOffset + oldChange.InsertionLength;

                    // Trivial case
                    if (NE <= DO)
                    {
                        oldChange.InsertionOffset += newChange.LengthDiff;
                        oldChange.DeletionOffset += newChange.LengthDiff;
                    }
                    else if (NS <= DO && DO < NE && NE <= IS)
                    {
                        AddPatternInstance(oldChange, newChange, "MR-05");
                    }
                    else if (NS <= DO && IS < NE && NE < IE)
                    {
                        AddPatternInstance(oldChange, newChange, "MR-06");
                    }
                    else if (NS <= DO && IE <= NE)
                    {
                        AddPatternInstance(oldChange, newChange, "MR-07");
                    }
                    else if (DO < NS && NE <= IS)
                    {
                        oldChange.InsertionOffset += newChange.LengthDiff;
                    }
                    else if (DO < NS && NS <= IS && IS < NE && NE < IE)
                    {
                        AddPatternInstance(oldChange, newChange, "MR-08");
                    }
                    else if (DO < NS && NS <= IS && IE <= NE)
                    {
                        AddPatternInstance(oldChange, newChange, "MR-09");
                    }
                    else if (IS < NS && NE < IE)
                    {
                        //oldChange.InsertionLength += newChange.LengthDiff;
                        AddPatternInstance(oldChange, newChange, "MR-095");
                    }
                    else if (IS < NS && NS < IE && IE <= NE)
                    {
                        AddPatternInstance(oldChange, newChange, "MR-10");
                    }
                    else if (IE <= NS)
                    {
                        // Do nothing.
                    }
                    else
                    {
                        Debug.Assert(false);
                    }
                }
                else if (oldChange.InsertionOffset < oldChange.DeletionOffset)
                {
                    int NS = newChange.Offset;
                    int NE = newChange.Offset + newChange.Length;

                    int DO = oldChange.DeletionOffset + oldChange.InsertionLength;

                    int IS = oldChange.InsertionOffset;
                    int IE = oldChange.InsertionOffset + oldChange.InsertionLength;

                    if (NE <= IS)
                    {
                        oldChange.InsertionOffset += newChange.LengthDiff;
                        oldChange.DeletionOffset += newChange.LengthDiff;
                    }
                    else if (NS <= IS && IS < NE && NE < IE)
                    {
                        AddPatternInstance(oldChange, newChange, "MR-11");
                    }
                    else if (NS <= IS && IE <= NE && NE < DO)
                    {
                        AddPatternInstance(oldChange, newChange, "MR-12");
                    }
                    else if (NS <= IS && DO <= NE)
                    {
                        AddPatternInstance(oldChange, newChange, "MR-13");
                    }
                    else if (IS < NS && NE < IE)
                    {
                        //oldChange.InsertionLength += newChange.LengthDiff;
                        AddPatternInstance(oldChange, newChange, "MR-135");
                    }
                    else if (IS < NS && NS < IE && IE <= NE && NE < DO)
                    {
                        AddPatternInstance(oldChange, newChange, "MR-14");
                    }
                    else if (IS < NS && NS < IE && DO <= NE)
                    {
                        AddPatternInstance(oldChange, newChange, "MR-15");
                    }
                    else if (IE <= NS && NE < DO)
                    {
                        oldChange.DeletionOffset += newChange.LengthDiff;
                    }
                    else if (IE < NS && NS < DO && DO <= NE)
                    {
                        AddPatternInstance(oldChange, newChange, "MR-16");
                    }
                    else if (DO <= NS)
                    {
                        // Do nothing.
                    }
                    else
                    {
                        Debug.Assert(false);
                    }
                }
            }
        }

        #endregion

        private void ProcessMove(Move newChange)
        {
            return;

            foreach (DocumentChange oldChange in ProcessedChanges.ToArray())
            {
                if (ConflictedChanges.Contains(oldChange)) { continue; }

                if (oldChange is Insert) { ProcessInsertBeforeMove((Insert)oldChange, newChange); }
                else if (oldChange is Delete) { ProcessDeleteBeforeMove((Delete)oldChange, newChange); }
                else if (oldChange is Replace) { ProcessReplaceBeforeMove((Replace)oldChange, newChange); }
                else if (oldChange is Move) { ProcessMoveBeforeMove((Move)oldChange, newChange); }
            }
        }

        private void ProcessInsertBeforeMove(Insert oldChange, Move newChange)
        {
            throw new NotImplementedException();
        }

        private void ProcessDeleteBeforeMove(Delete oldChange, Move newChange)
        {
            throw new NotImplementedException();
        }

        private void ProcessReplaceBeforeMove(Replace oldChange, Move newChange)
        {
            throw new NotImplementedException();
        }

        private void ProcessMoveBeforeMove(Move oldChange, Move newChange)
        {
            throw new NotImplementedException();

            // Case #1: some code was moved from the current file to another file.
            // Treat the move as a deletion
            if (oldChange.DeletedFrom == CurrentFile && oldChange.InsertedTo != CurrentFile)
            {
            }
            // Case #2: some code was moved from another file to the current file.
            // Treat the move as an insertion
            else if (oldChange.DeletedFrom != CurrentFile && oldChange.InsertedTo == CurrentFile)
            {
            }
            // Case #3: some code was moved within the current file.
            else if (oldChange.DeletedFrom == CurrentFile && oldChange.InsertedTo == CurrentFile)
            {
                if (oldChange.DeletionOffset <= oldChange.InsertionOffset)
                {
                }
                else if (oldChange.InsertionOffset < oldChange.DeletionOffset)
                {
                }
            }
        }

        private void AddPatternInstance(DocumentChange oldChange, DocumentChange newChange, string conflictType)
        {
            var patternInstance = new OperationConflictPatternInstance(
                oldChange, 2,
                string.Format("Type: {4}, {0}({1}) -> {2}({3})", oldChange.GetType().Name[0], oldChange.ID, newChange.GetType().Name[0], newChange.ID, conflictType),
                oldChange, newChange, conflictType);

            patternInstance.AddInvolvingEvent(string.Format("Jump to the 1st {0}({1})", oldChange.GetType().Name, oldChange.ID), oldChange.ID);
            patternInstance.AddInvolvingEvent(string.Format("Jump to the 2nd {0}({1})", newChange.GetType().Name, newChange.ID), newChange.ID);

            DetectedPatterns.Add(patternInstance);
            ConflictedChanges.Add(oldChange);
        }
    }
}
