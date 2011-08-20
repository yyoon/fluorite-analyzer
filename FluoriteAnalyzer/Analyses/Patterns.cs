using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FluoriteAnalyzer.Events;

namespace FluoriteAnalyzer.Analyses
{
    internal partial class Patterns : UserControl, IRedrawable
    {
        public Patterns(ILogProvider logProvider)
        {
            InitializeComponent();

            LogProvider = logProvider;
        }

        private ILogProvider LogProvider { get; set; }

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
            }
        }

        #region IRedrawable Members

        public void Redraw()
        {
            listViewPatterns.Items.Clear();
        }

        #endregion
    }
}
