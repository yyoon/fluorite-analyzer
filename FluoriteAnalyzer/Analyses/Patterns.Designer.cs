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
            this.buttonSearchFixTypos = new System.Windows.Forms.Button();
            this.listViewPatterns = new System.Windows.Forms.ListView();
            this.columnPatternID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnPatternLength = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnPatternTimeInVideo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnPatternInfo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // buttonSearchFixTypos
            // 
            this.buttonSearchFixTypos.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSearchFixTypos.Location = new System.Drawing.Point(686, 517);
            this.buttonSearchFixTypos.Name = "buttonSearchFixTypos";
            this.buttonSearchFixTypos.Size = new System.Drawing.Size(171, 25);
            this.buttonSearchFixTypos.TabIndex = 3;
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
            this.listViewPatterns.Size = new System.Drawing.Size(857, 511);
            this.listViewPatterns.TabIndex = 2;
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
            // Patterns
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.buttonSearchFixTypos);
            this.Controls.Add(this.listViewPatterns);
            this.Name = "Patterns";
            this.Size = new System.Drawing.Size(857, 542);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonSearchFixTypos;
        private System.Windows.Forms.ListView listViewPatterns;
        private System.Windows.Forms.ColumnHeader columnPatternID;
        private System.Windows.Forms.ColumnHeader columnPatternLength;
        private System.Windows.Forms.ColumnHeader columnPatternTimeInVideo;
        private System.Windows.Forms.ColumnHeader columnPatternInfo;
    }
}
