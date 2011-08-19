using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using FluoriteAnalyzer.Events;
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
        }

        private ILogProvider LogProvider { get; set; }

        private List<Event> FilteredEvents { get; set; }

        private void treeEvents_AfterCheck(object sender, TreeViewEventArgs e)
        {
            treeEvents.AfterCheck -= treeEvents_AfterCheck;

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

            treeEvents.Refresh();

            RebuildFilteredEventsList();

            treeEvents.AfterCheck += treeEvents_AfterCheck;
        }

        public void RebuildFilteredEventsList()
        {
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
                                                    anEvent.ParameterString
                                                });

                listViewEvents.Items.Add(item);
            }
        }

        public void lineChart_ChartDoubleClick(int timevalue)
        {
            // Find the closest filtered event from the event list
            int index = FilteredEvents.BinarySearch(new DummyEvent(timevalue),
                                                    new ComparisonComparer<Event>(
                                                        (x, y) => (int) (x.Timestamp - y.Timestamp)));
            if (index < 0)
            {
                ++index;
                index *= -1;
            }

            index = Utils.Utils.Clamp(index, 0, listViewEvents.Items.Count - 1);

            listViewEvents.SelectedItems.Clear();
            listViewEvents.Items[index].Selected = true;

            listViewEvents.EnsureVisible(index);
            listViewEvents.Focus();
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