using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using FluoriteAnalyzer.Events;
using System;
using System.IO;
using System.Xml.Serialization;
using System.Text;
using FluoriteAnalyzer.Forms;
using System.Text.RegularExpressions;

namespace FluoriteAnalyzer.Analyses
{
    internal partial class KeyStrokes : UserControl, IRedrawable
    {
        private Dictionary<string, int> keyCount;

        private static readonly string CUSTOM_GROUPS_FILE_NAME = "CustomGroups_Keystrokes.xml";

        public KeyStrokes(ILogProvider logProvider)
        {
            InitializeComponent();

            LoadCustomGroups();

            LogProvider = logProvider;
        }

        private ILogProvider LogProvider { get; set; }

        private List<CustomGroup> CustomGroups { get; set; }

        private string GetGroupForKey(string key)
        {
            foreach (CustomGroup customGroup in CustomGroups)
            {
                foreach (string pattern in customGroup.Patterns)
                {
                    if (Regex.IsMatch(key, pattern))
                    {
                        return customGroup.Name;
                    }
                }
            }

            return key;
        }

        private void LoadCustomGroups()
        {
            var finfo = new FileInfo(CUSTOM_GROUPS_FILE_NAME);

            if (finfo.Exists && ((finfo.Attributes & FileAttributes.Directory) == 0))
            {
                var serializer = new XmlSerializer(typeof(List<CustomGroup>));
                using (var reader = new StreamReader(CUSTOM_GROUPS_FILE_NAME, Encoding.Default))
                {
                    CustomGroups = (List<CustomGroup>)(serializer.Deserialize(reader));
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
            var serializer = new XmlSerializer(typeof(List<CustomGroup>));
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

        private void CountKeyStrokes()
        {
            CountFromInsertStringCommand();
            CountFromEditCommands();
            //CountFromOtherCommands(commands);
        }

        private void CountFromInsertStringCommand()
        {
            bool prevKeyUpper = false;

            // Count key strokes from InsertString command.
            foreach (InsertStringCommand insertStringCommand in LogProvider.LoggedEvents.OfType<InsertStringCommand>())
            {
                foreach (char ch in insertStringCommand.Data)
                {
                    if (char.IsUpper(ch))
                    {
                        if (prevKeyUpper == false)
                        {
                            AddKeyCount("Shift");
                        }

                        AddKeyCount(ch);
                    }
                    else if (char.IsLower(ch))
                    {
                        AddKeyCount(char.ToUpper(ch));
                    }
                    else
                    {
                        switch (ch)
                        {
                            case '\n':
                                AddKeyCount("Enter");
                                break;
                            case '\r':
                                AddKeyCount("Enter");
                                break;
                            case '\t':
                                AddKeyCount("Tab");
                                break;
                            case ' ':
                                AddKeyCount("Space");
                                break;

                            case '~':
                                AddKeyCount('`', true);
                                break;
                            case '!':
                                AddKeyCount('1', true);
                                break;
                            case '@':
                                AddKeyCount('2', true);
                                break;
                            case '#':
                                AddKeyCount('3', true);
                                break;
                            case '$':
                                AddKeyCount('4', true);
                                break;
                            case '%':
                                AddKeyCount('5', true);
                                break;
                            case '^':
                                AddKeyCount('6', true);
                                break;
                            case '&':
                                AddKeyCount('7', true);
                                break;
                            case '*':
                                AddKeyCount('8', true);
                                break;
                            case '(':
                                AddKeyCount('9', true);
                                break;
                            case ')':
                                AddKeyCount('0', true);
                                break;
                            case '_':
                                AddKeyCount('-', true);
                                break;
                            case '+':
                                AddKeyCount('=', true);
                                break;

                            case '{':
                                AddKeyCount('[', true);
                                break;
                            case '}':
                                AddKeyCount(']', true);
                                break;
                            case '|':
                                AddKeyCount('\\', true);
                                break;
                            case ':':
                                AddKeyCount(';', true);
                                break;
                            case '\"':
                                AddKeyCount('\'', true);
                                break;
                            case '<':
                                AddKeyCount(',', true);
                                break;
                            case '>':
                                AddKeyCount('.', true);
                                break;
                            case '?':
                                AddKeyCount('/', true);
                                break;

                            default:
                                AddKeyCount(ch);
                                break;
                        }
                    }

                    prevKeyUpper = char.IsUpper(ch);
                }
            }
        }

        private void CountFromEditCommands()
        {
            IEnumerable<Command> commands = LogProvider.LoggedEvents.OfType<Command>();

            CountSpecificCommand(commands, null, new[] {"Backspace"}, "eventLogger.styledTextCommand.DELETE_PREVIOUS");
            CountSpecificCommand(commands, null, new[] {"Delete"}, "org.eclipse.ui.edit.delete");

            CountSpecificCommand(commands, new[] {"Ctrl"}, new[] {"Backspace"},
                                 "org.eclipse.ui.edit.text.deletePreviousWord");

            // Arrows
            CountSpecificCommand(commands, null, new[] {"↓"}, "eventLogger.styledTextCommand.LINE_DOWN");
            CountSpecificCommand(commands, null, new[] {"↑"}, "eventLogger.styledTextCommand.LINE_UP");
            CountSpecificCommand(commands, null, new[] {"←"}, "eventLogger.styledTextCommand.COLUMN_PREVIOUS");
            CountSpecificCommand(commands, null, new[] {"→"}, "eventLogger.styledTextCommand.COLUMN_NEXT");

            CountSpecificCommand(commands, new[] {"Shift", "Ctrl"}, new[] {"→"},
                                 "org.eclipse.ui.edit.text.select.wordNext");
            CountSpecificCommand(commands, new[] {"Shift", "Ctrl"}, new[] {"←"},
                                 "org.eclipse.ui.edit.text.select.wordPrevious");

            CountSpecificCommand(commands, new[] {"Ctrl"}, new[] {"→"}, "org.eclipse.ui.edit.text.goto.wordNext");
            CountSpecificCommand(commands, new[] {"Ctrl"}, new[] {"←"}, "org.eclipse.ui.edit.text.goto.wordPrevious");

            CountSpecificCommand(commands, new[] {"Alt"}, new[] {"↑"}, "org.eclipse.ui.edit.text.moveLineUp");
            CountSpecificCommand(commands, new[] {"Alt"}, new[] {"↓"}, "org.eclipse.ui.edit.text.moveLineDown");

            // Home / End
            CountSpecificCommand(commands, null, new[] {"Home"}, "org.eclipse.ui.edit.text.goto.lineStart");
            CountSpecificCommand(commands, null, new[] {"End"}, "org.eclipse.ui.edit.text.goto.lineEnd");
            CountSpecificCommand(commands, new[] {"Shift"}, new[] {"Home"}, "org.eclipse.ui.edit.text.select.lineStart");
            CountSpecificCommand(commands, new[] {"Shift"}, new[] {"End"}, "org.eclipse.ui.edit.text.select.lineEnd");
            CountSpecificCommand(commands, new[] {"Ctrl"}, new[] {"Home"}, "org.eclipse.ui.edit.text.goto.textStart");

            // Page Up / Page Down
            CountSpecificCommand(commands, null, new[] {"Page Up"}, "eventLogger.styledTextCommand.PAGE_UP");
            CountSpecificCommand(commands, null, new[] {"Page Down"}, "eventLogger.styledTextCommand.PAGE_DOWN");
            CountSpecificCommand(commands, new[] {"Shift", "Ctrl"}, new[] {"Page Up"},
                                 "eventLogger.styledTextCommand.SELECT_PAGE_UP");
            CountSpecificCommand(commands, new[] {"Shift", "Ctrl"}, new[] {"Page Down"},
                                 "eventLogger.styledTextCommand.SELECT_PAGE_DOWN");
        }

        private void CountFromOtherCommands(IEnumerable<Command> commands)
        {
            // Undo / Redo (assuming they are all invoked by keyboard)
            CountSpecificCommand(commands, new[] { "Ctrl" }, new[] { "Z" }, "UndoCommand");
            CountSpecificCommand(commands, new[] { "Ctrl" }, new[] { "Y" }, "RedoCommand");

            // Cut / Copy / Paste
            CountSpecificCommand(commands, new[] { "Ctrl" }, new[] { "X" }, "CutCommand");
            CountSpecificCommand(commands, new[] { "Ctrl" }, new[] { "C" }, "CopyCommand");
            CountSpecificCommand(commands, new[] { "Ctrl" }, new[] { "V" }, "PasteCommand");

            // Select All
            CountSpecificCommand(commands, new[] { "Ctrl" }, new[] { "A" }, "org.eclipse.ui.edit.selectAll");

            // Goto matching bracket
            CountSpecificCommand(commands, new[] { "Shift", "Ctrl" }, new[] { "P" },
                                 "org.eclipse.jdt.ui.edit.text.java.goto.matching.bracket");

            // File save
            CountSpecificCommand(commands, new[] { "Ctrl" }, new[] { "S" }, "org.eclipse.ui.file.save");

            // Delete Line
            CountSpecificCommand(commands, new[] { "Ctrl" }, new[] { "D" }, "org.eclipse.ui.edit.text.delete.line");

            // Add block comment
            CountSpecificCommand(commands, new[] { "Shift", "Ctrl" }, new[] { "/" },
                                 "org.eclipse.jdt.ui.edit.text.java.add.block.comment");

            // Correct Indent
            CountSpecificCommand(commands, new[] { "Ctrl" }, new[] { "I" }, "org.eclipse.jdt.ui.edit.text.java.indent");

            // Format
            CountSpecificCommand(commands, new[] { "Shift", "Ctrl" }, new[] { "F" },
                                 "org.eclipse.jdt.ui.edit.text.java.format");
        }

        private void CountSpecificCommand(IEnumerable<Command> eclipseCommands, string[] specialKeys,
                                          string[] keyStrings, string commandID)
        {
            IEnumerable<Command> commands = eclipseCommands.Where(x => x.TypeOrCommandString == commandID);

            if (specialKeys != null)
            {
                int count = commands.Count();
                foreach (string specialKey in specialKeys)
                {
                    AddKeyCount(specialKey, count);
                }
            }

            if (keyStrings != null)
            {
                int count = commands.Select(x => x.RepeatCount).Sum();
                foreach (string keyString in keyStrings)
                {
                    AddKeyCount(keyString, count);
                }
            }
        }

        private void AddKeyCount(char ch, bool shift)
        {
            if (shift)
            {
                AddKeyCount("Shift");
            }

            AddKeyCount(ch);
        }

        private void AddKeyCount(char ch)
        {
            AddKeyCount(ch, 1);
        }

        private void AddKeyCount(string keyString)
        {
            AddKeyCount(keyString, 1);
        }

        private void AddKeyCount(char ch, int count)
        {
            AddKeyCount(ch.ToString(), count);
        }

        private void AddKeyCount(string keyString, int count)
        {
            if (keyCount == null)
            {
                return;
            }

            if (keyCount.ContainsKey(keyString))
            {
                keyCount[keyString] += count;
            }
            else
            {
                keyCount.Add(keyString, count);
            }
        }

        #region IRedrawable 멤버

        public void Redraw()
        {
            keyCount = new Dictionary<string, int>();

            CountKeyStrokes();

            chartKeyStrokes.Series[0].Points.Clear();

            var groups = keyCount.GroupBy(x => GetGroupForKey(x.Key))
                .Where(x => x.Key != "Ignored")
                .Select(x => new { x.Key, Sum = x.Select(y => y.Value).Sum() });
            foreach (var group in groups.OrderByDescending(x => x.Sum))
            {
                chartKeyStrokes.Series[0].Points.AddXY(group.Key, group.Sum);
            }

            textBox1.Text = string.Join(Environment.NewLine, groups.OrderByDescending(x => x.Sum).Select(x => x.Key + "\t" + x.Sum));

            chartKeyStrokes.ApplyPaletteColors();

            foreach (Series series in chartKeyStrokes.Series)
            {
                foreach (DataPoint point in series.Points)
                {
                    point.Color = Color.FromArgb(200, point.Color);
                }
            }
        }

        #endregion
    }
}