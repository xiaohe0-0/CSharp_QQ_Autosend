using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using Microsoft.Win32;
using System.Runtime.InteropServices;//这个必须要引用

namespace STH2
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private const int WM_SETTEXT = 0x000C;
        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(
          string lpClassName,
          string lpWindowName);

        [DllImport("User32.dll")]
        private static extern IntPtr FindWindowEx(
          IntPtr hwndParent,
          IntPtr hwndChildAfter,
          string lpszClass,
        string lpszWindows);
        [DllImport("User32.dll")]
        private static extern Int32 SendMessage(
          IntPtr hWnd,
          int Msg,
          IntPtr wParam,
        StringBuilder lParam);
        private void button1_Click(object sender, EventArgs e)
        {
            // 返回写字板主窗口句柄
            IntPtr hWnd = FindWindow(null, "1.txt - 记事本");
            if (!hWnd.Equals(IntPtr.Zero))
            {
                //返回写字板编辑窗口句柄
                IntPtr edithWnd = FindWindowEx(hWnd, IntPtr.Zero, "Edit", null);
                if (!edithWnd.Equals(IntPtr.Zero))
                    // 发送WM_SETTEXT 消息： "Hello World!"
                    SendMessage(edithWnd, WM_SETTEXT, IntPtr.Zero, new StringBuilder("我我我"));
            }
            else {
                MessageBox.Show("Not Found");
            }
        }
    }
}
