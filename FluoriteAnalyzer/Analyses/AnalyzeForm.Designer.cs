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
            this.tabKeyStrokes = new System.Windows.Forms.TabPage();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.adjustTimeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl.SuspendLayout();
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
            this.tabControl.Location = new System.Drawing.Point(12, 12);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(702, 490);
            this.tabControl.TabIndex = 0;
            // 
            // tabCommands
            // 
            this.tabCommands.Location = new System.Drawing.Point(4, 22);
            this.tabCommands.Name = "tabCommands";
            this.tabCommands.Padding = new System.Windows.Forms.Padding(3);
            this.tabCommands.Size = new System.Drawing.Size(694, 463);
            this.tabCommands.TabIndex = 0;
            this.tabCommands.Text = "Commands";
            this.tabCommands.UseVisualStyleBackColor = true;
            // 
            // tabVisualization
            // 
            this.tabVisualization.Location = new System.Drawing.Point(4, 22);
            this.tabVisualization.Name = "tabVisualization";
            this.tabVisualization.Padding = new System.Windows.Forms.Padding(3);
            this.tabVisualization.Size = new System.Drawing.Size(694, 463);
            this.tabVisualization.TabIndex = 1;
            this.tabVisualization.Text = "Visualization";
            this.tabVisualization.UseVisualStyleBackColor = true;
            // 
            // tabEvents
            // 
            this.tabEvents.Location = new System.Drawing.Point(4, 22);
            this.tabEvents.Name = "tabEvents";
            this.tabEvents.Size = new System.Drawing.Size(694, 463);
            this.tabEvents.TabIndex = 2;
            this.tabEvents.Text = "Events";
            this.tabEvents.UseVisualStyleBackColor = true;
            // 
            // tabPatterns
            // 
            this.tabPatterns.Location = new System.Drawing.Point(4, 22);
            this.tabPatterns.Name = "tabPatterns";
            this.tabPatterns.Size = new System.Drawing.Size(694, 464);
            this.tabPatterns.TabIndex = 3;
            this.tabPatterns.Text = "Patterns";
            this.tabPatterns.UseVisualStyleBackColor = true;
            // 
            // tabKeyStrokes
            // 
            this.tabKeyStrokes.Location = new System.Drawing.Point(4, 22);
            this.tabKeyStrokes.Name = "tabKeyStrokes";
            this.tabKeyStrokes.Size = new System.Drawing.Size(694, 463);
            this.tabKeyStrokes.TabIndex = 4;
            this.tabKeyStrokes.Text = "KeyStrokes";
            this.tabKeyStrokes.UseVisualStyleBackColor = true;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(5, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(726, 24);
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
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.toolsToolStripMenuItem.Text = "&Tools";
            // 
            // adjustTimeToolStripMenuItem
            // 
            this.adjustTimeToolStripMenuItem.Name = "adjustTimeToolStripMenuItem";
            this.adjustTimeToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.adjustTimeToolStripMenuItem.Text = "Adjust &Time";
            this.adjustTimeToolStripMenuItem.Click += new System.EventHandler(this.adjustTimeToolStripMenuItem_Click);
            // 
            // AnalyzeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(726, 514);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "AnalyzeForm";
            this.Text = "AnalyzeForm";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.AnalyzeForm_FormClosed);
            this.Load += new System.EventHandler(this.AnalyzeForm_Load);
            this.tabControl.ResumeLayout(false);
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
        private System.Windows.Forms.TabPage tabKeyStrokes;

    }
}