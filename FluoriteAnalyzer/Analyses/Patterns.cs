using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using FluoriteAnalyzer.Events;
using System.Diagnostics;
using FluoriteAnalyzer.PatternDetectors;

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

        private void listViewPatterns_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                ListViewItem item = listViewPatterns.GetItemAt(e.X, e.Y);
                if (item == null) { return; }

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

        private void listViewPatterns_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                ListViewItem item = listViewPatterns.GetItemAt(e.X, e.Y);
                if (item == null) { return; }

                ContextMenuStrip contextMenu = new ContextMenuStrip();

                PatternInstance instance = item.Tag as PatternInstance;
                foreach (var pair in instance.GetInvolvingEvents())
                {
                    ToolStripMenuItem menuItem = new ToolStripMenuItem(pair.Key);
                    int targetID = pair.Value;

                    menuItem.Click += delegate(object s, EventArgs ea)
                    {
                        PatternDoubleClick(targetID);
                    };

                    contextMenu.Items.Add(menuItem);
                }

                contextMenu.Show(this, e.Location);
            }
        }

        public event PatternDoubleClickHandler PatternDoubleClick;

        private void DetectPattern(IPatternDetector detector)
        {
            listViewPatterns.Items.Clear();
            listViewPatterns.Items.AddRange(detector.DetectAsListViewItems(LogProvider).ToArray());

            labelCount.Text = "Total: " + listViewPatterns.Items.Count;
        }

        private void buttonSearchFixTypos_Click(object sender, EventArgs e)
        {
            DetectPattern(TypoCorrectionDetector.GetInstance());
        }

        private void buttonSearchParameterTuning_Click(object sender, EventArgs e)
        {
            DetectPattern(ParameterTuningDetector.GetInstance());
        }

        private void buttonFindErrorRecoveries_Click(object sender, EventArgs e)
        {
            DetectPattern(ErrorRecoveryDetector.GetInstance());
        }

        private void buttonFindMovePatterns_Click(object sender, EventArgs e)
        {
            DetectPattern(MoveDetector.GetInstance());
        }
    }
}