using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;

namespace FluoriteAnalyzer.Forms
{
    public partial class LogMerger : Form
    {
        public LogMerger()
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

        private void buttonMerge_Click(object sender, EventArgs e)
        {
            if (listBox1.Items.Count == 0)
            {
                MessageBox.Show("Please add log files", "Error", MessageBoxButtons.OK);
                return;
            }

            List<FileInfo> fileInfos =
                listBox1.Items.Cast<string>().OrderBy(x => x).Select(x => new FileInfo(x)).ToList();

            var saveDialog = new SaveFileDialog();
            saveDialog.InitialDirectory = fileInfos[0].DirectoryName;
            saveDialog.FileName = Path.GetFileNameWithoutExtension(fileInfos[0].FullName) + "_Merged.xml";
            saveDialog.Filter = "Log Files|*.xml";

            DialogResult result = saveDialog.ShowDialog();
            if (result == DialogResult.Cancel)
            {
                return;
            }

            Merge(fileInfos, saveDialog.FileName);

            DialogResult = DialogResult.OK;
            Close();
        }

        private void Merge(List<FileInfo> fileInfos, string mergedFilePath)
        {
            var mergedLog = new XmlDocument();
            mergedLog.Load(fileInfos[0].FullName);

            XmlNode root = mergedLog.DocumentElement;
            long baseTimestamp = long.Parse(root.Attributes["startTimestamp"].Value);

            // last id + 1
            long id = long.Parse(root.LastChild.Attributes["__id"].Value) + 1;
            List<XmlComment> comments = new List<XmlComment>();
            comments.Add(GenerateCommentForFile(mergedLog, fileInfos[0], 0, id));

            foreach (FileInfo fileInfo in fileInfos.Skip(1))
            {
                var subsequentLog = new XmlDocument();
                subsequentLog.Load(fileInfo.FullName);

                long startTimestamp = long.Parse(subsequentLog.DocumentElement.Attributes["startTimestamp"].Value);
                long delta = startTimestamp - baseTimestamp;
                long startID = id;

                foreach (XmlNode node in subsequentLog.DocumentElement.ChildNodes)
                {
                    XmlNode copiedNode = mergedLog.ImportNode(node, true);
                    foreach (XmlAttribute attr in copiedNode.Attributes)
                    {
                        if (attr.Name.StartsWith("timestamp"))
                        {
                            attr.Value = (long.Parse(attr.Value) + delta).ToString();
                        }
                        else if (attr.Name == "__id")
                        {
                            attr.Value = id.ToString();
                        }
                    }

                    ++id;
                    root.AppendChild(copiedNode);
                }

                comments.Add(GenerateCommentForFile(mergedLog, fileInfo, startID, id));
            }

            var refChild = mergedLog.FirstChild;
            foreach (XmlComment comment in comments)
            {
                mergedLog.InsertBefore(comment, refChild);
            }

            mergedLog.Save(mergedFilePath);
        }

        private XmlComment GenerateCommentForFile(XmlDocument doc, FileInfo file, long startID, long endID)
        {
            string content = string.Format(
                "Original Log File: {0}, [{1}, {2})",
                file.Name, startID, endID);

            return doc.CreateComment(content);
        }
    }
}