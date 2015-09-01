using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace smpBayerRegCode
{
    public partial class loginform : Form
    {

        private refServiceHcwl.ServiceRD sh = new refServiceHcwl.ServiceRD();
      

        public loginform()
        {
            InitializeComponent();
           
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int j = Convert.ToInt16(sh.wsCmdScalar("select count(*) from ysfs where type='职员' and UPPER(ysid)='" + textBox1.Text.ToUpper() + "' and UPPER(nvl(RESERVE1,'!@#$%^&*()'))='" + (textBox2.Text == "" ? "!@#$%^&*()" : textBox2.Text).ToString() + "'", "Provider=MSDAORA.1:;Data Source=wmrdc;User ID=wmrdc;PassWord=wmrdc"));
            if (j == 0)
            //if (!(textBox1.Text=="1" && textBox2.Text=="1"))
            {
                MessageBox.Show("密码不正确，请重新输入", "提示", MessageBoxButtons.OK, MessageBoxIcon.Question,MessageBoxDefaultButton.Button1);
                textBox2.Focus();
                textBox2.SelectAll();
                return;
            }
            FrmMain.pFrmMain.usertxt = textBox1.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();

            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void loginform_Closing(object sender, CancelEventArgs e)
        {
            if (this.DialogResult != DialogResult.Cancel && this.DialogResult != DialogResult.OK)

                e.Cancel = true;
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            
        }

        private void loginform_KeyPress(object sender, KeyPressEventArgs e)
        {
       
            
        }

        private void loginform_Load(object sender, EventArgs e)
        {
            textBox1.Focus();
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                int j = Convert.ToInt16(sh.wsCmdScalar("select count(*) from ysfs where type='职员' and UPPER(ysid)='" + textBox1.Text.ToUpper() + "' and UPPER(nvl(RESERVE1,'!@#$%^&*()'))='" + (textBox2.Text == "" ? "!@#$%^&*()" : textBox2.Text).ToString() + "'", "Provider=MSDAORA.1:;Data Source=wmrdc;User ID=wmrdc;PassWord=wmrdc"));
                if (j == 0)
               // if (!(textBox1.Text=="1" && textBox2.Text=="1"))
                {
                    MessageBox.Show("密码不正确，请重新输入", "提示", MessageBoxButtons.OK, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                    textBox2.Focus();
                    textBox2.SelectAll();
                    return;
                }
                FrmMain.pFrmMain.usertxt = textBox1.Text;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                textBox1.Text = textBox1.Text.ToUpper();
                textBox2.Focus();
                textBox2.SelectAll();
            }
        }
    }
}