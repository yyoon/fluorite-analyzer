using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using FluoriteAnalyzer.Events;

namespace FluoriteAnalyzer.Analyses
{
    internal partial class KeyStrokes : UserControl, IRedrawable
    {
        private Dictionary<string, int> keyCount;

        public KeyStrokes(ILogProvider logProvider)
        {
            InitializeComponent();

            LogProvider = logProvider;
        }

        private ILogProvider LogProvider { get; set; }

        private void CountKeyStrokes()
        {
            CountFromInsertStringCommand();
            CountFromOtherCommands();
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

        private void CountFromOtherCommands()
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

            // Undo / Redo (assuming they are all invoked by keyboard)
            CountSpecificCommand(commands, new[] {"Ctrl"}, new[] {"Z"}, "UndoCommand");
            CountSpecificCommand(commands, new[] {"Ctrl"}, new[] {"Y"}, "RedoCommand");

            // Cut / Copy / Paste
            CountSpecificCommand(commands, new[] {"Ctrl"}, new[] {"X"}, "CutCommand");
            CountSpecificCommand(commands, new[] {"Ctrl"}, new[] {"C"}, "CopyCommand");
            CountSpecificCommand(commands, new[] {"Ctrl"}, new[] {"V"}, "PasteCommand");

            // Select All
            CountSpecificCommand(commands, new[] {"Ctrl"}, new[] {"A"}, "org.eclipse.ui.edit.selectAll");

            // Goto matching bracket
            CountSpecificCommand(commands, new[] {"Shift", "Ctrl"}, new[] {"P"},
                                 "org.eclipse.jdt.ui.edit.text.java.goto.matching.bracket");

            // File save
            CountSpecificCommand(commands, new[] {"Ctrl"}, new[] {"S"}, "org.eclipse.ui.file.save");

            // Delete Line
            CountSpecificCommand(commands, new[] {"Ctrl"}, new[] {"D"}, "org.eclipse.ui.edit.text.delete.line");

            // Add block comment
            CountSpecificCommand(commands, new[] {"Shift", "Ctrl"}, new[] {"/"},
                                 "org.eclipse.jdt.ui.edit.text.java.add.block.comment");

            // Correct Indent
            CountSpecificCommand(commands, new[] {"Ctrl"}, new[] {"I"}, "org.eclipse.jdt.ui.edit.text.java.indent");

            // Format
            CountSpecificCommand(commands, new[] {"Shift", "Ctrl"}, new[] {"F"},
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

            foreach (var kvp in keyCount.OrderByDescending(x => x.Value))
            {
                chartKeyStrokes.Series[0].Points.AddXY(kvp.Key, kvp.Value);
            }

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