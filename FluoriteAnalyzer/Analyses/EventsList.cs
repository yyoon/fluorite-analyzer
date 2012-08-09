using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FluoriteAnalyzer.Common;
using FluoriteAnalyzer.Events;
using FluoriteAnalyzer.Forms;
using FluoriteAnalyzer.Utils;

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

            SnapshotCalculator snapshotCalculator = new SnapshotCalculator(LogProvider);
            EntireSnapshot entireSnapshot = snapshotCalculator.CalculateSnapshotAtEvent(selectedEvent);

            if (entireSnapshot.CurrentFile == null) { return; }
            FileSnapshot curSnapshot = entireSnapshot.FileSnapshots[entireSnapshot.CurrentFile];

            if (curSnapshot.Content == null) { return; }

            // Set the current file name
            textCurrentFileName.Text = Path.GetFileName(entireSnapshot.CurrentFile);

            int insertionOffset, insertionLength, deletionOffset;
            string deletedText = GetOffsetAndLength(curSnapshot.LastChange, out insertionOffset, out insertionLength, out deletionOffset);
            int deletionLength = deletedText.Length;

            string currentSourceCode = curSnapshot.Content;
            currentSourceCode = currentSourceCode.Insert(deletionOffset, deletedText);

            // Set the text box content
            richTextSourceCode.Text = currentSourceCode;

            // Highlight the last change
            HighlightLastChange(curSnapshot.Content, curSnapshot.LastChange, insertionOffset, insertionLength, deletionOffset, deletionLength);
        }

        private void HighlightLastChange(string content, DocumentChange lastDocChange, int insertionOffset, int insertionLength, int deletionOffset, int deletionLength)
        {
            if (lastDocChange != null)
            {
                // Stupid workaround due to the weird behavior of RichTextBox (it translates \r\n into \n automatically).
                insertionOffset -= content.Substring(0, insertionOffset).Count(x => x == '\r');
                deletionOffset -= content.Substring(0, deletionOffset).Count(x => x == '\r');

                richTextSourceCode.Select(deletionOffset, deletionLength);
                richTextSourceCode.SelectionFont = StrikeoutFont;
                richTextSourceCode.SelectionBackColor = Color.Pink;

                richTextSourceCode.Select(insertionOffset, insertionLength);
                richTextSourceCode.SelectionBackColor = Color.LightGreen;

                if (insertionLength == 0)
                {
                    richTextSourceCode.Select(deletionOffset, 0);
                }
                else
                {
                    richTextSourceCode.Select(insertionOffset, 0);
                }
                richTextSourceCode.ScrollToCaret();
            }
        }

        private string GetOffsetAndLength(DocumentChange lastDocChange, out int insertionOffset, out int insertionLength, out int deletionOffset)
        {
            insertionOffset = 0;
            insertionLength = 0;
            deletionOffset = 0;

            if (lastDocChange is Delete)
            {
                var delete = (Delete) lastDocChange;
                deletionOffset = delete.Offset;
                insertionLength = 0;
                return delete.Text.Replace("\r\n", "\n");
            }
            else if (lastDocChange is Insert)
            {
                var insert = (Insert) lastDocChange;
                insertionOffset = insert.Offset;
                insertionLength = insert.Length - insert.Text.Count(x => x == '\r');
            }
            else if (lastDocChange is Replace)
            {
                var replace = (Replace) lastDocChange;
                string deletedText = replace.DeletedText.Replace("\r\n", "\n");
                insertionOffset = replace.Offset + deletedText.Length;
                insertionLength = replace.InsertedText.Length - replace.InsertedText.Count(x => x == '\r');
                deletionOffset = replace.Offset;
                return deletedText;
            }
            else if (lastDocChange is Move)
            {
                // only consider the current file
                var move = (Move) lastDocChange;
                insertionOffset = move.InsertionOffset;
                insertionLength = move.InsertionLength - move.InsertedText.Count(x => x == '\r');

                //return "";

                if (move.DeletedFrom == move.InsertedTo)
                {
                    deletionOffset = move.Offset;
                    string deletedText = move.DeletedText.Replace("\r\n", "\n");

                    if (deletionOffset > insertionOffset)
                    {
                        deletionOffset += insertionLength;
                    }
                    else
                    {
                        insertionOffset += deletedText.Length;
                    }

                    return deletedText;
                }
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

        private void buttonGoto_Click(object sender, EventArgs e)
        {
            InputStringForm inputForm = new InputStringForm();
            inputForm.Message = "Input ID:";
            DialogResult result = inputForm.ShowDialog(this);

            if (result != DialogResult.OK) { return; }

            int id;
            if (int.TryParse(inputForm.Value, out id))
            {
                SelectClosestEventByID(id);
            }
            else
            {
                MessageBox.Show("Invalid ID", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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