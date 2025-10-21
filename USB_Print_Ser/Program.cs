using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Print_Ser
{
    internal static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            // 从程序集属性中获取 GUID
            var assemblyGuid = Assembly.GetExecutingAssembly().GetCustomAttribute<GuidAttribute>()?.Value;

            if (string.IsNullOrEmpty(assemblyGuid))
            {
                MessageBox.Show("无法获取程序集 GUID，请确保已设置！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // 创建具有唯一名称的互斥体
            using (Mutex mutex = new Mutex(true, assemblyGuid, out bool createdNew))
            {
                // 如果未能创建互斥体，说明已有实例在运行
                if (!createdNew)
                {
                    MessageBox.Show("应用程序已在运行中,请勿重复运行！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form_Main());
            }
        }
    }
}
