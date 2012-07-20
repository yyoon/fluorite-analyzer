using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using FluoriteAnalyzer.Analyses;
using FluoriteAnalyzer.PatternDetectors;
using FluoriteAnalyzer.Events;

namespace FluoriteAnalyzer.Forms
{
    public partial class DetectMoves : Form
    {
        public DetectMoves()
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

            DetectMovesFromFiles(fileInfos);

            DialogResult = DialogResult.OK;
            Close();
        }

        private void DetectMovesFromFiles(List<FileInfo> fileInfos)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("Message");
            builder.AppendLine("=======");

            foreach (FileInfo fileInfo in fileInfos)
            {
                builder.AppendLine(DetectMovesFromFile(fileInfo));
            }

            MessageBox.Show(builder.ToString(), "DetectMoves");
        }

        private string DetectMovesFromFile(FileInfo fileInfo)
        {
            LogProvider provider = new LogProvider();
            provider.OpenLog(fileInfo.FullName);

            MoveDetector moveDetector = MoveDetector.GetInstance();
            var patterns = moveDetector.DetectAsPatternInstances(provider);

            var xmlDoc = new XmlDocument();
            xmlDoc.Load(fileInfo.FullName);

            var documentChanges = provider.LoggedEvents.OfType<DocumentChange>().ToList();

            foreach (MovePatternInstance pattern in patterns)
            {
                int startIndex = documentChanges.IndexOf(pattern.PrimaryEvent as DocumentChange);

                Delete delete = documentChanges[startIndex + 0] as Delete;
                Insert insert = documentChanges[startIndex + 1] as Insert;

                // Transform Insert -> Move and then remove Delete.
                SetMoveElement(xmlDoc, pattern, delete, insert);
                xmlDoc.DocumentElement.RemoveChild(Event.FindCorrespondingXmlElementFromXmlDocument(xmlDoc, delete));
            }

            string newPath = Path.Combine(fileInfo.DirectoryName,
                Path.GetFileNameWithoutExtension(fileInfo.Name) + textPostfix.Text + fileInfo.Extension);

            xmlDoc.Save(newPath);

            return string.Format("[{0}] {1} moves have been detected and written in the log", fileInfo.FullName, patterns.Count());
        }

        private static void SetMoveElement(XmlDocument xmlDoc, MovePatternInstance pattern, Delete delete, Insert insert)
        {
            XmlElement deleteElement = Event.FindCorrespondingXmlElementFromXmlDocument(xmlDoc, delete);
            XmlElement insertElement = Event.FindCorrespondingXmlElementFromXmlDocument(xmlDoc, insert);
            XmlElement child;
            XmlCDataSection cdata;
            XmlAttribute attr;

            XmlElement moveElement = insertElement;
            XmlElement textElement = moveElement.FirstChild as XmlElement;
            moveElement.Attributes["_type"].Value = "Move";
            moveElement.Attributes["offset"].Value = delete.Offset.ToString();
            moveElement.Attributes["length"].Value = delete.Length.ToString();

            attr = moveElement.Attributes["repeat"];
            if (attr == null)
            {
                attr = xmlDoc.CreateAttribute("repeat");
            }
            attr.Value = (delete.RepeatCount + insert.RepeatCount).ToString();

            moveElement.Attributes["timestamp"].Value = delete.Timestamp.ToString();

            if (insert.Timestamp2.HasValue == false)
            {
                attr = xmlDoc.CreateAttribute("timestamp2");
                attr.Value = insert.Timestamp.ToString();
                moveElement.Attributes.Append(attr);
            }

            attr = xmlDoc.CreateAttribute("insertionOffset");
            attr.Value = insert.Offset.ToString();
            moveElement.Attributes.Append(attr);

            attr = xmlDoc.CreateAttribute("insertionLength");
            attr.Value = insert.Length.ToString();
            moveElement.Attributes.Append(attr);

            attr = xmlDoc.CreateAttribute("originalDeleteID");
            attr.Value = delete.ID.ToString();
            moveElement.Attributes.Append(attr);

            // startLine & endLine
            attr = xmlDoc.CreateAttribute("startLine");
            attr.Value = delete.StartLine.ToString();
            moveElement.Attributes.Append(attr);

            attr = xmlDoc.CreateAttribute("endLine");
            attr.Value = delete.EndLine.ToString();
            moveElement.Attributes.Append(attr);

            // take out the first child
            moveElement.RemoveChild(textElement);

            // deletedFrom & insertedTo
            child = xmlDoc.CreateElement("deletedFrom");
            cdata = xmlDoc.CreateCDataSection(pattern.FromFile);
            child.AppendChild(cdata);
            moveElement.AppendChild(child);

            child = xmlDoc.CreateElement("insertedTo");
            cdata = xmlDoc.CreateCDataSection(pattern.ToFile);
            child.AppendChild(cdata);
            moveElement.AppendChild(child);

            // deletedText & insertedText
            child = xmlDoc.CreateElement("deletedText");
            cdata = xmlDoc.CreateCDataSection(delete.Text);
            child.AppendChild(cdata);
            moveElement.AppendChild(child);

            child = xmlDoc.CreateElement("insertedText");
            cdata = textElement.FirstChild as XmlCDataSection;
            textElement.RemoveChild(cdata);
            child.AppendChild(cdata);
            moveElement.AppendChild(child);
        }
    }
}
