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
    public partial class CountEditedFiles : Form
    {
        public CountEditedFiles()
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

            CountEditedFilesFromFiles(fileInfos);

            DialogResult = DialogResult.OK;
            Close();
        }

        private void CountEditedFilesFromFiles(List<FileInfo> fileInfos)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("Message");
            builder.AppendLine("=======");

            foreach (FileInfo fileInfo in fileInfos)
            {
                builder.AppendLine(CountEditedFilesFromFile(fileInfo));
            }

            Clipboard.SetText(builder.ToString());

            MessageBox.Show(builder.ToString(), "CountEditedFiles");
        }

        private string CountEditedFilesFromFile(FileInfo fileInfo)
        {
            LogProvider provider = new LogProvider();
            provider.OpenLog(fileInfo.FullName);

            var editedFiles = provider.LoggedEvents.OfType<FileOpenCommand>().Where(x => IsEdited(provider.LoggedEvents, x));

            StringBuilder builder = new StringBuilder();

            var maxEditedFilesList = new List<int>();

            foreach (int windowSize in Enumerable.Range(1, 20))
            {
                // convert windowSize(in hours) into milliseconds.
                long windowSizeInMilliseconds = windowSize * 60 * 60 * 1000;

                int maxEditedFiles = 0;
                var focsInCurrentWindow = new LinkedList<FileOpenCommand>();
                List<string> filePaths = new List<string>();
                foreach (var foc in editedFiles)
                {
                    focsInCurrentWindow.AddLast(foc);
                    while (foc.Timestamp - focsInCurrentWindow.First.Value.Timestamp > windowSizeInMilliseconds)
                    {
                        focsInCurrentWindow.RemoveFirst();
                    }

                    var groups = focsInCurrentWindow.GroupBy(x => x.FilePath);

                    if (groups.Count() > maxEditedFiles)
                    {
                        maxEditedFiles = groups.Count();
                        filePaths = groups.Select(x => x.Key).ToList();
                    }
                }

                //builder.AppendLine(string.Format("[{0} hr window]: {1}", windowSize, maxEditedFiles));
                //builder.AppendLine(string.Join(Environment.NewLine, filePaths));
                maxEditedFilesList.Add(maxEditedFiles);
            }

            builder.AppendLine(string.Join("\t", maxEditedFilesList));

            builder.AppendLine("Total Edited Files in this log: " + editedFiles.GroupBy(x => x.FilePath).Count());

            return builder.ToString();
        }

        private bool IsEdited(IEnumerable<Event> loggedEvents, FileOpenCommand foc)
        {
            // Get all the events happened between this file open command, and the next one.
            IEnumerable<Event> subsequentEvents = loggedEvents.SkipUntil(x => x == foc).TakeUntil(x => x != foc && x is FileOpenCommand);

            // See if there is a document change or not.
            return subsequentEvents.FirstOrDefault(x => x is DocumentChange) != null;
        }
    }
}
