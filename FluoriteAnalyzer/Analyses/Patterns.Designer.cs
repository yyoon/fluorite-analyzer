namespace FluoriteAnalyzer.Analyses
{
    partial class Patterns
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.listViewPatterns = new System.Windows.Forms.ListView();
            this.columnPatternID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnPatternLength = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnPatternTimeInVideo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnPatternInfo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.labelCount = new System.Windows.Forms.Label();
            this.buttonDetectPatterns = new System.Windows.Forms.Button();
            this.comboDetectors = new System.Windows.Forms.ComboBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.buttonLoadResults = new System.Windows.Forms.Button();
            this.buttonSaveResults = new System.Windows.Forms.Button();
            this.snapshotPreview1 = new FluoriteAnalyzer.Forms.Controls.SnapshotPreview();
            this.snapshotPreview2 = new FluoriteAnalyzer.Forms.Controls.SnapshotPreview();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
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
            this.listViewPatterns.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listViewPatterns.FullRowSelect = true;
            this.listViewPatterns.HideSelection = false;
            this.listViewPatterns.Location = new System.Drawing.Point(0, 0);
            this.listViewPatterns.Name = "listViewPatterns";
            this.listViewPatterns.Size = new System.Drawing.Size(563, 469);
            this.listViewPatterns.TabIndex = 0;
            this.listViewPatterns.UseCompatibleStateImageBehavior = false;
            this.listViewPatterns.View = System.Windows.Forms.View.Details;
            this.listViewPatterns.SelectedIndexChanged += new System.EventHandler(this.listViewPatterns_SelectedIndexChanged);
            this.listViewPatterns.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listViewPatterns_KeyDown);
            this.listViewPatterns.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listViewPatterns_MouseDoubleClick);
            this.listViewPatterns.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listViewPatterns_MouseDown);
            // 
            // columnPatternID
            // 
            this.columnPatternID.Text = "ID";
            this.columnPatternID.Width = 0;
            // 
            // columnPatternLength
            // 
            this.columnPatternLength.Text = "Length";
            this.columnPatternLength.Width = 0;
            // 
            // columnPatternTimeInVideo
            // 
            this.columnPatternTimeInVideo.Text = "Time in Video";
            this.columnPatternTimeInVideo.Width = 0;
            // 
            // columnPatternInfo
            // 
            this.columnPatternInfo.Text = "Additional Information";
            this.columnPatternInfo.Width = 400;
            // 
            // labelCount
            // 
            this.labelCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelCount.AutoSize = true;
            this.labelCount.Location = new System.Drawing.Point(3, 483);
            this.labelCount.Name = "labelCount";
            this.labelCount.Size = new System.Drawing.Size(0, 13);
            this.labelCount.TabIndex = 3;
            // 
            // buttonDetectPatterns
            // 
            this.buttonDetectPatterns.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonDetectPatterns.Location = new System.Drawing.Point(206, 475);
            this.buttonDetectPatterns.Name = "buttonDetectPatterns";
            this.buttonDetectPatterns.Size = new System.Drawing.Size(100, 21);
            this.buttonDetectPatterns.TabIndex = 2;
            this.buttonDetectPatterns.Text = "Detect Patterns";
            this.buttonDetectPatterns.UseVisualStyleBackColor = true;
            this.buttonDetectPatterns.Click += new System.EventHandler(this.buttonDetectPatterns_Click);
            // 
            // comboDetectors
            // 
            this.comboDetectors.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.comboDetectors.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboDetectors.FormattingEnabled = true;
            this.comboDetectors.Location = new System.Drawing.Point(0, 475);
            this.comboDetectors.Name = "comboDetectors";
            this.comboDetectors.Size = new System.Drawing.Size(200, 21);
            this.comboDetectors.TabIndex = 1;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.buttonSaveResults);
            this.splitContainer1.Panel1.Controls.Add(this.buttonLoadResults);
            this.splitContainer1.Panel1.Controls.Add(this.listViewPatterns);
            this.splitContainer1.Panel1.Controls.Add(this.comboDetectors);
            this.splitContainer1.Panel1.Controls.Add(this.buttonDetectPatterns);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(1000, 500);
            this.splitContainer1.SplitterDistance = 566;
            this.splitContainer1.TabIndex = 4;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.snapshotPreview1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.snapshotPreview2);
            this.splitContainer2.Size = new System.Drawing.Size(430, 500);
            this.splitContainer2.SplitterDistance = 248;
            this.splitContainer2.TabIndex = 0;
            this.splitContainer2.DoubleClick += new System.EventHandler(this.splitContainer2_DoubleClick);
            // 
            // buttonLoadResults
            // 
            this.buttonLoadResults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonLoadResults.Location = new System.Drawing.Point(357, 474);
            this.buttonLoadResults.Name = "buttonLoadResults";
            this.buttonLoadResults.Size = new System.Drawing.Size(100, 21);
            this.buttonLoadResults.TabIndex = 3;
            this.buttonLoadResults.Text = "Load Results";
            this.buttonLoadResults.UseVisualStyleBackColor = true;
            this.buttonLoadResults.Click += new System.EventHandler(this.buttonLoadResults_Click);
            // 
            // buttonSaveResults
            // 
            this.buttonSaveResults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSaveResults.Location = new System.Drawing.Point(463, 474);
            this.buttonSaveResults.Name = "buttonSaveResults";
            this.buttonSaveResults.Size = new System.Drawing.Size(100, 21);
            this.buttonSaveResults.TabIndex = 4;
            this.buttonSaveResults.Text = "Save Results";
            this.buttonSaveResults.UseVisualStyleBackColor = true;
            this.buttonSaveResults.Click += new System.EventHandler(this.buttonSaveResults_Click);
            // 
            // snapshotPreview1
            // 
            this.snapshotPreview1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.snapshotPreview1.Location = new System.Drawing.Point(0, 0);
            this.snapshotPreview1.Name = "snapshotPreview1";
            this.snapshotPreview1.Size = new System.Drawing.Size(430, 248);
            this.snapshotPreview1.TabIndex = 0;
            // 
            // snapshotPreview2
            // 
            this.snapshotPreview2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.snapshotPreview2.Location = new System.Drawing.Point(0, 0);
            this.snapshotPreview2.Name = "snapshotPreview2";
            this.snapshotPreview2.Size = new System.Drawing.Size(430, 248);
            this.snapshotPreview2.TabIndex = 0;
            // 
            // Patterns
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.labelCount);
            this.Name = "Patterns";
            this.Size = new System.Drawing.Size(1000, 500);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listViewPatterns;
        private System.Windows.Forms.ColumnHeader columnPatternID;
        private System.Windows.Forms.ColumnHeader columnPatternLength;
        private System.Windows.Forms.ColumnHeader columnPatternTimeInVideo;
        private System.Windows.Forms.ColumnHeader columnPatternInfo;
        private System.Windows.Forms.Label labelCount;
        private System.Windows.Forms.Button buttonDetectPatterns;
        private System.Windows.Forms.ComboBox comboDetectors;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private Forms.Controls.SnapshotPreview snapshotPreview1;
        private Forms.Controls.SnapshotPreview snapshotPreview2;
        private System.Windows.Forms.Button buttonSaveResults;
        private System.Windows.Forms.Button buttonLoadResults;
    }
}
