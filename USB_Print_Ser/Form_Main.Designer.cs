namespace Print_Ser
{
    partial class Form_Main
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
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Main));
            this.Grbox = new System.Windows.Forms.GroupBox();
            this.cmbPrintMode = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.chk_开机启动 = new System.Windows.Forms.CheckBox();
            this.chk_启动运行打印服务 = new System.Windows.Forms.CheckBox();
            this.chk_启动最小化窗口 = new System.Windows.Forms.CheckBox();
            this.num_服务端口 = new System.Windows.Forms.NumericUpDown();
            this.Btn_删除打印机 = new System.Windows.Forms.Button();
            this.Btn_添加打印机 = new System.Windows.Forms.Button();
            this.Btn_停止打印服务 = new System.Windows.Forms.Button();
            this.Btn_启动打印服务 = new System.Windows.Forms.Button();
            this.cmb_打印机列表 = new System.Windows.Forms.ComboBox();
            this.lab_服务端口 = new System.Windows.Forms.Label();
            this.lab_打印机 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.Grbox_list = new System.Windows.Forms.GroupBox();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panStatus = new System.Windows.Forms.Panel();
            this.lbl_status = new System.Windows.Forms.Label();
            this.lblStatustitle = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.rtbLog = new System.Windows.Forms.RichTextBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.rich_web_info = new System.Windows.Forms.RichTextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.Chk_启动运行文件服务 = new System.Windows.Forms.CheckBox();
            this.Num_网站端口 = new System.Windows.Forms.NumericUpDown();
            this.Num_限制上传大小 = new System.Windows.Forms.NumericUpDown();
            this.Lab_限制上传大小 = new System.Windows.Forms.Label();
            this.Lab_上传密码 = new System.Windows.Forms.Label();
            this.Tbx_上传密码 = new System.Windows.Forms.TextBox();
            this.Btn_默认目录 = new System.Windows.Forms.Button();
            this.Btn_预览 = new System.Windows.Forms.Button();
            this.Btn_启动停止文件服务 = new System.Windows.Forms.Button();
            this.Btn_浏览目录 = new System.Windows.Forms.Button();
            this.Lab_目录 = new System.Windows.Forms.Label();
            this.Tbx_网站目录 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.Grbox_Info = new System.Windows.Forms.GroupBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.richTextBox2 = new System.Windows.Forms.RichTextBox();
            this.Grbox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_服务端口)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.Grbox_list.SuspendLayout();
            this.panStatus.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Num_网站端口)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Num_限制上传大小)).BeginInit();
            this.tabPage4.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.Grbox_Info.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // Grbox
            // 
            this.Grbox.BackColor = System.Drawing.Color.AliceBlue;
            this.Grbox.Controls.Add(this.cmbPrintMode);
            this.Grbox.Controls.Add(this.label1);
            this.Grbox.Controls.Add(this.chk_开机启动);
            this.Grbox.Controls.Add(this.chk_启动运行打印服务);
            this.Grbox.Controls.Add(this.chk_启动最小化窗口);
            this.Grbox.Controls.Add(this.num_服务端口);
            this.Grbox.Controls.Add(this.Btn_删除打印机);
            this.Grbox.Controls.Add(this.Btn_添加打印机);
            this.Grbox.Controls.Add(this.Btn_停止打印服务);
            this.Grbox.Controls.Add(this.Btn_启动打印服务);
            this.Grbox.Controls.Add(this.cmb_打印机列表);
            this.Grbox.Controls.Add(this.lab_服务端口);
            this.Grbox.Controls.Add(this.lab_打印机);
            this.Grbox.Dock = System.Windows.Forms.DockStyle.Top;
            this.Grbox.Location = new System.Drawing.Point(3, 3);
            this.Grbox.Name = "Grbox";
            this.Grbox.Size = new System.Drawing.Size(661, 181);
            this.Grbox.TabIndex = 0;
            this.Grbox.TabStop = false;
            this.Grbox.Text = "设置";
            // 
            // cmbPrintMode
            // 
            this.cmbPrintMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPrintMode.FormattingEnabled = true;
            this.cmbPrintMode.Location = new System.Drawing.Point(89, 131);
            this.cmbPrintMode.Name = "cmbPrintMode";
            this.cmbPrintMode.Size = new System.Drawing.Size(98, 29);
            this.cmbPrintMode.TabIndex = 22;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 134);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 21);
            this.label1.TabIndex = 21;
            this.label1.Text = "打印模式:";
            // 
            // chk_开机启动
            // 
            this.chk_开机启动.AutoSize = true;
            this.chk_开机启动.Location = new System.Drawing.Point(211, 86);
            this.chk_开机启动.Name = "chk_开机启动";
            this.chk_开机启动.Size = new System.Drawing.Size(93, 25);
            this.chk_开机启动.TabIndex = 15;
            this.chk_开机启动.Text = "开机启动";
            this.chk_开机启动.UseVisualStyleBackColor = true;
            this.chk_开机启动.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // chk_启动运行打印服务
            // 
            this.chk_启动运行打印服务.AutoSize = true;
            this.chk_启动运行打印服务.Location = new System.Drawing.Point(312, 86);
            this.chk_启动运行打印服务.Name = "chk_启动运行打印服务";
            this.chk_启动运行打印服务.Size = new System.Drawing.Size(157, 25);
            this.chk_启动运行打印服务.TabIndex = 18;
            this.chk_启动运行打印服务.Text = "启动运行打印服务";
            this.chk_启动运行打印服务.UseVisualStyleBackColor = true;
            // 
            // chk_启动最小化窗口
            // 
            this.chk_启动最小化窗口.AutoSize = true;
            this.chk_启动最小化窗口.Location = new System.Drawing.Point(477, 86);
            this.chk_启动最小化窗口.Name = "chk_启动最小化窗口";
            this.chk_启动最小化窗口.Size = new System.Drawing.Size(141, 25);
            this.chk_启动最小化窗口.TabIndex = 20;
            this.chk_启动最小化窗口.Text = "启动最小化窗口";
            this.chk_启动最小化窗口.UseVisualStyleBackColor = true;
            // 
            // num_服务端口
            // 
            this.num_服务端口.BackColor = System.Drawing.Color.Linen;
            this.num_服务端口.ForeColor = System.Drawing.Color.RoyalBlue;
            this.num_服务端口.Location = new System.Drawing.Point(89, 83);
            this.num_服务端口.Maximum = new decimal(new int[] {
            9199,
            0,
            0,
            0});
            this.num_服务端口.Minimum = new decimal(new int[] {
            9100,
            0,
            0,
            0});
            this.num_服务端口.Name = "num_服务端口";
            this.num_服务端口.Size = new System.Drawing.Size(98, 29);
            this.num_服务端口.TabIndex = 19;
            this.num_服务端口.Tag = "9100";
            this.num_服务端口.Value = new decimal(new int[] {
            9100,
            0,
            0,
            0});
            // 
            // Btn_删除打印机
            // 
            this.Btn_删除打印机.BackColor = System.Drawing.Color.LightYellow;
            this.Btn_删除打印机.ForeColor = System.Drawing.Color.Indigo;
            this.Btn_删除打印机.Location = new System.Drawing.Point(317, 121);
            this.Btn_删除打印机.Name = "Btn_删除打印机";
            this.Btn_删除打印机.Size = new System.Drawing.Size(107, 47);
            this.Btn_删除打印机.TabIndex = 17;
            this.Btn_删除打印机.Text = "删除打印机";
            this.Btn_删除打印机.UseVisualStyleBackColor = false;
            this.Btn_删除打印机.Click += new System.EventHandler(this.Btn_删除打印机_Click);
            // 
            // Btn_添加打印机
            // 
            this.Btn_添加打印机.BackColor = System.Drawing.Color.LightYellow;
            this.Btn_添加打印机.ForeColor = System.Drawing.Color.Indigo;
            this.Btn_添加打印机.Location = new System.Drawing.Point(210, 121);
            this.Btn_添加打印机.Name = "Btn_添加打印机";
            this.Btn_添加打印机.Size = new System.Drawing.Size(107, 47);
            this.Btn_添加打印机.TabIndex = 16;
            this.Btn_添加打印机.Text = "添加打印机";
            this.Btn_添加打印机.UseVisualStyleBackColor = false;
            this.Btn_添加打印机.Click += new System.EventHandler(this.Btn_添加打印机_Click);
            // 
            // Btn_停止打印服务
            // 
            this.Btn_停止打印服务.BackColor = System.Drawing.Color.Silver;
            this.Btn_停止打印服务.Enabled = false;
            this.Btn_停止打印服务.ForeColor = System.Drawing.Color.White;
            this.Btn_停止打印服务.Location = new System.Drawing.Point(531, 121);
            this.Btn_停止打印服务.Name = "Btn_停止打印服务";
            this.Btn_停止打印服务.Size = new System.Drawing.Size(107, 47);
            this.Btn_停止打印服务.TabIndex = 5;
            this.Btn_停止打印服务.Text = "停止服务";
            this.Btn_停止打印服务.UseVisualStyleBackColor = false;
            this.Btn_停止打印服务.Click += new System.EventHandler(this.Btn_停止打印服务_Click);
            // 
            // Btn_启动打印服务
            // 
            this.Btn_启动打印服务.BackColor = System.Drawing.Color.DodgerBlue;
            this.Btn_启动打印服务.ForeColor = System.Drawing.Color.White;
            this.Btn_启动打印服务.Location = new System.Drawing.Point(424, 121);
            this.Btn_启动打印服务.Name = "Btn_启动打印服务";
            this.Btn_启动打印服务.Size = new System.Drawing.Size(107, 47);
            this.Btn_启动打印服务.TabIndex = 4;
            this.Btn_启动打印服务.Text = "启动服务";
            this.Btn_启动打印服务.UseVisualStyleBackColor = false;
            this.Btn_启动打印服务.Click += new System.EventHandler(this.Btn_启动打印服务_Click);
            // 
            // cmb_打印机列表
            // 
            this.cmb_打印机列表.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_打印机列表.FormattingEnabled = true;
            this.cmb_打印机列表.Location = new System.Drawing.Point(89, 37);
            this.cmb_打印机列表.Name = "cmb_打印机列表";
            this.cmb_打印机列表.Size = new System.Drawing.Size(549, 29);
            this.cmb_打印机列表.TabIndex = 3;
            // 
            // lab_服务端口
            // 
            this.lab_服务端口.AutoSize = true;
            this.lab_服务端口.Location = new System.Drawing.Point(7, 87);
            this.lab_服务端口.Name = "lab_服务端口";
            this.lab_服务端口.Size = new System.Drawing.Size(78, 21);
            this.lab_服务端口.TabIndex = 1;
            this.lab_服务端口.Text = "服务端口:";
            // 
            // lab_打印机
            // 
            this.lab_打印机.AutoSize = true;
            this.lab_打印机.Location = new System.Drawing.Point(23, 40);
            this.lab_打印机.Name = "lab_打印机";
            this.lab_打印机.Size = new System.Drawing.Size(62, 21);
            this.lab_打印机.TabIndex = 0;
            this.lab_打印机.Text = "打印机:";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(675, 523);
            this.tabControl1.TabIndex = 6;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.Grbox_list);
            this.tabPage1.Controls.Add(this.Grbox);
            this.tabPage1.Controls.Add(this.panStatus);
            this.tabPage1.Location = new System.Drawing.Point(4, 30);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(667, 489);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "打印服务器";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // Grbox_list
            // 
            this.Grbox_list.BackColor = System.Drawing.Color.Beige;
            this.Grbox_list.Controls.Add(this.listView1);
            this.Grbox_list.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Grbox_list.Location = new System.Drawing.Point(3, 184);
            this.Grbox_list.Name = "Grbox_list";
            this.Grbox_list.Size = new System.Drawing.Size(661, 271);
            this.Grbox_list.TabIndex = 16;
            this.Grbox_list.TabStop = false;
            this.Grbox_list.Text = "任务列表";
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.FullRowSelect = true;
            this.listView1.GridLines = true;
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(3, 25);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(655, 243);
            this.listView1.TabIndex = 8;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "序号";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "打印机名称";
            this.columnHeader2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader2.Width = 350;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "服务端口";
            this.columnHeader3.Width = 80;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "打印模式";
            this.columnHeader4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader4.Width = 100;
            // 
            // panStatus
            // 
            this.panStatus.Controls.Add(this.lbl_status);
            this.panStatus.Controls.Add(this.lblStatustitle);
            this.panStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panStatus.Location = new System.Drawing.Point(3, 455);
            this.panStatus.Name = "panStatus";
            this.panStatus.Size = new System.Drawing.Size(661, 31);
            this.panStatus.TabIndex = 17;
            // 
            // lbl_status
            // 
            this.lbl_status.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbl_status.ForeColor = System.Drawing.Color.Red;
            this.lbl_status.Location = new System.Drawing.Point(86, 0);
            this.lbl_status.Name = "lbl_status";
            this.lbl_status.Size = new System.Drawing.Size(575, 31);
            this.lbl_status.TabIndex = 1;
            this.lbl_status.Text = "未运行";
            this.lbl_status.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblStatustitle
            // 
            this.lblStatustitle.BackColor = System.Drawing.Color.Transparent;
            this.lblStatustitle.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblStatustitle.ForeColor = System.Drawing.Color.Green;
            this.lblStatustitle.Location = new System.Drawing.Point(0, 0);
            this.lblStatustitle.Name = "lblStatustitle";
            this.lblStatustitle.Size = new System.Drawing.Size(86, 31);
            this.lblStatustitle.TabIndex = 0;
            this.lblStatustitle.Text = "运行状态:";
            this.lblStatustitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.rtbLog);
            this.tabPage2.Location = new System.Drawing.Point(4, 30);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(667, 489);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "打印日志";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // rtbLog
            // 
            this.rtbLog.BackColor = System.Drawing.Color.SeaShell;
            this.rtbLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbLog.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.rtbLog.ForeColor = System.Drawing.Color.DarkGreen;
            this.rtbLog.Location = new System.Drawing.Point(3, 3);
            this.rtbLog.Name = "rtbLog";
            this.rtbLog.ReadOnly = true;
            this.rtbLog.Size = new System.Drawing.Size(661, 483);
            this.rtbLog.TabIndex = 2;
            this.rtbLog.Text = "";
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.groupBox5);
            this.tabPage3.Controls.Add(this.groupBox3);
            this.tabPage3.Location = new System.Drawing.Point(4, 30);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(667, 489);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "文件服务器";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.BackColor = System.Drawing.Color.Azure;
            this.groupBox5.Controls.Add(this.rich_web_info);
            this.groupBox5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox5.ForeColor = System.Drawing.Color.Black;
            this.groupBox5.Location = new System.Drawing.Point(3, 167);
            this.groupBox5.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox5.Size = new System.Drawing.Size(661, 319);
            this.groupBox5.TabIndex = 3;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "日志";
            // 
            // rich_web_info
            // 
            this.rich_web_info.BackColor = System.Drawing.Color.SeaShell;
            this.rich_web_info.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rich_web_info.ForeColor = System.Drawing.Color.DarkGreen;
            this.rich_web_info.Location = new System.Drawing.Point(4, 26);
            this.rich_web_info.Margin = new System.Windows.Forms.Padding(4);
            this.rich_web_info.Name = "rich_web_info";
            this.rich_web_info.Size = new System.Drawing.Size(653, 289);
            this.rich_web_info.TabIndex = 0;
            this.rich_web_info.Text = "";
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.Color.Azure;
            this.groupBox3.Controls.Add(this.Chk_启动运行文件服务);
            this.groupBox3.Controls.Add(this.Num_网站端口);
            this.groupBox3.Controls.Add(this.Num_限制上传大小);
            this.groupBox3.Controls.Add(this.Lab_限制上传大小);
            this.groupBox3.Controls.Add(this.Lab_上传密码);
            this.groupBox3.Controls.Add(this.Tbx_上传密码);
            this.groupBox3.Controls.Add(this.Btn_默认目录);
            this.groupBox3.Controls.Add(this.Btn_预览);
            this.groupBox3.Controls.Add(this.Btn_启动停止文件服务);
            this.groupBox3.Controls.Add(this.Btn_浏览目录);
            this.groupBox3.Controls.Add(this.Lab_目录);
            this.groupBox3.Controls.Add(this.Tbx_网站目录);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox3.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.groupBox3.ForeColor = System.Drawing.SystemColors.WindowText;
            this.groupBox3.Location = new System.Drawing.Point(3, 3);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox3.Size = new System.Drawing.Size(661, 164);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "设置(文件服务器)";
            // 
            // Chk_启动运行文件服务
            // 
            this.Chk_启动运行文件服务.AutoSize = true;
            this.Chk_启动运行文件服务.Location = new System.Drawing.Point(219, 125);
            this.Chk_启动运行文件服务.Name = "Chk_启动运行文件服务";
            this.Chk_启动运行文件服务.Size = new System.Drawing.Size(157, 25);
            this.Chk_启动运行文件服务.TabIndex = 34;
            this.Chk_启动运行文件服务.Text = "启动运行文件服务";
            this.Chk_启动运行文件服务.UseVisualStyleBackColor = true;
            // 
            // Num_网站端口
            // 
            this.Num_网站端口.BackColor = System.Drawing.Color.Linen;
            this.Num_网站端口.ForeColor = System.Drawing.Color.RoyalBlue;
            this.Num_网站端口.Location = new System.Drawing.Point(94, 80);
            this.Num_网站端口.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.Num_网站端口.Name = "Num_网站端口";
            this.Num_网站端口.Size = new System.Drawing.Size(108, 29);
            this.Num_网站端口.TabIndex = 32;
            this.Num_网站端口.Tag = "1314";
            this.Num_网站端口.Value = new decimal(new int[] {
            1314,
            0,
            0,
            0});
            // 
            // Num_限制上传大小
            // 
            this.Num_限制上传大小.BackColor = System.Drawing.Color.Linen;
            this.Num_限制上传大小.ForeColor = System.Drawing.Color.RoyalBlue;
            this.Num_限制上传大小.Location = new System.Drawing.Point(367, 80);
            this.Num_限制上传大小.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.Num_限制上传大小.Name = "Num_限制上传大小";
            this.Num_限制上传大小.Size = new System.Drawing.Size(98, 29);
            this.Num_限制上传大小.TabIndex = 30;
            this.Num_限制上传大小.Tag = "10000";
            this.Num_限制上传大小.Value = new decimal(new int[] {
            200,
            0,
            0,
            0});
            // 
            // Lab_限制上传大小
            // 
            this.Lab_限制上传大小.AutoSize = true;
            this.Lab_限制上传大小.Location = new System.Drawing.Point(215, 84);
            this.Lab_限制上传大小.Name = "Lab_限制上传大小";
            this.Lab_限制上传大小.Size = new System.Drawing.Size(146, 21);
            this.Lab_限制上传大小.TabIndex = 29;
            this.Lab_限制上传大小.Text = "上传限制大小(MB):";
            // 
            // Lab_上传密码
            // 
            this.Lab_上传密码.AutoSize = true;
            this.Lab_上传密码.Location = new System.Drawing.Point(10, 125);
            this.Lab_上传密码.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Lab_上传密码.Name = "Lab_上传密码";
            this.Lab_上传密码.Size = new System.Drawing.Size(78, 21);
            this.Lab_上传密码.TabIndex = 28;
            this.Lab_上传密码.Text = "上传密码:";
            // 
            // Tbx_上传密码
            // 
            this.Tbx_上传密码.BackColor = System.Drawing.Color.Linen;
            this.Tbx_上传密码.ForeColor = System.Drawing.SystemColors.WindowText;
            this.Tbx_上传密码.Location = new System.Drawing.Point(94, 121);
            this.Tbx_上传密码.Margin = new System.Windows.Forms.Padding(4);
            this.Tbx_上传密码.Name = "Tbx_上传密码";
            this.Tbx_上传密码.Size = new System.Drawing.Size(108, 29);
            this.Tbx_上传密码.TabIndex = 27;
            this.Tbx_上传密码.Text = "123456";
            // 
            // Btn_默认目录
            // 
            this.Btn_默认目录.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_默认目录.BackColor = System.Drawing.Color.SeaGreen;
            this.Btn_默认目录.FlatAppearance.BorderSize = 0;
            this.Btn_默认目录.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.Btn_默认目录.ForeColor = System.Drawing.Color.Lime;
            this.Btn_默认目录.Location = new System.Drawing.Point(567, 38);
            this.Btn_默认目录.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.Btn_默认目录.Name = "Btn_默认目录";
            this.Btn_默认目录.Size = new System.Drawing.Size(82, 29);
            this.Btn_默认目录.TabIndex = 26;
            this.Btn_默认目录.Text = "默认";
            this.Btn_默认目录.UseVisualStyleBackColor = false;
            this.Btn_默认目录.Click += new System.EventHandler(this.Btn_默认目录_Click);
            // 
            // Btn_预览
            // 
            this.Btn_预览.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_预览.BackColor = System.Drawing.Color.RoyalBlue;
            this.Btn_预览.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.Btn_预览.ForeColor = System.Drawing.Color.White;
            this.Btn_预览.Location = new System.Drawing.Point(484, 111);
            this.Btn_预览.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.Btn_预览.Name = "Btn_预览";
            this.Btn_预览.Size = new System.Drawing.Size(165, 46);
            this.Btn_预览.TabIndex = 25;
            this.Btn_预览.Text = "预览";
            this.Btn_预览.UseVisualStyleBackColor = false;
            this.Btn_预览.Click += new System.EventHandler(this.Btn_预览_Click);
            // 
            // Btn_启动停止文件服务
            // 
            this.Btn_启动停止文件服务.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_启动停止文件服务.BackColor = System.Drawing.Color.SeaGreen;
            this.Btn_启动停止文件服务.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.Btn_启动停止文件服务.ForeColor = System.Drawing.Color.White;
            this.Btn_启动停止文件服务.Location = new System.Drawing.Point(484, 69);
            this.Btn_启动停止文件服务.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.Btn_启动停止文件服务.Name = "Btn_启动停止文件服务";
            this.Btn_启动停止文件服务.Size = new System.Drawing.Size(165, 40);
            this.Btn_启动停止文件服务.TabIndex = 23;
            this.Btn_启动停止文件服务.Text = "启动";
            this.Btn_启动停止文件服务.UseVisualStyleBackColor = false;
            this.Btn_启动停止文件服务.Click += new System.EventHandler(this.Btn_启动文件服务_Click);
            // 
            // Btn_浏览目录
            // 
            this.Btn_浏览目录.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Btn_浏览目录.BackColor = System.Drawing.Color.SeaGreen;
            this.Btn_浏览目录.FlatAppearance.BorderSize = 0;
            this.Btn_浏览目录.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.Btn_浏览目录.ForeColor = System.Drawing.Color.Lime;
            this.Btn_浏览目录.Location = new System.Drawing.Point(484, 38);
            this.Btn_浏览目录.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.Btn_浏览目录.Name = "Btn_浏览目录";
            this.Btn_浏览目录.Size = new System.Drawing.Size(82, 29);
            this.Btn_浏览目录.TabIndex = 9;
            this.Btn_浏览目录.Text = "浏览...";
            this.Btn_浏览目录.UseVisualStyleBackColor = false;
            this.Btn_浏览目录.Click += new System.EventHandler(this.Btn_浏览目录_Click);
            // 
            // Lab_目录
            // 
            this.Lab_目录.AutoSize = true;
            this.Lab_目录.Location = new System.Drawing.Point(42, 41);
            this.Lab_目录.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Lab_目录.Name = "Lab_目录";
            this.Lab_目录.Size = new System.Drawing.Size(46, 21);
            this.Lab_目录.TabIndex = 5;
            this.Lab_目录.Text = "目录:";
            // 
            // Tbx_网站目录
            // 
            this.Tbx_网站目录.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Tbx_网站目录.BackColor = System.Drawing.Color.Linen;
            this.Tbx_网站目录.ForeColor = System.Drawing.SystemColors.WindowText;
            this.Tbx_网站目录.Location = new System.Drawing.Point(94, 38);
            this.Tbx_网站目录.Margin = new System.Windows.Forms.Padding(4);
            this.Tbx_网站目录.Name = "Tbx_网站目录";
            this.Tbx_网站目录.Size = new System.Drawing.Size(371, 29);
            this.Tbx_网站目录.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(42, 84);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 21);
            this.label3.TabIndex = 3;
            this.label3.Text = "端口:";
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.richTextBox1);
            this.tabPage4.Location = new System.Drawing.Point(4, 30);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(667, 489);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "使用说明";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // richTextBox1
            // 
            this.richTextBox1.BackColor = System.Drawing.Color.Azure;
            this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox1.Location = new System.Drawing.Point(3, 3);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(661, 483);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = resources.GetString("richTextBox1.Text");
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "notifyIcon1";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.Grbox_Info);
            this.tabPage5.Controls.Add(this.richTextBox2);
            this.tabPage5.Location = new System.Drawing.Point(4, 30);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Size = new System.Drawing.Size(667, 489);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "关于";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // Grbox_Info
            // 
            this.Grbox_Info.Controls.Add(this.pictureBox1);
            this.Grbox_Info.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Grbox_Info.Location = new System.Drawing.Point(0, 165);
            this.Grbox_Info.Name = "Grbox_Info";
            this.Grbox_Info.Size = new System.Drawing.Size(667, 324);
            this.Grbox_Info.TabIndex = 4;
            this.Grbox_Info.TabStop = false;
            this.Grbox_Info.Text = "关于(喜欢就打赏一下吧)";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImage = global::Print_Ser.Properties.Resources.收款码;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.InitialImage = null;
            this.pictureBox1.Location = new System.Drawing.Point(3, 25);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(661, 296);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // richTextBox2
            // 
            this.richTextBox2.BackColor = System.Drawing.Color.Azure;
            this.richTextBox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.richTextBox2.Location = new System.Drawing.Point(0, 0);
            this.richTextBox2.Name = "richTextBox2";
            this.richTextBox2.Size = new System.Drawing.Size(667, 165);
            this.richTextBox2.TabIndex = 3;
            this.richTextBox2.Text = "\nby：晨露流星\nVx：51529502\ngithub：https://github.com/ampetervip/XiuCaiPrintServer";
            // 
            // Form_Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(675, 523);
            this.Controls.Add(this.tabControl1);
            this.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(5);
            this.MaximizeBox = false;
            this.Name = "Form_Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "秀才USB打印服务器";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_Main_FormClosing);
            this.Load += new System.EventHandler(this.Form_Main_Load);
            this.Resize += new System.EventHandler(this.Form_Main_Resize);
            this.Grbox.ResumeLayout(false);
            this.Grbox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_服务端口)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.Grbox_list.ResumeLayout(false);
            this.panStatus.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Num_网站端口)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Num_限制上传大小)).EndInit();
            this.tabPage4.ResumeLayout(false);
            this.tabPage5.ResumeLayout(false);
            this.Grbox_Info.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox Grbox;
        private System.Windows.Forms.ComboBox cmb_打印机列表;
        private System.Windows.Forms.Label lab_服务端口;
        private System.Windows.Forms.Label lab_打印机;
        private System.Windows.Forms.Button Btn_停止打印服务;
        private System.Windows.Forms.Button Btn_启动打印服务;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.CheckBox chk_开机启动;
        private System.Windows.Forms.GroupBox Grbox_list;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.CheckBox chk_启动运行打印服务;
        private System.Windows.Forms.Button Btn_删除打印机;
        private System.Windows.Forms.Button Btn_添加打印机;
        private System.Windows.Forms.Panel panStatus;
        private System.Windows.Forms.Label lbl_status;
        private System.Windows.Forms.Label lblStatustitle;
        private System.Windows.Forms.NumericUpDown num_服务端口;
        private System.Windows.Forms.CheckBox chk_启动最小化窗口;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.RichTextBox rtbLog;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.RichTextBox rich_web_info;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button Btn_默认目录;
        private System.Windows.Forms.Button Btn_预览;
        private System.Windows.Forms.Button Btn_启动停止文件服务;
        private System.Windows.Forms.Button Btn_浏览目录;
        private System.Windows.Forms.Label Lab_目录;
        private System.Windows.Forms.TextBox Tbx_网站目录;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Label Lab_上传密码;
        private System.Windows.Forms.TextBox Tbx_上传密码;
        private System.Windows.Forms.NumericUpDown Num_限制上传大小;
        private System.Windows.Forms.Label Lab_限制上传大小;
        private System.Windows.Forms.NumericUpDown Num_网站端口;
        private System.Windows.Forms.CheckBox Chk_启动运行文件服务;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ComboBox cmbPrintMode;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.GroupBox Grbox_Info;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.RichTextBox richTextBox2;
    }
}

