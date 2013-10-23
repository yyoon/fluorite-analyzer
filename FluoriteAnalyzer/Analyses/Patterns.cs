using System;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using FluoriteAnalyzer.Commons;
using FluoriteAnalyzer.PatternDetectors;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using FluoriteAnalyzer.Utils;

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
                    ToolStripMenuItem menuItem = new ToolStripMenuItem(pair.Item1);
                    int targetID = pair.Item2;

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

        private void LoadPatterns(string filePath)
        {
            LoadPatterns(DetectionResult.LoadFromFile(filePath));
        }

        private void LoadPatterns(DetectionResult result)
        {
            if (result == null)
                throw new ArgumentNullException();

            if (result.LogPath != LogProvider.LogPath)
            {
                MessageBox.Show("This file was not created from the current log file.");
                return;
            }

            listViewPatterns.Items.Clear();
            listViewPatterns.Items.AddRange(
                AbstractPatternDetector.ConvertToListViewItems(
                    LogProvider, result.PatternInstances).ToArray());
        }

        private void SavePatterns(string saveFilePath)
        {
            if (listViewPatterns.Items.Count == 0)
            {
                MessageBox.Show("No detected patterns to be saved!");
                return;
            }

            DetectionResult result = new DetectionResult(
                LogProvider.LogPath,
                listViewPatterns.Items
                    .Cast<ListViewItem>()
                    .Select(x => x.Tag)
                    .Cast<PatternInstance>());

            result.SaveToFile(saveFilePath);
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

        private void splitContainer2_DoubleClick(object sender, EventArgs e)
        {
            // This one is fired only when the splitter area is double-clicked.
            splitContainer2.SplitterDistance = (splitContainer2.Height - 4) / 2;
        }

        private void listViewPatterns_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListViewItem selectedListViewItem = null;
            PatternInstance selectedPatternInstance = null;

            Utils.Utils.LockWindowUpdate(this.Handle);

            if (listViewPatterns.SelectedIndices.Count > 0)
            {
                selectedListViewItem = listViewPatterns.Items[listViewPatterns.SelectedIndices[0]];
                selectedPatternInstance = selectedListViewItem.Tag as PatternInstance;

                if (selectedPatternInstance is IPreviewablePatternInstance)
                {
                    IPreviewablePatternInstance previewable = (IPreviewablePatternInstance)selectedPatternInstance;

                    SnapshotCalculator snapshotCalculator = new SnapshotCalculator(LogProvider);
                    snapshotPreview1.SetSnapshot(snapshotCalculator.CalculateSnapshotAtID(previewable.FirstID));
                    snapshotPreview2.SetSnapshot(snapshotCalculator.CalculateSnapshotAtID(previewable.SecondID));
                }
            }
            else
            {
                snapshotPreview1.Clear();
                snapshotPreview2.Clear();
            }

            Utils.Utils.LockWindowUpdate((IntPtr)0);
        }

        private void listViewPatterns_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.C && (e.Modifiers & Keys.Control) == Keys.Control)
            {
                if (listViewPatterns.SelectedIndices.Count == 1)
                {
                    PatternInstance patternInst = listViewPatterns.Items[
                        listViewPatterns.SelectedIndices[0]].Tag as PatternInstance;

                    patternInst.CopyToClipboard();
                }
            }
        }

        private void buttonLoadResults_Click(object sender, EventArgs e)
        {
            var openDialog = new OpenFileDialog();
            openDialog.Multiselect = false;
            openDialog.Filter = "Detection Result Files|*.dtr";

            DialogResult result = openDialog.ShowDialog();
            if (result == DialogResult.Cancel)
            {
                return;
            }

            LoadPatterns(openDialog.FileName);
        }

        private void buttonSaveResults_Click(object sender, EventArgs e)
        {
            var saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Detection Result Files|*.dtr";

            DialogResult result = saveDialog.ShowDialog();
            if (result == DialogResult.Cancel)
            {
                return;
            }

            SavePatterns(saveDialog.FileName);
        }
    }
}
