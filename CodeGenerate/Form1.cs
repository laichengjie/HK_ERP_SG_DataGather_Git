using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Reflection;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace CodeGenerate
{
    public partial class Form1 : Form
    {
        private readonly string rootPath = Environment.CurrentDirectory;
        private readonly BindingFlags flags = BindingFlags.Instance | BindingFlags.Public;
        private string tab = "    ";
        private string enter = Environment.NewLine;
        private string doubleEnter = Environment.NewLine + Environment.NewLine;
        private string fourEnter = Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine;
        private string[] basClass = new string[] { "object", "int", "string", "byte", "decimal", "long", "short" ,"bool","datetime"}; //基本类型
        Assembly assembly = null;
        string absFile = null;
        string assemblyRootPath = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void miOpenAssembly_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "程序集|*.dll";
                dialog.CheckFileExists = true;
                dialog.CheckPathExists = true;
                dialog.AddExtension = true;
                dialog.DefaultExt = ".dll";
                dialog.Multiselect = false;
                dialog.RestoreDirectory = true;

                DialogResult dr = dialog.ShowDialog();
                if (dr == System.Windows.Forms.DialogResult.OK)
                {
                    assembly = Assembly.LoadFrom(dialog.FileName);
                    assemblyRootPath = Path.Combine(rootPath, Path.GetFileNameWithoutExtension(dialog.FileName));
                    if (!Directory.Exists(assemblyRootPath)) Directory.CreateDirectory(assemblyRootPath);
                    comboBox1.DataSource = assembly.GetTypes();
                }
            }
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            if (comboBox1.DataSource == null ||
                comboBox1.SelectedIndex == -1) return;

            lnkOpenFile.Text = "开始生成...";
            Type type = comboBox1.SelectedItem as Type;
            
            
            StringBuilder sb = new StringBuilder();
            sb.Append("using System;" + doubleEnter);
            sb.Append("namespace " + (string.IsNullOrEmpty(txtNameSpace.Text) ? type.Namespace : txtNameSpace.Text) + enter);
            sb.Append("{" + enter);
            sb.Append(tab);
            sb.Append("public ");
            if (chkPartial.Checked) sb.Append("partial ");
            sb.Append("class ");
            string className = string.IsNullOrEmpty(txtClassName.Text) ? type.Name : txtClassName.Text;
            sb.Append(className + " ");
            if (!string.IsNullOrEmpty(txtHeric.Text)) sb.Append(": " + txtHeric.Text);
            sb.Append(enter + tab);
            sb.Append("{ ");
            sb.Append(doubleEnter);
            sb.Append(tab + tab);
            sb.Append("#region 私有字段");
            sb.Append(fourEnter);
            sb.Append(tab + tab);
            sb.Append("#endregion ");
            sb.Append(doubleEnter);
            sb.Append(tab + tab);
            sb.Append("#region 公开属性");

            if (chkProperty.Checked)
            {
                foreach (PropertyInfo info in type.GetProperties(flags))
                {
                    sb.Append(doubleEnter);
                    sb.Append(tab + tab);
                    string propertyTypeName = info.PropertyType.Name;
                    string propertyTypeFulleName = info.PropertyType.FullName;

                    ////处理泛型，可空型
                    //if (propertyTypeName.EndsWith("`1"))
                    if (info.PropertyType.IsGenericType)
                    {
                        //{Name = "Nullable`1" FullName = "System.Nullable`1[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]"}
                        Regex regex = new Regex(@".*\[\[(?<Type>[^,]+)", RegexOptions.IgnoreCase);
                        string generic = Regex.Replace(propertyTypeName, "`[1-9]", string.Empty); //propertyTypeName.Replace();//("`1", string.Empty);
                        Match match = regex.Match(propertyTypeFulleName);
                        if (!match.Success) throw new NotSupportedException("检测到泛型属性，但无法匹配到它的类型！");

                        string genericName = match.Groups["Type"].Value; //泛型类型，T，包括了命名空间
                        foreach (string bas in basClass)
                        {
                            if (genericName.ToLower().Contains(bas)) genericName = bas;
                        }
                        propertyTypeName = generic + "<" + genericName + ">";
                    }
                    else //处理 基本类型
                    {

                        foreach (string lower in basClass)
                        {
                            if (propertyTypeName.ToLower().Contains(lower)) propertyTypeName = lower;
                        }
                    }


                    sb.Append("public " + propertyTypeName + " " + info.Name);
                    sb.Append(" { get; set; } ");

                }
            }

            sb.Append(doubleEnter);
            sb.Append(tab + tab);
            sb.Append("#endregion");
            sb.Append(doubleEnter);
            sb.Append(tab + tab);
            sb.Append("#region 辅助方法");
            sb.Append(fourEnter);
            sb.Append(tab + tab);
            sb.Append("#endregion");
            sb.Append(doubleEnter);
            sb.Append(tab + tab);
            sb.Append("#region 重写方法");
            sb.Append(fourEnter);
            sb.Append(tab + tab);
            sb.Append("#endregion");
            sb.Append(doubleEnter);
            sb.Append(tab + tab);
            sb.Append("#region 公开方法");
            sb.Append(fourEnter);
            sb.Append(tab + tab);
            sb.Append("#endregion");
            if (!string.IsNullOrEmpty(txtHeric.Text) && txtHeric.Text.StartsWith("I")) //接口成员
            {
                sb.Append(doubleEnter);
                sb.Append(tab + tab);
                sb.Append("#region ");
                sb.Append(txtClassName.Text + " 成员");
                sb.Append(fourEnter);
                sb.Append(tab + tab);
                sb.Append("#endregion");
            }
            sb.Append(doubleEnter);
            sb.Append(tab + "}" + enter);
            sb.Append("}");


            string fileName = string.IsNullOrEmpty(txtClassName.Text) ? (type.Name + ".cs") : (txtClassName.Text + ".cs");
            absFile = Path.Combine(assemblyRootPath, fileName);
            if (File.Exists(absFile)) File.Delete(absFile);
            string content = sb.ToString();

            txtProc.Text = content;
            using (StreamWriter sw = new StreamWriter(absFile, true))
            {
                sw.Write(sb.ToString());
            }

            lnkOpenFile.Text = "打开文件";
        }

        #region 记事本API

        [DllImport("User32.DLL")]
        public static extern int SendMessage(IntPtr hWnd,uint Msg, int wParam, string lParam);

        [DllImport("User32.DLL")]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent,IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        public const uint WM_SETTEXT = 0x000C;

        #endregion

        private void lnkOpenFile_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!File.Exists(absFile)) return;

            Process vProcess = Process.Start("notepad.exe");
            while (vProcess.MainWindowHandle == IntPtr.Zero) vProcess.Refresh();
            IntPtr vHandle = FindWindowEx(vProcess.MainWindowHandle,IntPtr.Zero, "Edit", null);

            SendMessage(vHandle, WM_SETTEXT, 0, File.ReadAllText(absFile));
        }

        private void lnkOpenPath_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!Directory.Exists(assemblyRootPath)) return;
            Process.Start(assemblyRootPath);
        }
    }
}
