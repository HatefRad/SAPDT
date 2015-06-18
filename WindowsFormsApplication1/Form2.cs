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

        String DBSelected = "", serverName = "", DBPW = "", SAPPW = "";

        public Form2()
        {
            InitializeComponent();
            this.FormClosing += Form2_FormClosing;
            button1.Enabled = false;
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

            if (serverName != "" && DBPW != "" && SAPPW != "")
                button1.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (radioButton1.Checked)
            {
                DBSelected = "PRODUCTION";
            }
            if (radioButton2.Checked)
            {
                DBSelected = "TEST";
            }

            this.Hide();
            var mainForm = new Form1(DBSelected, serverName, DBPW, SAPPW);
            mainForm.Show();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            //Servername
            serverName = textBox1.Text;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            //DBPW
            DBPW = textBox2.Text;

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            //SAPPW
            SAPPW = textBox3.Text;

        }
    }
}
