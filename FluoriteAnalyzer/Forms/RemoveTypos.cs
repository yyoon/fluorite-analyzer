using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using FluoriteAnalyzer.Analyses;
using FluoriteAnalyzer.PatternDetectors;
using System.Xml;
using FluoriteAnalyzer.Events;

namespace FluoriteAnalyzer.Forms
{
    public partial class RemoveTypos : Form
    {
        public RemoveTypos()
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

            if (string.IsNullOrEmpty(textPostfix.Text))
            {
                MessageBox.Show("Please enter any postfix to be used, or this tool would overwrite existing files.", "Warning", MessageBoxButtons.OK);
                textPostfix.Focus();
                return;
            }

            List<FileInfo> fileInfos =
                listBox1.Items
                .Cast<string>()
                .OrderBy(x => x)
                .Select(x => new FileInfo(x))
                .ToList();

            RemoveTyposFromFiles(fileInfos);

            DialogResult = DialogResult.OK;
            Close();
        }

        private void RemoveTyposFromFiles(List<FileInfo> fileInfos)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("Message");
            builder.AppendLine("=======");

            foreach (FileInfo fileInfo in fileInfos)
            {
                builder.AppendLine(RemoveTyposFromFile(fileInfo));
            }

            MessageBox.Show(builder.ToString(), "RemoveTypos");
        }

        private string RemoveTyposFromFile(FileInfo fileInfo)
        {
            LogProvider provider = new LogProvider();
            provider.OpenLog(fileInfo.FullName);

            TypoCorrectionDetector typoDetector = TypoCorrectionDetector.GetInstance();
            var patterns = typoDetector.DetectAsPatternInstances(provider);

            var xmlDoc = new XmlDocument();
            xmlDoc.Load(fileInfo.FullName);

            var documentChanges = provider.LoggedEvents.OfType<DocumentChange>().ToList();

            // This should be done in reverse order, to process consecutive typo corrections correctly.
            foreach (PatternInstance pattern in patterns.Reverse())
            {
                // Determine the type
                int startIndex = documentChanges.IndexOf(pattern.PrimaryEvent as DocumentChange);

                // Type 1: Insert -> Delete -> Insert
                if (documentChanges[startIndex + 1] is Delete)
                {
                    ProcessType1(xmlDoc, documentChanges, startIndex);
                }
                // Type 2: Insert -> Replace
                else if (documentChanges[startIndex + 1] is Replace)
                {
                    ProcessType2(xmlDoc, documentChanges, startIndex);
                }
            }

            string newPath = Path.Combine(fileInfo.DirectoryName,
                Path.GetFileNameWithoutExtension(fileInfo.Name) + textPostfix.Text + fileInfo.Extension);

            xmlDoc.Save(newPath);

            return string.Format("[{0}] {1} typo corrections have been removed", fileInfo.FullName, patterns.Count());
        }

        private static void ProcessType1(XmlDocument xmlDoc, List<DocumentChange> documentChanges, int startIndex)
        {
            Insert i1 = documentChanges[startIndex + 0] as Insert;
            Delete d1 = documentChanges[startIndex + 1] as Delete;
            Insert i2 = documentChanges[startIndex + 2] as Insert;

            // Calculate new values of i1
            StringBuilder resultingText = new StringBuilder(i1.Text);
            resultingText.Remove(d1.Offset - i1.Offset, d1.Length);
            resultingText.Insert(i2.Offset - i1.Offset, i2.Text);

            int resultingLength = i1.Length - d1.Length + i2.Length;

            long endingTimestamp = i2.Timestamp2.HasValue ? i2.Timestamp2.Value : i2.Timestamp;

            // Apply these in the XmlElement corresponding with i1.
            ApplyChanges(xmlDoc, i1, resultingText, resultingLength, endingTimestamp);

            // Remove XmlElements corresponding with d1 & i2.
            xmlDoc.DocumentElement.RemoveChild(Event.FindCorrespondingXmlElementFromXmlDocument(xmlDoc, d1));
            xmlDoc.DocumentElement.RemoveChild(Event.FindCorrespondingXmlElementFromXmlDocument(xmlDoc, i2));
        }

        private static void ProcessType2(XmlDocument xmlDoc, List<DocumentChange> documentChanges, int startIndex)
        {
            Insert i1 = documentChanges[startIndex + 0] as Insert;
            Replace r1 = documentChanges[startIndex + 1] as Replace;

            // Calculate new values of i1
            StringBuilder resultingText = new StringBuilder(i1.Text);
            resultingText.Remove(r1.Offset - i1.Offset, r1.Length);
            resultingText.Insert(r1.Offset - i1.Offset, r1.InsertedText);

            int resultingLength = i1.Length - r1.Length + r1.InsertionLength;

            long endingTimestamp = r1.Timestamp2.HasValue ? r1.Timestamp2.Value : r1.Timestamp;

            // Apply these in the XmlElement corresponding with i1.
            ApplyChanges(xmlDoc, i1, resultingText, resultingLength, endingTimestamp);

            // Remove XmlElements corresponding with d1 & i2.
            xmlDoc.DocumentElement.RemoveChild(Event.FindCorrespondingXmlElementFromXmlDocument(xmlDoc, r1));
        }

        private static void ApplyChanges(XmlDocument xmlDoc, Insert insert, StringBuilder resultingText, int resultingLength, long endingTimestamp)
        {
            insert.Text = resultingText.ToString();
            insert.Length = resultingLength;
            insert.Timestamp2 = endingTimestamp;

            XmlElement insertElement = Event.FindCorrespondingXmlElementFromXmlDocument(xmlDoc, insert);   // should be in there!
            insertElement.FirstChild.FirstChild.Value = resultingText.ToString();
            insertElement.Attributes["length"].Value = resultingLength.ToString();
            XmlAttribute timestamp2Attr = insertElement.Attributes["timestamp2"];
            if (timestamp2Attr == null) { timestamp2Attr = xmlDoc.CreateAttribute("timestamp2"); }
            timestamp2Attr.Value = endingTimestamp.ToString();
        }
    }
}
