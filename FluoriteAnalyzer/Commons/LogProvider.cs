using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluoriteAnalyzer.Events;
using System.Xml;
using System.Windows.Forms;

namespace FluoriteAnalyzer.Commons
{
    class LogProvider : ILogProvider
    {
        public string LogPath { get; private set; }
        public List<Event> LoggedEvents { get; private set; }

        public long? TimeDiff { get; set; }

        public void OpenLog(string filePath)
        {
            LogPath = filePath;

            ParseLog(LogPath);
        }

        public bool IsLogOpen()
        {
            return LogPath != null;
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

        bool ILogProvider.CausedByPaste(DocumentChange dc)
        {
            throw new NotImplementedException();
        }

        bool ILogProvider.CausedByAssist(DocumentChange dc)
        {
            Replace replace = dc as Replace;
            if (replace == null) { return false; }

            int index = LoggedEvents.IndexOf(dc);
            if (index < 0) { return false; }

            return (index > 0 && LoggedEvents[index - 1] is AssistCommand);
        }

        bool ILogProvider.CausedByAutoIndent(DocumentChange dc)
        {
            Replace replace = dc as Replace;
            if (replace == null) { return false; }

            return !string.IsNullOrEmpty(replace.DeletedText) && string.IsNullOrWhiteSpace(replace.DeletedText);
        }

        bool ILogProvider.CausedByInsertString(DocumentChange dc)
        {
            Insert insert = dc as Insert;
            if (insert == null) { return false; }

            int index = LoggedEvents.IndexOf(dc);
            if (index < 0) { return false; }

            if (index + 1 >= LoggedEvents.Count) { return false; }

            return (LoggedEvents[index + 1] is InsertStringCommand);
        }

        #endregion

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

                foreach (Event anEvent in LoggedEvents)
                {
                    anEvent.LogFilePath = logPath;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

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
    }
}
