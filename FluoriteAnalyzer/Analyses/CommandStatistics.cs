using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Xml.Serialization;
using FluoriteAnalyzer.Events;
using FluoriteAnalyzer.Forms;

namespace FluoriteAnalyzer.Analyses
{
    internal partial class CommandStatistics : UserControl, IRedrawable
    {
        private static readonly string CUSTOM_GROUPS_FILE_NAME = "CustomGroups.xml";

        public CommandStatistics(ILogProvider log)
        {
            InitializeComponent();

            LoadCustomGroups();

            LogProvider = log;
        }

        private ILogProvider LogProvider { get; set; }

        private List<CustomGroup> CustomGroups { get; set; }

        private string GetGroupForCommand(Command command)
        {
            string typeString = command.TypeOrCommandString;

            foreach (CustomGroup customGroup in CustomGroups)
            {
                foreach (string pattern in customGroup.Patterns)
                {
                    if (Regex.IsMatch(typeString, pattern))
                    {
                        return customGroup.Name;
                    }
                }
            }

            return typeString;
        }

        private void LoadCustomGroups()
        {
            var finfo = new FileInfo(CUSTOM_GROUPS_FILE_NAME);

            if (finfo.Exists && ((finfo.Attributes & FileAttributes.Directory) == 0))
            {
                var serializer = new XmlSerializer(typeof (List<CustomGroup>));
                using (var reader = new StreamReader(CUSTOM_GROUPS_FILE_NAME, Encoding.Default))
                {
                    CustomGroups = (List<CustomGroup>) (serializer.Deserialize(reader));
                }
            }

            if (CustomGroups == null || CustomGroups.Count == 0)
            {
                CustomGroups = new List<CustomGroup>();
                CustomGroups.Add(new CustomGroup("Ignored"));
            }

            listGroups.Items.Clear();
            listGroups.Items.AddRange(CustomGroups.Select(x => x.Name).ToArray());
        }

        public void SaveCustomGroups()
        {
            var serializer = new XmlSerializer(typeof (List<CustomGroup>));
            using (var writer = new StreamWriter(CUSTOM_GROUPS_FILE_NAME, false, Encoding.Default))
            {
                serializer.Serialize(writer, CustomGroups);
            }
        }

        private void listGroups_SelectedIndexChanged(object sender, EventArgs e)
        {
            buttonRemoveGroup.Enabled = listGroups.SelectedIndex > 0;

            listPatterns.Items.Clear();

            if (listGroups.SelectedIndex >= 0)
            {
                CustomGroup selectedGroup = CustomGroups[listGroups.SelectedIndex];
                listPatterns.Items.AddRange(selectedGroup.Patterns.ToArray());
            }

            buttonAddPattern.Enabled = buttonRemovePattern.Enabled = listGroups.SelectedIndex >= 0;
        }

        private void buttonAddGroup_Click(object sender, EventArgs e)
        {
            var inputForm = new InputStringForm();
            inputForm.Text = "Create New Custom Group";
            inputForm.Message = "Name of the custom group";

            DialogResult result = inputForm.ShowDialog();
            if (result == DialogResult.Cancel)
            {
                return;
            }

            var newGroup = new CustomGroup(inputForm.Value);
            CustomGroups.Add(newGroup);

            listGroups.Items.Add(newGroup);
            listGroups.SelectedIndex = listGroups.Items.Count - 1;

            Redraw();
        }

        private void buttonRemoveGroup_Click(object sender, EventArgs e)
        {
            int selectedIndex = listGroups.SelectedIndex;
            CustomGroups.RemoveAt(selectedIndex);

            listGroups.Items.RemoveAt(selectedIndex);
            if (listGroups.Items.Count > 0)
            {
                listGroups.SelectedIndex = Math.Min(selectedIndex, listGroups.Items.Count - 1);
            }

            Redraw();
        }

        private void listPatterns_SelectedIndexChanged(object sender, EventArgs e)
        {
            buttonRemovePattern.Enabled = listPatterns.SelectedIndices.Count > 0;
        }

        private void buttonAddPattern_Click(object sender, EventArgs e)
        {
            int selectedIndex = listGroups.SelectedIndex;
            if (selectedIndex < 0 || selectedIndex >= listGroups.Items.Count)
            {
                return;
            }

            CustomGroup group = CustomGroups[selectedIndex];

            var inputForm = new InputStringForm();
            inputForm.Text = "Add New Regex Pattern";
            inputForm.Message = "Regex Pattern";

            DialogResult result = inputForm.ShowDialog();
            if (result == DialogResult.Cancel)
            {
                return;
            }

            group.Patterns.Add(inputForm.Value);

            listPatterns.Items.Add(inputForm.Value);
            listPatterns.SelectedIndex = listPatterns.Items.Count - 1;

            Redraw();
        }

        private void buttonRemovePattern_Click(object sender, EventArgs e)
        {
            if (listPatterns.SelectedIndices.Count == 0)
            {
                return;
            }

            int selectedIndex = listGroups.SelectedIndex;
            if (selectedIndex < 0 || selectedIndex >= listGroups.Items.Count)
            {
                return;
            }

            CustomGroup group = CustomGroups[selectedIndex];

            foreach (int index in listPatterns.SelectedIndices.OfType<int>().OrderByDescending(x => x))
            {
                group.Patterns.RemoveAt(index);
                listPatterns.Items.RemoveAt(index);
            }

            Redraw();
        }

        #region IReloadable 멤버

        public void Redraw()
        {
            var usedColors = new HashSet<Color>();

            chartPie.Series[0].Points.Clear();

            var groups = LogProvider.LoggedEvents.OfType<Command>().GroupBy(x => GetGroupForCommand(x)).Where(
                x => x.Key != "Ignored")
                .Select(x => new {x.Key, Sum = x.Sum(y => y.RepeatCount)});
            foreach (var group in groups.OrderByDescending(x => x.Sum))
            {
                chartPie.Series[0].Points.AddXY(group.Key, group.Sum);
            }

            textBox1.Text = string.Join(Environment.NewLine, groups.OrderByDescending(x => x.Sum).Select(x => x.Key + "\t" + x.Sum));

            chartPie.ApplyPaletteColors();

            foreach (Series series in chartPie.Series)
            {
                foreach (DataPoint point in series.Points)
                {
                    point.Color = Color.FromArgb(200, point.Color);
                }
            }
        }

        #endregion

        private void listGroups_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = listGroups.IndexFromPoint(e.Location);
            if (index < 0 || index >= listGroups.Items.Count) { return; }

            var inputForm = new InputStringForm();
            inputForm.Text = "Modify Custom Group Name";
            inputForm.Message = "Name of the custom group";
            inputForm.Value = CustomGroups[index].Name;

            DialogResult result = inputForm.ShowDialog();
            if (result == DialogResult.Cancel)
            {
                return;
            }

            CustomGroups[index].Name = inputForm.Value;
            listGroups.SelectedIndex = index;

            Redraw();
        }

        private void listPatterns_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = listPatterns.IndexFromPoint(e.Location);
            if (index < 0 || index >= listPatterns.Items.Count) { return; }

            int selectedIndex = listGroups.SelectedIndex;
            if (selectedIndex < 0 || selectedIndex >= listGroups.Items.Count)
            {
                return;
            }

            CustomGroup group = CustomGroups[selectedIndex];

            var inputForm = new InputStringForm();
            inputForm.Text = "Modify Regex Pattern";
            inputForm.Message = "Regex Pattern";
            inputForm.Value = group.Patterns[index];

            DialogResult result = inputForm.ShowDialog();
            if (result == DialogResult.Cancel)
            {
                return;
            }

            group.Patterns[index] = inputForm.Value;
            listPatterns.SelectedIndex = index;

            Redraw();
        }
    }
}