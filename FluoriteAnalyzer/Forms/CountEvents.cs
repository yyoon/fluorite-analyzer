using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FluoriteAnalyzer.Commons;
using FluoriteAnalyzer.Events;
using FluoriteAnalyzer.Utils;

namespace FluoriteAnalyzer.Forms
{
    public partial class CountEvents : Form
    {
        public CountEvents()
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

            CountEventsFromFiles(fileInfos);

            DialogResult = DialogResult.OK;
            Close();
        }

        private void CountEventsFromFiles(List<FileInfo> fileInfos)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("Message");
            builder.AppendLine("=======");

            foreach (FileInfo fileInfo in fileInfos)
            {
                builder.AppendLine(CountEventsFromFile(fileInfo));
            }

            Clipboard.SetText(builder.ToString());

            MessageBox.Show(builder.ToString(), "CountEvents");
        }

        private string CountEventsFromFile(FileInfo fileInfo)
        {
            LogProvider provider = new LogProvider();
            provider.OpenLog(fileInfo.FullName);

            return "Total Events in this log: " + provider.LoggedEvents.Count;
        }
    }
}
