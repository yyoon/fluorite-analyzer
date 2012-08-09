namespace FluoriteAnalyzer.Analyses
{
    partial class EventsList
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.buttonGoto = new System.Windows.Forms.Button();
            this.buttonShowHideCode = new System.Windows.Forms.Button();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.treeEvents = new System.Windows.Forms.TreeView();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.listViewEvents = new FluoriteAnalyzer.Forms.Controls.ListViewNF();
            this.columnID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnTimestamp = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnTimeInVideo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnCategory = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnParameters = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.labelParameters = new System.Windows.Forms.Label();
            this.textParameters = new System.Windows.Forms.TextBox();
            this.snapshotPreview = new FluoriteAnalyzer.Forms.Controls.SnapshotPreview();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.buttonGoto);
            this.splitContainer1.Panel1.Controls.Add(this.buttonShowHideCode);
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.snapshotPreview);
            this.splitContainer1.Size = new System.Drawing.Size(1000, 500);
            this.splitContainer1.SplitterDistance = 674;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 2;
            // 
            // buttonGoto
            // 
            this.buttonGoto.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonGoto.Location = new System.Drawing.Point(415, 476);
            this.buttonGoto.Name = "buttonGoto";
            this.buttonGoto.Size = new System.Drawing.Size(75, 21);
            this.buttonGoto.TabIndex = 4;
            this.buttonGoto.Text = "&Goto ID";
            this.buttonGoto.UseVisualStyleBackColor = true;
            this.buttonGoto.Click += new System.EventHandler(this.buttonGoto_Click);
            // 
            // buttonShowHideCode
            // 
            this.buttonShowHideCode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonShowHideCode.Location = new System.Drawing.Point(496, 476);
            this.buttonShowHideCode.Name = "buttonShowHideCode";
            this.buttonShowHideCode.Size = new System.Drawing.Size(175, 21);
            this.buttonShowHideCode.TabIndex = 3;
            this.buttonShowHideCode.Text = "Show/Hide Code";
            this.buttonShowHideCode.UseVisualStyleBackColor = true;
            this.buttonShowHideCode.Click += new System.EventHandler(this.buttonShowHideCode_Click);
            // 
            // splitContainer
            // 
            this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.treeEvents);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer.Size = new System.Drawing.Size(671, 471);
            this.splitContainer.SplitterDistance = 170;
            this.splitContainer.SplitterWidth = 5;
            this.splitContainer.TabIndex = 2;
            // 
            // treeEvents
            // 
            this.treeEvents.CheckBoxes = true;
            this.treeEvents.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeEvents.Location = new System.Drawing.Point(0, 0);
            this.treeEvents.Name = "treeEvents";
            this.treeEvents.Size = new System.Drawing.Size(170, 471);
            this.treeEvents.TabIndex = 0;
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
            this.splitContainer2.Panel1.Controls.Add(this.listViewEvents);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.labelParameters);
            this.splitContainer2.Panel2.Controls.Add(this.textParameters);
            this.splitContainer2.Size = new System.Drawing.Size(496, 471);
            this.splitContainer2.SplitterDistance = 317;
            this.splitContainer2.TabIndex = 1;
            // 
            // listViewEvents
            // 
            this.listViewEvents.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnID,
            this.columnTimestamp,
            this.columnTimeInVideo,
            this.columnCategory,
            this.columnType,
            this.columnParameters});
            this.listViewEvents.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewEvents.FullRowSelect = true;
            this.listViewEvents.HideSelection = false;
            this.listViewEvents.Location = new System.Drawing.Point(0, 0);
            this.listViewEvents.MultiSelect = false;
            this.listViewEvents.Name = "listViewEvents";
            this.listViewEvents.ShowItemToolTips = true;
            this.listViewEvents.Size = new System.Drawing.Size(496, 317);
            this.listViewEvents.TabIndex = 0;
            this.listViewEvents.UseCompatibleStateImageBehavior = false;
            this.listViewEvents.View = System.Windows.Forms.View.Details;
            this.listViewEvents.SelectedIndexChanged += new System.EventHandler(this.listViewEvents_SelectedIndexChanged);
            // 
            // columnID
            // 
            this.columnID.Text = "ID";
            this.columnID.Width = 40;
            // 
            // columnTimestamp
            // 
            this.columnTimestamp.Text = "Timestamp";
            // 
            // columnTimeInVideo
            // 
            this.columnTimeInVideo.Text = "Time in Video";
            // 
            // columnCategory
            // 
            this.columnCategory.Text = "Category";
            this.columnCategory.Width = 100;
            // 
            // columnType
            // 
            this.columnType.Text = "Type";
            this.columnType.Width = 150;
            // 
            // columnParameters
            // 
            this.columnParameters.Text = "Parameters";
            this.columnParameters.Width = 300;
            // 
            // labelParameters
            // 
            this.labelParameters.AutoSize = true;
            this.labelParameters.Font = new System.Drawing.Font("Gulim", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelParameters.Location = new System.Drawing.Point(3, 0);
            this.labelParameters.Name = "labelParameters";
            this.labelParameters.Size = new System.Drawing.Size(80, 12);
            this.labelParameters.TabIndex = 1;
            this.labelParameters.Text = "Parameters";
            // 
            // textParameters
            // 
            this.textParameters.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textParameters.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textParameters.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textParameters.Location = new System.Drawing.Point(3, 15);
            this.textParameters.Multiline = true;
            this.textParameters.Name = "textParameters";
            this.textParameters.ReadOnly = true;
            this.textParameters.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textParameters.Size = new System.Drawing.Size(490, 132);
            this.textParameters.TabIndex = 0;
            // 
            // snapshotPreview
            // 
            this.snapshotPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.snapshotPreview.Location = new System.Drawing.Point(0, 0);
            this.snapshotPreview.Name = "snapshotPreview";
            this.snapshotPreview.Size = new System.Drawing.Size(321, 500);
            this.snapshotPreview.TabIndex = 0;
            // 
            // EventsList
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.splitContainer1);
            this.Name = "EventsList";
            this.Size = new System.Drawing.Size(1000, 500);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button buttonShowHideCode;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.TreeView treeEvents;
        private FluoriteAnalyzer.Forms.Controls.ListViewNF listViewEvents;
        private System.Windows.Forms.ColumnHeader columnID;
        private System.Windows.Forms.ColumnHeader columnTimestamp;
        private System.Windows.Forms.ColumnHeader columnTimeInVideo;
        private System.Windows.Forms.ColumnHeader columnCategory;
        private System.Windows.Forms.ColumnHeader columnType;
        private System.Windows.Forms.ColumnHeader columnParameters;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Label labelParameters;
        private System.Windows.Forms.TextBox textParameters;
        private System.Windows.Forms.Button buttonGoto;
        private FluoriteAnalyzer.Forms.Controls.SnapshotPreview snapshotPreview;

    }
}
