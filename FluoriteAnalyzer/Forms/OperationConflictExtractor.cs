using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FluoriteAnalyzer.Commons;
using FluoriteAnalyzer.PatternDetectors;

namespace FluoriteAnalyzer.Forms
{
    public partial class OperationConflictExtractor : Form
    {
        public OperationConflictExtractor()
        {
            InitializeComponent();
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            var openDialog = new OpenFileDialog();
            openDialog.Multiselect = true;
            openDialog.Filter = "Log Files|*.xml";

            DialogResult result = openDialog.ShowDialog();
            if (result == DialogResult.Cancel)
            {
                return;
            }

            listBox1.Items.AddRange(openDialog.FileNames.Where(x => !listBox1.Items.Contains(x)).ToArray());
        }

        private void buttonRemoveFiles_Click(object sender, EventArgs e)
        {
            List<int> indices = listBox1.SelectedIndices.Cast<int>().OrderByDescending(x => x).ToList();
            foreach (int index in indices)
            {
                listBox1.Items.RemoveAt(index);
            }
        }

        private void buttonExtract_Click(object sender, EventArgs e)
        {
            if (listBox1.Items.Count == 0)
            {
                MessageBox.Show("Please add log files", "Error", MessageBoxButtons.OK);
                return;
            }

            List<FileInfo> fileInfos =
                listBox1.Items
                .Cast<string>()
                .OrderBy(x => x)
                .Select(x => new FileInfo(x))
                .ToList();

            ExtractOperationConflicts(fileInfos);

            DialogResult = DialogResult.OK;
            Close();
        }

        private void ExtractOperationConflicts(List<FileInfo> fileInfos)
        {
            StringBuilder builder = new StringBuilder();

            foreach (FileInfo fileInfo in fileInfos)
            {
                LogProvider provider = new LogProvider();
                provider.OpenLog(fileInfo.FullName);

                OperationConflictDetector detector = OperationConflictDetector.GetInstance();
                var patterns = detector.DetectAsPatternInstances(provider);

                foreach (OperationConflictPatternInstance pattern in patterns)
                {
                    string line = string.Format("\"{0}\"\t{1}\t{2}\t{3}\t{4}\t{5}",
                        fileInfo.Name, pattern.ConflictType,
                        pattern.Before.GetType().Name, pattern.Before.ID,
                        pattern.After.GetType().Name, pattern.After.ID);
                    builder.AppendLine(line);
                }
            }

            if (string.IsNullOrEmpty(builder.ToString()))
            {
                MessageBox.Show("No patterns were found!");
            }
            else
            {
                Clipboard.SetText(builder.ToString(), TextDataFormat.Text);
                MessageBox.Show("Contents copied to the clipboard!");
            }
        }
    }
}
