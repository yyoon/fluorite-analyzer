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
    public partial class MainForm : Form, ILogProvider
    {
        public MainForm()
        {
            InitializeComponent();
            Icon = Resources.autoList;
        }

        private long? TimeDiff { get; set; }

        private string LogPath { get; set; }
        private List<Event> LoggedEvents { get; set; }

        #region Child analyze panels

        private readonly List<ToolWindow> childToolWindows = new List<ToolWindow>();
        private readonly List<IRedrawable> childPanels = new List<IRedrawable>();

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
            if (LogPath != null)
            {
                SaveCustomGroups();
            }

            LogPath = filePath;

            Text = LogPath;

            ParseLog(LogPath);

            InitializeAnalyzeWindows();

            RecentFiles.GetInstance().Touch(filePath);
        }

        private void InitializeAnalyzeWindows()
        {
            var commandStatistics = new CommandStatistics(this);
            commandStatistics.Dock = DockStyle.Fill;
            childPanels.Add(commandStatistics);

            var commandStatisticsForm = CreateToolWindow(Resources.Form_Title_Commands);
            commandStatisticsForm.Controls.Add(commandStatistics);
            commandStatisticsForm.Show();
            childToolWindows.Add(commandStatisticsForm);


            var lineChart = new LineChart(this);
            lineChart.Dock = DockStyle.Fill;
            lineChart.ChartDoubleClick += lineChart_ChartDoubleClick;
            childPanels.Add(lineChart);

            var lineChartForm = CreateToolWindow(Resources.Form_Title_LineGraph);
            lineChartForm.Controls.Add(lineChart);
            lineChartForm.Show();
            childToolWindows.Add(lineChartForm);


            var patterns = new Patterns(this);
            patterns.Dock = DockStyle.Fill;
            patterns.PatternDoubleClick += pattern_ItemDoubleClick;
            childPanels.Add(patterns);

            var patternsForm = CreateToolWindow(Resources.Form_Title_Patterns);
            patternsForm.Controls.Add(patterns);
            patternsForm.Show();
            childToolWindows.Add(patternsForm);


            var eventsList = new EventsList(this);
            eventsList.Dock = DockStyle.Fill;
            lineChart.ChartDoubleClick += eventsList.lineChart_ChartDoubleClick;
            patterns.PatternDoubleClick += eventsList.pattern_ItemDoubleClick;
            childPanels.Add(eventsList);

            var eventsListForm = CreateToolWindow(Resources.Form_Title_EventsList);
            eventsListForm.Controls.Add(eventsList);
            eventsListForm.Show();
            childToolWindows.Add(eventsListForm);


            var keyStrokes = new KeyStrokes(this);
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

        private void ParseLog(string logPath)
        {
            var log = new XmlDocument();
            log.Load(logPath);

            XmlNode events = log.DocumentElement;

            TimeDiff = null;
            foreach (XmlAttribute attr in events.Attributes)
            {
                if (attr.Name == "timeDiff")
                {
                    TimeDiff = int.Parse(attr.Value);
                    break;
                }
            }

            try
            {
                LoggedEvents =
                    events.ChildNodes.OfType<XmlElement>().Select(x => Event.CreateEventFromXmlElement(x)).ToList();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
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
            log.Load(LogPath);

            bool wasThere = false;
            foreach (XmlAttribute attr in log.DocumentElement.Attributes)
            {
                if (attr.Name == "timeDiff")
                {
                    attr.Value = diff.ToString();

                    wasThere = true;
                    break;
                }
            }

            if (!wasThere)
            {
                XmlAttribute attr = log.CreateAttribute("timeDiff");
                attr.Value = diff.ToString();

                log.DocumentElement.Attributes.Append(attr);
            }

            log.Save(LogPath);

            TimeDiff = diff;

            foreach (IRedrawable child in childPanels)
            {
                if (child is EventsList)
                {
                    ((EventsList)child).RebuildFilteredEventsList();
                    break;
                }
            }
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
            adjustTimeToolStripMenuItem.Enabled = LogPath != null;
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

        #region ILogProvider Members

        string ILogProvider.LogPath
        {
            get { return LogPath; }
        }

        IEnumerable<Event> ILogProvider.LoggedEvents
        {
            get { return LoggedEvents; }
        }

        long? ILogProvider.TimeDiff
        {
            get { return TimeDiff; }
        }

        string ILogProvider.GetVideoTime(Event anEvent)
        {
            return GetVideoTime(anEvent);
        }

        string ILogProvider.GetVideoTime(long timestamp)
        {
            return GetVideoTime(timestamp);
        }

        #endregion

        private string GetVideoTime(Event anEvent)
        {
            return TimeDiff.HasValue ? GetVideoTime(anEvent.Timestamp) : "";
        }

        private string GetVideoTime(long timestamp)
        {
            if (!TimeDiff.HasValue)
            {
                return "";
            }

            long adjustedTimestamp = timestamp + TimeDiff.Value;
            adjustedTimestamp /= 1000;
            var seconds = (int)(adjustedTimestamp % 60);
            adjustedTimestamp /= 60;
            var minutes = (int)(adjustedTimestamp % 60);
            adjustedTimestamp /= 60;
            long hours = adjustedTimestamp;

            return string.Format("{0:00}", hours) + ":" + string.Format("{0:00}", minutes) + ":" +
                   string.Format("{0:00}", seconds);
        }

        private class ToolWindow : Form
        {
            public ToolWindow() : base()
            {
                FormClosing += new FormClosingEventHandler(ToolWindow_FormClosing);
            }

            void ToolWindow_FormClosing(object sender, FormClosingEventArgs e)
            {
                if (e.CloseReason != CloseReason.MdiFormClosing)
                {
                    e.Cancel = true;
                    WindowState = FormWindowState.Minimized;
                }
            }

            protected override void WndProc(ref Message m)
            {
                switch (m.Msg)
                {
                    case 0x00A1:    // WM_NCLBUTTONDOWN
                        if (Control.ModifierKeys == Keys.Shift)
                        {
                            this.WindowState = FormWindowState.Normal;
                            this.BringToFront();

                            if (this.Parent != null)
                            {
                                this.Location = new Point { X = 0, Y = 0 };
                                this.Size = new Size { Width = this.Parent.ClientRectangle.Width / 2, Height = this.Parent.ClientRectangle.Height };
                            }
                        }
                        break;

                    case 0x00A4:    // WM_NCRBUTTONDOWN
                        if (Control.ModifierKeys == Keys.Shift)
                        {
                            this.WindowState = FormWindowState.Normal;
                            this.BringToFront();

                            if (this.Parent != null)
                            {
                                int halfWidth = Width = this.Parent.ClientRectangle.Width / 2;
                                this.Location = new Point { X = halfWidth, Y = 0 };
                                this.Size = new Size { Width = this.Parent.ClientRectangle.Width - halfWidth, Height = this.Parent.ClientRectangle.Height };
                            }
                        }
                        break;
                }

                base.WndProc(ref m);
            }
        }

        private ToolWindow CreateToolWindow(string formTitle)
        {
            var toolWindow = new ToolWindow();
            toolWindow.MdiParent = this;
            toolWindow.Text = formTitle;
            toolWindow.TopLevel = false;
            toolWindow.Size = new Size(ClientRectangle.Width, ClientRectangle.Height);
            toolWindow.KeyPreview = true;

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
            IRedrawable commandStatistics = childPanels.Find(x => x is CommandStatistics);
            if (commandStatistics != null)
            {
                ((CommandStatistics)commandStatistics).SaveCustomGroups();
            }
        }
    }
}