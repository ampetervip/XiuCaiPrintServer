using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Print_Ser
{
    internal class IniFileHelper
    {
        // INI文件路径
        public string FilePath { get; private set; }

        // 构造函数：指定INI文件路径
        public IniFileHelper(string filePath)
        {
            FilePath = filePath;
        }

        // 写入INI文件
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool WritePrivateProfileString(
            string lpAppName,       // 节名（[Section]）
            string lpKeyName,       // 键名（Key）
            string lpString,        // 值（Value）
            string lpFileName       // INI文件路径
        );

        // 读取INI文件
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int GetPrivateProfileString(
            string lpAppName,       // 节名
            string lpKeyName,       // 键名
            string lpDefault,       // 默认值（当键不存在时返回）
            StringBuilder lpReturnedString, // 接收结果的字符串
            int nSize,              // 字符串缓冲区大小
            string lpFileName       // INI文件路径
        );

        // 写入字符串
        public bool WriteString(string section, string key, string value)
        {
            return WritePrivateProfileString(section, key, value, FilePath);
        }

        // 读取字符串（默认值：空字符串）
        public string ReadString(string section, string key, string defaultValue = "")
        {
            StringBuilder sb = new StringBuilder(255); // 缓冲区大小，可根据需求调整
            GetPrivateProfileString(section, key, defaultValue, sb, sb.Capacity, FilePath);
            return sb.ToString();
        }

        // 写入整数
        public bool WriteInt(string section, string key, int value)
        {
            return WriteString(section, key, value.ToString());
        }

        // 读取整数（默认值：0）
        public int ReadInt(string section, string key, int defaultValue = 0)
        {
            string value = ReadString(section, key, defaultValue.ToString());
            return int.TryParse(value, out int result) ? result : defaultValue;
        }

        // 写入布尔值
        public bool WriteBool(string section, string key, bool value)
        {
            return WriteString(section, key, value ? "1" : "0"); // 用1/0表示true/false
        }

        // 读取布尔值（默认值：false）
        public bool ReadBool(string section, string key, bool defaultValue = false)
        {
            string value = ReadString(section, key, defaultValue ? "1" : "0");
            return value == "1";
        }
    }
}
