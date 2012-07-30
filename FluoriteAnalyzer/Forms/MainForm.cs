using System;
using System.Collections.Generic;
using System.Drawing;
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

            LogProvider = new LogProvider();
        }

        private LogProvider LogProvider { get; set; }

        #region Child analyze panels

        private readonly List<ToolWindow> childToolWindows = new List<ToolWindow>();
        private readonly List<IRedrawable> childPanels = new List<IRedrawable>();

        private CommandStatistics commandStatistics;
        private LineChart lineChart;
        private Patterns patterns;
        private EventsList eventsList;
        private KeyStrokes keyStrokes;

        #endregion

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
            CloseCurrentLog();

            Text = filePath; // Set the title.

            LogProvider.OpenLog(filePath);

            InitializeAnalyzeWindows();

            RecentFiles.GetInstance().Touch(filePath);
        }

        private void CloseCurrentLog()
        {
            if (LogProvider.IsLogOpen())
            {
                foreach (var childToolWindow in childToolWindows)
                {
                    childToolWindow.ForceClose();
                }

                childToolWindows.Clear();
                childPanels.Clear();

                SaveCustomGroups();
            }
        }

        private void InitializeAnalyzeWindows()
        {
            commandStatistics = new CommandStatistics(LogProvider);
            commandStatistics.Dock = DockStyle.Fill;
            childPanels.Add(commandStatistics);

            var commandStatisticsForm = CreateToolWindow(Resources.Form_Title_Commands);
            commandStatisticsForm.Controls.Add(commandStatistics);
            commandStatisticsForm.Show();
            childToolWindows.Add(commandStatisticsForm);


            lineChart = new LineChart(LogProvider);
            lineChart.Dock = DockStyle.Fill;
            lineChart.ChartDoubleClick += lineChart_ChartDoubleClick;
            childPanels.Add(lineChart);

            var lineChartForm = CreateToolWindow(Resources.Form_Title_LineGraph);
            lineChartForm.Controls.Add(lineChart);
            lineChartForm.Show();
            childToolWindows.Add(lineChartForm);


            patterns = new Patterns(LogProvider);
            patterns.Dock = DockStyle.Fill;
            patterns.PatternDoubleClick += pattern_ItemDoubleClick;
            childPanels.Add(patterns);

            var patternsForm = CreateToolWindow(Resources.Form_Title_Patterns);
            patternsForm.Controls.Add(patterns);
            patternsForm.Show();
            childToolWindows.Add(patternsForm);


            eventsList = new EventsList(LogProvider);
            eventsList.Dock = DockStyle.Fill;
            lineChart.ChartDoubleClick += eventsList.lineChart_ChartDoubleClick;
            patterns.PatternDoubleClick += eventsList.pattern_ItemDoubleClick;
            childPanels.Add(eventsList);

            var eventsListForm = CreateToolWindow(Resources.Form_Title_EventsList);
            eventsListForm.Controls.Add(eventsList);
            eventsListForm.Show();
            childToolWindows.Add(eventsListForm);


            keyStrokes = new KeyStrokes(LogProvider);
            keyStrokes.Dock = DockStyle.Fill;
            childPanels.Add(keyStrokes);

            var keyStrokesForm = CreateToolWindow(Resources.Form_Title_Keystrokes);
            keyStrokesForm.Controls.Add(keyStrokes);
            keyStrokesForm.Show();
            childToolWindows.Add(keyStrokesForm);


            LayoutMdi(MdiLayout.Cascade);


            Redraw();
        }

        private void Redraw()
        {
            foreach (IRedrawable child in childPanels)
            {
                child.Redraw();
            }
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
                var isc = (InsertStringCommand)Event.CreateEventFromXmlElement(element);
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

                            if (oldText.Length % currentMultiplier != 0)
                            {
                                for (int j = 1; j < currentMultiplier; ++j)
                                {
                                    ++i;
                                    log.DocumentElement.RemoveChild(list[i]);
                                }
                                break;
                            }

                            for (int j = 0; j < oldText.Length / currentMultiplier; ++j)
                            {
                                newText += oldText[j * currentMultiplier];
                            }
                            element.ChildNodes[0].ChildNodes[0].Value = newText;

                            if (element.Attributes["repeat"] != null)
                            {
                                element.Attributes["repeat"].Value =
                                    (int.Parse(element.Attributes["repeat"].Value) / currentMultiplier).ToString();
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
                                (int.Parse(element.Attributes["repeat"].Value) / currentMultiplier).ToString();
                        }
                        break;
                }
            }

            string saveFileName = Path.Combine(
                Path.GetDirectoryName(openDialog.FileName),
                Path.GetFileNameWithoutExtension(openDialog.FileName) + "_DuplicateFixed.xml");

            log.Save(saveFileName);
        }

        private void adjustTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var atForm = new AdjustTimeForm();
            DialogResult result = atForm.ShowDialog();

            if (result == DialogResult.Cancel)
            {
                return;
            }

            int diff = atForm.VideoTick - atForm.LogTick;

            var log = new XmlDocument();
            log.Load(LogProvider.LogPath);

            XmlAttribute attr = log.DocumentElement.Attributes["timeDiff"];
            if (attr != null)
            {
                attr.Value = diff.ToString();
            }
            else
            {
                attr = log.CreateAttribute("timeDiff");
                attr.Value = diff.ToString();

                log.DocumentElement.Attributes.Append(attr);
            }

            log.Save(LogProvider.LogPath);

            LogProvider.TimeDiff = diff;

            foreach (IRedrawable child in childPanels)
            {
                if (child is EventsList)
                {
                    ((EventsList)child).RebuildFilteredEventsList();
                    break;
                }
            }
        }

        private void extractAnnotationsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AnnotationExtractor extractor = new AnnotationExtractor();
            extractor.ShowDialog(this);
        }

        private void removeTyposToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveTypos removeTypos = new RemoveTypos();
            removeTypos.ShowDialog(this);
        }

        private void detectMovesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DetectMoves detectMoves = new DetectMoves();
            detectMoves.ShowDialog(this);
        }

        private void calculateActiveWorkingTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CalculateActiveWorkingTime calcTime = new CalculateActiveWorkingTime();
            calcTime.ShowDialog(this);
        }

        private void extractOperationConflictsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OperationConflictExtractor extractor = new OperationConflictExtractor();
            extractor.ShowDialog(this);
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            SaveCustomGroups();
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

        private void toolsToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            adjustTimeToolStripMenuItem.Enabled = LogProvider.IsLogOpen();
        }

        private void windowToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            // toolStripSeparator2 is the one under "Window" menu.
            int sepIndex = windowToolStripMenuItem.DropDownItems.IndexOf(toolStripSeparator2);

            // Clear the windows list
            while (windowToolStripMenuItem.DropDownItems.Count > sepIndex + 1)
            {
                windowToolStripMenuItem.DropDownItems.RemoveAt(sepIndex + 1);
            }

            windowToolStripMenuItem.DropDownItems.AddRange(
                childToolWindows.Select(x => new ToolStripMenuItem(
                                                 x.Text,
                                                 null,
                                                 delegate { x.Focus(); }) { Checked = x.ContainsFocus })
                    .ToArray()
                );
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

        private ToolWindow CreateToolWindow(string formTitle)
        {
            var toolWindow = new ToolWindow();
            toolWindow.MdiParent = this;
            toolWindow.Text = formTitle;
            toolWindow.Size = new Size(ClientRectangle.Width, ClientRectangle.Height);

            return toolWindow;
        }

        private void lineChart_ChartDoubleClick(int timevalue)
        {
            BringEventsListToFront();
        }

        private void pattern_ItemDoubleClick(int startingID)
        {
            BringEventsListToFront();
        }

        private void BringEventsListToFront()
        {
            var eventsListForm = this.childToolWindows.Find(x => x.Text == Resources.Form_Title_EventsList);
            if (eventsListForm != null)
            {
                eventsListForm.BringToFront();
                eventsListForm.Focus();
            }
        }

        private void SaveCustomGroups()
        {
            if (commandStatistics != null)
            {
                ((CommandStatistics)commandStatistics).SaveCustomGroups();
            }

            if (keyStrokes != null)
            {
                ((KeyStrokes)keyStrokes).SaveCustomGroups();
            }
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (!e.Shift && e.Control && !e.Alt && e.KeyCode == Keys.F)
            {
                toolStripTextSearch.Focus();
                toolStripTextSearch.SelectAll();
            }
        }

        private void toolStripTextSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (!string.IsNullOrEmpty(toolStripTextSearch.Text) && eventsList != null)
                {
                    eventsList.SearchString(toolStripTextSearch.Text);
                    toolStripTextSearch.Focus();
                    toolStripTextSearch.SelectAll();
                }

                e.SuppressKeyPress = true;
            }
        }
    }
}