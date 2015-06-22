using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SAPbobsCOM;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {

        String DBSel, ServerName, SAPSec, DBSec;

        public Form1(String DBSelected, String SNAME, String SAPPW, String DBPW)
        {
            InitializeComponent();

            this.FormClosing += Form1_FormClosing;

            String noticeUno = "\n\tPlease select your CSV file to import in SAP\n\n";
            String noticeDue = "\n\n\tThe columns must be separated with \" ; \" symbol! \n\n";

            richTextBox3.Text = System.Environment.NewLine + "\t\t----------------------------------------------";
            richTextBox3.Text += System.Environment.NewLine + "\t\t Selected database: " + DBSelected;
            richTextBox3.Text += System.Environment.NewLine + "\t\t Selected server: " + SNAME;
            richTextBox3.Text += System.Environment.NewLine + "\t\t---------------------------------------------";
            richTextBox3.Text += System.Environment.NewLine;
            richTextBox3.AppendText(noticeUno);
            richTextBox3.Text += "\t\t\t\t!!! NOTICE !!!";
            richTextBox3.AppendText(noticeDue);

            ServerName = SNAME;
            SAPSec = SAPPW;
            DBSec = DBPW;
            
            if (DBSelected == "production")
                DBSel = "SBO1130_DIA";
            else
                DBSel = "SBO1131_DIA";
        }

        private void Form1_FormClosing(Object sender, FormClosingEventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) // Test result.
            {
                String file = openFileDialog1.FileName;
                
                try
                {
                    richTextBox3.Hide();

                    String noticeThree = "\n\t!!!  NOTICE The columns must be separated with \" ; \" symbol! \r\n\n";
                    String noticeFour = "\n\t!!!  You can verify the content of your file below: \r\n\n";
                    
                    richTextBox1.Text = System.Environment.NewLine;
                    richTextBox1.AppendText(noticeThree);
                    richTextBox1.AppendText(noticeFour);
                    richTextBox1.Text += System.IO.File.ReadAllText(file);
                    button1.Text = "File is loaded.";
                    button1.Enabled = false;
                    button6.Enabled = true;
                    button2.Enabled = true;
                    button2.Tag = file;
                }
                catch (IOException)
                {
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

            DialogResult dialogResult = MessageBox.Show("This action will import your file in database.\nAre you sure you want to proceed?", "WARNING", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                richTextBox2.Text = "\r\n---------------------------------------------------------------------";
                richTextBox2.Text += "\r\n      PLEASE WAIT !!!     \r\n";
                String file = ((string)((Button)sender).Tag);
                Program(file, DBSel, ServerName, SAPSec, DBSec);   
            }
            else if (dialogResult == DialogResult.No)
            {
                return;
            }
        }
        
        private void Program(String file, String DBSelected, String serverName, String SAPPW, String DBPW)
        {
            Company oCom = new Company();

            oCom.Server = serverName;
            oCom.LicenseServer = serverName;
            oCom.CompanyDB = DBSelected;
            oCom.DbServerType = BoDataServerTypes.dst_MSSQL2008;
            oCom.UserName = "manager";
            oCom.Password = SAPPW;
            oCom.UseTrusted = false;
            oCom.DbUserName = "sa";
            oCom.DbPassword = DBPW;

            try
            {
                oCom.Connect();

                if (!oCom.Connected)
                {
                    richTextBox2.Text += "\r\n   Unable to connect to DB: " + DBSelected + ". Please contact DEV.\r\n";
                    throw new Exception(oCom.GetLastErrorDescription());
                }

                StringBuilder sb = new StringBuilder();

                richTextBox2.Text += "\r\n   Connected to " + DBSelected + "..\r\n";
                richTextBox2.Text += "\r\n      Processing the description file..\r\n";
                richTextBox2.Text += "---------------------------------------------------------------------";
                foreach (string line in System.IO.File.ReadAllLines(file))
                {
                    string TableName, SKU, FieldAlias, LangCode, Trans;
                    string TranEntry;

                    string[] fields = line.Split(';');

                    TableName = fields[0];
                    SKU = fields[1];
                    FieldAlias = fields[2];
                    LangCode = fields[3];
                    Trans = fields[4];

                    String Lang = "";
                    String TransType = "";

                    if (FieldAlias == "U_DescAggMod")
                        TransType = "short description";
                    else
                        TransType = "description";

                    switch (LangCode)
                    {
                        default:
                            Lang = "Other";
                            break;
                        case "3":
                            Lang = "English";
                            break;
                        case "13":
                            Lang = "Italian";
                            break;
                        case "9":
                            Lang = "German";
                            break;
                        case "22":
                            Lang = "French";
                            break;
                        case "23":
                            Lang = "Spanish";
                            break;
                        case "30":
                            Lang = "Japanese";
                            break;
                    }

                    Recordset oRec = (SAPbobsCOM.Recordset)oCom.GetBusinessObject(BoObjectTypes.BoRecordset);

                    oRec.DoQuery(string.Format("select ItemCode from OITM where ItemName = '{0}'", SKU));
                    if (oRec.RecordCount > 0)
                        oRec.MoveFirst();
                    else
                    {
                        richTextBox2.Text += Environment.NewLine + "ItemName not FOUND!!!";
                        throw new Exception("ItemName Not FOUND!!!");
                    }

                    string ItemCode = (string)oRec.Fields.Item("ItemCode").Value;

                    oRec.DoQuery(string.Format("select TranEntry from OMLT where TableName = '{0}' and FieldAlias = '{1}' and PK = '{2}'",
                        TableName, FieldAlias, ItemCode));

                    if (oRec.RecordCount > 0)
                    {
                        oRec.MoveFirst();
                        TranEntry = oRec.Fields.Item("TranEntry").Value.ToString();
                    }
                    else
                    {
                        TranEntry = "-1";
                    }

                    MultiLanguageTranslations oMlt = (SAPbobsCOM.MultiLanguageTranslations)oCom.GetBusinessObject(BoObjectTypes.oMultiLanguageTranslations);

                    if (TranEntry == "-1")
                    {

                        oMlt.TableName = TableName;
                        oMlt.FieldAlias = FieldAlias;
                        oMlt.PrimaryKeyofobject = ItemCode;

                        oMlt.TranslationsInUserLanguages.LanguageCodeOfUserLanguage = System.Convert.ToInt32(LangCode);
                        oMlt.TranslationsInUserLanguages.Translationscontent = Trans;

                        // tranentry not found
                        if (oMlt.Add() != 0)
                            throw new Exception(oCom.GetLastErrorDescription());

                        sb.Insert(0, string.Format("\n{0} {1} for SKU: {2} has been processed.", Lang, TransType, SKU));
                        richTextBox2.Text += Environment.NewLine;
                        richTextBox2.Text += sb;
                    }
                    else
                    {
                        if (!oMlt.GetByKey(System.Convert.ToInt32(TranEntry)))
                            throw new Exception("OMLT NOT FOUND !!!");

                        bool TransFound = false;
                        for (int i = 0; i < oMlt.TranslationsInUserLanguages.Count; i++)
                        {
                            oMlt.TranslationsInUserLanguages.SetCurrentLine(i);

                            if (oMlt.TranslationsInUserLanguages.LanguageCodeOfUserLanguage == System.Convert.ToInt32(LangCode))
                            {
                                TransFound = true;
                                if (oMlt.TranslationsInUserLanguages.Translationscontent != Trans)
                                {
                                    oMlt.TranslationsInUserLanguages.Translationscontent = Trans;
                                    if (oMlt.Update() != 0)
                                        throw new Exception(oCom.GetLastErrorDescription());

                                    sb.Insert(0, string.Format("\n{0} {1} for SKU: {2} has been updated.", Lang, TransType, SKU));
                                    richTextBox2.Text += Environment.NewLine;
                                    richTextBox2.Text += sb;
                                    break;
                                }
                            }
                        }

                        if (!TransFound)
                        {
                            oMlt.TranslationsInUserLanguages.Add();
                            oMlt.TranslationsInUserLanguages.LanguageCodeOfUserLanguage = System.Convert.ToInt32(LangCode);
                            oMlt.TranslationsInUserLanguages.Translationscontent = Trans;
                            if (oMlt.Update() != 0)
                                throw new Exception(oCom.GetLastErrorDescription());

                            sb.Insert(0, string.Format("\n{0} {1} for SKU: {2} has been inserted.", Lang, TransType, SKU));
                            richTextBox2.Text += Environment.NewLine;
                            richTextBox2.Text += sb;
                        }
                    }
                }
                richTextBox2.Text += Environment.NewLine + "\nDescriptions are successfully loaded in SAP !!!";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                richTextBox2.Text += "\r\n\t\t !!! There was an error. Please try again or contact DEV";
            }
            finally
            {
                if (oCom.Connected)
                    oCom.Disconnect();
                MessageBox.Show("Finished!");
                button1.Enabled = true;
                button1.Text = "Select CSV File";
                button2.Enabled = false;
                button6.Enabled = false;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            String dev = "dev@dianacorp.com";
            MessageBox.Show("For support please contact: \r\n" + dev, "Ver. 0.1");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Hide();
            var DBForm = new Form2();
            DBForm.Show();
        }

        private void richTextBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            richTextBox3.Show();
            richTextBox1.Clear();
            button1.Text = "Select CSV File";
            button1.Enabled = true;
            button2.Enabled = false;
            button6.Enabled = false;
        }
    }
}