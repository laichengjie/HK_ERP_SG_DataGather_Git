namespace CodeGenerate
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.文件FToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.打开ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.miOpenAssembly = new System.Windows.Forms.ToolStripMenuItem();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.lblClassName = new System.Windows.Forms.Label();
            this.txtClassName = new System.Windows.Forms.TextBox();
            this.lable3 = new System.Windows.Forms.Label();
            this.txtHeric = new System.Windows.Forms.TextBox();
            this.lnkOpenFile = new System.Windows.Forms.LinkLabel();
            this.txtProc = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.chkPartial = new System.Windows.Forms.CheckBox();
            this.lnkOpenPath = new System.Windows.Forms.LinkLabel();
            this.label3 = new System.Windows.Forms.Label();
            this.chkProperty = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtNameSpace = new System.Windows.Forms.TextBox();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(59, 34);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(491, 20);
            this.comboBox1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "类型：";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.文件FToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(581, 25);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 文件FToolStripMenuItem
            // 
            this.文件FToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.打开ToolStripMenuItem});
            this.文件FToolStripMenuItem.Name = "文件FToolStripMenuItem";
            this.文件FToolStripMenuItem.Size = new System.Drawing.Size(58, 21);
            this.文件FToolStripMenuItem.Text = "文件(&F)";
            // 
            // 打开ToolStripMenuItem
            // 
            this.打开ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miOpenAssembly});
            this.打开ToolStripMenuItem.Name = "打开ToolStripMenuItem";
            this.打开ToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.打开ToolStripMenuItem.Text = "打开";
            // 
            // miOpenAssembly
            // 
            this.miOpenAssembly.Name = "miOpenAssembly";
            this.miOpenAssembly.Size = new System.Drawing.Size(112, 22);
            this.miOpenAssembly.Text = "程序集";
            this.miOpenAssembly.Click += new System.EventHandler(this.miOpenAssembly_Click);
            // 
            // btnGenerate
            // 
            this.btnGenerate.Location = new System.Drawing.Point(475, 466);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(75, 30);
            this.btnGenerate.TabIndex = 3;
            this.btnGenerate.Text = "生成片段";
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // lblClassName
            // 
            this.lblClassName.AutoSize = true;
            this.lblClassName.Location = new System.Drawing.Point(12, 86);
            this.lblClassName.Name = "lblClassName";
            this.lblClassName.Size = new System.Drawing.Size(41, 12);
            this.lblClassName.TabIndex = 4;
            this.lblClassName.Text = "类名：";
            // 
            // txtClassName
            // 
            this.txtClassName.Location = new System.Drawing.Point(59, 83);
            this.txtClassName.Name = "txtClassName";
            this.txtClassName.Size = new System.Drawing.Size(491, 21);
            this.txtClassName.TabIndex = 5;
            // 
            // lable3
            // 
            this.lable3.AutoSize = true;
            this.lable3.Location = new System.Drawing.Point(12, 113);
            this.lable3.Name = "lable3";
            this.lable3.Size = new System.Drawing.Size(41, 12);
            this.lable3.TabIndex = 4;
            this.lable3.Text = "继承：";
            // 
            // txtHeric
            // 
            this.txtHeric.Location = new System.Drawing.Point(59, 110);
            this.txtHeric.Name = "txtHeric";
            this.txtHeric.Size = new System.Drawing.Size(491, 21);
            this.txtHeric.TabIndex = 5;
            // 
            // lnkOpenFile
            // 
            this.lnkOpenFile.AutoSize = true;
            this.lnkOpenFile.Location = new System.Drawing.Point(57, 475);
            this.lnkOpenFile.Name = "lnkOpenFile";
            this.lnkOpenFile.Size = new System.Drawing.Size(53, 12);
            this.lnkOpenFile.TabIndex = 6;
            this.lnkOpenFile.TabStop = true;
            this.lnkOpenFile.Text = "等待文件";
            this.lnkOpenFile.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkOpenFile_LinkClicked);
            // 
            // txtProc
            // 
            this.txtProc.Location = new System.Drawing.Point(59, 169);
            this.txtProc.Multiline = true;
            this.txtProc.Name = "txtProc";
            this.txtProc.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtProc.Size = new System.Drawing.Size(491, 289);
            this.txtProc.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 138);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "分部：";
            // 
            // chkPartial
            // 
            this.chkPartial.AutoSize = true;
            this.chkPartial.Location = new System.Drawing.Point(59, 138);
            this.chkPartial.Name = "chkPartial";
            this.chkPartial.Size = new System.Drawing.Size(15, 14);
            this.chkPartial.TabIndex = 8;
            this.chkPartial.UseVisualStyleBackColor = true;
            // 
            // lnkOpenPath
            // 
            this.lnkOpenPath.AutoSize = true;
            this.lnkOpenPath.Location = new System.Drawing.Point(127, 475);
            this.lnkOpenPath.Name = "lnkOpenPath";
            this.lnkOpenPath.Size = new System.Drawing.Size(65, 12);
            this.lnkOpenPath.TabIndex = 9;
            this.lnkOpenPath.TabStop = true;
            this.lnkOpenPath.Text = "打开文件夹";
            this.lnkOpenPath.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkOpenPath_LinkClicked);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(129, 138);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "生成属性：";
            // 
            // chkProperty
            // 
            this.chkProperty.AutoSize = true;
            this.chkProperty.Location = new System.Drawing.Point(200, 138);
            this.chkProperty.Name = "chkProperty";
            this.chkProperty.Size = new System.Drawing.Size(15, 14);
            this.chkProperty.TabIndex = 8;
            this.chkProperty.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 60);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 12);
            this.label4.TabIndex = 4;
            this.label4.Text = "空间：";
            // 
            // txtNameSpace
            // 
            this.txtNameSpace.Location = new System.Drawing.Point(59, 57);
            this.txtNameSpace.Name = "txtNameSpace";
            this.txtNameSpace.Size = new System.Drawing.Size(491, 21);
            this.txtNameSpace.TabIndex = 5;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(581, 508);
            this.Controls.Add(this.lnkOpenPath);
            this.Controls.Add(this.chkProperty);
            this.Controls.Add(this.chkPartial);
            this.Controls.Add(this.txtProc);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lnkOpenFile);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtHeric);
            this.Controls.Add(this.lable3);
            this.Controls.Add(this.txtNameSpace);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtClassName);
            this.Controls.Add(this.lblClassName);
            this.Controls.Add(this.btnGenerate);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "基本属性生成器-Tian";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 文件FToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 打开ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem miOpenAssembly;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.Label lblClassName;
        private System.Windows.Forms.TextBox txtClassName;
        private System.Windows.Forms.Label lable3;
        private System.Windows.Forms.TextBox txtHeric;
        private System.Windows.Forms.LinkLabel lnkOpenFile;
        private System.Windows.Forms.TextBox txtProc;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chkPartial;
        private System.Windows.Forms.LinkLabel lnkOpenPath;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox chkProperty;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtNameSpace;
    }
}

