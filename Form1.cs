using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data;


namespace smpBayerRegCode
{
    public partial class FrmMain : Form
    {
        private string gConnWmprodStr = "Provider=MSDAORA.1:;Data Source=wmrdc;User ID=wmrdc;PassWord=wmrdc";
        private string oracleconstr = "Data Source=wmrdc;User ID=wmrdc;PassWord=wmrdc";
        private refServiceHcwl.ServiceRD sh = new refServiceHcwl.ServiceRD();
        private List<string> gsplist = new List<string>();
        public static FrmMain pFrmMain = null;
   

        public FrmMain()
        {
            InitializeComponent();
            pFrmMain = this;

        }

        public string usertxt
        {
            get { return label14.Text; }
            set { label14.Text = value; }
        }

        private void BindCartonDtl()
        {
            string sql = null;
            if (radioButton4.Checked)
                sql = "select im.sku_desc as AAA,cd.batch_nbr AS BBB,cd.to_be_pakd_units as CCC,cd.carton_seq_nbr as DDD,size_desc as EEE,cd.carton_nbr as FFF ,case ch.CARTON_CREATION_CODE when 5 then '零箱' else '整箱' end  as GGG from carton_dtl cd,carton_hdr ch,item_master im where cd.sku_id=im.sku_id and cd.carton_nbr=ch.carton_nbr and cd.carton_nbr='" + textBox1.Text + "' ";
            else
                sql = "select im.sku_desc as AAA,pd.batch_nbr AS BBB,pd.orig_pkt_qty as CCC,pd.pkt_seq_nbr as DDD,size_desc as EEE,pd.pkt_ctrl_nbr as FFF ,'' as GGG  from pkt_dtl pd,item_master im where pd.sku_id=im.sku_id and pd.pkt_ctrl_nbr='" + textBox1.Text + "' ";
            
            dataGrid1.DataSource = sh.wsGetTable(sql, gConnWmprodStr);
            
            //设置列宽
            DataGridTableStyle dgts = new DataGridTableStyle();
            dgts.MappingName = "table1";//table1为WebService(sh)的wsGetTable方法传过来的表名称

            DataGridColumnStyle col1 = new DataGridTextBoxColumn();
            col1.MappingName = "AAA";//好怪！select语句中一定要as
            col1.HeaderText = "品名规格";
            col1.Width = 100;
            DataGridColumnStyle col2 = new DataGridTextBoxColumn();
            col2.MappingName = "BBB";//好怪！select语句中一定要as
            col2.HeaderText = "批号";
            col2.Width = 70;//50
            DataGridColumnStyle col3 = new DataGridTextBoxColumn();
            col3.MappingName = "CCC";//好怪！select语句中一定要as
            col3.HeaderText = "数量";
            col3.Width = 50;//30
            DataGridColumnStyle col4 = new DataGridTextBoxColumn();
            col4.MappingName = "DDD";//好怪！select语句中一定要as
            col4.HeaderText = "序号";

            DataGridColumnStyle col5 = new DataGridTextBoxColumn();
            col5.MappingName = "EEE";//好怪！select语句中一定要as
            col5.HeaderText = "商品代码";
            col5.Width = 50;//25

            DataGridColumnStyle col6 = new DataGridTextBoxColumn();
            col6.MappingName = "FFF";//好怪！select语句中一定要as
            col6.HeaderText = "出库箱号/PKT号";
            col6.Width = 0;//25

            DataGridColumnStyle col7 = new DataGridTextBoxColumn();
            col7.MappingName = "GGG";//好怪！select语句中一定要as
            col7.HeaderText = "出库箱类型";
            col7.Width = 0;//25

            dgts.GridColumnStyles.Clear();
            dgts.GridColumnStyles.Add(col1);
            dgts.GridColumnStyles.Add(col2);
            dgts.GridColumnStyles.Add(col3);
            dgts.GridColumnStyles.Add(col4);
            dgts.GridColumnStyles.Add(col5);
            dgts.GridColumnStyles.Add(col6);
            dgts.GridColumnStyles.Add(col7);
            dataGrid1.TableStyles.Clear();
            dataGrid1.TableStyles.Add(dgts);
            //==================

            textBox2.Focus();
        }

        public void asn_very()
        {
            int unit_shpd=0;
            int unit_rcvd=0;
            
            string sql_asn_very = null;
            sql_asn_very = "select ad.batch_nbr 批号,ad.units_rcvd 已收,ad.units_shpd-ad.units_rcvd 未收,ad.units_shpd 总数,qi.qi_qty 抽样数 from asn_dtl ad left join (select c.sku_id,c.batch_nbr,c.shpmt_nbr,sum(c.qi_qty) qi_qty from c_qi_inspection c group by c.sku_id,c.batch_nbr,c.shpmt_nbr) qi on qi.sku_id=ad.sku_id and qi.batch_nbr=ad.batch_nbr and qi.shpmt_nbr=ad.shpmt_nbr where ad.shpmt_nbr='" + textBox5.Text + "'";
            DataTable tabel_very = sh.wsGetTable(sql_asn_very, gConnWmprodStr);
            dataGrid3.DataSource = tabel_very;

            foreach (DataRow row_very in tabel_very.Rows)
            {
                unit_rcvd += Convert.ToInt32(row_very[1]);
                unit_shpd += Convert.ToInt32(row_very[3]);
            }

            label23.Text = unit_shpd.ToString();
            label24.Text = unit_rcvd.ToString();
            label25.Text = (unit_shpd - unit_rcvd).ToString();
            textBox5.Focus();
            textBox5.SelectAll();

        }

      

        public void asn_query()
        {   // 校验是否混批
            string sql1 = null;
            sql1 = "select 货箱号,批号,GSP号 from (select to_char(c.create_date_time,'yyyymmdd') 扫描日期,c.cntr_nbr 货箱号,c.batch_nbr 批号,c.gsp_nbr GSP号,v.整箱应扫,v.零头应扫,c.rcvd_shpmt_nbr ASN号, case when v.零头应扫<>0 and length(c.gsp_nbr)=20 then 0 when v.整箱应扫<>0 and length(c.gsp_nbr)=22 and substr(c.gsp_nbr,11,8)=lpad(c.batch_nbr,8,'0') ";
            sql1 = sql1 + "then 0 else 1 end 校验结果 from c_gsp_nbr_trkg c left join v_inbound_gsp_count v on v.CASE_NBR=c.cntr_nbr left join case_hdr ch on ch.case_nbr=c.cntr_nbr where c.i_o_flag='I' and c.whse='S00' and c.stat_code='0' and ch.stat_code<='90' and c.rcvd_shpmt_nbr='";
            sql1 = sql1 +textBox4.Text+ "') where 校验结果='1'";
             

            DataTable t1 = sh.wsGetTable(sql1, gConnWmprodStr);
            label13.Text = t1.Rows.Count.ToString();
            dataGrid2.DataSource = t1;
            
            //查询已收未收数量
            string sql2 = null;
            sql2 = "select ah.units_shpd,ah.units_rcvd from asn_hdr ah where ah.shpmt_nbr='" + textBox4.Text + "'" ;
            DataTable t2 = sh.wsGetTable(sql2, gConnWmprodStr);
            

            if (t2.Rows.Count != 0)
            {
                int i1 = Convert.ToInt32(t2.Rows[0][0]);
                int i2 = Convert.ToInt32(t2.Rows[0][1]);

                label17.Text = i2.ToString();
                label18.Text = (i1 - i2).ToString();
            }
            textBox4.Focus();
            textBox4.SelectAll();
          
          
        }

        private void LocationScan()
        {
            textBox2.Focus();
            textBox2.SelectAll();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            //sh.Credentials = System.Net.CredentialCache.DefaultCredentials;//这两句在NET 1.1或2.0下同样有效//如果WebService支持匿名访问，则用此句
           
            textBox3.Text = "";
            label14.Text = "";

            loginform mylogin = new loginform();

            mylogin.ShowDialog();

            textBox2.Focus();

            //LocationScan();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (((sender as TextBox).Text == null) || ((sender as TextBox).Text.Trim() == "")) return;

            if (e.KeyValue != 13) return;

            //if (radioButton4.Checked)
            // statusBar1.Text = sh.wsCmdScalar("select case ch.carton_creation_code when 0 then '货箱' when 1 then '托盘' when 2 then '整箱' when 3 then '整箱' when 4 then '整箱' when 5 then '零箱' end from carton_hdr ch where carton_nbr='" + (sender as TextBox).Text+"' ", gConnWmprodStr);
            //else
            //    statusBar1.Text = sh.wsCmdScalar("select case ch.carton_creation_code when 0 then '货箱' when 1 then '托盘' when 2 then '整箱' when 3 then '整箱' when 4 then '整箱' when 5 then '零箱' end from carton_hdr ch where pkt_ctrl_nbr='" + (sender as TextBox).Text + "' ", gConnWmprodStr);

            BindCartonDtl();
        }

        private void FrmMain_Closed(object sender, EventArgs e)
        {
            if (sh != null) sh.Dispose();
        }

        private void submit()
        {   
            if (CheckBox1.Checked)
            {
                int len = textBox2.Text.Length;
                if (!((len == 4) || (len == 22) || (len == 26)))
                {
                    MessageBox.Show("箱号位数错误", "提示", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
                    return;
                }

                if (label14.Text == "")
                {
                    MessageBox.Show("登录出错", "提示", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
                    return;
                }

                if (textBox2.Text.Substring(0, 8) != dataGrid1[dataGrid1.CurrentRowIndex, 4].ToString())
                {
                    MessageBox.Show("商品代码错误", "提示", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
                    return;
                }

                if (textBox2.Text.Substring(8, 10) != dataGrid1[dataGrid1.CurrentRowIndex, 1].ToString().PadLeft(10, '0'))
                {
                    //if (textBox2.Text.Substring(8, 10) != dataGrid1[dataGrid1.CurrentRowIndex, 1].ToString() + ' ' + dataGrid1[dataGrid1.CurrentRowIndex, 1].ToString().Substring(dataGrid1[dataGrid1.CurrentRowIndex, 1].ToString().Length - 2))
                   // {
                        MessageBox.Show("原箱批号有误", "提示", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
                        return;
                   // }
                }
            }

            if ((textBox2.Text.Trim() == "") || (textBox2.Text.Trim() == null))
            {
                MessageBox.Show("请输入原箱号", "提示", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
                textBox2.Focus();
                return;
            }

            string sORIG_TYPE = null;
            if (radioButton5.Checked) { sORIG_TYPE = "原箱号"; }
            else if (radioButton4.Checked) { sORIG_TYPE = "电子监管码"; }

            string NbrType = null;
            if (radioButton4.Checked) NbrType = "CARTON_NBR";
            else NbrType = "PKT_CTRL_NBR";



            

            if (checkBox2.Checked )
            {
                uploadbar.Visible = true;
                uploadbar.Maximum = gsplist.Count;

                for (int n = 0; n < gsplist.Count; n++)
                {
                    try
                    {
                        if (label14.Text == "")
                        {
                            MessageBox.Show("登录出错", "提示", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
                            return;
                        }

                        sh.wsCmd("insert into bayer_case (carton_nbr,        carton_seq_nbr,orig_nbr,          orig_type,        nbr_type,       NATIONAL_BARCODE_NO,   batch_nbr) values ('" +
                                                           dataGrid1[dataGrid1.CurrentRowIndex, 5].ToString() + "'," + dataGrid1[dataGrid1.CurrentRowIndex, 3].ToString() + ",'" + textBox2.Text + "','" + sORIG_TYPE + "','" + NbrType + "','" + gsplist[n] + "','" + dataGrid1[dataGrid1.CurrentRowIndex, 1].ToString() + "')", gConnWmprodStr);
                        // MessageBox.Show("成功！");
                        uploadbar.Value = n;
                    }
                    catch
                    {
                        DialogResult res;
                        res = MessageBox.Show("该监管码已存在,是否覆盖？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
                        if (res == System.Windows.Forms.DialogResult.Yes)
                        {
                            try
                            {
                                sh.wsCmd("update bayer_case set carton_nbr='" + dataGrid1[dataGrid1.CurrentRowIndex, 5].ToString() + "',carton_seq_nbr='" + dataGrid1[dataGrid1.CurrentRowIndex, 3].ToString() + "',orig_nbr='" + textBox2.Text + "',orig_type='" + sORIG_TYPE + "',nbr_type='" + NbrType + "',batch_nbr='" + dataGrid1[dataGrid1.CurrentRowIndex, 1].ToString() + "' where NATIONAL_BARCODE_NO='" + gsplist[n] + "'", gConnWmprodStr);
                            }
                            catch
                            {
                                gsplist.Clear();
                                label9.Text = "0";
                                uploadbar.Value = 0;
                                uploadbar.Visible = false;
                                MessageBox.Show("扫描失败", "提示", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
                                return;
                            }
                        }

                    }
                }
            }

            else
            {


                string[] gsparry = new string[gsplist.Count];

                gsparry = gsplist.ToArray();

                try
                {
                    if (label14.Text == "")
                    {
                        MessageBox.Show("登录出错", "提示", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
                        return;
                    }
                    int i = sh.gspupload_test(oracleconstr, dataGrid1[dataGrid1.CurrentRowIndex, 5].ToString(), dataGrid1[dataGrid1.CurrentRowIndex, 3].ToString(), textBox2.Text, sORIG_TYPE, NbrType, dataGrid1[dataGrid1.CurrentRowIndex, 1].ToString(),label14.Text.ToString(), gsparry);
                    MessageBox.Show("成功上传" + i.ToString() + "条记录", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
                    textBox2.Text = "";

                }
                catch (Exception e)
                {
                    MessageBox.Show("监管码有重复，确认正确后请勾选覆盖扫码进行扫码！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
                    //return;
                }
            }

            LocationScan();

            CalcOrigNum();
            gsplist.Clear();
            label9.Text = "0";
            uploadbar.Value = 0;
            uploadbar.Visible = false;
            
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (textBox3.Enabled==false)
            {
                if (e.KeyValue != 13) return;
                if (((sender as TextBox).Text == null) || ((sender as TextBox).Text.Trim() == "")) return;

                textBox3.Text = "";
                submit();
            }
            else 
            {
                if (e.KeyValue != 13) return;
                textBox3.Focus();
            }
        }

        private void menuItem2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }



        private void CalcOrigNum()
        {
            //label2.Text = sh.wsCmdScalar("select count(distinct orig_nbr) from bayer_case where CARTON_NBR='" + dataGrid1[dataGrid1.CurrentRowIndex, 5].ToString() + "'", gConnWmprodStr);
            label3.Text = sh.wsCmdScalar("select count(*) from bayer_case where CARTON_NBR='" + dataGrid1[dataGrid1.CurrentRowIndex, 5].ToString() + "' and CARTON_SEQ_NBR=" + dataGrid1[dataGrid1.CurrentRowIndex, 3].ToString() + " and ORIG_TYPE='电子监管码' ", gConnWmprodStr);
        }


        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            textBox3.Enabled = (sender as RadioButton).Checked;
        }


        private void textBox3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue != 13) return;

            //if ((textBox2.Text.Trim() == "" )|| (textBox2.Text.Trim() == null))
            //{
            //    MessageBox.Show("请输入原箱号", "提示", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
            //    textBox2.Focus();
            //    return;
            //}

            //if ((textBox3.Text.Trim() == "") || (textBox3.Text.Trim() == null))
            //{
            //    MessageBox.Show("请输入电子监管码", "提示", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
            //    textBox2.Focus();
            //    return;
            //}

            ////如果是整箱，不能扫电子监管码

            //if (dataGrid1[dataGrid1.CurrentRowIndex, 6].ToString() == "整箱") 
            //{
            //    MessageBox.Show("整箱，不需要输入电子监管码", "提示", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
            //    return;
            //}

            //string tempstr = textBox3.Text.Trim();

            if (textBox3.Text.Length != 20)
            {
                MessageBox.Show("电子监管码位数不对", "提示", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
                textBox2.Focus();
                return;
            }


            
            if (gsplist.Contains(textBox3.Text))
            {
                textBox3.SelectAll();
                MessageBox.Show("监管码重复", "提示", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
                return;
            }
            gsplist.Add(textBox3.Text);
            label9.Text = gsplist.Count.ToString();
            
            

            //submit();
            textBox3.Text = "";
            textBox3.Focus();
        }

        private void label1_ParentChanged(object sender, EventArgs e)
        {

        }

        private void label6_ParentChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            submit();
        }

        private void label5_ParentChanged(object sender, EventArgs e)
        {

        }

        private void label3_ParentChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            gsplist.Clear();
            textBox3.Text = "";
            textBox3.Focus();
            label9.Text = "0";
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (tabControl1.SelectedIndex==0)
            {
                textBox1.Focus();
            }
            if (tabControl1.SelectedIndex == 1)
            {
                textBox4.Focus();
            }
            if (tabControl1.SelectedIndex == 2)
            {
                textBox5.Focus();
            }
        }

        

        private void tabPage1_GotFocus(object sender, EventArgs e)
        {
            textBox1.Focus(); 
        }

        private void textBox4_KeyDown(object sender, KeyEventArgs e)
        {
            if (((sender as TextBox).Text == null) || ((sender as TextBox).Text.Trim() == "")) return;

            if (e.KeyValue != 13) return;

            asn_query();

           

            
        }

        private void label14_EnabledChanged(object sender, EventArgs e)
        {

        }

        private void label15_ParentChanged(object sender, EventArgs e)
        {

        }

        private void textBox5_KeyDown(object sender, KeyEventArgs e)
        {
            if (((sender as TextBox).Text == null) || ((sender as TextBox).Text.Trim() == "")) return;

            if (e.KeyValue != 13) return;

            asn_very();

        }

     

    }
}

/*
 工具->选项->设备工具，可以设置模拟器、默认连接设备、设备屏幕大小等
 
 RF在通过USB线连接调试的过程中，wsCmdScalar总是报错"无法从传输连接中读取数据".只能拿起来运行才行！
*/