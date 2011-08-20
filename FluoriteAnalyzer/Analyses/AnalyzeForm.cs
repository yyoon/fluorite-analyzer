using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using FluoriteAnalyzer.Events;
using FluoriteAnalyzer.Forms;
using FluoriteAnalyzer.Properties;

namespace FluoriteAnalyzer.Analyses
{
    public partial class AnalyzeForm : Form, ILogProvider
    {
        public AnalyzeForm(string logPath)
        {
            InitializeComponent();

            Icon = Resources.docfind;

            LogPath = logPath;
        }

        #region ILoggedEvents 멤버

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

        private long? TimeDiff { get; set; }

        private string LogPath { get; set; }
        private List<Event> LoggedEvents { get; set; }

        #region Child analyze panels

        private readonly List<IRedrawable> childPanels = new List<IRedrawable>();

        #endregion

        #region Event Handlers / Common methods

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
            var seconds = (int) (adjustedTimestamp%60);
            adjustedTimestamp /= 60;
            var minutes = (int) (adjustedTimestamp%60);
            adjustedTimestamp /= 60;
            long hours = adjustedTimestamp;

            return string.Format("{0:00}", hours) + ":" + string.Format("{0:00}", minutes) + ":" +
                   string.Format("{0:00}", seconds);
        }

        private void AnalyzeForm_Load(object sender, EventArgs e)
        {
            Text = LogPath;

            ParseLog(LogPath);

            // Initialize child panels
            var commandStatistics = new CommandStatistics(this);
            commandStatistics.Dock = DockStyle.Fill;
            tabCommands.Controls.Add(commandStatistics);
            childPanels.Add(commandStatistics);

            var lineChart = new LineChart(this);
            lineChart.Dock = DockStyle.Fill;
            lineChart.ChartDoubleClick += lineChart_ChartDoubleClick;
            tabVisualization.Controls.Add(lineChart);
            childPanels.Add(lineChart);

            var patterns = new Patterns(this);
            patterns.Dock = DockStyle.Fill;
            patterns.PatternDoubleClick += pattern_ItemDoubleClick;
            tabPatterns.Controls.Add(patterns);
            childPanels.Add(patterns);

            var eventsList = new EventsList(this);
            eventsList.Dock = DockStyle.Fill;
            lineChart.ChartDoubleClick += eventsList.lineChart_ChartDoubleClick;
            patterns.PatternDoubleClick += eventsList.pattern_ItemDoubleClick;
            tabEvents.Controls.Add(eventsList);
            childPanels.Add(eventsList);

            var keyStrokes = new KeyStrokes(this);
            keyStrokes.Dock = DockStyle.Fill;
            tabKeyStrokes.Controls.Add(keyStrokes);
            childPanels.Add(keyStrokes);

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

        private void AnalyzeForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            IRedrawable commandStatistics = childPanels.Find(x => x is CommandStatistics);
            if (commandStatistics != null)
            {
                ((CommandStatistics) commandStatistics).SaveCustomGroups();
            }
        }

        #endregion

        #region Third Tab (Events List)

        #endregion

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
                    ((EventsList) child).RebuildFilteredEventsList();
                    break;
                }
            }
        }

        private void lineChart_ChartDoubleClick(int timevalue)
        {
            // Move the tab
            tabControl.SelectTab(tabEvents);
        }

        private void pattern_ItemDoubleClick(int startingID)
        {
            // Move the tab
            tabControl.SelectTab(tabEvents);
        }
    }
}