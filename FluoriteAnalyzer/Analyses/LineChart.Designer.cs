namespace FluoriteAnalyzer.Analyses
{
    partial class LineChart
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
            this.groupYValue = new System.Windows.Forms.GroupBox();
            this.radioASTNodeCount = new System.Windows.Forms.RadioButton();
            this.radioActiveCodeLength = new System.Windows.Forms.RadioButton();
            this.radioDocumentLength = new System.Windows.Forms.RadioButton();
            this.radioExpressionCount = new System.Windows.Forms.RadioButton();
            this.groupChangeCount = new System.Windows.Forms.GroupBox();
            this.radioAltogether = new System.Windows.Forms.RadioButton();
            this.radioPerFile = new System.Windows.Forms.RadioButton();
            this.groupLineChartOptions = new System.Windows.Forms.GroupBox();
            this.checkTwoPointsForReplace = new System.Windows.Forms.CheckBox();
            this.chartLine = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.groupYValue.SuspendLayout();
            this.groupChangeCount.SuspendLayout();
            this.groupLineChartOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartLine)).BeginInit();
            this.SuspendLayout();
            // 
            // groupYValue
            // 
            this.groupYValue.Controls.Add(this.radioASTNodeCount);
            this.groupYValue.Controls.Add(this.radioActiveCodeLength);
            this.groupYValue.Controls.Add(this.radioDocumentLength);
            this.groupYValue.Controls.Add(this.radioExpressionCount);
            this.groupYValue.Location = new System.Drawing.Point(3, 3);
            this.groupYValue.Name = "groupYValue";
            this.groupYValue.Size = new System.Drawing.Size(315, 66);
            this.groupYValue.TabIndex = 1;
            this.groupYValue.TabStop = false;
            this.groupYValue.Text = "Y Value";
            // 
            // radioASTNodeCount
            // 
            this.radioASTNodeCount.AutoSize = true;
            this.radioASTNodeCount.Location = new System.Drawing.Point(170, 42);
            this.radioASTNodeCount.Name = "radioASTNodeCount";
            this.radioASTNodeCount.Size = new System.Drawing.Size(118, 16);
            this.radioASTNodeCount.TabIndex = 3;
            this.radioASTNodeCount.TabStop = true;
            this.radioASTNodeCount.Text = "AST Node Count";
            this.radioASTNodeCount.UseVisualStyleBackColor = true;
            this.radioASTNodeCount.CheckedChanged += new System.EventHandler(this.general_CheckedChanged);
            // 
            // radioActiveCodeLength
            // 
            this.radioActiveCodeLength.AutoSize = true;
            this.radioActiveCodeLength.Checked = true;
            this.radioActiveCodeLength.Location = new System.Drawing.Point(170, 20);
            this.radioActiveCodeLength.Name = "radioActiveCodeLength";
            this.radioActiveCodeLength.Size = new System.Drawing.Size(133, 16);
            this.radioActiveCodeLength.TabIndex = 1;
            this.radioActiveCodeLength.TabStop = true;
            this.radioActiveCodeLength.Text = "Active Code Length";
            this.radioActiveCodeLength.UseVisualStyleBackColor = true;
            this.radioActiveCodeLength.CheckedChanged += new System.EventHandler(this.general_CheckedChanged);
            // 
            // radioDocumentLength
            // 
            this.radioDocumentLength.AutoSize = true;
            this.radioDocumentLength.Location = new System.Drawing.Point(6, 20);
            this.radioDocumentLength.Name = "radioDocumentLength";
            this.radioDocumentLength.Size = new System.Drawing.Size(122, 16);
            this.radioDocumentLength.TabIndex = 0;
            this.radioDocumentLength.Text = "Document Length";
            this.radioDocumentLength.UseVisualStyleBackColor = true;
            this.radioDocumentLength.CheckedChanged += new System.EventHandler(this.general_CheckedChanged);
            // 
            // radioExpressionCount
            // 
            this.radioExpressionCount.AutoSize = true;
            this.radioExpressionCount.Location = new System.Drawing.Point(6, 42);
            this.radioExpressionCount.Name = "radioExpressionCount";
            this.radioExpressionCount.Size = new System.Drawing.Size(158, 16);
            this.radioExpressionCount.TabIndex = 2;
            this.radioExpressionCount.TabStop = true;
            this.radioExpressionCount.Text = "Expression Node Count";
            this.radioExpressionCount.UseVisualStyleBackColor = true;
            this.radioExpressionCount.CheckedChanged += new System.EventHandler(this.general_CheckedChanged);
            // 
            // groupChangeCount
            // 
            this.groupChangeCount.Controls.Add(this.radioAltogether);
            this.groupChangeCount.Controls.Add(this.radioPerFile);
            this.groupChangeCount.Location = new System.Drawing.Point(324, 3);
            this.groupChangeCount.Name = "groupChangeCount";
            this.groupChangeCount.Size = new System.Drawing.Size(118, 66);
            this.groupChangeCount.TabIndex = 4;
            this.groupChangeCount.TabStop = false;
            this.groupChangeCount.Text = "Change Count";
            // 
            // radioAltogether
            // 
            this.radioAltogether.AutoSize = true;
            this.radioAltogether.Location = new System.Drawing.Point(6, 42);
            this.radioAltogether.Name = "radioAltogether";
            this.radioAltogether.Size = new System.Drawing.Size(79, 16);
            this.radioAltogether.TabIndex = 1;
            this.radioAltogether.Text = "Altogether";
            this.radioAltogether.UseVisualStyleBackColor = true;
            this.radioAltogether.CheckedChanged += new System.EventHandler(this.general_CheckedChanged);
            // 
            // radioPerFile
            // 
            this.radioPerFile.AutoSize = true;
            this.radioPerFile.Checked = true;
            this.radioPerFile.Location = new System.Drawing.Point(6, 20);
            this.radioPerFile.Name = "radioPerFile";
            this.radioPerFile.Size = new System.Drawing.Size(66, 16);
            this.radioPerFile.TabIndex = 0;
            this.radioPerFile.TabStop = true;
            this.radioPerFile.Text = "Per File";
            this.radioPerFile.UseVisualStyleBackColor = true;
            this.radioPerFile.CheckedChanged += new System.EventHandler(this.general_CheckedChanged);
            // 
            // groupLineChartOptions
            // 
            this.groupLineChartOptions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupLineChartOptions.Controls.Add(this.checkTwoPointsForReplace);
            this.groupLineChartOptions.Location = new System.Drawing.Point(448, 3);
            this.groupLineChartOptions.Name = "groupLineChartOptions";
            this.groupLineChartOptions.Size = new System.Drawing.Size(549, 66);
            this.groupLineChartOptions.TabIndex = 5;
            this.groupLineChartOptions.TabStop = false;
            this.groupLineChartOptions.Text = "Other Options";
            // 
            // checkTwoPointsForReplace
            // 
            this.checkTwoPointsForReplace.AutoSize = true;
            this.checkTwoPointsForReplace.Enabled = false;
            this.checkTwoPointsForReplace.Location = new System.Drawing.Point(7, 20);
            this.checkTwoPointsForReplace.Name = "checkTwoPointsForReplace";
            this.checkTwoPointsForReplace.Size = new System.Drawing.Size(210, 16);
            this.checkTwoPointsForReplace.TabIndex = 0;
            this.checkTwoPointsForReplace.Text = "Add two points for Replace event";
            this.checkTwoPointsForReplace.UseVisualStyleBackColor = true;
            this.checkTwoPointsForReplace.CheckedChanged += new System.EventHandler(this.general_CheckedChanged);
            // 
            // chartLine
            // 
            this.chartLine.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.chartLine.BorderSkin.SkinStyle = System.Windows.Forms.DataVisualization.Charting.BorderSkinStyle.Emboss;
            chartArea1.AxisX.Title = "Time (sec)";
            chartArea1.BackColor = System.Drawing.Color.White;
            chartArea1.Name = "ChartArea1";
            this.chartLine.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chartLine.Legends.Add(legend1);
            this.chartLine.Location = new System.Drawing.Point(3, 75);
            this.chartLine.Name = "chartLine";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.StepLine;
            series1.Legend = "Legend1";
            series1.Name = "File1";
            this.chartLine.Series.Add(series1);
            this.chartLine.Size = new System.Drawing.Size(994, 422);
            this.chartLine.TabIndex = 3;
            this.chartLine.Text = "chart1";
            this.chartLine.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.chartLine_MouseDoubleClick);
            // 
            // LineChart
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.chartLine);
            this.Controls.Add(this.groupLineChartOptions);
            this.Controls.Add(this.groupChangeCount);
            this.Controls.Add(this.groupYValue);
            this.Name = "LineChart";
            this.Size = new System.Drawing.Size(1000, 500);
            this.groupYValue.ResumeLayout(false);
            this.groupYValue.PerformLayout();
            this.groupChangeCount.ResumeLayout(false);
            this.groupChangeCount.PerformLayout();
            this.groupLineChartOptions.ResumeLayout(false);
            this.groupLineChartOptions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartLine)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupYValue;
        private System.Windows.Forms.RadioButton radioASTNodeCount;
        private System.Windows.Forms.RadioButton radioActiveCodeLength;
        private System.Windows.Forms.RadioButton radioDocumentLength;
        private System.Windows.Forms.RadioButton radioExpressionCount;
        private System.Windows.Forms.GroupBox groupChangeCount;
        private System.Windows.Forms.RadioButton radioAltogether;
        private System.Windows.Forms.RadioButton radioPerFile;
        private System.Windows.Forms.GroupBox groupLineChartOptions;
        private System.Windows.Forms.CheckBox checkTwoPointsForReplace;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartLine;
    }
}
