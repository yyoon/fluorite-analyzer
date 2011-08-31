using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FluoriteAnalyzer.Forms
{
    public partial class ToolWindow : Form
    {
        public ToolWindow()
        {
            InitializeComponent();

            TopLevel = false;
            FormClosing += new FormClosingEventHandler(ToolWindow_FormClosing);
        }

        void ToolWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            switch (e.CloseReason)
            {
                case CloseReason.None:
                case CloseReason.MdiFormClosing:
                    break;

                default:
                    e.Cancel = true;
                    WindowState = FormWindowState.Minimized;
                    break;
            }
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case 0x00A1:    // WM_NCLBUTTONDOWN
                    if (ModifierKeys == Keys.Shift)
                    {
                        this.WindowState = FormWindowState.Normal;
                        this.BringToFront();

                        if (this.Parent != null)
                        {
                            this.Location = new Point { X = 0, Y = 0 };
                            this.Size = new Size { Width = this.Parent.ClientRectangle.Width / 2, Height = this.Parent.ClientRectangle.Height };
                        }
                    }
                    break;

                case 0x00A4:    // WM_NCRBUTTONDOWN
                    if (ModifierKeys == Keys.Shift)
                    {
                        this.WindowState = FormWindowState.Normal;
                        this.BringToFront();

                        if (this.Parent != null)
                        {
                            int halfWidth = Width = this.Parent.ClientRectangle.Width / 2;
                            this.Location = new Point { X = halfWidth, Y = 0 };
                            this.Size = new Size { Width = this.Parent.ClientRectangle.Width - halfWidth, Height = this.Parent.ClientRectangle.Height };
                        }
                    }
                    break;
            }

            base.WndProc(ref m);
        }
    }
}
