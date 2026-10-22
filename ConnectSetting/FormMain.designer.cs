namespace ConnectSetting
{
    partial class FormMain
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.lblCallType = new System.Windows.Forms.Label();
            this.rdgDatabase = new DevExpress.XtraEditors.RadioGroup();
            this.tabControl = new DevExpress.XtraTab.XtraTabControl();
            this.tabAdo = new DevExpress.XtraTab.XtraTabPage();
            this.panelControl4 = new DevExpress.XtraEditors.PanelControl();
            this.lueDB = new DevExpress.XtraEditors.LookUpEdit();
            this.lueLoginType = new DevExpress.XtraEditors.LookUpEdit();
            this.btnRefreshDB = new DevExpress.XtraEditors.SimpleButton();
            this.btnTestDB = new DevExpress.XtraEditors.SimpleButton();
            this.lblSelectDB = new System.Windows.Forms.Label();
            this.txtPwd = new ConnectSetting.HK_TextEdit();
            this.lblPwd = new System.Windows.Forms.Label();
            this.txtUser = new DevExpress.XtraEditors.TextEdit();
            this.lblUser = new System.Windows.Forms.Label();
            this.lblLoginType = new System.Windows.Forms.Label();
            this.txtServer = new DevExpress.XtraEditors.TextEdit();
            this.lblServer = new System.Windows.Forms.Label();
            this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
            this.btnExit = new DevExpress.XtraEditors.SimpleButton();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.formStyle = new DevExpress.LookAndFeel.DefaultLookAndFeel(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rdgDatabase.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabControl)).BeginInit();
            this.tabControl.SuspendLayout();
            this.tabAdo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl4)).BeginInit();
            this.panelControl4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lueDB.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueLoginType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPwd.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtUser.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtServer.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).BeginInit();
            this.panelControl2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.lblCallType);
            this.panelControl1.Controls.Add(this.rdgDatabase);
            this.panelControl1.Controls.Add(this.tabControl);
            this.panelControl1.Controls.Add(this.panelControl2);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl1.Location = new System.Drawing.Point(0, 0);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(419, 353);
            this.panelControl1.TabIndex = 0;
            // 
            // lblCallType
            // 
            this.lblCallType.AutoSize = true;
            this.lblCallType.Location = new System.Drawing.Point(18, 14);
            this.lblCallType.Name = "lblCallType";
            this.lblCallType.Size = new System.Drawing.Size(41, 12);
            this.lblCallType.TabIndex = 4;
            this.lblCallType.Text = "数据库";
            // 
            // rdgDatabase
            // 
            this.rdgDatabase.Location = new System.Drawing.Point(82, 8);
            this.rdgDatabase.Name = "rdgDatabase";
            this.rdgDatabase.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[] {
            new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "平台正式库"),
            new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "平台测试库"),
            new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "ERP正式库"),
            new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "ERP测试库")});
            this.rdgDatabase.Size = new System.Drawing.Size(325, 25);
            this.rdgDatabase.TabIndex = 1;
            this.rdgDatabase.SelectedIndexChanged += new System.EventHandler(this.rdgDatabase_SelectedIndexChanged);
            // 
            // tabControl
            // 
            this.tabControl.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.tabControl.Appearance.Options.UseBackColor = true;
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tabControl.Location = new System.Drawing.Point(2, 39);
            this.tabControl.LookAndFeel.SkinName = "Blue";
            this.tabControl.Margin = new System.Windows.Forms.Padding(1);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedTabPage = this.tabAdo;
            this.tabControl.ShowTabHeader = DevExpress.Utils.DefaultBoolean.False;
            this.tabControl.Size = new System.Drawing.Size(415, 277);
            this.tabControl.TabIndex = 2;
            this.tabControl.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.tabAdo});
            // 
            // tabAdo
            // 
            this.tabAdo.Controls.Add(this.panelControl4);
            this.tabAdo.Name = "tabAdo";
            this.tabAdo.Size = new System.Drawing.Size(408, 270);
            this.tabAdo.Text = "ADO";
            // 
            // panelControl4
            // 
            this.panelControl4.Controls.Add(this.lueDB);
            this.panelControl4.Controls.Add(this.lueLoginType);
            this.panelControl4.Controls.Add(this.btnRefreshDB);
            this.panelControl4.Controls.Add(this.btnTestDB);
            this.panelControl4.Controls.Add(this.lblSelectDB);
            this.panelControl4.Controls.Add(this.txtPwd);
            this.panelControl4.Controls.Add(this.lblPwd);
            this.panelControl4.Controls.Add(this.txtUser);
            this.panelControl4.Controls.Add(this.lblUser);
            this.panelControl4.Controls.Add(this.lblLoginType);
            this.panelControl4.Controls.Add(this.txtServer);
            this.panelControl4.Controls.Add(this.lblServer);
            this.panelControl4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl4.Location = new System.Drawing.Point(0, 0);
            this.panelControl4.Name = "panelControl4";
            this.panelControl4.Size = new System.Drawing.Size(408, 270);
            this.panelControl4.TabIndex = 0;
            // 
            // lueDB
            // 
            this.lueDB.Location = new System.Drawing.Point(102, 181);
            this.lueDB.Name = "lueDB";
            this.lueDB.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.lueDB.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "数据库")});
            this.lueDB.Properties.NullText = "";
            this.lueDB.Size = new System.Drawing.Size(217, 21);
            this.lueDB.TabIndex = 15;
            // 
            // lueLoginType
            // 
            this.lueLoginType.Location = new System.Drawing.Point(102, 67);
            this.lueLoginType.Name = "lueLoginType";
            this.lueLoginType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.lueLoginType.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("TypeName", "身份验证")});
            this.lueLoginType.Properties.NullText = "";
            this.lueLoginType.Size = new System.Drawing.Size(217, 21);
            this.lueLoginType.TabIndex = 14;
            this.lueLoginType.EditValueChanged += new System.EventHandler(this.lueLoginType_EditValueChanged);
            // 
            // btnRefreshDB
            // 
            this.btnRefreshDB.Location = new System.Drawing.Point(330, 180);
            this.btnRefreshDB.Name = "btnRefreshDB";
            this.btnRefreshDB.Size = new System.Drawing.Size(70, 23);
            this.btnRefreshDB.TabIndex = 6;
            this.btnRefreshDB.Text = "刷新数据库";
            this.btnRefreshDB.Click += new System.EventHandler(this.btnRefreshDB_Click);
            // 
            // btnTestDB
            // 
            this.btnTestDB.Location = new System.Drawing.Point(330, 142);
            this.btnTestDB.Name = "btnTestDB";
            this.btnTestDB.Size = new System.Drawing.Size(70, 23);
            this.btnTestDB.TabIndex = 4;
            this.btnTestDB.Text = "测试连接";
            this.btnTestDB.Click += new System.EventHandler(this.btnTestDB_Click);
            // 
            // lblSelectDB
            // 
            this.lblSelectDB.AutoSize = true;
            this.lblSelectDB.Location = new System.Drawing.Point(22, 184);
            this.lblSelectDB.Name = "lblSelectDB";
            this.lblSelectDB.Size = new System.Drawing.Size(65, 12);
            this.lblSelectDB.TabIndex = 13;
            this.lblSelectDB.Text = "选择数据库";
            // 
            // txtPwd
            // 
            this.txtPwd.Location = new System.Drawing.Point(102, 143);
            this.txtPwd.Name = "txtPwd";
            this.txtPwd.Properties.PasswordChar = '*';
            this.txtPwd.Size = new System.Drawing.Size(217, 21);
            this.txtPwd.TabIndex = 3;
            // 
            // lblPwd
            // 
            this.lblPwd.AutoSize = true;
            this.lblPwd.Location = new System.Drawing.Point(22, 148);
            this.lblPwd.Name = "lblPwd";
            this.lblPwd.Size = new System.Drawing.Size(29, 12);
            this.lblPwd.TabIndex = 11;
            this.lblPwd.Text = "密码";
            // 
            // txtUser
            // 
            this.txtUser.Location = new System.Drawing.Point(102, 105);
            this.txtUser.Name = "txtUser";
            this.txtUser.Size = new System.Drawing.Size(217, 21);
            this.txtUser.TabIndex = 2;
            // 
            // lblUser
            // 
            this.lblUser.AutoSize = true;
            this.lblUser.Location = new System.Drawing.Point(22, 110);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(41, 12);
            this.lblUser.TabIndex = 9;
            this.lblUser.Text = "登录名";
            // 
            // lblLoginType
            // 
            this.lblLoginType.AutoSize = true;
            this.lblLoginType.Location = new System.Drawing.Point(22, 70);
            this.lblLoginType.Name = "lblLoginType";
            this.lblLoginType.Size = new System.Drawing.Size(53, 12);
            this.lblLoginType.TabIndex = 7;
            this.lblLoginType.Text = "身份验证";
            // 
            // txtServer
            // 
            this.txtServer.Location = new System.Drawing.Point(102, 29);
            this.txtServer.Name = "txtServer";
            this.txtServer.Size = new System.Drawing.Size(217, 21);
            this.txtServer.TabIndex = 0;
            // 
            // lblServer
            // 
            this.lblServer.AutoSize = true;
            this.lblServer.Location = new System.Drawing.Point(22, 34);
            this.lblServer.Name = "lblServer";
            this.lblServer.Size = new System.Drawing.Size(41, 12);
            this.lblServer.TabIndex = 5;
            this.lblServer.Text = "服务器";
            // 
            // panelControl2
            // 
            this.panelControl2.Appearance.BackColor = System.Drawing.SystemColors.Control;
            this.panelControl2.Appearance.Options.UseBackColor = true;
            this.panelControl2.Controls.Add(this.btnExit);
            this.panelControl2.Controls.Add(this.btnSave);
            this.panelControl2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelControl2.Location = new System.Drawing.Point(2, 316);
            this.panelControl2.Name = "panelControl2";
            this.panelControl2.Size = new System.Drawing.Size(415, 35);
            this.panelControl2.TabIndex = 0;
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(332, 6);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(70, 23);
            this.btnExit.TabIndex = 1;
            this.btnExit.Text = "退出(&C)";
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(251, 6);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(70, 23);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "保存(&S)";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // formStyle
            // 
            this.formStyle.LookAndFeel.SkinName = "Blue";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(419, 353);
            this.Controls.Add(this.panelControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "帐套服务器设置";
            this.Load += new System.EventHandler(this.FormMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.panelControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rdgDatabase.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabControl)).EndInit();
            this.tabControl.ResumeLayout(false);
            this.tabAdo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl4)).EndInit();
            this.panelControl4.ResumeLayout(false);
            this.panelControl4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lueDB.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueLoginType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPwd.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtUser.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtServer.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).EndInit();
            this.panelControl2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.PanelControl panelControl2;
        private DevExpress.XtraEditors.SimpleButton btnExit;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        public DevExpress.LookAndFeel.DefaultLookAndFeel formStyle;
        private System.Windows.Forms.Label lblCallType;
        private DevExpress.XtraEditors.RadioGroup rdgDatabase;
        private DevExpress.XtraTab.XtraTabControl tabControl;
        private DevExpress.XtraTab.XtraTabPage tabAdo;
        private DevExpress.XtraEditors.PanelControl panelControl4;
        private DevExpress.XtraEditors.LookUpEdit lueDB;
        private DevExpress.XtraEditors.LookUpEdit lueLoginType;
        private DevExpress.XtraEditors.SimpleButton btnRefreshDB;
        private DevExpress.XtraEditors.SimpleButton btnTestDB;
        private System.Windows.Forms.Label lblSelectDB;
        private ConnectSetting.HK_TextEdit txtPwd;
        private System.Windows.Forms.Label lblPwd;
        private DevExpress.XtraEditors.TextEdit txtUser;
        private System.Windows.Forms.Label lblUser;
        private System.Windows.Forms.Label lblLoginType;
        private DevExpress.XtraEditors.TextEdit txtServer;
        private System.Windows.Forms.Label lblServer;
    }
}

