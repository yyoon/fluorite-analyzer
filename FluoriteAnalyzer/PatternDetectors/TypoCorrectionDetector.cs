using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FluoriteAnalyzer.Analyses;
using FluoriteAnalyzer.Events;
using System.Diagnostics;

namespace FluoriteAnalyzer.PatternDetectors
{
    class TypoCorrectionDetector : IPatternDetector
    {
        private static TypoCorrectionDetector _instance = null;
        internal static TypoCorrectionDetector GetInstance()
        {
            return _instance ?? (_instance = new TypoCorrectionDetector());
        }

        public IEnumerable<ListViewItem> Detect(ILogProvider logProvider)
        {
            int timethreshold = 3000;

            List<Event> completeList = logProvider.LoggedEvents.ToList();
            List<DocumentChange> dcList = logProvider.LoggedEvents.OfType<DocumentChange>().ToList();

            foreach (int i in Enumerable.Range(0, dcList.Count))
            {
                DocumentChange dc = dcList[i];

                // Insert - Delete - Insert+
                if (dc is Insert && logProvider.CausedByInsertString(dc) && !string.IsNullOrWhiteSpace(((Insert)dc).Text)) 
                {
                    int offset = dc.Offset;
                    int length = dc.Length;

                    if (i + 1 < dcList.Count && dcList[i + 1] is Delete && (dcList[i + 1].Timestamp - dc.Timestamp) < timethreshold)
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
                                    var item = new ListViewItem(new[]
                                                                    {
                                                                        dc.ID.ToString(),
                                                                        "3",
                                                                        logProvider.GetVideoTime(dc),
                                                                        "\"" + ((Insert) dcList[i]).Text + "\" - \"" +
                                                                        ((Delete) dcList[i + 1]).Text + "\" + \"" +
                                                                        ((Insert) dcList[i + 2]).Text + "\"",
                                                                    });

                                    yield return item;
                                }
                            }
                        }
                    }
                }

                // Insert - Replace - Insert*
                if (dc is Insert && logProvider.CausedByInsertString(dc) && !string.IsNullOrWhiteSpace(((Insert)dc).Text)) 
                {
                    int offset = dc.Offset;
                    int length = dc.Length;

                    if (i + 1 < dcList.Count && dcList[i + 1] is Replace &&
                        !logProvider.CausedByAutoIndent(dcList[i + 1]) &&
                        (dcList[i + 1].Timestamp - dc.Timestamp) < timethreshold)
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
                                        var item = new ListViewItem(new[]
                                                                    {
                                                                        dc.ID.ToString(),
                                                                        "3",
                                                                        logProvider.GetVideoTime(dc),
                                                                        "\"" + ((Insert) dcList[i]).Text + "\" - \"" +
                                                                        replace.DeletedText + "\" + \"" +
                                                                        replace.InsertedText +
                                                                        ((Insert) dcList[i + 2]).Text + "\"",
                                                                    });

                                        yield return item;
                                    }
                                }
                                else
                                {
                                    var item = new ListViewItem(new[]
                                                                {
                                                                    dc.ID.ToString(),
                                                                    "2",
                                                                    logProvider.GetVideoTime(dc),
                                                                    "\"" + ((Insert) dcList[i]).Text + "\" - \"" +
                                                                    ((Replace) dcList[i + 1]).DeletedText + "\" + \"" +
                                                                    ((Replace) dcList[i + 1]).InsertedText + "\"",
                                                                });

                                    yield return item;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
