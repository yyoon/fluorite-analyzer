using System;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using FluoriteAnalyzer.Common;
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

            // Detect all the detectors using reflection.
            // Use AbstractPatternDetector as a base class,
            // and find all subclasses in this assembly.
            comboDetectors.Items.Clear();

            Type baseType = typeof(AbstractPatternDetector);
            var patternDetectors = baseType.Assembly.GetTypes().Where(x => x.IsSubclassOf(baseType));

            comboDetectors.Items.AddRange(patternDetectors.Select(x => x.Name).ToArray());
            if (comboDetectors.Items.Count > 0)
            {
                comboDetectors.SelectedIndex = 0;
            }
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

        private void buttonDetectPatterns_Click(object sender, EventArgs e)
        {
            // Make sure that something is selected.
            if (comboDetectors.SelectedIndex < 0 || comboDetectors.SelectedIndex >= comboDetectors.Items.Count)
            {
                // Do nothing in these cases
                return;
            }

            Type baseType = typeof(AbstractPatternDetector);
            Assembly assembly = baseType.Assembly;
            string detectorName = comboDetectors.Items[comboDetectors.SelectedIndex].ToString();

            Type detectorType = assembly.GetType(baseType.Namespace + "." + detectorName);
            MethodInfo instanceGetter = detectorType.GetMethod("GetInstance", BindingFlags.NonPublic | BindingFlags.Static);
            IPatternDetector patternDetector = instanceGetter.Invoke(null, null) as IPatternDetector;

            DetectPattern(patternDetector);
        }
    }
}
