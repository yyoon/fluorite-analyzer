using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using FluoriteAnalyzer.Events;

namespace FluoriteAnalyzer.Forms
{
    public partial class AnnotationExtractor : Form
    {
        public AnnotationExtractor()
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

            ExtractAnnotations(fileInfos);

            DialogResult = DialogResult.OK;
            Close();
        }

        private void ExtractAnnotations(List<FileInfo> fileInfos)
        {
            StringBuilder builder = new StringBuilder();

            foreach (FileInfo fileInfo in fileInfos)
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(fileInfo.FullName);

                var annotations = xmlDoc.FirstChild.ChildNodes
                    .OfType<XmlElement>()
                    .Select(x => Event.CreateEventFromXmlElement(x))
                    .OfType<Annotation>();

                foreach (Annotation annotation in annotations)
                {
                    string line = string.Format("{0}\t{1}\t{2}\t{3}\t{4}",
                        textParticipantID.Text, fileInfo.Name, annotation.ID,
                        annotation.Selection, annotation.Comment);
                    builder.AppendLine(line);
                }
            }

            if (string.IsNullOrEmpty(builder.ToString()))
            {
                MessageBox.Show("No annotations were found!");
            }
            else
            {
                Clipboard.SetText(builder.ToString(), TextDataFormat.Text);
                MessageBox.Show("Contents copied to the clipboard!");
            }
        }
    }
}
