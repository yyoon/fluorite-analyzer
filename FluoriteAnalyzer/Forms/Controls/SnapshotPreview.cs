using System.Drawing;
using System.IO;
using System.Windows.Forms;
using FluoriteAnalyzer.Commons;

namespace FluoriteAnalyzer.Forms.Controls
{
    public partial class SnapshotPreview : UserControl
    {
        public SnapshotPreview()
        {
            InitializeComponent();

            CodeFont = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            StrikeoutFont = new Font(CodeFont, FontStyle.Strikeout);
        }

        private Font CodeFont { get; set; }
        private Font StrikeoutFont { get; set; }

        public void Clear()
        {
            // Remember the focused control
            Control focusedControl = Utils.Utils.FindFocusedControl(TopLevelControl);

            this.tabControl.TabPages.Clear();

            // Restore back the focus
            if (focusedControl != null) { focusedControl.Focus(); }
        }

        public void SetSnapshot(EntireSnapshot snapshot)
        {
            // Remember the focused control
            Control focusedControl = Utils.Utils.FindFocusedControl(TopLevelControl);

            // Clear the tab panel.
            Clear();

            foreach (string filePath in snapshot.FilePaths)
            {
                TabPage newPage = new TabPage(Path.GetFileName(filePath));

                RichTextBox richText = new RichTextBox();
                richText.Dock = DockStyle.Fill;
                richText.Font = CodeFont;

                snapshot.FileSnapshots[filePath].DisplayInRichTextBox(richText, StrikeoutFont);

                newPage.Controls.Add(richText);

                this.tabControl.TabPages.Add(newPage);
            }

            // Select the first tab.
            if (this.tabControl.TabPages.Count > 0)
            {
                this.tabControl.SelectedTab = this.tabControl.TabPages[0];
            }

            // Restore back the focus
            if (focusedControl != null) { focusedControl.Focus(); }
        }
    }
}
