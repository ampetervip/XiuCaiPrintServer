using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;

namespace Print_Ser
{
    internal class WebFileServer
    {
        private HttpListener listener;
        public string BaseFolder { get; set; }// 文件夹路径
        public int Port { get; set; } = 8080;// 默认端口8080
        private string UPLOAD_PASSWORD = "223300"; // 上传密码
        private long MAX_FILE_SIZE = (long)4000 * 1024 * 1024; // 50MB                                                  
        private static readonly string[] Units = { "B", "KB", "MB", "GB", "TB" };// 文件大小单位
        /**
     * 将字节数转换为带单位的易读字符串
     * @param bytes 字节数
     * @return 格式化后的字符串，如"50 MB"
     */
        public static string FormatFileSize(long bytes)
        {
            if (bytes < 0)
            {
                throw new ArgumentException("文件大小不能为负数");
            }
            if (bytes == 0)
            {
                return "0 B";
            }

            // 计算最合适的单位
            int unitIndex = 0;
            double size = bytes;

            // 当大小大于等于1024且还有更大单位时，继续转换
            while (size >= 1024 && unitIndex < Units.Length - 1)
            {
                size /= 1024;
                unitIndex++;
            }

            // C# 中使用 {0}、{1} 作为占位符
            return String.Format("{0:0.##} {1}", size, Units[unitIndex]);
        }

        public WebFileServer(string baseFolder, int port)
        {
            BaseFolder = baseFolder;
            Port = port;

            if (!Directory.Exists(BaseFolder))
            {
                Directory.CreateDirectory(BaseFolder);
            }
        }

        public void SetUploadPassword(string CusTomPassword)
        {
            if (string.IsNullOrWhiteSpace(CusTomPassword))
            {
                Console.WriteLine("密码不能为空，请重新设置...");
                return;
            }
            UPLOAD_PASSWORD = CusTomPassword;
            Console.WriteLine($"上传密码已设置为: {UPLOAD_PASSWORD}");

        }

        public void SetMaxFileSize(long size)
        {
            if (size <= 0)
            {
                Console.WriteLine("文件大小必须大于0，请重新设置...");
                return;
            }
            MAX_FILE_SIZE = size * 1024 * 1024;
            Console.WriteLine($"最大文件大小已设置为: {FormatFileSize(MAX_FILE_SIZE)}");
        }

        public void StartServer()
        {
            if (!HttpListener.IsSupported)
            {
                Console.WriteLine("HttpListener不受当前操作系统支持。");
                return;
            }

            if (listener == null)
            {
                listener = new HttpListener();
                listener.Prefixes.Add($"http://*:{Port}/");
            }

            if (!listener.IsListening)
            {
                try
                {
                    listener.Start();
                    listener.BeginGetContext(new AsyncCallback(OnRequestReceive), listener);
                    Console.WriteLine($"服务器已启动，监听端口 {Port}...");
                    Console.WriteLine($"文件根目录: {BaseFolder}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"启动服务器失败: {ex.Message}");
                }
            }
        }

        public void StopServer()
        {
            if (listener != null)
            {
                listener.Stop();
                listener.Close();
                listener = null;
                Console.WriteLine("服务器已停止。");
            }
        }

        private void OnRequestReceive(IAsyncResult result)
        {
            if (listener == null || !listener.IsListening)
                return;

            try
            {
                var context = listener.EndGetContext(result);
                listener.BeginGetContext(new AsyncCallback(OnRequestReceive), listener);

                var request = context.Request;
                var response = context.Response;

                ProcessRequest(request, response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"处理请求时发生错误: {ex.Message}");
            }
        }

        private void ProcessRequest(HttpListenerRequest request, HttpListenerResponse response)
        {
            try
            {
                string localPath = request.Url.LocalPath.TrimStart('/');
                string filePath = Path.Combine(BaseFolder, localPath.Replace("/", "\\"));
                string currentPath = request.Url.LocalPath.TrimEnd('/');

                // 处理文件上传 (POST请求)
                if (request.HttpMethod == "POST" && request.ContentType?.StartsWith("multipart/form-data") == true)
                {
                    HandleFileUpload(request, response, filePath);
                    return;
                }

                // 处理下载请求
                if (request.Url.Query.Contains("download=1"))
                {
                    if (Directory.Exists(filePath))
                    {
                        HandleDirectoryDownload(response, filePath);
                    }
                    else
                    {
                        SendErrorResponse(response, 404, "未找到文件夹，无法打包下载...");
                    }
                    return;
                }

                // 处理目录浏览
                if (Directory.Exists(filePath))
                {
                    GenerateDirectoryListing(response, filePath, currentPath, request.Url.AbsoluteUri);
                    return;
                }

                // 处理文件下载
                if (File.Exists(filePath))
                {
                    SendFileResponse(response, filePath);
                    return;
                }

                // 未找到资源
                SendErrorResponse(response, 404, "未找到请求的资源...");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"处理请求时出错: {ex.Message}");
                SendErrorResponse(response, 500, "服务器内部错误，请稍后再试...");
            }
            finally
            {
                response.OutputStream.Close();
            }
        }

        private void GenerateDirectoryListing(HttpListenerResponse response, string filePath, string currentPath, string baseUrl)
        {
            response.ContentType = "text/html; charset=utf-8";
            using (StreamWriter writer = new StreamWriter(response.OutputStream, Encoding.UTF8))
            {
                writer.WriteLine("<!DOCTYPE html>");
                writer.WriteLine("<html lang=\"zh-CN\">");
                writer.WriteLine("<head>");
                writer.WriteLine("<meta charset=\"UTF-8\">");
                writer.WriteLine("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
                writer.WriteLine("<title>秀才文件服务器</title>");
                writer.WriteLine("<style>");
                writer.WriteLine("body { font-family: Arial, sans-serif; margin: 0; padding: 0; background-color: #f4f4f4; }");
                writer.WriteLine(".container { max-width: 1200px; margin: 0 auto; padding: 20px; background-color: #fff; box-shadow: 0 0 10px rgba(0, 0, 0, 0.1); }");
                writer.WriteLine("h1 { text-align: center; color: #333; margin-bottom: 30px; }");
                writer.WriteLine(".breadcrumb { margin-bottom: 20px; }");
                writer.WriteLine("table { width: 100%; border-collapse: collapse; margin-top: 20px; }");
                writer.WriteLine("th, td { padding: 12px; border: 1px solid #ddd; }");
                writer.WriteLine("th { background-color: #f2f2f2; text-align: center; }");
                writer.WriteLine("td { text-align: center; }");
                writer.WriteLine("td.name { text-align: left; }");
                writer.WriteLine("tr:nth-child(even) { background-color: #f9f9f9; }");
                writer.WriteLine(".folder { font-weight: bold; color: #007bff; text-decoration: none; margin-left: 5px;}");
                writer.WriteLine(".file { color: #555; text-decoration: none; margin-left: 5px;}");
                writer.WriteLine(".footer { text-align: center; margin-top: 30px; padding: 15px; background-color: #f9f9f9; border-top: 1px solid #ccc; }");
                writer.WriteLine(".download-button { display: inline-block; padding: 8px 12px; text-decoration: none; border-radius: 4px; }");
                writer.WriteLine(".download-button.file { background-color: #007bff; color: white; }");
                writer.WriteLine(".download-button.folder { background-color: #28a745; color: white; }");
                writer.WriteLine("#showUploadBtn {");
                writer.WriteLine("    padding: 10px 20px;");
                writer.WriteLine("    background-color: #4CAF50;");
                writer.WriteLine("    color: white;");
                writer.WriteLine("    border: none;");
                writer.WriteLine("    border-radius: 4px;");
                writer.WriteLine("    cursor: pointer;");
                writer.WriteLine("    font-size: 16px;");
                writer.WriteLine("    margin-bottom: 20px;");
                writer.WriteLine("}");
                writer.WriteLine("#showUploadBtn:hover {");
                writer.WriteLine("    background-color: #45a049;");
                writer.WriteLine("}");

                // 拖放区域样式 - 默认隐藏
                writer.WriteLine("#dropArea {");
                writer.WriteLine("    border: 4px dashed #ccc;");
                writer.WriteLine("    border-radius: 10px;");
                writer.WriteLine("    padding: 40px;");
                writer.WriteLine("    text-align: center;");
                writer.WriteLine("    margin-bottom: 30px;");
                writer.WriteLine("    transition: all 0.3s;");
                writer.WriteLine("    display: none; /* 默认隐藏 */");
                writer.WriteLine("}");
                writer.WriteLine("#dropArea.highlight {");
                writer.WriteLine("    border-color: #007bff;");
                writer.WriteLine("    background-color: #e6f2ff;");
                writer.WriteLine("}");
                writer.WriteLine("#passwordModal {");
                writer.WriteLine("    display: none;");
                writer.WriteLine("    position: fixed;");
                writer.WriteLine("    top: 0;");
                writer.WriteLine("    left: 0;");
                writer.WriteLine("    width: 100%;");
                writer.WriteLine("    height: 100%;");
                writer.WriteLine("    background-color: rgba(0,0,0,0.5);");
                writer.WriteLine("    z-index: 1000;");
                writer.WriteLine("}");
                writer.WriteLine(".modalContent {");
                writer.WriteLine("    background-color: #fff;");
                writer.WriteLine("    margin: 15% auto;");
                writer.WriteLine("    padding: 20px;");
                writer.WriteLine("    width: 300px;");
                writer.WriteLine("    border-radius: 5px;");
                writer.WriteLine("}");
                writer.WriteLine("#passwordInput {");
                writer.WriteLine("    width: 100%;");
                writer.WriteLine("    padding: 8px;");
                writer.WriteLine("    margin: 10px 0;");
                writer.WriteLine("    box-sizing: border-box;");
                writer.WriteLine("}");
                writer.WriteLine(".upload-status {");
                writer.WriteLine("    margin-top: 20px;");
                writer.WriteLine("    padding: 10px;");
                writer.WriteLine("    display: none;");
                writer.WriteLine("}");
                writer.WriteLine(".success {");
                writer.WriteLine("    background-color: #d4edda;");
                writer.WriteLine("    color: #155724;");
                writer.WriteLine("}");
                writer.WriteLine(".error {");
                writer.WriteLine("    background-color: #f8d7da;");
                writer.WriteLine("    color: #721c24;");
                writer.WriteLine("}");

                // 进度条样式
                writer.WriteLine(".progress-container {");
                writer.WriteLine("    width: 100%;");
                writer.WriteLine("    height: 20px;");
                writer.WriteLine("    background-color: #f1f1f1;");
                writer.WriteLine("    border-radius: 10px;");
                writer.WriteLine("    margin: 10px 0;");
                writer.WriteLine("    overflow: hidden;");
                writer.WriteLine("    display: none;");
                writer.WriteLine("}");
                writer.WriteLine(".progress-bar {");
                writer.WriteLine("    height: 100%;");
                writer.WriteLine("    background-color: #4CAF50;");
                writer.WriteLine("    width: 0%;");
                writer.WriteLine("    transition: width 0.3s ease;");
                writer.WriteLine("}");
                writer.WriteLine(".file-progress {");
                writer.WriteLine("    margin: 5px 0;");
                writer.WriteLine("    font-size: 14px;");
                writer.WriteLine("}");

                writer.WriteLine("@media (max-width: 767px) {");
                writer.WriteLine("  .container { padding: 10px; }");
                writer.WriteLine("  h1 { font-size: 24px; margin-bottom: 20px; }");
                writer.WriteLine("  th, td { padding: 8px; }");
                writer.WriteLine("  .download-button { padding: 6px 10px; }");
                writer.WriteLine("  .modalContent { width: 80%; }");
                writer.WriteLine("}");
                writer.WriteLine("</style>");
                writer.WriteLine("</head>");
                writer.WriteLine("<body>");
                writer.WriteLine("<div class=\"container\">");
                writer.WriteLine("<h1>===秀才文件服务器===</h1>");

                // 新增上传按钮，控制拖放区域显示
                writer.WriteLine("<button id=\"showUploadBtn\">上传文件</button>");

                // 拖放上传区域 - 默认隐藏
                writer.WriteLine("<div id=\"dropArea\">");
                writer.WriteLine($"    <p>拖放文件到此处上传，或点击选择文件,注:单文件限制大小为:{FormatFileSize(MAX_FILE_SIZE)}</p>");
                writer.WriteLine("    <input type=\"file\" id=\"fileInput\" multiple style=\"display: none\">");
                writer.WriteLine("    <button onclick=\"document.getElementById('fileInput').click()\">选择文件</button>");
                writer.WriteLine("    <button onclick=\"document.getElementById('dropArea').style.display = 'none'; document.getElementById('showUploadBtn').style.display = 'block';\" style=\"margin-left: 10px;\">取消上传</button>");

                // 上传进度显示区域
                writer.WriteLine("    <div id=\"progressContainer\" class=\"progress-container\">");
                writer.WriteLine("        <div id=\"progressBar\" class=\"progress-bar\"></div>");
                writer.WriteLine("    </div>");
                writer.WriteLine("    <div id=\"fileProgress\" class=\"file-progress\"></div>");
                writer.WriteLine("</div>");

                // 上传状态显示
                writer.WriteLine("<div id=\"uploadStatus\" class=\"upload-status\"></div>");

                // 密码验证弹窗
                writer.WriteLine("<div id=\"passwordModal\">");
                writer.WriteLine("    <div class=\"modalContent\">");
                writer.WriteLine("        <h3>请输入上传密码</h3>");
                writer.WriteLine("        <input type=\"password\" id=\"passwordInput\" placeholder=\"输入密码\">");
                writer.WriteLine("        <button onclick=\"submitPassword()\">确认</button>");
                writer.WriteLine("        <button onclick=\"cancelUpload()\">取消</button>");
                writer.WriteLine("    </div>");
                writer.WriteLine("</div>");

                // 路径导航和文件列表
                writer.WriteLine("<div class=\"breadcrumb\">");
                string[] pathParts = currentPath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                string accumulatedPath = "";
                writer.WriteLine("路径：<a href=\"/\">根目录</a>");
                foreach (string part in pathParts)
                {
                    accumulatedPath += "/" + part;
                    writer.WriteLine($" / <a href=\"{accumulatedPath}\">{part}</a>");
                }
                writer.WriteLine("</div>");

                writer.WriteLine("<table>");
                writer.WriteLine("<thead>");
                writer.WriteLine("<tr>");
                writer.WriteLine("<th class=\"name\">名称</th>");
                writer.WriteLine("<th>大小</th>");
                writer.WriteLine("<th>修改时间</th>");
                writer.WriteLine("<th>操作</th>");
                writer.WriteLine("</tr>");
                writer.WriteLine("</thead>");
                writer.WriteLine("<tbody>");

                var directories = Directory.GetDirectories(filePath).OrderBy(d => d).ToList();
                var files = Directory.GetFiles(filePath).OrderBy(f => f).ToList();

                foreach (var dir in directories)
                {
                    string dirName = Path.GetFileName(dir);
                    string dirUrl = baseUrl.TrimEnd('/') + "/" + dirName;

                    DirectoryInfo directoryInfo = new DirectoryInfo(dir);
                    writer.WriteLine("<tr>");
                    writer.WriteLine($"<td class=\"name\">📁<a href=\"{dirUrl}/\" class=\"folder\">{dirName}</a></td>");
                    writer.WriteLine($"<td>—</td>");
                    writer.WriteLine($"<td>{directoryInfo.LastWriteTime:yyyy-MM-dd HH:mm:ss}</td>");
                    writer.WriteLine($"<td><a href=\"{dirUrl}?download=1\" class=\"download-button folder\">打包下载</a></td>");
                    writer.WriteLine("</tr>");
                }

                foreach (var file in files)
                {
                    string fileName = Path.GetFileName(file);
                    string fileUrl = baseUrl.TrimEnd('/') + "/" + fileName;

                    FileInfo fileInfo = new FileInfo(file);
                    string fileSize;
                    if (fileInfo.Length < 1024)
                        fileSize = $"{fileInfo.Length} B";
                    else if (fileInfo.Length < 1024 * 1024)
                        fileSize = $"{fileInfo.Length / 1024.0:F2} KB";
                    else
                        fileSize = $"{fileInfo.Length / (1024.0 * 1024.0):F2} MB";

                    // 为不同压缩文件添加特殊图标
                    string fileIcon = "📄";
                    if (fileName.EndsWith(".rar", StringComparison.OrdinalIgnoreCase))
                    {
                        fileIcon = "🗜️";
                    }
                    else if (fileName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                    {
                        fileIcon = "🤐";
                    }
                    else if (fileName.EndsWith(".7z", StringComparison.OrdinalIgnoreCase))
                    {
                        fileIcon = "🧩";
                    }
                    else if (fileName.EndsWith(".tar", StringComparison.OrdinalIgnoreCase) ||
                             fileName.EndsWith(".gz", StringComparison.OrdinalIgnoreCase) ||
                             fileName.EndsWith(".bz2", StringComparison.OrdinalIgnoreCase))
                    {
                        fileIcon = "📦";
                    }
                    else if (Directory.Exists(file))
                    {
                        fileIcon = "📁";
                    }

                    writer.WriteLine("<tr>");
                    writer.WriteLine($"<td class=\"name\">{fileIcon}<a href=\"{fileUrl}\" class=\"file\">{fileName}</a></td>");
                    writer.WriteLine($"<td>{fileSize}</td>");
                    writer.WriteLine($"<td>{fileInfo.LastWriteTime:yyyy-MM-dd HH:mm:ss}</td>");
                    writer.WriteLine($"<td><a href=\"{fileUrl}\" class=\"download-button file\">文件下载</a></td>");
                    writer.WriteLine("</tr>");
                }

                writer.WriteLine("</tbody>");
                writer.WriteLine("</table>");

                writer.WriteLine("<div class=\"footer\">");
                writer.WriteLine("<p>中国.上海</br>© 2025 阳光有点冷.微信:51529502</p>");
                writer.WriteLine("</div>");
                writer.WriteLine("</div>");

                // 上传脚本 - 包含进度条功能
                writer.WriteLine("<script>");
                writer.WriteLine("let dropArea = document.getElementById('dropArea');");
                writer.WriteLine("let fileInput = document.getElementById('fileInput');");
                writer.WriteLine("let passwordModal = document.getElementById('passwordModal');");
                writer.WriteLine("let passwordInput = document.getElementById('passwordInput');");
                writer.WriteLine("let uploadStatus = document.getElementById('uploadStatus');");
                writer.WriteLine("let filesToUpload = null;");
                writer.WriteLine("let showUploadBtn = document.getElementById('showUploadBtn');");
                writer.WriteLine("let progressContainer = document.getElementById('progressContainer');");
                writer.WriteLine("let progressBar = document.getElementById('progressBar');");
                writer.WriteLine("let fileProgress = document.getElementById('fileProgress');");
                writer.WriteLine("let totalFiles = 0;");
                writer.WriteLine("let uploadedFiles = 0;");
                writer.WriteLine("let totalSize = 0;");
                writer.WriteLine("let uploadedSize = 0;");

                // 上传按钮点击事件，显示拖放区域
                writer.WriteLine("showUploadBtn.addEventListener('click', function() {");
                writer.WriteLine("    dropArea.style.display = 'block';");
                writer.WriteLine("    showUploadBtn.style.display = 'none';");
                writer.WriteLine("});");

                writer.WriteLine("['dragenter', 'dragover', 'dragleave', 'drop'].forEach(eventName => {");
                writer.WriteLine("    dropArea.addEventListener(eventName, preventDefaults, false);");
                writer.WriteLine("});");

                writer.WriteLine("function preventDefaults(e) {");
                writer.WriteLine("    e.preventDefault();");
                writer.WriteLine("    e.stopPropagation();");
                writer.WriteLine("}");

                writer.WriteLine("['dragenter', 'dragover'].forEach(eventName => {");
                writer.WriteLine("    dropArea.addEventListener(eventName, highlight, false);");
                writer.WriteLine("});");

                writer.WriteLine("['dragleave', 'drop'].forEach(eventName => {");
                writer.WriteLine("    dropArea.addEventListener(eventName, unhighlight, false);");
                writer.WriteLine("});");

                writer.WriteLine("function highlight() {");
                writer.WriteLine("    dropArea.classList.add('highlight');");
                writer.WriteLine("}");

                writer.WriteLine("function unhighlight() {");
                writer.WriteLine("    dropArea.classList.remove('highlight');");
                writer.WriteLine("}");

                writer.WriteLine("dropArea.addEventListener('drop', handleDrop, false);");
                writer.WriteLine("fileInput.addEventListener('change', handleFiles, false);");

                writer.WriteLine("function handleDrop(e) {");
                writer.WriteLine("    const dt = e.dataTransfer;");
                writer.WriteLine("    filesToUpload = dt.files;");
                writer.WriteLine("    prepareUploadInfo();");
                writer.WriteLine("}");

                writer.WriteLine("function handleFiles() {");
                writer.WriteLine("    filesToUpload = fileInput.files;");
                writer.WriteLine("    prepareUploadInfo();");
                writer.WriteLine("}");

                writer.WriteLine("function prepareUploadInfo() {");
                writer.WriteLine("    if (filesToUpload.length > 0) {");
                writer.WriteLine("        totalFiles = filesToUpload.length;");
                writer.WriteLine("        totalSize = 0;");
                writer.WriteLine("        uploadedFiles = 0;");
                writer.WriteLine("        uploadedSize = 0;");
                writer.WriteLine("        ");
                writer.WriteLine("        // 计算总文件大小");
                writer.WriteLine("        for (let i = 0; i < totalFiles; i++) {");
                writer.WriteLine("            totalSize += filesToUpload[i].size;");
                writer.WriteLine("        }");
                writer.WriteLine("        ");
                writer.WriteLine("        passwordModal.style.display = 'block';");
                writer.WriteLine("        passwordInput.value = '';");
                writer.WriteLine("        passwordInput.focus();");
                writer.WriteLine("    }");
                writer.WriteLine("}");

                writer.WriteLine("function submitPassword() {");
                writer.WriteLine("    let password = passwordInput.value;");
                writer.WriteLine("    if (!password) {");
                writer.WriteLine("        alert('请输入密码');");
                writer.WriteLine("        return;");
                writer.WriteLine("    }");
                writer.WriteLine("    passwordModal.style.display = 'none';");
                writer.WriteLine("    progressContainer.style.display = 'block';");
                writer.WriteLine("    uploadStatus.style.display = 'none';");
                writer.WriteLine("    uploadFiles(password);");
                writer.WriteLine("}");

                writer.WriteLine("function cancelUpload() {");
                writer.WriteLine("    passwordModal.style.display = 'none';");
                writer.WriteLine("    filesToUpload = null;");
                writer.WriteLine("    passwordInput.value = '';");
                writer.WriteLine("    progressContainer.style.display = 'none';");
                writer.WriteLine("    progressBar.style.width = '0%';");
                writer.WriteLine("    fileProgress.textContent = '';");
                writer.WriteLine("}");

                writer.WriteLine("function uploadFiles(password) {");
                writer.WriteLine("    if (!filesToUpload || filesToUpload.length === 0) return;");

                writer.WriteLine("    let successCount = 0;");
                writer.WriteLine("    let errorCount = 0;");

                writer.WriteLine("    for (let i = 0; i < totalFiles; i++) {");
                writer.WriteLine("        uploadSingleFile(filesToUpload[i], password, i, (success) => {");
                writer.WriteLine("            if (success) {");
                writer.WriteLine("                successCount++;");
                writer.WriteLine("            } else {");
                writer.WriteLine("                errorCount++;");
                writer.WriteLine("            }");
                writer.WriteLine("            ");
                writer.WriteLine("            if (successCount + errorCount === totalFiles) {");
                writer.WriteLine("                progressContainer.style.display = 'none';");
                writer.WriteLine("                showUploadStatus(`上传完成: 成功 ${successCount} 个, 失败 ${errorCount} 个`);");
                writer.WriteLine("                if (successCount > 0) {");
                writer.WriteLine("                    setTimeout(() => window.location.reload(), 2000);");
                writer.WriteLine("                }");
                writer.WriteLine("            }");
                writer.WriteLine("        });");
                writer.WriteLine("    }");
                writer.WriteLine("}");

                writer.WriteLine("function uploadSingleFile(file, password, index, callback) {");
                writer.WriteLine("    let formData = new FormData();");
                writer.WriteLine("    formData.append('file', file);");
                writer.WriteLine("    formData.append('password', password);");

                writer.WriteLine("    // 创建带进度的上传请求");
                writer.WriteLine("    const xhr = new XMLHttpRequest();");
                writer.WriteLine("    xhr.open('POST', window.location.href);");

                writer.WriteLine("    // 上传进度处理");
                writer.WriteLine("    xhr.upload.addEventListener('progress', function(e) {");
                writer.WriteLine("        if (e.lengthComputable) {");
                writer.WriteLine("            // 单个文件的上传进度");
                writer.WriteLine("            const filePercent = (e.loaded / e.total) * 100;");
                writer.WriteLine("            ");
                writer.WriteLine("            // 计算总进度");
                writer.WriteLine("            const fileContribution = (e.loaded / totalSize) * 100;");
                writer.WriteLine("            const overallPercent = ((uploadedFiles / totalFiles) * 100) + (fileContribution / totalFiles);");
                writer.WriteLine("            ");
                writer.WriteLine("            // 更新UI");
                writer.WriteLine("            progressBar.style.width = overallPercent + '%';");
                writer.WriteLine("            fileProgress.textContent = `正在上传: ${file.name} (${Math.round(filePercent)}%)`;");
                writer.WriteLine("        }");
                writer.WriteLine("    });");

                writer.WriteLine("    // 上传完成处理");
                writer.WriteLine("    xhr.addEventListener('load', function() {");
                writer.WriteLine("        uploadedFiles++;");
                writer.WriteLine("        if (xhr.status === 200) {");
                writer.WriteLine("            console.log('上传成功:', xhr.responseText);");
                writer.WriteLine("            callback(true);");
                writer.WriteLine("        } else if (xhr.status === 403) {");
                writer.WriteLine("            alert('密码错误，上传被拒绝');");
                writer.WriteLine("            callback(false);");
                writer.WriteLine("        } else if (xhr.status === 413) {");
                writer.WriteLine("            alert('文件过大，超过上传限制');");
                writer.WriteLine("            callback(false);");
                writer.WriteLine("        } else {");
                writer.WriteLine("            alert('上传失败: ' + xhr.statusText);");
                writer.WriteLine("            callback(false);");
                writer.WriteLine("        }");
                writer.WriteLine("    });");

                writer.WriteLine("    // 上传错误处理");
                writer.WriteLine("    xhr.addEventListener('error', function() {");
                writer.WriteLine("        uploadedFiles++;");
                writer.WriteLine("        alert('上传失败: 网络错误');");
                writer.WriteLine("        callback(false);");
                writer.WriteLine("    });");

                writer.WriteLine("    // 发送请求");
                writer.WriteLine("    xhr.send(formData);");
                writer.WriteLine("}");

                writer.WriteLine("function showUploadStatus(message) {");
                writer.WriteLine("    uploadStatus.textContent = message;");
                writer.WriteLine("    uploadStatus.style.display = 'block';");
                writer.WriteLine("    uploadStatus.className = 'upload-status ' + (message.includes('失败') && message.includes('0 个成功') ? 'error' : 'success');");
                writer.WriteLine("}");
                writer.WriteLine("</script>");

                writer.WriteLine("</body></html>");
            }
        }

        private void HandleFileUpload(HttpListenerRequest request, HttpListenerResponse response, string targetDirectory)
        {
            try
            {
                if (!Directory.Exists(targetDirectory))
                {
                    SendErrorResponse(response, 404, "目标目录不存在");
                    return;
                }

                if (request.ContentLength64 > MAX_FILE_SIZE)
                {
                    SendErrorResponse(response, 413, "文件过大，超过上传限制");
                    return;
                }

                // 使用更可靠的方法解析多部分表单数据
                var boundary = "--" + request.ContentType.Split(';')[1].Split('=')[1].Trim();
                byte[] boundaryBytes = Encoding.UTF8.GetBytes(boundary);
                byte[] endBoundaryBytes = Encoding.UTF8.GetBytes(boundary + "--");

                using (var requestStream = request.InputStream)
                using (var memoryStream = new MemoryStream())
                {
                    // 将请求流复制到内存以便处理
                    requestStream.CopyTo(memoryStream);
                    memoryStream.Position = 0;
                    byte[] requestData = memoryStream.ToArray();

                    // 查找密码部分
                    string password = ExtractPassword(requestData, boundaryBytes);
                    if (password != UPLOAD_PASSWORD)
                    {
                        SendErrorResponse(response, 403, "密码错误，拒绝上传");
                        return;
                    }

                    // 查找并处理文件部分
                    int fileCount = ExtractAndSaveFiles(requestData, boundaryBytes, endBoundaryBytes, targetDirectory);

                    if (fileCount > 0)
                    {
                        response.StatusCode = 200;
                        WriteResponse(response, $"成功上传 {fileCount} 个文件");
                    }
                    else
                    {
                        SendErrorResponse(response, 400, "未找到有效的文件数据");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"文件上传错误: {ex.Message}");
                SendErrorResponse(response, 500, $"上传失败: {ex.Message}");
            }
        }

        private string ExtractPassword(byte[] requestData, byte[] boundaryBytes)
        {
            int passwordStart = FindSequence(requestData, Encoding.UTF8.GetBytes("name=\"password\""), 0);
            if (passwordStart == -1)
                return null;

            // 找到内容开始位置（空行之后）
            int contentStart = FindSequence(requestData, Encoding.UTF8.GetBytes("\r\n\r\n"), passwordStart);
            if (contentStart == -1)
                return null;
            contentStart += 4; // 跳过 "\r\n\r\n"

            // 找到边界位置（内容结束）
            int boundaryPos = FindSequence(requestData, boundaryBytes, contentStart);
            if (boundaryPos == -1)
                return null;

            // 提取密码
            int passwordLength = boundaryPos - contentStart;
            byte[] passwordBytes = new byte[passwordLength];
            Array.Copy(requestData, contentStart, passwordBytes, 0, passwordLength);
            return Encoding.UTF8.GetString(passwordBytes).TrimEnd('\r', '\n');
        }

        private int ExtractAndSaveFiles(byte[] requestData, byte[] boundaryBytes, byte[] endBoundaryBytes, string targetDirectory)
        {
            int fileCount = 0;
            int currentPosition = 0;
            int dataLength = requestData.Length;

            while (currentPosition < dataLength)
            {
                // 查找下一个边界
                int boundaryPos = FindSequence(requestData, boundaryBytes, currentPosition);
                if (boundaryPos == -1)
                    break;

                // 检查是否是结束边界
                if (CheckSequenceAt(requestData, endBoundaryBytes, boundaryPos))
                    break;

                boundaryPos += boundaryBytes.Length;
                currentPosition = boundaryPos;

                // 查找文件名
                int fileNamePos = FindSequence(requestData, Encoding.UTF8.GetBytes("filename=\""), currentPosition);
                if (fileNamePos == -1)
                {
                    currentPosition = boundaryPos;
                    continue;
                }

                fileNamePos += 10; // 跳过 "filename=\""
                int fileNameEnd = FindSequence(requestData, Encoding.UTF8.GetBytes("\""), fileNamePos);
                if (fileNameEnd == -1)
                {
                    currentPosition = boundaryPos;
                    continue;
                }

                // 提取文件名
                byte[] fileNameBytes = new byte[fileNameEnd - fileNamePos];
                Array.Copy(requestData, fileNamePos, fileNameBytes, 0, fileNameBytes.Length);
                string fileName = Encoding.UTF8.GetString(fileNameBytes);
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    currentPosition = boundaryPos;
                    continue;
                }

                // 清理文件名
                fileName = Path.GetFileName(fileName);
                string filePath = Path.Combine(targetDirectory, fileName);

                // 处理文件重名
                if (File.Exists(filePath))
                {
                    string fileExt = Path.GetExtension(fileName);
                    string fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
                    int counter = 1;

                    while (File.Exists(filePath))
                    {
                        fileName = $"{fileNameWithoutExt}({counter}){fileExt}";
                        filePath = Path.Combine(targetDirectory, fileName);
                        counter++;
                    }
                }

                // 找到文件内容开始位置（在Content-Type之后的空行）
                int contentStart = FindSequence(requestData, Encoding.UTF8.GetBytes("\r\n\r\n"), fileNameEnd);
                if (contentStart == -1)
                {
                    currentPosition = boundaryPos;
                    continue;
                }
                contentStart += 4; // 跳过 "\r\n\r\n"

                // 找到内容结束位置（下一个边界）
                int contentEnd = FindSequence(requestData, boundaryBytes, contentStart);
                if (contentEnd == -1)
                {
                    currentPosition = boundaryPos;
                    continue;
                }
                contentEnd -= 2; // 去除最后的 "\r\n"

                // 提取并保存文件内容
                int contentLength = contentEnd - contentStart;
                byte[] fileContent = new byte[contentLength];
                Array.Copy(requestData, contentStart, fileContent, 0, contentLength);
                File.WriteAllBytes(filePath, fileContent);

                Console.WriteLine($"文件上传成功: {filePath}");
                fileCount++;
                currentPosition = contentEnd;
            }

            return fileCount;
        }

        private int FindSequence(byte[] data, byte[] sequence, int startIndex)
        {
            if (sequence.Length == 0)
                return -1;

            for (int i = startIndex; i <= data.Length - sequence.Length; i++)
            {
                bool match = true;
                for (int j = 0; j < sequence.Length; j++)
                {
                    if (data[i + j] != sequence[j])
                    {
                        match = false;
                        break;
                    }
                }
                if (match)
                    return i;
            }
            return -1;
        }

        private bool CheckSequenceAt(byte[] data, byte[] sequence, int position)
        {
            if (position + sequence.Length > data.Length)
                return false;

            for (int i = 0; i < sequence.Length; i++)
            {
                if (data[position + i] != sequence[i])
                    return false;
            }
            return true;
        }

        private void HandleDirectoryDownload(HttpListenerResponse response, string directoryPath)
        {
            try
            {
                string zipFileName = $"{Path.GetFileName(directoryPath)}.zip";
                string zipFilePath = Path.Combine(Path.GetTempPath(), zipFileName);

                if (File.Exists(zipFilePath))
                {
                    File.Delete(zipFilePath);
                }

                CreateZipFromDirectory(directoryPath, zipFilePath);

                byte[] buffer = File.ReadAllBytes(zipFilePath);
                response.ContentLength64 = buffer.Length;
                response.ContentType = "application/zip";
                response.AddHeader("Content-Disposition", $"attachment; filename=\"{Uri.EscapeDataString(zipFileName)}\"");
                response.OutputStream.Write(buffer, 0, buffer.Length);

                try
                {
                    File.Delete(zipFilePath);
                }
                catch { }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"打包下载错误: {ex.Message}");
                SendErrorResponse(response, 500, "打包文件夹时发生错误");
            }
        }

        private void SendFileResponse(HttpListenerResponse response, string filePath)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(filePath);
                response.ContentLength64 = fileInfo.Length;
                response.ContentType = GetMimeType(filePath);
                response.AddHeader("Content-Disposition", $"attachment; filename=\"{Uri.EscapeDataString(fileInfo.Name)}\"");

                using (var fileStream = File.OpenRead(filePath))
                {
                    byte[] buffer = new byte[4096];
                    int bytesRead;
                    while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        response.OutputStream.Write(buffer, 0, bytesRead);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"发送文件错误: {ex.Message}");
                SendErrorResponse(response, 500, "读取文件时发生错误");
            }
        }

        private void SendErrorResponse(HttpListenerResponse response, int statusCode, string message)
        {
            response.StatusCode = statusCode;
            WriteResponse(response, message);
        }

        private void WriteResponse(HttpListenerResponse response, string message)
        {
            response.ContentType = "text/plain; charset=utf-8";
            using (var writer = new StreamWriter(response.OutputStream, Encoding.UTF8))
            {
                writer.Write(message);
            }
        }

        private void CreateZipFromDirectory(string sourceDirectory, string destinationZipFilePath)
        {
            try
            {
                ZipFile.CreateFromDirectory(sourceDirectory, destinationZipFilePath, CompressionLevel.Fastest, false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"创建ZIP文件错误: {ex.Message}");
                throw;
            }
        }

        private string GetMimeType(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLowerInvariant();

            switch (extension)
            {
                case ".txt": return "text/plain";
                case ".pdf": return "application/pdf";
                case ".doc": return "application/msword";
                case ".docx": return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                case ".xls": return "application/vnd.ms-excel";
                case ".xlsx": return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                case ".ppt": return "application/vnd.ms-powerpoint";
                case ".pptx": return "application/vnd.openxmlformats-officedocument.presentationml.presentation";
                case ".jpg":
                case ".jpeg": return "image/jpeg";
                case ".png": return "image/png";
                case ".gif": return "image/gif";
                case ".zip": return "application/zip";
                case ".rar": return "application/x-rar-compressed";
                case ".7z": return "application/x-7z-compressed";
                case ".tar": return "application/x-tar";
                case ".gz": return "application/gzip";
                case ".bz2": return "application/x-bzip2";
                case ".html": return "text/html";
                case ".css": return "text/css";
                case ".js": return "application/javascript";
                default: return "application/octet-stream";
            }
        }
    }
}
