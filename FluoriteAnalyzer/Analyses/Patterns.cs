using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using FluoriteAnalyzer.Events;

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

            List<DocumentChange> list = LogProvider.LoggedEvents.OfType<DocumentChange>().ToList();
            foreach (int i in Enumerable.Range(0, list.Count))
            {
                DocumentChange dc = list[i];

                // Insert - Delete - Insert+
                if (dc is Insert)
                {
                    int offset = dc.Offset;
                    int length = dc.Length;

                    if (i + 1 < list.Count && list[i + 1] is Delete)
                    {
                        int offset2 = list[i + 1].Offset;
                        int length2 = list[i + 1].Length;

                        if (offset <= offset2 && offset2 + length2 <= offset + length)
                        {
                            if (i + 2 < list.Count && list[i + 2] is Insert)
                            {
                                int offset3 = list[i + 2].Offset;

                                if (offset2 == offset3)
                                {
                                    var item = new ListViewItem(new[]
                                                                    {
                                                                        dc.ID.ToString(),
                                                                        "3",
                                                                        LogProvider.GetVideoTime(dc),
                                                                        "\"" + ((Insert) list[i]).Text + "\" - \"" +
                                                                        ((Delete) list[i + 1]).Text + "\" + \"" +
                                                                        ((Insert) list[i + 2]).Text + "\"",
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

                    if (i + 1 < list.Count && list[i + 1] is Replace)
                    {
                        int offset2 = list[i + 1].Offset;
                        int length2 = list[i + 1].Length;

                        if (offset <= offset2 && offset2 + length2 <= offset + length)
                        {
                            if (i + 2 < list.Count && list[i + 2] is Insert)
                            {
                                int offset3 = list[i + 2].Offset;

                                if (offset3 == offset2 + ((Replace) list[i + 1]).InsertionLength)
                                {
                                    var item = new ListViewItem(new[]
                                                                    {
                                                                        dc.ID.ToString(),
                                                                        "3",
                                                                        LogProvider.GetVideoTime(dc),
                                                                        "\"" + ((Insert) list[i]).Text + "\" - \"" +
                                                                        ((Replace) list[i + 1]).DeletedText + "\" + \"" +
                                                                        ((Replace) list[i + 1]).InsertedText +
                                                                        ((Insert) list[i + 2]).Text + "\"",
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
                                                                    "\"" + ((Insert) list[i]).Text + "\" - \"" +
                                                                    ((Replace) list[i + 1]).DeletedText + "\" + \"" +
                                                                    ((Replace) list[i + 1]).InsertedText + "\"",
                                                                });

                                listViewPatterns.Items.Add(item);
                            }
                        }
                    }
                }
            }
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
    }
}