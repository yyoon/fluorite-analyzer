using System.ComponentModel;
using System.Windows.Forms;

namespace FluoriteAnalyzer.Forms
{
    public partial class AdjustTimeForm : Form
    {
        public AdjustTimeForm()
        {
            InitializeComponent();
        }

        public int VideoTick
        {
            get { return ((int.Parse(textHour.Text)*60 + int.Parse(textMinute.Text))*60 + int.Parse(textSecond.Text))*1000; }
        }

        public int LogTick
        {
            get { return int.Parse(textTick.Text); }
        }

        private void textHour_Validating(object sender, CancelEventArgs e)
        {
            int result;
            if (!int.TryParse(textHour.Text, out result) || result < 0)
            {
                textHour.SelectAll();
                e.Cancel = true;
            }
        }

        private void textMinute_Validating(object sender, CancelEventArgs e)
        {
            int result;
            if (!int.TryParse(textMinute.Text, out result) || result < 0 || result >= 60)
            {
                textMinute.SelectAll();
                e.Cancel = true;
            }
        }

        private void textSecond_Validating(object sender, CancelEventArgs e)
        {
            int result;
            if (!int.TryParse(textSecond.Text, out result) || result < 0 || result >= 60)
            {
                textSecond.SelectAll();
                e.Cancel = true;
            }
        }

        private void textTick_Validating(object sender, CancelEventArgs e)
        {
            int result;
            if (!int.TryParse(textTick.Text, out result) || result < 0)
            {
                textTick.SelectAll();
                e.Cancel = true;
            }
        }
    }
}