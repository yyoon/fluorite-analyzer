using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using FluoriteAnalyzer.Analyses;
using FluoriteAnalyzer.Events;
using FluoriteAnalyzer.Properties;
using FluoriteAnalyzer.Utils;

namespace FluoriteAnalyzer.Forms
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            Icon = Resources.autoList;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            RecentFiles.Load();
        }

        private void openLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenLog();
        }

        private void toolStripButtonOpen_Click(object sender, EventArgs e)
        {
            OpenLog();
        }

        private void OpenLog()
        {
            var openDialog = new OpenFileDialog();
            openDialog.Multiselect = false;
            openDialog.Filter = Resources.Fluorite_Filter_String;

            DialogResult result = openDialog.ShowDialog();
            if (result == DialogResult.Cancel)
            {
                return;
            }

            OpenLog(openDialog.FileName);
        }

        private void OpenLog(string filePath)
        {
            var analyzeForm = new AnalyzeForm(filePath);
            analyzeForm.MdiParent = this;
            analyzeForm.Show();
            RecentFiles.GetInstance().Touch(filePath);
        }

        private void logMergerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var logMerger = new LogMerger();
            logMerger.ShowDialog();
        }

        private void fixInsertStringCommandRepeatCountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var openDialog = new OpenFileDialog();
            openDialog.Multiselect = false;
            openDialog.Filter = Resources.Fluorite_Filter_String;

            DialogResult result = openDialog.ShowDialog();
            if (result == DialogResult.Cancel)
            {
                return;
            }

            var log = new XmlDocument();
            log.Load(openDialog.FileName);

            foreach (XmlElement element in
                log.DocumentElement.ChildNodes.OfType<XmlElement>()
                    .Where(x => x.Name == "Command" && x.Attributes["_type"].Value == "InsertStringCommand"))
            {
                var isc = (InsertStringCommand) Event.CreateEventFromXmlElement(element);
                int length = isc.Data.Length;
                if (element.Attributes.GetNamedItem("repeat") != null)
                {
                    element.Attributes["repeat"].Value = length.ToString();
                }
            }

            log.Save(openDialog.FileName);
        }

        private void duplicateFixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var openDialog = new OpenFileDialog();
            openDialog.Multiselect = false;
            openDialog.Filter = Resources.Fluorite_Filter_String;

            DialogResult result = openDialog.ShowDialog();
            if (result == DialogResult.Cancel)
            {
                return;
            }

            var log = new XmlDocument();
            log.Load(openDialog.FileName);

            var dict = new Dictionary<string, int>();
            string currentFile = null;

            List<XmlElement> list =
                log.DocumentElement.ChildNodes.OfType<XmlElement>().Where(x => x.Name == "Command").ToList();

            //foreach (XmlElement element in
            //    log.DocumentElement.ChildNodes.OfType<XmlElement>().Where(x => x.Name == "Command"))
            for (int i = 0; i < list.Count; ++i)
            {
                XmlElement element = list[i];

                switch (element.Attributes["_type"].Value)
                {
                    case "FileOpenCommand":
                        currentFile = element.ChildNodes[0].ChildNodes[0].Value;

                        if (dict.ContainsKey(currentFile))
                        {
                            dict[currentFile]++;
                        }
                        else
                        {
                            dict.Add(currentFile, 1);
                        }

                        break;

                    case "InsertStringCommand":
                        {
                            int currentMultiplier = dict[currentFile];
                            if (currentMultiplier <= 1)
                            {
                                break;
                            }

                            string oldText = element.ChildNodes[0].ChildNodes[0].Value;
                            string newText = "";

                            if (oldText.Length%currentMultiplier != 0)
                            {
                                for (int j = 1; j < currentMultiplier; ++j)
                                {
                                    ++i;
                                    log.DocumentElement.RemoveChild(list[i]);
                                }
                                break;
                            }

                            for (int j = 0; j < oldText.Length/currentMultiplier; ++j)
                            {
                                newText += oldText[j*currentMultiplier];
                            }
                            element.ChildNodes[0].ChildNodes[0].Value = newText;

                            if (element.Attributes["repeat"] != null)
                            {
                                element.Attributes["repeat"].Value =
                                    (int.Parse(element.Attributes["repeat"].Value)/currentMultiplier).ToString();
                            }
                        }
                        break;

                    case "EclipseCommand":
                        if (element.Attributes["commandID"].Value.StartsWith("eventLogger.styledTextCommand"))
                        {
                            int currentMultiplier = dict[currentFile];
                            if (currentMultiplier <= 1)
                            {
                                break;
                            }

                            element.Attributes["repeat"].Value =
                                (int.Parse(element.Attributes["repeat"].Value)/currentMultiplier).ToString();
                        }
                        break;
                }
            }

            string saveFileName = Path.Combine(
                Path.GetDirectoryName(openDialog.FileName),
                Path.GetFileNameWithoutExtension(openDialog.FileName) + "_DuplicateFixed.xml");

            log.Save(saveFileName);
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            RecentFiles.Save();
        }

        private void fileToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            recentFilesToolStripMenuItem.DropDownItems.Clear();

            if (RecentFiles.GetInstance().IsEmpty())
            {
                recentFilesToolStripMenuItem.Enabled = false;
            }
            else
            {
                recentFilesToolStripMenuItem.Enabled = true;

                int count = RecentFiles.GetInstance().Count;

                recentFilesToolStripMenuItem.DropDownItems.AddRange(
                    Enumerable.Range(0, count)
                        .Select(x => new ToolStripMenuItem(
                                         "&" + (x + 1) + ": " + RecentFiles.GetInstance().List[count - 1 - x],
                                         null,
                                         delegate { OpenLog(RecentFiles.GetInstance().List[count - 1 - x]); }))
                        .ToArray()
                    );
            }
        }

        private void cascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        private void tileHorizontallyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void closeAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Form child in MdiChildren)
            {
                child.Close();
            }
        }
    }
}