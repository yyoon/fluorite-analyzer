using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using FluoriteAnalyzer.Events;
using System.Diagnostics;

namespace FluoriteAnalyzer.Analyses
{
    internal partial class Patterns : UserControl, IRedrawable
    {
        #region delegates

        public delegate void PatternDoubleClickHandler(int startingID);

        #endregion

        public Patterns(ILogProvider logProvider)
        {
            InitializeComponent();

            LogProvider = logProvider;
        }

        private ILogProvider LogProvider { get; set; }

        #region IRedrawable Members

        public void Redraw()
        {
            listViewPatterns.Items.Clear();
        }

        #endregion

        public event PatternDoubleClickHandler PatternDoubleClick;

        private void buttonSearchFixTypos_Click(object sender, EventArgs e)
        {
            listViewPatterns.Items.Clear();
            int timethreshold = 3000;

            List<Event> completeList = LogProvider.LoggedEvents.ToList();
            List<DocumentChange> dcList = LogProvider.LoggedEvents.OfType<DocumentChange>().ToList();

            foreach (int i in Enumerable.Range(0, dcList.Count))
            {
                DocumentChange dc = dcList[i];

                // Insert - Delete - Insert+
                if (dc is Insert)
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
                                                                        LogProvider.GetVideoTime(dc),
                                                                        "\"" + ((Insert) dcList[i]).Text + "\" - \"" +
                                                                        ((Delete) dcList[i + 1]).Text + "\" + \"" +
                                                                        ((Insert) dcList[i + 2]).Text + "\"",
                                                                    });

                                    listViewPatterns.Items.Add(item);
                                }
                            }
                        }
                    }
                }

                // Insert - Replace - Insert*
                if (dc is Insert)
                {
                    int offset = dc.Offset;
                    int length = dc.Length;

                    if (i + 1 < dcList.Count && dcList[i + 1] is Replace && (dcList[i + 1].Timestamp - dc.Timestamp) < timethreshold)
                    {
                        Replace replace = (Replace) dcList[i + 1];

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
                                                                        LogProvider.GetVideoTime(dc),
                                                                        "\"" + ((Insert) dcList[i]).Text + "\" - \"" +
                                                                        replace.DeletedText + "\" + \"" +
                                                                        replace.InsertedText +
                                                                        ((Insert) dcList[i + 2]).Text + "\"",
                                                                    });

                                        listViewPatterns.Items.Add(item);
                                    }
                                }
                                else
                                {
                                    var item = new ListViewItem(new[]
                                                                {
                                                                    dc.ID.ToString(),
                                                                    "2",
                                                                    LogProvider.GetVideoTime(dc),
                                                                    "\"" + ((Insert) dcList[i]).Text + "\" - \"" +
                                                                    ((Replace) dcList[i + 1]).DeletedText + "\" + \"" +
                                                                    ((Replace) dcList[i + 1]).InsertedText + "\"",
                                                                });

                                    listViewPatterns.Items.Add(item);
                                }
                            }
                        }
                    }
                }
            }

            labelCount.Text = "Total: " + listViewPatterns.Items.Count;
        }

        private void listViewPatterns_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewItem item = listViewPatterns.GetItemAt(e.X, e.Y);
            if (item == null)
            {
                return;
            }

            int startingID = -1;
            if (int.TryParse(item.SubItems[0].Text, out startingID))
            {
                if (PatternDoubleClick != null)
                {
                    PatternDoubleClick(startingID);
                }
            }
        }

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

        private void buttonSearchParameterTuning_Click(object sender, EventArgs e)
        {
            listViewPatterns.Items.Clear();

            // we only consider "Create" because people often do not close the application at all.
            List<Event> list = LogProvider.LoggedEvents.Where(x => x is DocumentChange || (x is RunCommand && !((RunCommand)x).IsTerminate)).ToList();

            Event lastRun = null;

            List<ParameterTuneElement> deleteOffsets = new List<ParameterTuneElement>();

            foreach (int i in Enumerable.Range(0, list.Count))
            {
                if (list[i] is RunCommand)
                {
                    RunCommand currentRun = (RunCommand) list[i];

                    if (lastRun != null)
                    {
                        // TODO: Make it as an option! Current value is 1 min.
                        if (currentRun.Timestamp - lastRun.Timestamp <= 60000)
                        {
                            foreach (var element in deleteOffsets)
                            {
                                if (element.Confirmed)
                                {
                                    var item = new ListViewItem(new[]
                                                                    {
                                                                        lastRun.ID.ToString(),
                                                                        "",
                                                                        LogProvider.GetVideoTime(lastRun),
                                                                        "Deleted: \"" + element.DeletedText + "\", Inserted: \"" + element.InsertedText + "\"",
                                                                    });
                                    listViewPatterns.Items.Add(item);
                                }
                            }
                        }
                    }

                    lastRun = currentRun;
                    deleteOffsets.Clear();
                }
                else if (list[i] is DocumentChange)
                {
                    DocumentChange dc = (DocumentChange) list[i];
                    if (dc is Delete)
                    {
                        Delete delete = (Delete) dc;

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
                        Insert insert = (Insert) dc;

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
                    else if (dc is Replace && !LogProvider.CausedByAutoIndent(dc))
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
    }
}