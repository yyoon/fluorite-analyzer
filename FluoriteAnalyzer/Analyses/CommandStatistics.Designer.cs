namespace FluoriteAnalyzer.Analyses
{
    partial class CommandStatistics
    {
        /// <summary> 
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title1 = new System.Windows.Forms.DataVisualization.Charting.Title();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.chartPie = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.buttonRemovePattern = new System.Windows.Forms.Button();
            this.buttonAddPattern = new System.Windows.Forms.Button();
            this.listPatterns = new System.Windows.Forms.ListBox();
            this.buttonRemoveGroup = new System.Windows.Forms.Button();
            this.buttonAddGroup = new System.Windows.Forms.Button();
            this.listGroups = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartPie)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.textBox1);
            this.splitContainer1.Panel1.Controls.Add(this.chartPie);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.buttonRemovePattern);
            this.splitContainer1.Panel2.Controls.Add(this.buttonAddPattern);
            this.splitContainer1.Panel2.Controls.Add(this.listPatterns);
            this.splitContainer1.Panel2.Controls.Add(this.buttonRemoveGroup);
            this.splitContainer1.Panel2.Controls.Add(this.buttonAddGroup);
            this.splitContainer1.Panel2.Controls.Add(this.listGroups);
            this.splitContainer1.Panel2.Controls.Add(this.label1);
            this.splitContainer1.Size = new System.Drawing.Size(1000, 500);
            this.splitContainer1.SplitterDistance = 304;
            this.splitContainer1.TabIndex = 4;
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(763, 0);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(237, 304);
            this.textBox1.TabIndex = 4;
            // 
            // chartPie
            // 
            this.chartPie.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.chartPie.BorderSkin.SkinStyle = System.Windows.Forms.DataVisualization.Charting.BorderSkinStyle.Emboss;
            chartArea1.Area3DStyle.Enable3D = true;
            chartArea1.Name = "ChartArea1";
            this.chartPie.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chartPie.Legends.Add(legend1);
            this.chartPie.Location = new System.Drawing.Point(0, 0);
            this.chartPie.Name = "chartPie";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Pie;
            series1.CustomProperties = "PieLabelStyle=Outside";
            series1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            series1.Label = "#VALX (#VALY / #PERCENT)";
            series1.Legend = "Legend1";
            series1.LegendText = "#VALX (#PERCENT)";
            series1.Name = "Series1";
            this.chartPie.Series.Add(series1);
            this.chartPie.Size = new System.Drawing.Size(757, 304);
            this.chartPie.TabIndex = 3;
            this.chartPie.Text = "chart1";
            title1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            title1.Name = "Title1";
            title1.Text = "Used Commands";
            title1.TextStyle = System.Windows.Forms.DataVisualization.Charting.TextStyle.Shadow;
            this.chartPie.Titles.Add(title1);
            // 
            // buttonRemovePattern
            // 
            this.buttonRemovePattern.Location = new System.Drawing.Point(707, 30);
            this.buttonRemovePattern.Name = "buttonRemovePattern";
            this.buttonRemovePattern.Size = new System.Drawing.Size(87, 21);
            this.buttonRemovePattern.TabIndex = 8;
            this.buttonRemovePattern.Text = "Remove";
            this.buttonRemovePattern.UseVisualStyleBackColor = true;
            this.buttonRemovePattern.Click += new System.EventHandler(this.buttonRemovePattern_Click);
            // 
            // buttonAddPattern
            // 
            this.buttonAddPattern.Location = new System.Drawing.Point(707, 3);
            this.buttonAddPattern.Name = "buttonAddPattern";
            this.buttonAddPattern.Size = new System.Drawing.Size(87, 21);
            this.buttonAddPattern.TabIndex = 7;
            this.buttonAddPattern.Text = "Add";
            this.buttonAddPattern.UseVisualStyleBackColor = true;
            this.buttonAddPattern.Click += new System.EventHandler(this.buttonAddPattern_Click);
            // 
            // listPatterns
            // 
            this.listPatterns.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.listPatterns.FormattingEnabled = true;
            this.listPatterns.ItemHeight = 12;
            this.listPatterns.Location = new System.Drawing.Point(467, 3);
            this.listPatterns.Name = "listPatterns";
            this.listPatterns.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listPatterns.Size = new System.Drawing.Size(233, 136);
            this.listPatterns.TabIndex = 6;
            this.listPatterns.SelectedIndexChanged += new System.EventHandler(this.listPatterns_SelectedIndexChanged);
            this.listPatterns.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listPatterns_MouseDoubleClick);
            // 
            // buttonRemoveGroup
            // 
            this.buttonRemoveGroup.Location = new System.Drawing.Point(335, 30);
            this.buttonRemoveGroup.Name = "buttonRemoveGroup";
            this.buttonRemoveGroup.Size = new System.Drawing.Size(87, 21);
            this.buttonRemoveGroup.TabIndex = 3;
            this.buttonRemoveGroup.Text = "Remove";
            this.buttonRemoveGroup.UseVisualStyleBackColor = true;
            this.buttonRemoveGroup.Click += new System.EventHandler(this.buttonRemoveGroup_Click);
            // 
            // buttonAddGroup
            // 
            this.buttonAddGroup.Location = new System.Drawing.Point(335, 3);
            this.buttonAddGroup.Name = "buttonAddGroup";
            this.buttonAddGroup.Size = new System.Drawing.Size(87, 21);
            this.buttonAddGroup.TabIndex = 2;
            this.buttonAddGroup.Text = "Add";
            this.buttonAddGroup.UseVisualStyleBackColor = true;
            this.buttonAddGroup.Click += new System.EventHandler(this.buttonAddGroup_Click);
            // 
            // listGroups
            // 
            this.listGroups.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.listGroups.FormattingEnabled = true;
            this.listGroups.ItemHeight = 12;
            this.listGroups.Location = new System.Drawing.Point(94, 3);
            this.listGroups.Name = "listGroups";
            this.listGroups.Size = new System.Drawing.Size(233, 136);
            this.listGroups.TabIndex = 1;
            this.listGroups.SelectedIndexChanged += new System.EventHandler(this.listGroups_SelectedIndexChanged);
            this.listGroups.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listGroups_MouseDoubleClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "List of Groups";
            // 
            // CommandStatistics
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.splitContainer1);
            this.Name = "CommandStatistics";
            this.Size = new System.Drawing.Size(1000, 500);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chartPie)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartPie;
        private System.Windows.Forms.Button buttonRemovePattern;
        private System.Windows.Forms.Button buttonAddPattern;
        private System.Windows.Forms.ListBox listPatterns;
        private System.Windows.Forms.Button buttonRemoveGroup;
        private System.Windows.Forms.Button buttonAddGroup;
        private System.Windows.Forms.ListBox listGroups;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
    }
}
