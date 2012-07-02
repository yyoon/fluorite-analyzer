namespace FluoriteAnalyzer.Forms
{
    partial class AdjustTimeForm
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
            this.textHour = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textMinute = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textSecond = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textTick = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textHour
            // 
            this.textHour.Location = new System.Drawing.Point(12, 12);
            this.textHour.Name = "textHour";
            this.textHour.Size = new System.Drawing.Size(20, 21);
            this.textHour.TabIndex = 0;
            this.textHour.Text = "0";
            this.textHour.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textHour.Validating += new System.ComponentModel.CancelEventHandler(this.textHour_Validating);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(38, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(12, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "h";
            // 
            // textMinute
            // 
            this.textMinute.Location = new System.Drawing.Point(56, 12);
            this.textMinute.Name = "textMinute";
            this.textMinute.Size = new System.Drawing.Size(20, 21);
            this.textMinute.TabIndex = 2;
            this.textMinute.Text = "0";
            this.textMinute.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textMinute.Validating += new System.ComponentModel.CancelEventHandler(this.textMinute_Validating);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(82, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(16, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "m";
            // 
            // textSecond
            // 
            this.textSecond.Location = new System.Drawing.Point(104, 12);
            this.textSecond.Name = "textSecond";
            this.textSecond.Size = new System.Drawing.Size(20, 21);
            this.textSecond.TabIndex = 4;
            this.textSecond.Text = "0";
            this.textSecond.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textSecond.Validating += new System.ComponentModel.CancelEventHandler(this.textSecond_Validating);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(130, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "s  in Video";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(38, 57);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(123, 12);
            this.label4.TabIndex = 6;
            this.label4.Text = "is synchronized with";
            // 
            // textTick
            // 
            this.textTick.Location = new System.Drawing.Point(12, 95);
            this.textTick.Name = "textTick";
            this.textTick.Size = new System.Drawing.Size(112, 21);
            this.textTick.TabIndex = 7;
            this.textTick.Text = "0";
            this.textTick.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textTick.Validating += new System.ComponentModel.CancelEventHandler(this.textTick_Validating);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(130, 98);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(67, 12);
            this.label5.TabIndex = 8;
            this.label5.Text = "tick  in Log";
            // 
            // buttonOK
            // 
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(40, 159);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 9;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            this.buttonCancel.CausesValidation = false;
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(122, 159);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 10;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // AdjustTimeForm
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(209, 194);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textTick);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textSecond);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textMinute);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textHour);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AdjustTimeForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Adjust Time";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textHour;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textMinute;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textSecond;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textTick;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
    }
}