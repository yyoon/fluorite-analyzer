using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FluoriteAnalyzer.Common;
using FluoriteAnalyzer.Events;

namespace FluoriteAnalyzer.PatternDetectors
{
    class TypoCorrectionDetector : AbstractPatternDetector
    {
        private static TypoCorrectionDetector _instance = null;
        internal static TypoCorrectionDetector GetInstance()
        {
            return _instance ?? (_instance = new TypoCorrectionDetector());
        }

        private static readonly int TIME_THRESHOLD = 3000;  // millisecond

        public override IEnumerable<PatternInstance> DetectAsPatternInstances(ILogProvider logProvider)
        {
            List<Event> completeList = logProvider.LoggedEvents.ToList();
            List<DocumentChange> dcList = logProvider.LoggedEvents.OfType<DocumentChange>().ToList();

            List<PatternInstance> detectedPatterns = new List<PatternInstance>();

            foreach (int i in Enumerable.Range(0, dcList.Count))
            {
                DocumentChange dc = dcList[i];

                // Insert - Delete - Insert+
                DetectInsertDeleteInsertPattern(logProvider, dcList, detectedPatterns, i, dc);

                // Insert - Replace - Insert*
                DetectInsertReplaceInsertPattern(logProvider, completeList, dcList, detectedPatterns, i, dc);
            }

            return detectedPatterns;
        }

        private static void DetectInsertDeleteInsertPattern(ILogProvider logProvider, List<DocumentChange> dcList, List<PatternInstance> detectedPatterns, int i, DocumentChange dc)
        {
            if (dc is Insert && logProvider.CausedByInsertString(dc) && !string.IsNullOrWhiteSpace(((Insert)dc).Text))
            {
                int offset = dc.Offset;
                int length = dc.Length;

                if (i + 1 < dcList.Count && dcList[i + 1] is Delete && (dcList[i + 1].Timestamp - dc.Timestamp) < TIME_THRESHOLD)
                {
                    int offset2 = dcList[i + 1].Offset;
                    int length2 = dcList[i + 1].Length;

                    if (offset <= offset2 && offset2 + length2 <= offset + length)
                    {
                        if (i + 2 < dcList.Count && dcList[i + 2] is Insert)
                        {
                            int offset3 = dcList[i + 2].Offset;

                            if (offset2 == offset3)
                            {
                                var result = new PatternInstance(
                                    dc,
                                    3,
                                    "Type #1: \"" + ((Insert)dcList[i]).Text + "\" - \"" +
                                        ((Delete)dcList[i + 1]).Text + "\" + \"" +
                                        ((Insert)dcList[i + 2]).Text + "\""
                                    );

                                detectedPatterns.Add(result);
                            }
                        }
                    }
                }
            }
        }

        private static void DetectInsertReplaceInsertPattern(ILogProvider logProvider, List<Event> completeList, List<DocumentChange> dcList, List<PatternInstance> detectedPatterns, int i, DocumentChange dc)
        {
            if (dc is Insert && logProvider.CausedByInsertString(dc) && !string.IsNullOrWhiteSpace(((Insert)dc).Text))
            {
                int offset = dc.Offset;
                int length = dc.Length;

                if (i + 1 < dcList.Count && dcList[i + 1] is Replace &&
                    !logProvider.CausedByAutoIndent(dcList[i + 1]) &&
                    (dcList[i + 1].Timestamp - dc.Timestamp) < TIME_THRESHOLD)
                {
                    Replace replace = (Replace)dcList[i + 1];

                    // Check the prev/next command
                    int indexOfReplace = completeList.IndexOf(replace);
                    Debug.Assert(indexOfReplace >= 0);

                    // If previous command is content assist, don't count it.
                    // If next command is PasteCommand, don't count it.
                    if (!(completeList[indexOfReplace - 1] is AssistCommand) &&
                        !(completeList.Count > indexOfReplace + 1 && completeList[indexOfReplace + 1] is PasteCommand))
                    {

                        int offset2 = replace.Offset;
                        int length2 = replace.Length;

                        if (offset <= offset2 && offset2 + length2 <= offset + length)
                        {
                            if (i + 2 < dcList.Count && dcList[i + 2] is Insert)
                            {
                                int offset3 = dcList[i + 2].Offset;

                                if (offset3 == offset2 + replace.InsertionLength)
                                {
                                    var result = new PatternInstance(
                                        dc,
                                        3,
                                        "Type #2: \"" + ((Insert)dcList[i]).Text + "\" - \"" +
                                            replace.DeletedText + "\" + \"" +
                                            replace.InsertedText +
                                            ((Insert)dcList[i + 2]).Text + "\""
                                        );

                                    detectedPatterns.Add(result);
                                }
                            }
                            else
                            {
                                var result = new PatternInstance(
                                    dc,
                                    2,
                                    "Type #3: \"" + ((Insert)dcList[i]).Text + "\" - \"" +
                                        ((Replace)dcList[i + 1]).DeletedText + "\" + \"" +
                                        ((Replace)dcList[i + 1]).InsertedText + "\""
                                    );

                                detectedPatterns.Add(result);
                            }
                        }
                    }
                }
            }
        }
    }
}
