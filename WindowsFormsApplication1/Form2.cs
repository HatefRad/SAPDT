using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            this.FormClosing += Form2_FormClosing;
        }

        private void Form2_FormClosing(Object sender, FormClosingEventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }
        
        private void domainUpDown1_SelectedItemChanged(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            String DBSelected = "";

            if (radioButton1.Checked)
            {
                DBSelected = "PRODUCTION";
            }
            if (radioButton2.Checked)
            {
                DBSelected = "TEST";
            }

            this.Hide();
            var mainForm = new Form1(DBSelected);
            mainForm.Show();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
