using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FluoriteAnalyzer.Events;
using FluoriteAnalyzer.Utils;
using System.IO;

namespace FluoriteAnalyzer.Analyses
{
    internal partial class EventsList : UserControl, IRedrawable
    {
        private Dictionary<string, bool> _checkDict;

        public EventsList(ILogProvider logProvider)
        {
            InitializeComponent();

            LogProvider = logProvider;

            StrikeoutFont = new Font(richTextSourceCode.Font, FontStyle.Strikeout);

            SelectedEvent = null;


            SuppressNoInitialSnapshotWarning = false;
        }

        private ILogProvider LogProvider { get; set; }

        private List<Event> FilteredEvents { get; set; }

        private Font StrikeoutFont { get; set; }

        private ListViewItem SelectedListViewItem { get; set; }
        private Event SelectedEvent { get; set; }


        private bool SuppressNoInitialSnapshotWarning { get; set; }

        private void treeEvents_AfterCheck(object sender, TreeViewEventArgs e)
        {
            treeEvents.AfterCheck -= treeEvents_AfterCheck;

            if (e.Node.Nodes.Count == 0)
            {
                _checkDict[e.Node.Text] = e.Node.Checked;
            }
            else
            {
                var stack = new Stack<TreeNode>();
                foreach (TreeNode childNode in e.Node.Nodes)
                {
                    stack.Push(childNode);
                }

                bool allChecked = true;

                while (stack.Count > 0)
                {
                    TreeNode node = stack.Pop();
                    if (node.Checked == false)
                    {
                        allChecked = false;
                        break;
                    }

                    foreach (TreeNode childNode in node.Nodes)
                    {
                        stack.Push(childNode);
                    }
                }

                stack.Clear();
                stack.Push(e.Node);

                while (stack.Count > 0)
                {
                    TreeNode node = stack.Pop();
                    node.Checked = allChecked ^ true;
                    if (node.Level == 2 && _checkDict.ContainsKey(node.Text))
                    {
                        _checkDict[node.Text] = node.Checked;
                    }

                    foreach (TreeNode childNode in node.Nodes)
                    {
                        stack.Push(childNode);
                    }
                }
            }

            treeEvents.Refresh();

            RebuildFilteredEventsList();

            treeEvents.AfterCheck += treeEvents_AfterCheck;
        }

        public void RebuildFilteredEventsList()
        {
            // Before clearing the list view, remember the selected event's ID.
            int lastSelectedID = SelectedEvent != null ? SelectedEvent.ID : -1;

            // Clear the list view.
            listViewEvents.Items.Clear();

            FilteredEvents = LogProvider.LoggedEvents.Where(x => _checkDict[x.TypeOrCommandString]).ToList();

            foreach (Event anEvent in FilteredEvents)
            {
                var item = new ListViewItem(new[]
                                                {
                                                    anEvent.ID.ToString(),
                                                    anEvent.Timestamp.ToString(),
                                                    LogProvider.GetVideoTime(anEvent),
                                                    anEvent.EventType.ToString(),
                                                    anEvent.TypeString,
                                                    anEvent.ParameterStringPlain
                                                });

                item.Tag = anEvent;
                listViewEvents.Items.Add(item);
            }

            // Restore the selected item to be selected.
            SelectClosestEventByID(lastSelectedID);
        }

        public void lineChart_ChartDoubleClick(int timevalue)
        {
            SelectClosestEventByTimestamp(timevalue);
        }

        public void pattern_ItemDoubleClick(int startingID)
        {
            SelectClosestEventByID(startingID);
        }

        private void SelectClosestEventByTimestamp(int timevalue)
        {
            // Find the closest filtered event from the event list
            int index = FilteredEvents.BinarySearch(new DummyEvent(timevalue),
                                                    new ComparisonComparer<Event>(
                                                        (x, y) => (int)(x.Timestamp - y.Timestamp)));

            // If it's not found (negative index), just select the closest one.
            if (index < 0)
            {
                ++index;
                index *= -1;
            }

            SelectEventByItemIndex(index);
        }

        private void SelectClosestEventByID(int startingID)
        {
            // Find the closest filtered event from the event list
            int index = FilteredEvents.BinarySearch(new DummyEvent(0) { ID = startingID },
                                                    new ComparisonComparer<Event>(
                                                        (x, y) => (x.ID - y.ID)));

            // If it's not found (negative index), just select the closest one.
            if (index < 0)
            {
                ++index;
                index *= -1;
            }

            SelectEventByItemIndex(index);
        }

        private void SelectEventByItemIndex(int index)
        {
            // This method should do nothing when there is no item at all.
            if (listViewEvents.Items.Count == 0)
                return;

            index = Utils.Utils.Clamp(index, 0, listViewEvents.Items.Count - 1);

            listViewEvents.SelectedItems.Clear();
            listViewEvents.Items[index].Selected = true;
            listViewEvents.Items[index].Focused = true;

            listViewEvents.EnsureVisible(index);
            listViewEvents.Focus();
        }

        private void buttonShowHideCode_Click(object sender, EventArgs e)
        {
            // Toggle the source code panel visibility
            splitContainer1.Panel2Collapsed ^= true;
        }

        private void DrawSnapshot()
        {
            Utils.Utils.LockWindowUpdate(richTextSourceCode.Handle);
            ReproduceSnapshot();
            Utils.Utils.LockWindowUpdate((IntPtr) 0);
        }

        private void listViewEvents_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedListViewItem = null;
            SelectedEvent = null;
            if (listViewEvents.SelectedIndices.Count > 0)
            {
                SelectedListViewItem = listViewEvents.Items[listViewEvents.SelectedIndices[0]];
                SelectedEvent = SelectedListViewItem.Tag as Event;

                // Show parameters
                textParameters.Text = SelectedEvent.ParameterStringComplex;
            }
            else
            {
                // Remove parameters
                textParameters.Text = string.Empty;
            }

            DrawSnapshot();
        }

        private void ReproduceSnapshot()
        {
            textCurrentFileName.Text = "";
            richTextSourceCode.Text = "";

            if (listViewEvents.SelectedIndices.Count == 0)
            {
                // Nothing is selected. Just leave.
                return;
            }

            int selectedIndex = listViewEvents.SelectedIndices[0];
            ListViewItem item = listViewEvents.Items[selectedIndex];
            var selectedEvent = item.Tag as Event;
            if (selectedEvent == null)
            {
                // Tag is not properly set. Leave.
                return;
            }

            // Calculate from the beginning.
            var files = new Dictionary<string, StringBuilder>();
            IEnumerable<Event> docChangeEvents = LogProvider.LoggedEvents
                .TakeUntil(x => x == selectedEvent)
                .Where(x => ((x is FileOpenCommand && ((FileOpenCommand) x).FilePath != "null") || x is DocumentChange));

            string currentFile = null;
            DocumentChange lastDocChange = null;
            foreach (Event docChangeEvent in docChangeEvents)
            {
                if (docChangeEvent is FileOpenCommand)
                {
                    var fileOpenCommand = (FileOpenCommand) docChangeEvent;
                    currentFile = fileOpenCommand.FilePath;
                    lastDocChange = null;

                    if (!files.ContainsKey(currentFile))
                    {
                        // The first snapshot of this file is needed here.
                        // If we don't have, show an error message and leave.
                        if (fileOpenCommand.Snapshot == null)
                        {
                            if (!SuppressNoInitialSnapshotWarning)
                            {
                                MessageBox.Show(
                                    "This log file does not have initial snapshots. Source code cannot be reproduced!",
                                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                SuppressNoInitialSnapshotWarning = true;
                            }

                            files.Add(currentFile, null);
                        }
                        else
                        {
                            string snapshot = fileOpenCommand.Snapshot;
                            //snapshot = snapshot.Replace("\r\n", "\n");
                            files.Add(currentFile, new StringBuilder(snapshot));
                        }
                    }
                    else if (fileOpenCommand.Snapshot != null)
                    {
                        string snapshot = fileOpenCommand.Snapshot;
                        //snapshot = snapshot.Replace("\r\n", "\n");
                        files[currentFile] = new StringBuilder(snapshot);
                    }
                }
                else if (docChangeEvent is DocumentChange)
                {
                    if (currentFile == null)
                    {
                        MessageBox.Show(
                            "Invalid log file. No FileOpenCommand before a DocumentChange event",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    ApplyDocumentChange(files[currentFile], docChangeEvent);

                    lastDocChange = (DocumentChange) docChangeEvent;
                }
            }

            if (currentFile == null)
            {
                //MessageBox.Show(
                //    "This log file does not have any FileOpenCommand.", "Error", MessageBoxButtons.OK,
                //    MessageBoxIcon.Warning);
                return;
            }

            if (files[currentFile] == null)
            {
                return;
            }

            // Set the current file name
            textCurrentFileName.Text = Path.GetFileName(currentFile);

            int offset, insertionLength;
            string deletedText = GetOffsetAndLength(lastDocChange, out offset, out insertionLength);
            int deletionLength = deletedText.Length - deletedText.Count(x => x == '\r');

            string currentSourceCode = files[currentFile].ToString();
            currentSourceCode = currentSourceCode.Insert(offset, deletedText);

            // Set the text box content
            richTextSourceCode.Text = currentSourceCode;

            // Highlight the last inserted text
            if (lastDocChange != null)
            {
                // Stupid workaround due to the weird behavior of RichTextBox (it translates \r\n into \n automatically).
                offset -= files[currentFile].ToString().Substring(0, offset).Count(x => x == '\r');

                // Highlight deletedText
                richTextSourceCode.Select(offset, deletionLength);
                richTextSourceCode.SelectionFont = StrikeoutFont;
                richTextSourceCode.SelectionBackColor = Color.Pink;

                richTextSourceCode.Select(offset + deletionLength, insertionLength);
                richTextSourceCode.SelectionBackColor = Color.DarkSeaGreen;

                richTextSourceCode.Select(offset, 0);
                richTextSourceCode.ScrollToCaret();
            }
        }

        private void ApplyDocumentChange(StringBuilder builder, Event docChangeEvent)
        {
            if (builder == null)
            {
                return;
            }

            if (docChangeEvent is Delete)
            {
                var delete = (Delete) docChangeEvent;
                builder.Remove(delete.Offset, delete.Length);
            }
            else if (docChangeEvent is Insert)
            {
                var insert = (Insert) docChangeEvent;
                //builder.Insert(insert.Offset, insert.Text.Replace("\r\n", "\n"));
                builder.Insert(insert.Offset, insert.Text);
            }
            else if (docChangeEvent is Replace)
            {
                var replace = (Replace) docChangeEvent;
                builder.Remove(replace.Offset, replace.Length);
                //builder.Insert(replace.Offset, replace.InsertedText.Replace("\r\n", "\n"));
                builder.Insert(replace.Offset, replace.InsertedText);
            }
        }

        private string GetOffsetAndLength(DocumentChange lastDocChange, out int offset, out int length)
        {
            offset = 0;
            length = 0;

            if (lastDocChange is Delete)
            {
                var delete = (Delete) lastDocChange;
                offset = delete.Offset;
                length = 0;
                return delete.Text.Replace("\r\n", "\n");
            }
            else if (lastDocChange is Insert)
            {
                var insert = (Insert) lastDocChange;
                offset = insert.Offset;
                length = insert.Length - insert.Text.Count(x => x == '\r');
            }
            else if (lastDocChange is Replace)
            {
                var replace = (Replace) lastDocChange;
                offset = replace.Offset;
                length = replace.InsertedText.Length - replace.InsertedText.Count(x => x == '\r');
                return replace.DeletedText.Replace("\r\n", "\n");
            }

            return "";
        }

        internal void SearchString(string str)
        {
            Event selectedEvent = SelectedEvent;

            IEnumerable<Event> events = FilteredEvents;
            if (selectedEvent != null)
            {
                events = FilteredEvents.SkipUntil(x => x == SelectedEvent);
            }

            // TODO: Open a search result window
            Event eventFound = events.Where(x => x.ContainsString(str)).FirstOrDefault();
            if (eventFound != null)
            {
                // Deselect the previously selected one
                if (SelectedListViewItem != null)
                {
                    SelectedListViewItem.Selected = false;
                }

                // Select the item which was found
                ListViewItem itemFound = listViewEvents.Items.Cast<ListViewItem>().Where(x => x.Tag == eventFound).First();
                itemFound.Selected = true;
                listViewEvents.EnsureVisible(itemFound.Index);

                Focus();
            }
            else
            {
                MessageBox.Show("The specified string was not found.", "Search", MessageBoxButtons.OK);
            }
        }

        #region IRedrawable 멤버

        public void Redraw()
        {
            treeEvents.AfterCheck -= treeEvents_AfterCheck;

            treeEvents.Nodes.Clear();

            TreeNode root = treeEvents.Nodes.Add("All Events");
            TreeNode annotation = root.Nodes.Add("Annotation");
            TreeNode documentChange = root.Nodes.Add("DocumentChange");
            TreeNode command = root.Nodes.Add("Command");

            annotation.Nodes.AddRange(
                LogProvider.LoggedEvents.OfType<Annotation>()
                    .Select(x => x.TypeOrCommandString)
                    .Distinct()
                    .OrderBy(x => x)
                    .Select(x => new TreeNode(x))
                    .ToArray());

            documentChange.Nodes.AddRange(
                LogProvider.LoggedEvents.OfType<DocumentChange>()
                    .Select(x => x.TypeOrCommandString)
                    .Distinct()
                    .OrderBy(x => x)
                    .Select(x => new TreeNode(x))
                    .ToArray());

            command.Nodes.AddRange(
                LogProvider.LoggedEvents.OfType<Command>()
                    .Select(x => x.TypeOrCommandString)
                    .Distinct()
                    .OrderBy(x => x)
                    .Select(x => new TreeNode(x))
                    .ToArray());

            // Initially check everything
            _checkDict = new Dictionary<string, bool>();
            treeEvents.ExpandAll();

            foreach (TreeNode node1 in treeEvents.Nodes)
            {
                node1.Checked = true;
                foreach (TreeNode node2 in node1.Nodes)
                {
                    node2.Checked = true;
                    foreach (TreeNode node3 in node2.Nodes)
                    {
                        node3.Checked = true;
                        _checkDict.Add(node3.Text, true);
                    }
                }
            }

            treeEvents.AfterCheck += treeEvents_AfterCheck;
            RebuildFilteredEventsList();
        }

        #endregion
    }
}