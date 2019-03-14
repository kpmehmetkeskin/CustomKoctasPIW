using System;
using System.Threading;
using System.Windows.Forms;

namespace CustomKoctasPickInWatch
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            LearningForm lf = new LearningForm();
            lf.Show();
        }
    }
}
