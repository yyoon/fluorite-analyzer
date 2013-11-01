namespace FluoriteAnalyzer.Analyses
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Forms;
    using FluoriteAnalyzer.Commons;
    using FluoriteAnalyzer.Events;
    using FluoriteAnalyzer.Forms;
    using FluoriteAnalyzer.Utils;

    /// <summary>
    /// Events list control shown to the user.
    /// </summary>
    internal partial class EventsList : UserControl, IRedrawable
    {
        /// <summary>
        /// Keeps which type of commands are checked in the tree view.
        /// </summary>
        private Dictionary<string, bool> checkDict;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventsList"/> class.
        /// </summary>
        /// <param name="logProvider">The log provider.</param>
        public EventsList(ILogProvider logProvider)
        {
            this.InitializeComponent();

            this.LogProvider = logProvider;

            this.SelectedEvent = null;

            this.SnapshotCalculator = new SnapshotCalculator(logProvider);
        }

        /// <summary>
        /// Gets or sets the log provider.
        /// </summary>
        /// <value>
        /// The log provider.
        /// </value>
        private ILogProvider LogProvider { get; set; }

        /// <summary>
        /// Gets or sets the filtered events.
        /// </summary>
        /// <value>
        /// The filtered events.
        /// </value>
        private List<Event> FilteredEvents { get; set; }

        /// <summary>
        /// Gets or sets the selected list view item.
        /// </summary>
        /// <value>
        /// The selected list view item.
        /// </value>
        private ListViewItem SelectedListViewItem { get; set; }

        /// <summary>
        /// Gets or sets the selected event.
        /// </summary>
        /// <value>
        /// The selected event.
        /// </value>
        private Event SelectedEvent { get; set; }

        /// <summary>
        /// Gets or sets the snapshot calculator.
        /// </summary>
        /// <value>
        /// The snapshot calculator.
        /// </value>
        private SnapshotCalculator SnapshotCalculator { get; set; }

        /// <summary>
        /// Rebuilds the filtered events list.
        /// </summary>
        public void RebuildFilteredEventsList()
        {
            // Before clearing the list view, remember the selected event's ID.
            int lastSelectedID = this.SelectedEvent != null ? this.SelectedEvent.ID : -1;

            // Clear the list view.
            this.listViewEvents.Items.Clear();

            this.FilteredEvents = this.LogProvider.LoggedEvents
                .Where(x => this.checkDict[x.TypeOrCommandString]).ToList();

            foreach (Event anEvent in this.FilteredEvents)
            {
                var item = new ListViewItem(new[]
                                                {
                                                    anEvent.ID.ToString(),
                                                    anEvent.Timestamp.ToString(),
                                                    this.LogProvider.GetVideoTime(anEvent),
                                                    anEvent.EventType.ToString(),
                                                    anEvent.TypeString,
                                                    anEvent.ParameterStringPlain
                                                });

                item.Tag = anEvent;
                this.listViewEvents.Items.Add(item);
            }

            // Number of Items.
            this.labelNumItems.Text = this.listViewEvents.Items.Count.ToString();

            // Restore the selected item to be selected.
            this.SelectClosestEventByID(lastSelectedID);
        }

        /// <summary>
        /// Called when the line chart is double-clicked.
        /// </summary>
        /// <param name="timevalue">The time value.</param>
        public void LineChart_ChartDoubleClick(int timevalue)
        {
            this.SelectClosestEventByTimestamp(timevalue);
        }

        /// <summary>
        /// Called when a pattern is double-clicked.
        /// </summary>
        /// <param name="startingID">The starting ID.</param>
        public void Pattern_ItemDoubleClick(int startingID)
        {
            this.SelectClosestEventByID(startingID);
        }

        #region IRedrawable 멤버

        /// <summary>
        /// Redraws this control.
        /// </summary>
        void IRedrawable.Redraw()
        {
            this.treeEvents.AfterCheck -= this.TreeEvents_AfterCheck;

            this.treeEvents.Nodes.Clear();

            TreeNode root = this.treeEvents.Nodes.Add("All Events");
            TreeNode annotation = root.Nodes.Add("Annotation");
            TreeNode documentChange = root.Nodes.Add("DocumentChange");
            TreeNode command = root.Nodes.Add("Command");

            annotation.Nodes.AddRange(
                this.LogProvider.LoggedEvents.OfType<Annotation>()
                    .Select(x => x.TypeOrCommandString)
                    .Distinct()
                    .OrderBy(x => x)
                    .Select(x => new TreeNode(x))
                    .ToArray());

            documentChange.Nodes.AddRange(
                this.LogProvider.LoggedEvents.OfType<DocumentChange>()
                    .Select(x => x.TypeOrCommandString)
                    .Distinct()
                    .OrderBy(x => x)
                    .Select(x => new TreeNode(x))
                    .ToArray());

            command.Nodes.AddRange(
                this.LogProvider.LoggedEvents.OfType<Command>()
                    .Select(x => x.TypeOrCommandString)
                    .Distinct()
                    .OrderBy(x => x)
                    .Select(x => new TreeNode(x))
                    .ToArray());

            // Initially check everything
            this.checkDict = new Dictionary<string, bool>();
            this.treeEvents.ExpandAll();

            foreach (TreeNode node1 in this.treeEvents.Nodes)
            {
                node1.Checked = true;
                foreach (TreeNode node2 in node1.Nodes)
                {
                    node2.Checked = true;
                    foreach (TreeNode node3 in node2.Nodes)
                    {
                        node3.Checked = true;
                        this.checkDict.Add(node3.Text, true);
                    }
                }
            }

            this.treeEvents.AfterCheck += this.TreeEvents_AfterCheck;
            this.RebuildFilteredEventsList();
        }

        #endregion

        /// <summary>
        /// Searches the given string from the event list.
        /// </summary>
        /// <param name="str">The string.</param>
        internal void SearchString(string str)
        {
            Event selectedEvent = this.SelectedEvent;

            IEnumerable<Event> events = this.FilteredEvents;
            if (selectedEvent != null)
            {
                events = this.FilteredEvents.SkipUntil(x => x == this.SelectedEvent);
            }

            // TODO: Open a search result window
            Event eventFound = events.Where(x => x.ContainsString(str)).FirstOrDefault();
            if (eventFound != null)
            {
                // Deselect the previously selected one
                if (this.SelectedListViewItem != null)
                {
                    this.SelectedListViewItem.Selected = false;
                }

                // Select the item which was found
                ListViewItem itemFound = this.listViewEvents.Items
                    .Cast<ListViewItem>()
                    .Where(x => x.Tag == eventFound).First();
                itemFound.Selected = true;
                this.listViewEvents.EnsureVisible(itemFound.Index);

                this.Focus();
            }
            else
            {
                MessageBox.Show(
                    "The specified string was not found.",
                    "Search",
                    MessageBoxButtons.OK);
            }
        }

        /// <summary>
        /// Handles the AfterCheck event of the treeEvents control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">
        /// The <see cref="TreeViewEventArgs"/> instance containing the event data.
        /// </param>
        private void TreeEvents_AfterCheck(object sender, TreeViewEventArgs e)
        {
            this.treeEvents.AfterCheck -= this.TreeEvents_AfterCheck;

            if (e.Node.Nodes.Count == 0)
            {
                this.checkDict[e.Node.Text] = e.Node.Checked;
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
                    if (node.Level == 2 && this.checkDict.ContainsKey(node.Text))
                    {
                        this.checkDict[node.Text] = node.Checked;
                    }

                    foreach (TreeNode childNode in node.Nodes)
                    {
                        stack.Push(childNode);
                    }
                }
            }

            this.treeEvents.Refresh();

            this.RebuildFilteredEventsList();

            this.treeEvents.AfterCheck += this.TreeEvents_AfterCheck;
        }

        /// <summary>
        /// Selects the closest event by timestamp.
        /// </summary>
        /// <param name="timevalue">The time value.</param>
        private void SelectClosestEventByTimestamp(int timevalue)
        {
            // Find the closest filtered event from the event list
            int index = this.FilteredEvents.BinarySearch(
                new DummyEvent(timevalue),
                new ComparisonComparer<Event>(
                    (x, y) => (int)(x.Timestamp - y.Timestamp)));

            // If it's not found (negative index), just select the closest one.
            if (index < 0)
            {
                ++index;
                index *= -1;
            }

            this.SelectEventByItemIndex(index);
        }

        /// <summary>
        /// Selects the closest event by ID.
        /// </summary>
        /// <param name="startingID">The starting ID.</param>
        private void SelectClosestEventByID(int startingID)
        {
            // Find the closest filtered event from the event list
            int index = this.FilteredEvents.BinarySearch(
                new DummyEvent(0) { ID = startingID },
                new ComparisonComparer<Event>(
                    (x, y) => (x.ID - y.ID)));

            // If it's not found (negative index), just select the closest one.
            if (index < 0)
            {
                ++index;
                index *= -1;
            }

            this.SelectEventByItemIndex(index);
        }

        /// <summary>
        /// Selects the event by index.
        /// </summary>
        /// <param name="index">The index.</param>
        private void SelectEventByItemIndex(int index)
        {
            // This method should do nothing when there is no item at all.
            if (this.listViewEvents.Items.Count == 0)
            {
                return;
            }

            index = Utils.Clamp(index, 0, this.listViewEvents.Items.Count - 1);

            this.listViewEvents.SelectedItems.Clear();
            this.listViewEvents.Items[index].Selected = true;
            this.listViewEvents.Items[index].Focused = true;

            this.listViewEvents.EnsureVisible(index);
            this.listViewEvents.Focus();
        }

        /// <summary>
        /// Handles the Click event of the buttonShowHideCode control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void ButtonShowHideCode_Click(object sender, EventArgs e)
        {
            // Toggle the source code panel visibility
            this.splitContainer1.Panel2Collapsed ^= true;
        }

        /// <summary>
        /// Draws the snapshot. Locks the UI before and after reproducing the snapshot.
        /// </summary>
        private void DrawSnapshot()
        {
            Utils.LockWindowUpdate(this.Handle);
            this.ReproduceSnapshot();
            Utils.LockWindowUpdate((IntPtr)0);
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the listViewEvents control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void ListViewEvents_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.SelectedListViewItem = null;
            this.SelectedEvent = null;
            if (this.listViewEvents.SelectedIndices.Count > 0)
            {
                this.SelectedListViewItem = this.listViewEvents
                    .Items[this.listViewEvents.SelectedIndices[0]];
                this.SelectedEvent = this.SelectedListViewItem.Tag as Event;

                // Show parameters
                this.textParameters.Text = this.SelectedEvent.ParameterStringComplex;
            }
            else
            {
                // Remove parameters
                this.textParameters.Text = string.Empty;
            }

            this.DrawSnapshot();
        }

        /// <summary>
        /// Reproduces the snapshot.
        /// </summary>
        private void ReproduceSnapshot()
        {
            this.snapshotPreview.Clear();

            if (this.listViewEvents.SelectedIndices.Count == 0)
            {
                return;
            }

            int selectedIndex = this.listViewEvents.SelectedIndices[0];
            ListViewItem item = this.listViewEvents.Items[selectedIndex];
            var selectedEvent = item.Tag as Event;
            if (selectedEvent == null)
            {
                return;
            }

            // Do the calculation.
            EntireSnapshot entireSnapshot =
                this.SnapshotCalculator.CalculateSnapshotAtEvent(selectedEvent);

            this.snapshotPreview.SetSnapshot(entireSnapshot);
        }

        /// <summary>
        /// Handles the Click event of the buttonGoto control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void ButtonGoto_Click(object sender, EventArgs e)
        {
            InputStringForm inputForm = new InputStringForm();
            inputForm.Message = "Input ID:";
            DialogResult result = inputForm.ShowDialog(this);

            if (result != DialogResult.OK)
            {
                return;
            }

            int id;
            if (int.TryParse(inputForm.Value, out id))
            {
                this.SelectClosestEventByID(id);
            }
            else
            {
                MessageBox.Show(
                    "Invalid ID",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}