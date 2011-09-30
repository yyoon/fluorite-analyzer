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

        public event PatternDoubleClickHandler PatternDoubleClick;

        private void DetectPattern(IPatternDetector detector)
        {
            listViewPatterns.Items.Clear();
            listViewPatterns.Items.AddRange(detector.Detect(LogProvider).ToArray());

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
    }
}