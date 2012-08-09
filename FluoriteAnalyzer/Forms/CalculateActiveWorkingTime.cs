using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FluoriteAnalyzer.Commons;
using FluoriteAnalyzer.Events;

namespace FluoriteAnalyzer.Forms
{
    public partial class CalculateActiveWorkingTime : Form
    {
        public CalculateActiveWorkingTime()
        {
            InitializeComponent();
        }

        private static readonly long TIME_THRESHOLD = 300000;    // 5 mintes

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

        private void buttonGo_Click(object sender, EventArgs e)
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

            CalculateActiveWorkingTimeFromFiles(fileInfos);

            DialogResult = DialogResult.OK;
            Close();
        }

        private void CalculateActiveWorkingTimeFromFiles(List<FileInfo> fileInfos)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("Message");
            builder.AppendLine("=======");

            foreach (FileInfo fileInfo in fileInfos)
            {
                builder.AppendLine(CalculateActiveWorkingTimeFromFile(fileInfo));
            }

            Clipboard.SetText(builder.ToString());

            MessageBox.Show(builder.ToString(), "CalculateActiveWorkingTime");
        }

        private string CalculateActiveWorkingTimeFromFile(FileInfo fileInfo)
        {
            LogProvider provider = new LogProvider();
            provider.OpenLog(fileInfo.FullName);

            long totalTime = 0;
            long lastTimestamp = 0;

            foreach (Event anEvent in provider.LoggedEvents)
            {
                long lastTimestampOfCurrentEvent = anEvent.Timestamp2.HasValue ?
                    anEvent.Timestamp2.Value : anEvent.Timestamp;

                long currentTimespan = lastTimestampOfCurrentEvent - lastTimestamp;
                if (currentTimespan < TIME_THRESHOLD)
                {
                    totalTime += currentTimespan;
                }

                lastTimestamp = lastTimestampOfCurrentEvent;
            }

            long seconds = totalTime / 1000;
            long minutes = seconds / 60;
            long hours = minutes / 60;

            return string.Format("[{0}] {1} ms = {2} s = {3} m = {4} h",
                fileInfo.FullName, totalTime, seconds, minutes, hours);
        }
    }
}
