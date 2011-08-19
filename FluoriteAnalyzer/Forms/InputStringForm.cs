using System.Windows.Forms;

namespace FluoriteAnalyzer.Forms
{
    public partial class InputStringForm : Form
    {
        public InputStringForm()
        {
            InitializeComponent();
        }

        public string Message
        {
            get { return label1.Text; }

            set { label1.Text = value; }
        }

        public string Value
        {
            get { return textBox1.Text; }

            set { textBox1.Text = value; }
        }
    }
}