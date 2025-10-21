using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Print_Ser
{
    public partial class Form_Main : Form
    {
        #region 常量定义
        private const int MAX_LOG_SIZE = 10 * 1024 * 1024; // 日志最大10MB
        private const int BUFFER_SIZE = 8192; // 缓冲区大小
        private const int CLIENT_TIMEOUT = 5 * 60 * 1000; // 客户端超时时间(5分钟)
        private const int MAX_CONCURRENT_CLIENTS = 10; // 最大并发客户端数

        // 打印机驱动类型常量
        private const int PRINTER_DRIVER_XPS = 0x00000008;
        private const int ERROR_INSUFFICIENT_BUFFER = 122;
        #endregion

        #region 字段

        private WebFileServer server;

        // 程序目录下的config.ini
        string iniPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DConfig.ini");
        string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PrintServer.log");

        // TCP 服务器相关变量
        private bool isServerRunning = false;

        // 选中的打印机
        private string selectedPrinter = string.Empty;
        private int PrinterPort = 9100;

        // 选中的打印模式
        public string SelectedPrintMode { get; private set; }

        // 用于显示日志的委托
        private delegate void UpdateLogDelegate(string message);
        private UpdateLogDelegate logDelegate;

        private readonly object _lockObj = new object();
        private readonly SemaphoreSlim _clientSemaphore = new SemaphoreSlim(MAX_CONCURRENT_CLIENTS);

        // 多服务器实例管理（每个端口一个服务器）
        private List<TcpListener> servers = new List<TcpListener>();
        private List<Task> serverTasks = new List<Task>();
        private Dictionary<int, string> portToPrinter = new Dictionary<int, string>();
        private Dictionary<int, string> portToPrintMode = new Dictionary<int, string>(); // 端口-打印模式映射
        private Dictionary<int, CancellationTokenSource> portCancellationSources = new Dictionary<int, CancellationTokenSource>();
        #endregion

        public Form_Main()
        {
            InitializeComponent();
            initver();
            cmbPrintMode.Items.AddRange(new[] { "自动", "RAW", "XPS" });
            cmbPrintMode.SelectedIndex = 0; // 默认自动
            logDelegate = new UpdateLogDelegate(UpdateLog);

            // 初始化日志
            InitializeLog();

            // 加载打印机列表
            LoadPrinters();

            // 初始化ListView（添加第四列打印模式）
            InitializeListView();

            DLoadIni();

            // 添加未处理异常捕获
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Application.ThreadException += Application_ThreadException;
        }

        #region 初始化ListView（添加打印模式列）
        private void InitializeListView()
        {
            listView1.View = View.Details;
            listView1.Columns.Clear();
            listView1.Columns.Add("序号", 60);
            listView1.Columns.Add("打印机名称", 350);
            listView1.Columns.Add("端口", 80);
            listView1.Columns.Add("打印模式", 100); // 第四列：打印模式
            listView1.FullRowSelect = true;
        }
        #endregion

        //获取本机IP地址
        private string GetLocalIPAddress()
        {
            try
            {
                var interfaces = NetworkInterface.GetAllNetworkInterfaces()
                    .Where(ni => ni.OperationalStatus == OperationalStatus.Up)
                    .OrderByDescending(ni =>
                    {
                        if (ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
                            ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
                            return 2;
                        return 1;
                    });

                foreach (var ni in interfaces)
                {
                    var properties = ni.GetIPProperties();
                    foreach (var addr in properties.UnicastAddresses)
                    {
                        if (addr.Address.AddressFamily == AddressFamily.InterNetwork &&
                            !IPAddress.IsLoopback(addr.Address) &&
                            properties.GatewayAddresses.Any())
                        {
                            return addr.Address.ToString();
                        }
                    }
                }
                return "未找到可用的局域网IP地址";
            }
            catch (Exception ex)
            {
                return $"获取IP地址时出错: {ex.Message}";
            }
        }

        #region 异常处理
        private void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            UpdateLog($"应用程序线程异常: {e.Exception.Message}\r\n堆栈跟踪: {e.Exception.StackTrace}");
            SaveLogToFile($"[{DateTime.Now:HH:mm:ss}] 应用程序线程异常: {e.Exception.Message}\r\n堆栈跟踪: {e.Exception.StackTrace}\r\n");
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            UpdateLog($"未处理的异常: {ex?.Message}\r\n堆栈跟踪: {ex?.StackTrace}");
            SaveLogToFile($"[{DateTime.Now:HH:mm:ss}] 未处理的异常: {ex?.Message}\r\n堆栈跟踪: {ex?.StackTrace}\r\n");

            if (e.IsTerminating && isServerRunning)
            {
                UpdateLog("发生致命错误，尝试重启服务...");
                Task.Run(() =>
                {
                    Thread.Sleep(5000);
                    this.Invoke((Action)(() =>
                    {
                        Btn_停止打印服务_Click(Btn_停止打印服务, EventArgs.Empty);
                        Btn_启动打印服务_Click(Btn_启动打印服务, EventArgs.Empty);
                    }));
                });
            }
        }
        #endregion

        #region 初始化
        private void initver()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            this.Text += "_Ver" + fvi.FileVersion;
        }

        private void InitializeLog()
        {
            try
            {
                if (File.Exists(logFilePath) && new FileInfo(logFilePath).Length > MAX_LOG_SIZE)
                {
                    string backupLogPath = $"{logFilePath}.{DateTime.Now:yyyyMMddHHmmss}";
                    File.Move(logFilePath, backupLogPath);
                }
            }
            catch (Exception ex)
            {
                UpdateLog($"初始化日志失败: {ex.Message}");
            }
        }
        #endregion

        #region 开机启动设置
        private void SetAutoStart(bool enable)
        {
            string appName = Process.GetCurrentProcess().ProcessName;
            string appPath = Process.GetCurrentProcess().MainModule.FileName;

            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(
                @"Software\Microsoft\Windows\CurrentVersion\Run", true))
            {
                if (enable)
                {
                    key.SetValue(appName, $"\"{appPath}\"");
                }
                else
                {
                    key.DeleteValue(appName, false);
                }
            }
        }

        private bool IsAutoStartEnabled()
        {
            string appName = Process.GetCurrentProcess().ProcessName;
            string appPath = Process.GetCurrentProcess().MainModule.FileName;
            string expectedValue = $"\"{appPath}\"";

            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(
                @"Software\Microsoft\Windows\CurrentVersion\Run", false))
            {
                if (key == null)
                    return false;

                object registryValue = key.GetValue(appName);
                return registryValue != null && registryValue.ToString() == expectedValue;
            }
        }
        #endregion

        #region INI文件操作（适配第四列打印模式）
        private void DLoadIni()
        {
            chk_开机启动.Checked = IsAutoStartEnabled();

            IniFileHelper ini = new IniFileHelper(iniPath);

            // 从配置读取并加载任务列表（包含第四列打印模式）
            var savedData = ini.ReadString("打印服务器设置", "任务列表", string.Empty);
            if (!string.IsNullOrEmpty(savedData))
            {
                listView1.Items.Clear();
                try
                {
                    string decodedData = DecodeFromBase64(savedData);
                    var rows = decodedData.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

                    foreach (var row in rows)
                    {
                        if (string.IsNullOrWhiteSpace(row))
                            continue;

                        var columns = row.Split('\t');
                        if (columns.Length < 3)
                            continue;

                        var listItem = new ListViewItem(columns[0]);
                        listItem.SubItems.Add(columns[1]); // 打印机名称
                        listItem.SubItems.Add(columns[2]); // 端口
                        // 第四列：打印模式（兼容旧配置，无数据则默认"自动"）
                        listItem.SubItems.Add(columns.Length >= 4 ? columns[3] : "自动");
                        listView1.Items.Add(listItem);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"加载任务列表失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            string DPort = ini.ReadString("打印服务器设置", "端口", "9100");
            num_服务端口.Text = DPort;

            bool autoStart = ini.ReadBool("打印服务器设置", "启动服务", false);
            chk_启动运行打印服务.Checked = autoStart;

            bool autoMin = ini.ReadBool("打印服务器设置", "启动最小化", false);
            chk_启动最小化窗口.Checked = autoMin;

            // 文件服务器设置（保持不变）
            string programDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string webDirectory = Path.Combine(programDirectory, "SerFile");
            Tbx_网站目录.Text = ini.ReadString("文件服务器设置", "文件目录", webDirectory);
            Num_网站端口.Text = ini.ReadString("文件服务器设置", "网站端口", "1314");

            string DGetPassword = ini.ReadString("文件服务器设置", "上传密码", "MTIzNDU2");
            if (DGetPassword.Length > 0)
            {
                DGetPassword = DecodeFromBase64(DGetPassword);
            }
            Tbx_上传密码.Text = DGetPassword;
            Num_限制上传大小.Text = ini.ReadString("文件服务器设置", "限制上传大小", "100");

            Chk_启动运行文件服务.Checked = ini.ReadBool("文件服务器设置", "启动服务", false);
        }

        private void DSaveIni()
        {
            var dataBuilder = new StringBuilder();
            foreach (ListViewItem item in listView1.Items)
            {
                for (int i = 0; i < item.SubItems.Count; i++)
                {
                    dataBuilder.Append(item.SubItems[i].Text);
                    if (i < item.SubItems.Count - 1)
                    {
                        dataBuilder.Append('\t');
                    }
                }
                dataBuilder.AppendLine();
            }

            string encodedData = EncodeToBase64(dataBuilder.ToString());

            IniFileHelper ini = new IniFileHelper(iniPath);
            ini.WriteString("打印服务器设置", "端口", num_服务端口.Text.Trim());
            ini.WriteBool("打印服务器设置", "启动服务", chk_启动运行打印服务.Checked);
            ini.WriteBool("打印服务器设置", "启动最小化", chk_启动最小化窗口.Checked);
            ini.WriteString("打印服务器设置", "任务列表", encodedData);

            // 文件服务器设置（保持不变）
            ini.WriteString("文件服务器设置", "文件目录", Tbx_网站目录.Text.Trim());
            ini.WriteString("文件服务器设置", "网站端口", Num_网站端口.Text.Trim());
            ini.WriteString("文件服务器设置", "上传密码", EncodeToBase64(Tbx_上传密码.Text.Trim()));
            ini.WriteString("文件服务器设置", "限制上传大小", Num_限制上传大小.Text.Trim());
            ini.WriteBool("文件服务器设置", "启动服务", Chk_启动运行文件服务.Checked);
        }

        // 将字符串编码为Base64
        private string EncodeToBase64(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(bytes);
        }

        // 从Base64解码字符串
        private string DecodeFromBase64(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            byte[] bytes = Convert.FromBase64String(input);
            return System.Text.Encoding.UTF8.GetString(bytes);
        }
        #endregion

        #region 打印机操作（新增打印模式处理）
        private void LoadPrinters()
        {
            try
            {
                if (cmb_打印机列表 == null)
                {
                    throw new Exception("打印机选择控件未初始化");
                }

                cmb_打印机列表.Items.Clear();

                // 获取所有本地打印机
                foreach (string printer in PrinterSettings.InstalledPrinters)
                {
                    cmb_打印机列表.Items.Add(printer);
                }

                // 选择默认打印机
                if (cmb_打印机列表.Items.Count > 0)
                {
                    cmb_打印机列表.SelectedIndex = 0;
                    selectedPrinter = cmb_打印机列表.SelectedItem.ToString();
                    cmb_打印机列表.SelectedIndexChanged += (s, e) =>
                    {
                        if (cmb_打印机列表.SelectedItem != null)
                        {
                            selectedPrinter = cmb_打印机列表.SelectedItem.ToString();
                        }
                    };
                    UpdateLog($"已加载 {cmb_打印机列表.Items.Count} 台打印机");
                }
                else
                {
                    UpdateLog("未找到任何打印机，请检查打印机连接");
                }
            }
            catch (Exception ex)
            {
                UpdateLog($"加载打印机失败: {ex.Message}");
            }
        }

        // 检查打印机状态
        private bool IsPrinterAvailable(string printerName)
        {
            try
            {
                var printer = new PrinterSettings();
                printer.PrinterName = printerName;
                return printer.IsValid;
            }
            catch
            {
                return false;
            }
        }

        // 获取打印机驱动类型（判断是否为XPS驱动）
        private string GetPrinterDataType(string printerName, string printMode)
        {
            // 若手动指定模式，则直接返回
            if (printMode == "RAW" || printMode == "XPS")
            {
                UpdateLog($"手动指定打印模式: {printMode}");
                return printMode == "XPS" ? "XPS_PASS" : "RAW";
            }

            // 自动模式：检测打印机驱动类型
            IntPtr hPrinter = IntPtr.Zero;
            try
            {
                if (!OpenPrinter(printerName, out hPrinter, IntPtr.Zero))
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    UpdateLog($"打开打印机失败，错误码: {errorCode} - {GetWin32ErrorMessage(errorCode)}");
                    return "RAW"; // 失败时默认RAW
                }

                int bufferSize = 0;
                if (!GetPrinterDriver(hPrinter, null, 8, IntPtr.Zero, 0, ref bufferSize))
                {
                    int error = Marshal.GetLastWin32Error();
                    if (error != ERROR_INSUFFICIENT_BUFFER)
                    {
                        UpdateLog($"获取驱动信息失败，错误码: {error}");
                        return "RAW";
                    }
                }

                IntPtr driverInfoBuffer = Marshal.AllocHGlobal(bufferSize);
                try
                {
                    if (!GetPrinterDriver(hPrinter, null, 8, driverInfoBuffer, bufferSize, ref bufferSize))
                    {
                        int errorCode = Marshal.GetLastWin32Error();
                        UpdateLog($"获取驱动信息失败，错误码: {errorCode}");
                        return "RAW";
                    }

                    // 读取DRIVER_INFO_8结构的dwPrinterDriverAttributes字段（偏移量8）
                    uint attributes = (uint)Marshal.ReadInt32(driverInfoBuffer, 8);
                    bool isXpsDriver = (attributes & PRINTER_DRIVER_XPS) != 0;
                    string dataType = isXpsDriver ? "XPS_PASS" : "RAW";
                    UpdateLog($"自动检测打印机驱动类型: {dataType}");
                    return dataType;
                }
                finally
                {
                    Marshal.FreeHGlobal(driverInfoBuffer);
                }
            }
            catch (Exception ex)
            {
                UpdateLog($"检测打印机驱动类型失败: {ex.Message}");
                return "RAW";
            }
            finally
            {
                if (hPrinter != IntPtr.Zero)
                    ClosePrinter(hPrinter);
            }
        }
        #endregion

        #region 服务控制（适配打印模式）
        private void Btn_启动打印服务_Click(object sender, EventArgs e)
        {
            if (listView1.Items.Count == 0)
            {
                MessageBox.Show("任务列表不能为空，请先添加！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (isServerRunning)
            {
                Btn_停止打印服务_Click(Btn_停止打印服务, EventArgs.Empty);
            }

            try
            {
                // 1. 验证所有任务的有效性（包含打印模式）
                List<Tuple<string, int, string>> validTasks = new List<Tuple<string, int, string>>();
                foreach (ListViewItem item in listView1.Items)
                {
                    string printerName = item.SubItems[1].Text;
                    string portText = item.SubItems[2].Text;
                    string printMode = item.SubItems[3].Text; // 第四列：打印模式

                    // 验证端口
                    if (!int.TryParse(portText, out int port) || port < 1 || port > 65535)
                    {
                        MessageBox.Show($"无效的端口号: {portText}（必须是1-65535之间的整数）", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // 验证端口唯一性
                    if (validTasks.Any(t => t.Item2 == port))
                    {
                        MessageBox.Show($"端口 {port} 被重复使用，请修改", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // 验证打印模式
                    if (!new[] { "自动", "RAW", "XPS" }.Contains(printMode))
                    {
                        MessageBox.Show($"无效的打印模式: {printMode}（仅支持自动/RAW/XPS）", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // 检查打印机是否可用
                    if (!IsPrinterAvailable(printerName))
                    {
                        MessageBox.Show($"打印机 [{printerName}] 不可用，请检查连接", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    validTasks.Add(Tuple.Create(printerName, port, printMode));
                }

                // 2. 启动所有验证通过的服务器
                isServerRunning = true;
                foreach (var task in validTasks)
                {
                    StartPrintServer(task.Item1, task.Item2, task.Item3); // 传入打印模式
                }

                // 3. 更新UI状态
                Btn_启动打印服务.Enabled = false;
                Btn_启动打印服务.BackColor = Color.Silver;
                Btn_停止打印服务.Enabled = true;
                Btn_停止打印服务.BackColor = Color.DodgerBlue;

                cmb_打印机列表.Enabled = false;
                num_服务端口.Enabled = false;
                cmbPrintMode.Enabled = false;
                Btn_添加打印机.Enabled = false;
                Btn_添加打印机.BackColor = Color.Silver;
                Btn_删除打印机.Enabled = false;
                Btn_删除打印机.BackColor = Color.Silver;
                lbl_status.Text = "服务已启动";
                UpdateLog($"所有服务启动完成，共 {validTasks.Count} 个打印机端口");
            }
            catch (Exception ex)
            {
                Btn_停止打印服务_Click(Btn_停止打印服务, EventArgs.Empty);
                UpdateLog($"启动服务失败: {ex.Message}");
                MessageBox.Show($"启动服务时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Btn_停止打印服务_Click(object sender, EventArgs e)
        {
            try
            {
                if (isServerRunning)
                {
                    isServerRunning = false;

                    // 取消所有端口的令牌
                    foreach (var cts in portCancellationSources.Values)
                    {
                        try
                        {
                            cts.Cancel();
                        }
                        catch { }
                    }

                    // 停止所有服务器
                    foreach (var server in servers)
                    {
                        try
                        {
                            server.Stop();
                        }
                        catch (Exception ex)
                        {
                            UpdateLog($"停止服务器时出错: {ex.Message}");
                        }
                    }

                    // 等待所有任务结束
                    Task.WaitAll(serverTasks.ToArray(), 5000);

                    // 清空管理集合
                    servers.Clear();
                    serverTasks.Clear();
                    portToPrinter.Clear();
                    portToPrintMode.Clear(); // 清空打印模式映射
                    portCancellationSources.Clear();

                    // 更新UI
                    Btn_停止打印服务.Enabled = false;
                    Btn_停止打印服务.BackColor = Color.Silver;
                    Btn_启动打印服务.Enabled = true;
                    Btn_启动打印服务.BackColor = Color.DodgerBlue;
                    cmb_打印机列表.Enabled = true;
                    num_服务端口.Enabled = true;
                    cmbPrintMode.Enabled = true; // 启用打印模式选择
                    Btn_添加打印机.Enabled = true;
                    Btn_添加打印机.BackColor = Color.LightYellow;
                    Btn_删除打印机.Enabled = true;
                    Btn_删除打印机.BackColor = Color.LightYellow;
                    lbl_status.Text = "服务已停止";
                    UpdateLog("所有服务已停止");
                }
            }
            catch (Exception ex)
            {
                UpdateLog($"停止服务失败: {ex.Message}");
            }
        }
        #endregion

        #region 日志管理
        private void UpdateLog(string message)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(logDelegate, message);
                return;
            }

            if (rtbLog == null)
                return;

            string logEntry = $"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}";
            rtbLog.AppendText(logEntry);
            rtbLog.ScrollToCaret();

            // 保存到文件
            SaveLogToFile(logEntry);

            // 限制内存中日志数量
            if (rtbLog.Lines.Length > 10000)
            {
                var lines = rtbLog.Lines;
                rtbLog.Lines = lines.Skip(lines.Length - 5000).ToArray();
                rtbLog.ScrollToCaret();
            }
        }

        private void SaveLogToFile(string logEntry)
        {
            try
            {
                lock (_lockObj)
                {
                    if (File.Exists(logFilePath) && new FileInfo(logFilePath).Length > MAX_LOG_SIZE)
                    {
                        string backupLogPath = $"{logFilePath}.{DateTime.Now:yyyyMMddHHmmss}";
                        File.Move(logFilePath, backupLogPath);
                    }

                    File.AppendAllText(logFilePath, logEntry);
                }
            }
            catch (Exception ex)
            {
                rtbLog.AppendText($"[{DateTime.Now:HH:mm:ss}] 日志保存失败: {ex.Message}{Environment.NewLine}");
            }
        }
        #endregion

        #region 窗口事件
        protected override void OnClosing(CancelEventArgs e)
        {
            if (isServerRunning)
            {
                Btn_停止打印服务_Click(Btn_停止打印服务, EventArgs.Empty);
            }
            _clientSemaphore.Dispose();
            base.OnClosing(e);
        }

        private void Form_Main_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                notifyIcon1.Visible = true;
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.ShowInTaskbar = true;
            this.WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
        }

        private void Form_Main_Load(object sender, EventArgs e)
        {
            if (chk_启动运行打印服务.Checked)
            {
                this.BeginInvoke(new Action(() =>
                {
                    Btn_启动打印服务_Click(Btn_启动打印服务, EventArgs.Empty);
                }));
            }
            if (Chk_启动运行文件服务.Checked)
            {
                this.BeginInvoke(new Action(() =>
                {
                    Btn_启动文件服务_Click(Btn_启动停止文件服务, EventArgs.Empty);
                }));
            }
            if (chk_启动最小化窗口.Checked)
            {
                this.BeginInvoke(new Action(() =>
                {
                    this.WindowState = FormWindowState.Minimized;
                    this.ShowInTaskbar = false;
                }));
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                SetAutoStart(chk_开机启动.Checked);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"设置失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                chk_开机启动.Checked = !chk_开机启动.Checked;
            }
        }

        private void Form_Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "确定要退出程序吗？",
                "确认退出",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.No)
            {
                e.Cancel = true;
                return;
            }

            DSaveIni();
        }
        #endregion

        #region 任务管理（适配第四列打印模式）
        // 添加打印机任务（新增打印模式选择）
        private void Btn_添加打印机_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cmb_打印机列表.Text))
            {
                MessageBox.Show("请选择打印机", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int port = (int)num_服务端口.Value;
            if (port < 1 || port > 65535)
            {
                MessageBox.Show("端口号必须在1-65535之间", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }



            string printMode = cmbPrintMode.Text; 

            // 验证重复
            foreach (ListViewItem item in listView1.Items)
            {
                if (item.SubItems[1].Text == cmb_打印机列表.Text)
                {
                    MessageBox.Show("已存在相同的打印机，请勿重复添加！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (item.SubItems[2].Text == port.ToString())
                {
                    MessageBox.Show("已存在相同的端口号，请勿重复添加！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            // 添加到列表
            var newItem = new ListViewItem((listView1.Items.Count + 1).ToString());
            newItem.SubItems.Add(cmb_打印机列表.Text);
            newItem.SubItems.Add(port.ToString());
            newItem.SubItems.Add(printMode); // 第四列：打印模式
            listView1.Items.Add(newItem);
            DSaveIni();
        }

        private void Btn_删除打印机_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                MessageBox.Show("请先选择要删除的项目", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                foreach (ListViewItem item in listView1.SelectedItems.Cast<ListViewItem>().ToList())
                {
                    listView1.Items.Remove(item);
                }

                // 重新编号
                for (int i = 0; i < listView1.Items.Count; i++)
                {
                    listView1.Items[i].Text = (i + 1).ToString();
                }
                DSaveIni();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"删除项目时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region TCP服务器与客户端处理（适配打印模式）
        private void StartPrintServer(string printerName, int port, string printMode)
        {
            try
            {
                var cts = new CancellationTokenSource();
                portCancellationSources[port] = cts;

                TcpListener server = new TcpListener(IPAddress.Any, port);
                servers.Add(server);
                portToPrinter[port] = printerName;
                portToPrintMode[port] = printMode; // 保存打印模式

                server.Start();
                UpdateLog($"打印机 [{printerName}] 的服务器已启动，监听端口: {port}，打印模式: {printMode}");

                var task = Task.Run(() => ServerLoopAsync(server, port, cts.Token), cts.Token);
                serverTasks.Add(task);

                task.ContinueWith(t =>
                {
                    if (t.IsFaulted && isServerRunning)
                    {
                        UpdateLog($"端口 {port} 服务器异常终止，尝试重启...");
                        this.Invoke((Action)(() =>
                        {
                            if (isServerRunning && !portToPrinter.ContainsKey(port))
                            {
                                StartPrintServer(printerName, port, printMode);
                            }
                        }));
                    }
                }, TaskContinuationOptions.OnlyOnFaulted);
            }
            catch (SocketException ex)
            {
                UpdateLog($"端口 {port} 启动失败（可能被占用）: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                UpdateLog($"启动打印机 [{printerName}] 服务失败: {ex.Message}");
                throw;
            }
        }

        private async Task ServerLoopAsync(TcpListener server, int port, CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    if (server.Pending())
                    {
                        TcpClient client = await server.AcceptTcpClientAsync();
                        string printerName = portToPrinter[port];
                        string printMode = portToPrintMode[port]; // 获取打印模式
                        UpdateLog($"端口 {port} 接收到客户端连接: {((IPEndPoint)client.Client.RemoteEndPoint)?.Address}，将使用打印机 [{printerName}]（模式: {printMode}）");

                        await _clientSemaphore.WaitAsync(cancellationToken);

                        _ = HandleClientWithPrinterAsync(client, printerName, printMode, cancellationToken)
                            .ContinueWith(t =>
                            {
                                _clientSemaphore.Release();
                                if (t.IsFaulted)
                                {
                                    UpdateLog($"客户端处理异常: {t.Exception?.InnerException?.Message}");
                                }
                            });
                    }
                    else
                    {
                        await Task.Delay(100, cancellationToken);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                UpdateLog($"端口 {port} 服务器已取消");
            }
            catch (Exception ex)
            {
                if (!cancellationToken.IsCancellationRequested)
                    UpdateLog($"端口 {port} 服务器错误: {ex.Message}");
                throw;
            }
        }

        private async Task HandleClientWithPrinterAsync(TcpClient client, string printerName, string printMode, CancellationToken cancellationToken)
        {
            if (client == null) return;

            try
            {
                client.ReceiveTimeout = CLIENT_TIMEOUT;
                client.SendTimeout = CLIENT_TIMEOUT;

                using (NetworkStream stream = client.GetStream())
                using (MemoryStream ms = new MemoryStream())
                {
                    byte[] buffer = new byte[BUFFER_SIZE];
                    int bytesRead;

                    UpdateLog("开始接收打印数据...");

                    while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
                    {
                        await ms.WriteAsync(buffer, 0, bytesRead, cancellationToken);

                        if (ms.Length > 100 * 1024 * 1024) // 限制100MB
                        {
                            UpdateLog("接收数据超过最大限制，中断连接");
                            break;
                        }
                    }

                    UpdateLog($"已接收 {ms.Length} 字节的打印数据");

                    if (ms.Length > 0)
                    {
                        if (!IsPrinterAvailable(printerName))
                        {
                            UpdateLog($"打印机 [{printerName}] 不可用，无法打印");
                            return;
                        }

                        // 传入打印模式进行打印
                        await Task.Run(() => PrintBytesToPrinter(ms.ToArray(), printerName, printMode), cancellationToken);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                UpdateLog("客户端处理已取消");
            }
            catch (Exception ex)
            {
                UpdateLog($"处理客户端错误: {ex.Message}");
            }
            finally
            {
                try
                {
                    client.Close();
                }
                catch { }
                UpdateLog("客户端连接已关闭");
            }
        }

        // 打印数据（适配打印模式）
        private void PrintBytesToPrinter(byte[] data, string printerName, string printMode)
        {
            if (data == null || data.Length == 0)
            {
                UpdateLog("没有可打印的数据");
                return;
            }

            if (string.IsNullOrEmpty(printerName))
            {
                UpdateLog("打印机名称为空");
                return;
            }

            try
            {
                UpdateLog($"正在向打印机 [{printerName}] 发送数据（模式: {printMode}）...");

                IntPtr hPrinter = new IntPtr(0);
                RawPrinterHelper.DOCINFO di = new RawPrinterHelper.DOCINFO();
                di.pDocName = "USB Print Server Document";
                // 根据打印模式获取数据类型
                di.pDataType = GetPrinterDataType(printerName, printMode);

                bool success = RawPrinterHelper.OpenPrinter(printerName.Normalize(), ref hPrinter, IntPtr.Zero);
                if (!success)
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    UpdateLog($"无法打开打印机 [{printerName}]，错误代码: {errorCode} - {GetWin32ErrorMessage(errorCode)}");
                    return;
                }

                try
                {
                    success = RawPrinterHelper.StartDocPrinter(hPrinter, 1, di);
                    if (success)
                    {
                        try
                        {
                            success = RawPrinterHelper.WritePrinter(hPrinter, data, data.Length, out int bytesWritten);
                            if (success)
                            {
                                UpdateLog($"成功向 [{printerName}] 发送 {bytesWritten} 字节（数据类型: {di.pDataType}）");
                            }
                            else
                            {
                                int errorCode = Marshal.GetLastWin32Error();
                                UpdateLog($"向 [{printerName}] 发送数据失败，错误代码: {errorCode} - {GetWin32ErrorMessage(errorCode)}");
                            }
                        }
                        finally
                        {
                            RawPrinterHelper.EndDocPrinter(hPrinter);
                        }
                    }
                    else
                    {
                        int errorCode = Marshal.GetLastWin32Error();
                        UpdateLog($"无法开始 [{printerName}] 的打印作业，错误代码: {errorCode} - {GetWin32ErrorMessage(errorCode)}");
                    }
                }
                finally
                {
                    RawPrinterHelper.ClosePrinter(hPrinter);
                }
            }
            catch (Exception ex)
            {
                UpdateLog($"打印到 [{printerName}] 错误: {ex.Message}");
            }
        }

        // 获取Windows错误信息
        private string GetWin32ErrorMessage(int errorCode)
        {
            try
            {
                return new System.ComponentModel.Win32Exception(errorCode).Message;
            }
            catch
            {
                return "未知错误";
            }
        }
        #endregion

        #region 原始打印机操作辅助类（扩展支持XPS）
        internal static class RawPrinterHelper
        {
            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
            public class DOCINFO
            {
                [MarshalAs(UnmanagedType.LPStr)] public string pDocName;
                [MarshalAs(UnmanagedType.LPStr)] public string pOutputFile;
                [MarshalAs(UnmanagedType.LPStr)] public string pDataType; // 支持XPS_PASS/RAW
            }

            [DllImport("winspool.Drv", EntryPoint = "OpenPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern bool OpenPrinter([MarshalAs(UnmanagedType.LPStr)] string szPrinter, ref IntPtr hPrinter, IntPtr pd);

            [DllImport("winspool.Drv", EntryPoint = "ClosePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern bool ClosePrinter(IntPtr hPrinter);

            [DllImport("winspool.Drv", EntryPoint = "StartDocPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern bool StartDocPrinter(IntPtr hPrinter, Int32 level, [In, MarshalAs(UnmanagedType.LPStruct)] DOCINFO di);

            [DllImport("winspool.Drv", EntryPoint = "EndDocPrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern bool EndDocPrinter(IntPtr hPrinter);

            [DllImport("winspool.Drv", EntryPoint = "WritePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern bool WritePrinter(IntPtr hPrinter, byte[] pBytes, Int32 dwCount, out Int32 dwWritten);
        }
        #endregion

        #region P/Invoke声明（打印机驱动检测）
        [DllImport("winspool.drv", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool GetPrinterDriver(IntPtr hPrinter, string pEnvironment, int Level, IntPtr pDriverInfo, int cbBuf, ref int pcbNeeded);

        [DllImport("winspool.drv", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool OpenPrinter(string pPrinterName, out IntPtr phPrinter, IntPtr pDefault);

        [DllImport("winspool.drv", SetLastError = true)]
        private static extern bool ClosePrinter(IntPtr hPrinter);
        #endregion

        #region 文件服务器相关（保持不变）
        private void Btn_浏览目录_Click(object sender, EventArgs e)
        {
            using (var folderBrowserDialog = new FolderBrowserDialog())
            {
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    Tbx_网站目录.Text = folderBrowserDialog.SelectedPath;
                }
            }
        }

        private void Btn_默认目录_Click(object sender, EventArgs e)
        {
            Tbx_网站目录.Text = DGetDir_Web();
        }

        private void Btn_预览_Click(object sender, EventArgs e)
        {
            string host = $"http://{GetLocalIPAddress()}";
            string port = Num_网站端口.Text;
            string url = host + ":" + port;

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("打开网址时出现错误: " + ex.Message);
            }
        }

        private void Btn_启动文件服务_Click(object sender, EventArgs e)
        {
            rich_web_info.Clear();
            if (Btn_启动停止文件服务.Text=="启动")
            {
                string baseFolder = Tbx_网站目录.Text;
                int port = int.Parse(Num_网站端口.Text.Trim());
                if (!DirectoryExists(baseFolder))
                {
                    MessageBox.Show("目录不存在，请输入有效的目录路径", "提示");
                    Btn_启动停止文件服务.Enabled = true;
                    return;
                }
                if (IsPortInUse(port))
                {
                    MessageBox.Show("端口已被占用，请选择另一个端口", "提示");
                    Btn_启动停止文件服务.Enabled = true;
                    return;
                }
                Btn_启动停止文件服务.Text = "停止";
                Btn_启动停止文件服务.BackColor = Color.SeaGreen;
                Tbx_网站目录.Enabled = false;
                Btn_浏览目录.Enabled = false;
                Btn_浏览目录.BackColor = Color.Silver;
                Btn_默认目录.Enabled = false;
                Btn_默认目录.BackColor = Color.Silver;
                Num_网站端口.Enabled = false;
                Tbx_上传密码.Enabled = false;
                Num_限制上传大小.Enabled = false;
                Btn_预览.Enabled = true;
                Btn_预览.BackColor = Color.RoyalBlue;

                server = new WebFileServer(baseFolder, port);
                server.SetUploadPassword(Tbx_上传密码.Text.Trim());
                server.SetMaxFileSize(((long)Num_限制上传大小.Value));
                server.StartServer();
                rich_web_info.AppendText("[" + DateTime.Now.ToString() + "]: " + "文件服务器已启动...\n");
                rich_web_info.AppendText("[" + DateTime.Now.ToString() + "]: " + "访问地址: " + $"http://{GetLocalIPAddress()}:" + port + "\n");
            }
            else
            {
                    if (server != null)
                    {
                        rich_web_info.AppendText("[" + DateTime.Now.ToString() + "]: " + "文件服务器已停止...\n");
                        server.StopServer();
                        server = null;
                    }
                    else
                    {
                        rich_web_info.AppendText("[" + DateTime.Now.ToString() + "]: " + "文件服务器尚未启动或已经停止...\n");
                    }
                    Btn_启动停止文件服务.Text = "启动";
                    Btn_启动停止文件服务.BackColor = Color.Crimson;
                    Tbx_网站目录.Enabled = true;
                    Btn_浏览目录.Enabled = true;
                    Btn_浏览目录.BackColor = Color.SeaGreen;
                    Btn_默认目录.Enabled = true;
                    Btn_默认目录.BackColor = Color.SeaGreen;
                    Num_网站端口.Enabled = true;
                    Tbx_上传密码.Enabled = true;
                    Num_限制上传大小.Enabled = true;
                    Btn_预览.Enabled = false;
                    Btn_预览.BackColor = Color.Silver;
            }
        }
        private string DGetDir_Web()
        {
            string programDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string webDirectory = Path.Combine(programDirectory, "SerFile");

            if (!Directory.Exists(webDirectory))
            {
                Directory.CreateDirectory(webDirectory);
            }

            return webDirectory;
        }
        private bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }
        private bool IsPortInUse(int port)
        {
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] endPoints = ipGlobalProperties.GetActiveTcpListeners();

            foreach (IPEndPoint endPoint in endPoints)
            {
                if (endPoint.Port == port)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion
    }
}