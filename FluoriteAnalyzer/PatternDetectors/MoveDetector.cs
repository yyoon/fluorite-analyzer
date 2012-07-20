using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluoriteAnalyzer.Analyses;
using FluoriteAnalyzer.Events;
using System.IO;

namespace FluoriteAnalyzer.PatternDetectors
{
    class MoveDetector : AbstractPatternDetector
    {
        private static MoveDetector _instance = null;
        internal static MoveDetector GetInstance()
        {
            return _instance ?? (_instance = new MoveDetector());
        }

        public override IEnumerable<PatternInstance> DetectAsPatternInstances(ILogProvider logProvider)
        {
            List<Event> completeList = logProvider.LoggedEvents.ToList();
            List<DocumentChange> dcList = logProvider.LoggedEvents.OfType<DocumentChange>().ToList();

            List<MovePatternInstance> detectedPatterns = new List<MovePatternInstance>();

            foreach (int i in Enumerable.Range(0, dcList.Count - 1))
            {
                if (dcList[i] is Delete && dcList[i + 1] is Insert)
                {
                    Delete delete = dcList[i + 0] as Delete;
                    Insert insert = dcList[i + 1] as Insert;

                    if (TestEquivalent(delete.Text, insert.Text))
                    {
                        FileOpenCommand fromFileOpenCommand = FindClosestFileOpenCommand(completeList, delete);
                        FileOpenCommand toFileOpenCommand = FindClosestFileOpenCommand(completeList, insert);

                        if (fromFileOpenCommand == null || toFileOpenCommand == null) { continue; }

                        detectedPatterns.Add(new MovePatternInstance(
                            delete, insert, 2,
                            "From: " + Path.GetFileName(fromFileOpenCommand.FilePath) + ", To: " + Path.GetFileName(toFileOpenCommand.FilePath) + ", \"" + delete.Text + "\"",
                            fromFileOpenCommand.FilePath,
                            toFileOpenCommand.FilePath
                        ));
                    }
                }
            }

            return detectedPatterns;
        }

        private bool TestEquivalent(string lhs, string rhs)
        {
            string[] lhsSplit = lhs.Split(new char[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            string[] rhsSplit = rhs.Split(new char[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            if (lhsSplit.Length == 0 || rhsSplit.Length == 0) { return false; }

            string lhsJoin = string.Join(" ", lhsSplit);
            string rhsJoin = string.Join(" ", rhsSplit);

            return lhsJoin == rhsJoin;
        }

        private FileOpenCommand FindClosestFileOpenCommand(List<Event> completeList, Event anEvent)
        {
            int eventIndex = completeList.IndexOf(anEvent);
            if (eventIndex == -1) { return null; }

            for (int i = eventIndex - 1; i >= 0; --i)
            {
                if (completeList[i] is FileOpenCommand)
                {
                    return completeList[i] as FileOpenCommand;
                }
            }

            return null;
        }
    }
}
