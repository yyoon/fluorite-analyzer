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
            this.listViewPatterns.FullRowSelect = true;
            this.listViewPatterns.Location = new System.Drawing.Point(0, 0);
            this.listViewPatterns.Name = "listViewPatterns";
            this.listViewPatterns.Size = new System.Drawing.Size(999, 472);
            this.listViewPatterns.TabIndex = 0;
            this.listViewPatterns.UseCompatibleStateImageBehavior = false;
            this.listViewPatterns.View = System.Windows.Forms.View.Details;
            this.listViewPatterns.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listViewPatterns_MouseDoubleClick);
            this.listViewPatterns.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listViewPatterns_MouseDown);
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
            // labelCount
            // 
            this.labelCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelCount.AutoSize = true;
            this.labelCount.Location = new System.Drawing.Point(3, 483);
            this.labelCount.Name = "labelCount";
            this.labelCount.Size = new System.Drawing.Size(0, 12);
            this.labelCount.TabIndex = 3;
            // 
            // buttonDetectPatterns
            // 
            this.buttonDetectPatterns.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonDetectPatterns.Location = new System.Drawing.Point(799, 475);
            this.buttonDetectPatterns.Name = "buttonDetectPatterns";
            this.buttonDetectPatterns.Size = new System.Drawing.Size(200, 22);
            this.buttonDetectPatterns.TabIndex = 2;
            this.buttonDetectPatterns.Text = "Detect Patterns";
            this.buttonDetectPatterns.UseVisualStyleBackColor = true;
            this.buttonDetectPatterns.Click += new System.EventHandler(this.buttonDetectPatterns_Click);
            // 
            // comboDetectors
            // 
            this.comboDetectors.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.comboDetectors.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboDetectors.FormattingEnabled = true;
            this.comboDetectors.Location = new System.Drawing.Point(593, 477);
            this.comboDetectors.Name = "comboDetectors";
            this.comboDetectors.Size = new System.Drawing.Size(200, 20);
            this.comboDetectors.TabIndex = 1;
            // 
            // Patterns
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.comboDetectors);
            this.Controls.Add(this.buttonDetectPatterns);
            this.Controls.Add(this.labelCount);
            this.Controls.Add(this.listViewPatterns);
            this.Name = "Patterns";
            this.Size = new System.Drawing.Size(1000, 500);
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
    }
}
