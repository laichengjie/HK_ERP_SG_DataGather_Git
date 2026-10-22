using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;
using DevExpress.XtraEditors;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.IO;
using System.Text.RegularExpressions;

namespace ConnectSetting
{
    public partial class FormMain : Form
    {
        string filePath = System.Environment.CurrentDirectory + @"\ConnectConfig.DAT";
        string connString = string.Empty;
        private int currentIndex = 0; //当前选中数据库类型索引

        private bool isEdit = false;//是否有修改
        private bool isNeedTest = false; //保存时判断是否要去测试连接
        private XElement xlRoot;  //保存时的xml的根节点
        private XDocument xdoc; //xml文档

        public FormMain()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            InitializeComponent();
            tabControl.ShowTabHeader =  DevExpress.Utils.DefaultBoolean.False;
            
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            InitLoginType();
            
            //加载xml
            if (File.Exists(filePath))
            {
                xdoc = XDocument.Load(filePath);
            }
            else
            {
                xdoc = new XDocument(
                    new XDeclaration("1.0", System.Text.Encoding.Default.ToString(), "yes"),
                    new XElement("HK",
                        new XAttribute("DESC","恒康数据提报平台－帐套连接信息")));

                foreach (Database data in Enum.GetValues(typeof(Database)))
                {
                    xdoc.Root.Add(
                        new XElement(data.ToString(),
                            new XComment(GetDatabaseString(data)),
                            new XElement("Server", string.Empty),
                            new XElement("LoginType", string.Empty),
                            new XElement("UserID", string.Empty),
                            new XElement("Password", string.Empty),
                            new XElement("GroupDB", string.Empty)
                            ));
                }
            }
            xlRoot = xdoc.Root;

            //默认显示Web正式库
            rdgDatabase.SelectedIndex = 0;
            currentIndex = 0;
            SetControl((Database)(currentIndex + 1));
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            //if (rdgDatabase.SelectedIndex == 0) //ado
            //{
            //    if (!CheckDataAdo())
            //        return;
            //}
            //else if (rdgDatabase.SelectedIndex == 1)//wcf
            //{
            //    if (!CheckData())
            //        return;
            //    if (isNeedTest)
            //    {
            //        if (!TestURL())
            //        {
            //            if (MessageBox.Show("存在失败连接，是否仍继续保存？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            //            {
            //                return;
            //            }
            //        }
            //    }
            //}
            //try
            //{
            //    //保存XML
            //    XName xn = "HK";
            //    string callTypeValue=Encrypt.EncryptString("Ado");
            //    string loginType=Encrypt.EncryptString("Sql");
            //    if (rdgDatabase.SelectedIndex==1)
            //    {
            //        callTypeValue=Encrypt.EncryptString("Wcf");
            //    }
            //    if (lueLoginType.EditValue!=null && lueLoginType.EditValue.ToString().Trim() == "0")
            //    {
            //        loginType=Encrypt.EncryptString("Win");
            //    }
            //    XDocument xDoc = new XDocument(
            //            new XDeclaration("1.0", System.Text.Encoding.Default.ToString(), "yes"),
            //            new XElement(xn,
            //                new XElement("CallType",
            //                    new XComment("通迅方式"),
            //                    new XElement("CallTypeValue", callTypeValue)),
            //                new XElement("Ado",
            //                    new XComment("Ado连接方式"),
            //                    new XElement("Server", Encrypt.EncryptString(txtServer.Text.Trim())),
            //                    new XElement("LoginType", loginType),
            //                    new XElement("UserID", Encrypt.EncryptString(txtUser.Text.Trim())),
            //                    new XElement("Pwd", Encrypt.EncryptString(txtPwd.Text.Trim())),
            //                    new XElement("GroupDB", Encrypt.EncryptString(lueDB.Text.Trim()))),
            //                new XElement("Web",
            //                    new XComment("Web Service方式"),
            //                    new XElement("CertServer", txtCertServer.Text.Trim()),
            //                    new XElement("Port", txtPort.Text.Trim()),
            //                    new XElement("Master",chk_Master.Checked),
            //                    new XElement("CertServer2",txtCertServer2.Text.Trim()),
            //                    new XElement("Port2",txtPort2.Text.Trim()),
            //                    new XElement("Master2", chk_Master2.Checked),
            //                    new XElement("VirtualDirectory", txtVirtualDirectory.Text.Trim()),
            //                    new XComment("验证方式:0不验证 1密码验证 2证书"),
            //                    new XElement("IdentifyType", rdoType.SelectedIndex.ToString()),
            //                    new XElement("CertName", txtCertName.Text.Trim()),
            //                    new XElement("CertPassword", txtCertPassword.Text.Trim()),
            //                    new XComment("证书路径"),
            //                    new XElement("Certificate", btnCertificate.Text.Trim())),
            //                new XElement("Other",
            //                        new XComment("修改时间"),
            //                        new XElement("ModifyDTM", DateTime.Now.ToString()))));

            //    string str = xDoc.Declaration + "\n";
            //    str += xDoc;
            //    xlElement = XElement.Parse(str);
            //    xlElement.Save(filePath);
            //    isEdit = false;
            //    isNeedTest = false;
            //    MessageBox.Show("保存成功！", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //}
            //catch (Exception msg)
            //{
            //    MessageBox.Show("保存失败："+msg.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}

            //点击保存时需要保存当前的配置
            this.SaveControl((Database)(currentIndex + 1));
            
            try
            {
                string message = "";
                foreach (Database data in Enum.GetValues(typeof(Database)))
                {
                    string nodeName = data.ToString();
                    if (xlRoot.Element(nodeName) == null ||
                        xlRoot.Element(nodeName).Element("Server") == null ||
                        string.IsNullOrEmpty(xlRoot.Element(nodeName).Element("Server").Value))
                    {
                        message += this.GetDatabaseString(data) + "、";
                    }
                }

                if (!string.IsNullOrEmpty(message))
                {
                    DialogResult dr = MessageBox.Show("您还没有配置" + message + "的连接信息，确定保存吗？", string.Empty, MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                    if (dr == System.Windows.Forms.DialogResult.OK)
                    {
                        xdoc.Save(filePath);
                        MessageBox.Show("保存成功！", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else //全部配置完，直接保存
                {
                    xdoc.Save(filePath);
                    MessageBox.Show("保存成功！", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("保存失败：" + ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #region 自定义方法

        /// <summary>
        /// 检查必填栏ado
        /// </summary>
        /// <returns></returns>
        private bool CheckDataAdo()
        {
            if (txtServer.Text.Trim() == string.Empty)
            {
                MessageBox.Show("服务器不能为空！", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtServer.Focus();
                return false;
            }
            if (lueLoginType.Text.Trim() == string.Empty)
            {
                MessageBox.Show("身份验证不能为空！", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                lueLoginType.Focus();
                return false;
            }
            if (lueLoginType.EditValue.ToString().Trim()=="1")
            {
                if (txtUser.Text.Trim() == string.Empty)
                {
                    if (txtUser.Text.Trim() == string.Empty)
                    {
                        MessageBox.Show("登录名不能为空！", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        txtUser.Focus();
                        return false;
                    }
                    return false;
                }
            }
            if (lueDB.Text.Trim() == string.Empty)
            {
                if (lueDB.Text.Trim() == string.Empty)
                {
                    MessageBox.Show("请选择数据库！", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    lueDB.Focus();
                    return false;
                }
                return false;
            }
            return true;
        }

        /// <summary>
        /// 根据数据库类型设置控件显示的值
        /// </summary>
        private void SetControl(Database data)
        {
            if (xlRoot == null) return;
            string nodeName = data.ToString();
            XElement xlNode = xlRoot.Element(nodeName);//当前要保存的节点
            if (xlNode == null)
            {
                xlNode = new XElement(nodeName, new XComment(nodeName + "连接信息"));
                xlRoot.Add(xlNode); //添加子级
            }

            try
            {
                //服务器
                XElement xlServer = xlNode.Element("Server");
                if (xlServer == null)
                {
                    xlServer = new XElement("Server");
                    xlNode.Add(xlServer);
                }
                txtServer.Text = SecurityHelper.DecryptDes(xlServer.Value);

                //登录类型
                XElement xlLogin = xlNode.Element("LoginType");
                if (xlLogin == null)
                {
                    xlLogin = new XElement("LoginType");
                    xlNode.Add(xlLogin);
                }
                string loginType = SecurityHelper.DecryptDes(xlLogin.Value);
                lueLoginType.EditValue = "0";
                if (loginType == "Sql") lueLoginType.EditValue = "1";

                //用户名
                XElement xlUser = xlNode.Element("UserID");
                if (xlUser == null)
                {
                    xlUser = new XElement("UserID");
                    xlNode.Add(xlUser);
                }
                txtUser.Text = SecurityHelper.DecryptDes(xlUser.Value);
                //密码
                XElement xlPassword = xlNode.Element("Password");
                if (xlPassword == null)
                {
                    xlPassword = new XElement("Password");
                    xlNode.Add(xlPassword);
                }
                txtPwd.Text = SecurityHelper.DecryptDes(xlPassword.Value);
                //数据库
                XElement xlGroupDB = xlNode.Element("GroupDB");
                if (xlGroupDB == null)
                {
                    xlGroupDB = new XElement("GroupDB");
                    xlNode.Add(xlGroupDB);
                }
                DataTable dtGroupDB = new DataTable();
                dtGroupDB.Columns.Add("Name");
                DataRow dr = dtGroupDB.NewRow();
                dr[0] = SecurityHelper.DecryptDes(xlGroupDB.Value);
                dtGroupDB.Rows.Add(dr);
                lueDB.Properties.DataSource = dtGroupDB;
                lueDB.EditValue = SecurityHelper.DecryptDes(xlGroupDB.Value);
            }
            catch
            {
                
                //throw;
            }
        }

        /// <summary>
        /// 根据控件的值保存到临时xml
        /// </summary>
        /// <param name="data"></param>
        private void SaveControl(Database data)
        {
            if (xlRoot == null) return;
            string nodeName = data.ToString();
            XElement xlNode = xlRoot.Element(nodeName);//当前要保存的节点
            string loginType = SecurityHelper.EncryptDes("Sql");
            if (lueLoginType.EditValue != null && lueLoginType.EditValue.ToString().Trim() == "0")
            {
                loginType = SecurityHelper.EncryptDes("Win");
            }

            xlNode.Element("Server").Value = SecurityHelper.EncryptDes(txtServer.Text);
            xlNode.Element("LoginType").Value = loginType;
            xlNode.Element("UserID").Value = SecurityHelper.EncryptDes(txtUser.Text);
            xlNode.Element("Password").Value = SecurityHelper.EncryptDes(txtPwd.Text);
            xlNode.Element("GroupDB").Value = SecurityHelper.EncryptDes(lueDB.Text);
        }

        /// <summary>
        /// 保存配置文件
        /// </summary>
        /// <param name="data"></param>
        private void SaveConfig(Database data)
        {
            //保存Xml
            XName xn = "HK";
            string nodeName = data.ToString();
            string loginType = SecurityHelper.EncryptDes("Sql");
            if (lueLoginType.EditValue != null && lueLoginType.EditValue.ToString().Trim() == "0")
            {
                loginType = SecurityHelper.EncryptDes("Win");
            }
            XDocument xDoc = new XDocument(
                    new XDeclaration("1.0", System.Text.Encoding.Default.ToString(), "yes"),
                    new XElement(xn,
                        new XElement(nodeName,
                            new XComment(nodeName + "连接信息"),
                            new XElement("Server", SecurityHelper.EncryptDes(txtServer.Text.Trim())),
                            new XElement("LoginType", loginType),
                            new XElement("UserID", SecurityHelper.EncryptDes(txtUser.Text.Trim())),
                            new XElement("Pwd", SecurityHelper.EncryptDes(txtPwd.Text.Trim())),
                            new XElement("GroupDB", SecurityHelper.EncryptDes(lueDB.Text.Trim()))),
                        new XElement("Other",
                                new XComment("修改时间"),
                                new XElement("ModifyDTM", DateTime.Now.ToString()))));

            string str = xDoc.Declaration + "\n";
            str += xDoc;
            xlRoot = XElement.Parse(str);
            xlRoot.Save(filePath);
        }

        #endregion

        #region Ado处理

        /// <summary>
        /// 初始化身份验证类型
        /// </summary>
        private void InitLoginType()
        {
            DataTable dtType = new DataTable(); //身份验证类型
            dtType.Columns.Add("TypeCode", typeof(string));
            dtType.Columns.Add("TypeName", typeof(string));
            DataRow dr = dtType.NewRow();
            dr[0] = "0";
            dr[1] = "Windows 身份验证";
            dtType.Rows.Add(dr);

            dr = dtType.NewRow();
            dr[0] = "1";
            dr[1] = "SQL Server 身份验证";
            dtType.Rows.Add(dr);

            lueLoginType.Properties.DisplayMember = "TypeName";
            lueLoginType.Properties.ValueMember = "TypeCode";
            lueLoginType.Properties.DataSource = dtType;

            lueDB.Properties.DisplayMember = "Name";
            lueDB.Properties.ValueMember = "Name";
        }

        private void lueLoginType_EditValueChanged(object sender, EventArgs e)
        {
            if (lueLoginType.EditValue.ToString().Trim() == "0")
            {
                txtUser.Text = string.Empty;
                txtUser.Enabled = false;
                txtPwd.Text = string.Empty;
                txtPwd.Enabled = false;
            }
            else
            {
                txtUser.Enabled = true;
                txtPwd.Enabled = true;
            }
        }

        private void btnTestDB_Click(object sender, EventArgs e)
        {
            if (CheckNetwork())
            {
                MessageBox.Show("连接成功!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            else
            {
                if (this.lueLoginType.EditValue!=null && this.lueLoginType.EditValue.ToString().Trim() == "0")
                {
                    MessageBox.Show("连接失败，请检查当前登录Windows用户是否具有访问数据库的权限!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
                else
                {
                    MessageBox.Show("连接失败，请检查当前网络是否正常或登录用户是否正确!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            }
        }

        private void btnRefreshDB_Click(object sender, EventArgs e)
        {
            if (CheckNetwork())
            {
                lueDB.Properties.DataSource = GetDB();
            }
            else
            {
                MessageBox.Show("连接数据库失败，请检查数据库配置!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }


        private void rdgDatabase_SelectedIndexChanged(object sender, EventArgs e)
        {
            RadioGroup group = sender as RadioGroup;
            if (group == null) return;

            this.SaveControl((Database)(currentIndex + 1));
            this.SetControl((Database)(group.SelectedIndex + 1));
            currentIndex = group.SelectedIndex;
        }

        /// <summary>
        /// 检查数据库连接
        /// </summary>
        /// <returns></returns>
        private bool CheckNetwork()
        {
            if (this.lueLoginType.EditValue!=null && this.lueLoginType.EditValue.ToString().Trim() == "0")
            {
                connString = string.Format("Data Source={0};Initial Catalog=master;Integrated Security=True", this.txtServer.Text);
            }
            else
            {
                connString = string.Format("Data Source={0};Initial Catalog=master;User ID={1};Password={2};pooling = true; Max Pool Size = 15; Min Pool Size = 10;Timeout = 900", new object[] { this.txtServer.Text, this.txtUser.Text, this.txtPwd.Text });
            }
            try
            {
                SqlConnection connection = new SqlConnection();
                connection.ConnectionString = connString;
                connection.Open();
                connection.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 获取所有的数据库
        /// </summary>
        /// <returns></returns>
        private DataTable GetDB()
        {
            DataTable dt = new DataTable();
            SqlConnection cn = new SqlConnection(connString);
            cn.Open();
            SqlDataAdapter adt = new SqlDataAdapter("Select Name From master.dbo.sysdatabases", cn);
            adt.Fill(dt);
            cn.Close();
            return dt;
        }

        /// <summary>
        /// 取枚举字符
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private string GetDatabaseString(Database data)
        {
            switch (data)
            {
                case Database.Web :
                    return "平台正式库";

                case Database.WebTest:
                    return "平台测试库";

                case Database.ERP:
                    return "ERP正式库";

                case Database.ERPTest:
                    return "ERP测试库";

                default :
                    return string.Empty;
            }
        }

        #endregion

        

       
    }

    /// <summary>
    /// 数据库
    /// </summary>
    public enum Database
    {
        /// <summary>
        /// 平台正式库
        /// </summary>
        Web = 1,

        /// <summary>
        /// 平台测试库
        /// </summary>
        WebTest = 2,

        /// <summary>
        /// ERP正式库
        /// </summary>
        ERP = 3,

        /// <summary>
        /// ERP测试库
        /// </summary>
        ERPTest = 4
    }

    public class SecurityHelper
    {
        private const string defaultKey = "Yuanyuan";
        private static byte[] keys = Encoding.Default.GetBytes("Yuanyuan"); //Keys长度必须为８位

        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="decryptString">要解密的字符串</param>
        /// <returns></returns>
        public static string DecryptDes(string decryptString)
        {
            return DecryptDes(decryptString, defaultKey);
        }

        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="decryptString">要解密的字符串</param>
        ///  <param name="decryptKey">密钥，必须为８位?</param>
        /// <returns></returns>
        public static string DecryptDes(string decryptString, string decryptKey)
        {
            //规定密钥只能是数字与字母的组合，并且长度不得小于８位
            Regex regex = new Regex(@"^[0-9a-zA-Z]{8,}");
            if (!regex.IsMatch(decryptKey)) throw new ArgumentOutOfRangeException("decryptKey", "密钥必须由字母或者数字组成，并且长度不得小于８位！");

            try
            {
                using (DESCryptoServiceProvider provider = new DESCryptoServiceProvider())
                {
                    byte[] inputByte = Convert.FromBase64String(decryptString);
                    byte[] inputKey = Encoding.Default.GetBytes(decryptKey.Substring(0, 8));
                    byte[] inputIV = Encoding.Default.GetBytes(decryptKey);

                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, provider.CreateDecryptor(keys, inputIV), CryptoStreamMode.Write))
                        {
                            cs.Write(inputByte, 0, inputByte.Length);
                            cs.FlushFinalBlock();
                            return Encoding.Default.GetString(ms.ToArray());
                        }
                    }
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }

        }

        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="encryptString">要加密的字符串</param>
        /// <returns></returns>
        public static string EncryptDes(string encryptString)
        {
            return EncryptDes(encryptString, defaultKey);
        }

        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="encryptString">加密的字符串</param>
        /// <returns></returns>
        public static string EncryptDes(string encryptString, string encryptKey)
        {
            //规定密钥只能是数字与字母的组合，并且长度不得小于８位
            Regex regex = new Regex(@"^[0-9a-zA-Z]{8,}");
            if (!regex.IsMatch(encryptKey)) throw new ArgumentOutOfRangeException("decryptKey", "密钥必须由字母或者数字组成，并且长度不得小于８位！");

            try
            {
                using (DESCryptoServiceProvider provider = new DESCryptoServiceProvider())
                {
                    byte[] inputByte = Encoding.Default.GetBytes(encryptString);
                    byte[] inputKey = Encoding.Default.GetBytes(encryptKey.Substring(0, 8));
                    byte[] inputIV = Encoding.Default.GetBytes(encryptKey);

                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, provider.CreateEncryptor(keys, inputIV), CryptoStreamMode.Write))
                        {
                            cs.Write(inputByte, 0, inputByte.Length);
                            cs.FlushFinalBlock();
                            return Convert.ToBase64String(ms.ToArray());
                        }
                    }
                }
            }
            catch (Exception)
            {

                return string.Empty;
            }
        }
    }
}
