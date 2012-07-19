using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluoriteAnalyzer.Events;
using System.Windows.Forms;
using FluoriteAnalyzer.Analyses;

namespace FluoriteAnalyzer.PatternDetectors
{
    class ParameterTuningDetector : AbstractPatternDetector
    {
        private class ParameterTuneElement
        {
            public ParameterTuneElement(Delete delete)
            {
                DeleteOffset = delete.Offset;
                DeletedText = delete.Text;
                InsertedText = null;
                Confirmed = false;
            }

            public ParameterTuneElement(Replace replace)
            {
                DeleteOffset = replace.Offset;
                DeletedText = replace.DeletedText;
                InsertedText = replace.InsertedText;
                Confirmed = true;
            }

            public int DeleteOffset { get; set; }
            public bool Confirmed { get; set; }

            public string DeletedText { get; set; }
            public string InsertedText { get; set; }
        }

        private static ParameterTuningDetector _instance = null;
        internal static ParameterTuningDetector GetInstance()
        {
            return _instance ?? (_instance = new ParameterTuningDetector());
        }

        public override IEnumerable<PatternInstance> DetectAsPatternInstances(ILogProvider logProvider)
        {
            // we only consider "Create" because people often do not close the application at all.
            List<Event> list = logProvider.LoggedEvents.Where(x => x is DocumentChange || (x is RunCommand && !((RunCommand)x).IsTerminate)).ToList();

            Event lastRun = null;

            List<ParameterTuneElement> deleteOffsets = new List<ParameterTuneElement>();

            foreach (int i in Enumerable.Range(0, list.Count))
            {
                if (list[i] is RunCommand)
                {
                    RunCommand currentRun = (RunCommand)list[i];

                    if (lastRun != null)
                    {
                        // TODO: Make it as an option! Current value is 1 min.
                        if (currentRun.Timestamp - lastRun.Timestamp <= 45000)
                        {
                            foreach (var element in deleteOffsets)
                            {
                                if (CheckElement(element))
                                {
                                    var result = new PatternInstance(
                                        lastRun,
                                        -1,
                                        "Deleted: \"" + element.DeletedText + "\", Inserted: \"" + element.InsertedText + "\""
                                        );
                                    yield return result;
                                }
                            }
                        }
                    }

                    lastRun = currentRun;
                    deleteOffsets.Clear();
                }
                else if (list[i] is DocumentChange)
                {
                    DocumentChange dc = (DocumentChange)list[i];
                    if (dc is Delete)
                    {
                        Delete delete = (Delete)dc;

                        // TODO: Merge delete offsets if necessary!
                        foreach (int j in Enumerable.Range(0, deleteOffsets.Count))
                        {
                            if (deleteOffsets[j].Confirmed) { continue; }

                            if (deleteOffsets[j].DeleteOffset > delete.Offset)
                            {
                                deleteOffsets[j].DeleteOffset -= delete.Length;
                            }
                        }

                        deleteOffsets.Add(new ParameterTuneElement(delete));
                    }
                    else if (dc is Insert)
                    {
                        Insert insert = (Insert)dc;

                        foreach (int j in Enumerable.Range(0, deleteOffsets.Count))
                        {
                            if (deleteOffsets[j].Confirmed) { continue; }

                            if (deleteOffsets[j].DeleteOffset > insert.Offset)
                            {
                                deleteOffsets[j].DeleteOffset += insert.Length;
                            }
                            else if (deleteOffsets[j].DeleteOffset == insert.Offset)
                            {
                                deleteOffsets[j].InsertedText = insert.Text;
                                deleteOffsets[j].Confirmed = true;
                            }
                        }
                    }
                    else if (dc is Replace && !logProvider.CausedByAutoIndent(dc) && !logProvider.CausedByAssist(dc))
                    {
                        Replace replace = (Replace)dc;

                        deleteOffsets.Add(new ParameterTuneElement(replace));

                        foreach (int j in Enumerable.Range(0, deleteOffsets.Count))
                        {
                            if (deleteOffsets[j].Confirmed) { continue; }

                            if (deleteOffsets[j].DeleteOffset == replace.Offset)
                            {
                                deleteOffsets[j].DeleteOffset -= replace.Length;
                                deleteOffsets[j].DeleteOffset += replace.InsertionLength;
                            }
                        }
                    }
                }
            }
        }

        private static bool CheckElement(ParameterTuneElement element)
        {
            if (element.Confirmed == false) { return false; }

            if (element.DeletedText.Contains('\r') ||
                element.DeletedText.Contains('\n') ||
                element.DeletedText.Contains(' ') ||
                string.IsNullOrEmpty(element.DeletedText.Trim()) ||
                element.InsertedText.Contains('\r') ||
                element.InsertedText.Contains('\n') ||
                element.InsertedText.Contains(' ') ||
                string.IsNullOrEmpty(element.InsertedText.Trim()))
            {
                return false;
            }

            return true;
        }
    }
}
