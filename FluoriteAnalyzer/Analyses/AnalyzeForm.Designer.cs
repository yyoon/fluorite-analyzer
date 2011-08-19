namespace FluoriteAnalyzer.Analyses
{
    partial class AnalyzeForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabCommands = new System.Windows.Forms.TabPage();
            this.tabVisualization = new System.Windows.Forms.TabPage();
            this.tabEvents = new System.Windows.Forms.TabPage();
            this.tabPatterns = new System.Windows.Forms.TabPage();
            this.buttonSearchFixTypos = new System.Windows.Forms.Button();
            this.listViewPatterns = new System.Windows.Forms.ListView();
            this.columnPatternID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnPatternLength = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnPatternTimeInVideo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnPatternInfo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.adjustTimeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabKeyStrokes = new System.Windows.Forms.TabPage();
            this.tabControl.SuspendLayout();
            this.tabPatterns.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Controls.Add(this.tabCommands);
            this.tabControl.Controls.Add(this.tabVisualization);
            this.tabControl.Controls.Add(this.tabEvents);
            this.tabControl.Controls.Add(this.tabPatterns);
            this.tabControl.Controls.Add(this.tabKeyStrokes);
            this.tabControl.Location = new System.Drawing.Point(14, 12);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(819, 451);
            this.tabControl.TabIndex = 0;
            // 
            // tabCommands
            // 
            this.tabCommands.Location = new System.Drawing.Point(4, 22);
            this.tabCommands.Name = "tabCommands";
            this.tabCommands.Padding = new System.Windows.Forms.Padding(3);
            this.tabCommands.Size = new System.Drawing.Size(811, 425);
            this.tabCommands.TabIndex = 0;
            this.tabCommands.Text = "Commands";
            this.tabCommands.UseVisualStyleBackColor = true;
            // 
            // tabVisualization
            // 
            this.tabVisualization.Location = new System.Drawing.Point(4, 22);
            this.tabVisualization.Name = "tabVisualization";
            this.tabVisualization.Padding = new System.Windows.Forms.Padding(3);
            this.tabVisualization.Size = new System.Drawing.Size(811, 425);
            this.tabVisualization.TabIndex = 1;
            this.tabVisualization.Text = "Visualization";
            this.tabVisualization.UseVisualStyleBackColor = true;
            // 
            // tabEvents
            // 
            this.tabEvents.Location = new System.Drawing.Point(4, 22);
            this.tabEvents.Name = "tabEvents";
            this.tabEvents.Size = new System.Drawing.Size(811, 425);
            this.tabEvents.TabIndex = 2;
            this.tabEvents.Text = "Events";
            this.tabEvents.UseVisualStyleBackColor = true;
            // 
            // tabPatterns
            // 
            this.tabPatterns.Controls.Add(this.buttonSearchFixTypos);
            this.tabPatterns.Controls.Add(this.listViewPatterns);
            this.tabPatterns.Location = new System.Drawing.Point(4, 22);
            this.tabPatterns.Name = "tabPatterns";
            this.tabPatterns.Size = new System.Drawing.Size(811, 425);
            this.tabPatterns.TabIndex = 3;
            this.tabPatterns.Text = "Patterns";
            this.tabPatterns.UseVisualStyleBackColor = true;
            // 
            // buttonSearchFixTypos
            // 
            this.buttonSearchFixTypos.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSearchFixTypos.Location = new System.Drawing.Point(611, 402);
            this.buttonSearchFixTypos.Name = "buttonSearchFixTypos";
            this.buttonSearchFixTypos.Size = new System.Drawing.Size(199, 23);
            this.buttonSearchFixTypos.TabIndex = 1;
            this.buttonSearchFixTypos.Text = "Find Fix Typos Patterns";
            this.buttonSearchFixTypos.UseVisualStyleBackColor = true;
            this.buttonSearchFixTypos.Click += new System.EventHandler(this.buttonSearchFixTypos_Click);
            // 
            // listViewPatterns
            // 
            this.listViewPatterns.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewPatterns.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnPatternID,
            this.columnPatternLength,
            this.columnPatternTimeInVideo,
            this.columnPatternInfo});
            this.listViewPatterns.FullRowSelect = true;
            this.listViewPatterns.Location = new System.Drawing.Point(0, 0);
            this.listViewPatterns.Name = "listViewPatterns";
            this.listViewPatterns.Size = new System.Drawing.Size(811, 396);
            this.listViewPatterns.TabIndex = 0;
            this.listViewPatterns.UseCompatibleStateImageBehavior = false;
            this.listViewPatterns.View = System.Windows.Forms.View.Details;
            // 
            // columnPatternID
            // 
            this.columnPatternID.Text = "ID";
            this.columnPatternID.Width = 40;
            // 
            // columnPatternLength
            // 
            this.columnPatternLength.Text = "Length";
            this.columnPatternLength.Width = 40;
            // 
            // columnPatternTimeInVideo
            // 
            this.columnPatternTimeInVideo.Text = "Time in Video";
            // 
            // columnPatternInfo
            // 
            this.columnPatternInfo.Text = "Additional Information";
            this.columnPatternInfo.Width = 400;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(847, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.Visible = false;
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.adjustTimeToolStripMenuItem});
            this.toolsToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.MatchOnly;
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(47, 20);
            this.toolsToolStripMenuItem.Text = "&Tools";
            // 
            // adjustTimeToolStripMenuItem
            // 
            this.adjustTimeToolStripMenuItem.Name = "adjustTimeToolStripMenuItem";
            this.adjustTimeToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.adjustTimeToolStripMenuItem.Text = "Adjust &Time";
            this.adjustTimeToolStripMenuItem.Click += new System.EventHandler(this.adjustTimeToolStripMenuItem_Click);
            // 
            // tabKeyStrokes
            // 
            this.tabKeyStrokes.Location = new System.Drawing.Point(4, 22);
            this.tabKeyStrokes.Name = "tabKeyStrokes";
            this.tabKeyStrokes.Size = new System.Drawing.Size(811, 425);
            this.tabKeyStrokes.TabIndex = 4;
            this.tabKeyStrokes.Text = "KeyStrokes";
            this.tabKeyStrokes.UseVisualStyleBackColor = true;
            // 
            // AnalyzeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(847, 474);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "AnalyzeForm";
            this.Text = "AnalyzeForm";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.AnalyzeForm_FormClosed);
            this.Load += new System.EventHandler(this.AnalyzeForm_Load);
            this.tabControl.ResumeLayout(false);
            this.tabPatterns.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabCommands;
        private System.Windows.Forms.TabPage tabVisualization;
        private System.Windows.Forms.TabPage tabEvents;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem adjustTimeToolStripMenuItem;
        private System.Windows.Forms.TabPage tabPatterns;
        private System.Windows.Forms.Button buttonSearchFixTypos;
        private System.Windows.Forms.ListView listViewPatterns;
        private System.Windows.Forms.ColumnHeader columnPatternID;
        private System.Windows.Forms.ColumnHeader columnPatternLength;
        private System.Windows.Forms.ColumnHeader columnPatternTimeInVideo;
        private System.Windows.Forms.ColumnHeader columnPatternInfo;
        private System.Windows.Forms.TabPage tabKeyStrokes;

    }
}