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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //需要调用的API
        //找到窗口（进程名称 可空，窗口名称）
        [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "FindWindow")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("User32.dll")]
        private static extern IntPtr FindWindowEx(IntPtr hwndParent,IntPtr hwndChildAfter,string lpszClass,string lpszWindows);

        //把窗口放到最前面
        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        //模拟键盘事件 
        [DllImport("User32.dll")]
        public static extern void keybd_event(Byte bVk, Byte bScan, Int32 dwFlags, Int32 dwExtraInfo);
        //释放按键的常量
        private const int KEYEVENTF_KEYUP = 2;
        //发送消息
        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("User32.dll")]
        private static extern Int32 SendMessage(IntPtr hWnd, int Msg,IntPtr wParam,StringBuilder lParam);
        
        [DllImport("user32.dll")]//获取窗口大小
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]//获取窗口坐标
        public struct RECT
        {
            public int Left; //最左坐标
            public int Top; //最上坐标
            public int Right; //最右坐标
            public int Bottom; //最下坐标
        }
        
        ////显示窗口
        //[DllImport("user32.dll")]
        //private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        #region SendMessage 参数
        private const int WM_KEYDOWN = 0X100;
        private const int WM_KEYUP = 0X101;
        private const int WM_SYSCHAR = 0X106;
        private const int WM_SYSKEYUP = 0X105;
        private const int WM_SYSKEYDOWN = 0X104;
        private const int WM_CHAR = 0X102;
        private const int WM_SETTEXT = 0x000C;
        #endregion

        const int WM_COPYDATA = 0x004A;

        //鼠标事件
        //private readonly int MOUSEEVENTF_LEFTDOWN = 0x2;
        //private readonly int MOUSEEVENTF_LEFTUP = 0x4;
        //[DllImport("user32")]
        //public static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);


        /// <summary>
        /// 发送一个字符串
        /// </summary>
        /// <param name="myIntPtr">窗口句柄</param>
        /// <param name="Input">字符串</param>
        public void InputStr(IntPtr k, string Input)
        {
            //不能发送汉字，只能发送键盘上有的内容 也可以模拟shift+！等
            //byte[] ch = (ASCIIEncoding.ASCII.GetBytes(Input));
            byte[] ch = Encoding.GetEncoding("GB2312").GetBytes(Input);
            for (int i = 0; i < ch.Length; i++)
            {
                SendMessage(k, WM_CHAR, ch[i], 0);
            }

            //IntPtr hwnd4 = FindWindowEx(k, IntPtr.Zero, "AfxWnd42", null);
            //IntPtr hwnd5 = FindWindowEx(hwnd4, IntPtr.Zero, "RichEdit20A", null);
            ////string formName = textBox_name.Text.ToString().Trim();
            //StringBuilder sendBuilder = new StringBuilder(Input);
            //SendMessage(hwnd5, WM_SETTEXT, IntPtr.Zero, sendBuilder);
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            string formName = textBox_name.Text.ToString().Trim();
            IntPtr k = FindWindow(null, formName);
            if (k.ToString() == "0")
                MessageBox.Show(this, "没有找到该程序");
            else
            {
                label7.Text = "已设置";
                label7.ForeColor = Color.Red;
                timer1.Start();
            }
        }

       //初始化时间
        private void Form1_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < 24;i++)
                comboBox_hour.Items.Add(i);
            for (int i = 0; i < 60; i++)
                comboBox_minute.Items.Add(i);
            for (int i = 0; i < 60; i++)
                comboBox_second.Items.Add(i);

            int nowHour = int.Parse(DateTime.Now.Hour.ToString());
            int nowMinute = int.Parse(DateTime.Now.Minute.ToString());
            int nowSecond = int.Parse(DateTime.Now.Second.ToString());

            comboBox_hour.SelectedIndex = nowHour;
            comboBox_minute.SelectedIndex = nowMinute;
            comboBox_second.SelectedIndex = nowSecond;

            timer2.Start();

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string nowHour = DateTime.Now.Hour.ToString();
            string nowMinute = DateTime.Now.Minute.ToString();
            string nowSecond = DateTime.Now.Second.ToString();

            string checkHour = comboBox_hour.Text.ToString().Trim();
            string checkMinute = comboBox_minute.Text.ToString().Trim();
            string checkSecond = comboBox_second.Text.ToString().Trim();

            if (string.Compare(nowHour, checkHour) == 0 && string.Compare(nowMinute, checkMinute) == 0 && string.Compare(nowSecond, checkSecond) == 0)
            {
                string formName = textBox_name.Text.ToString().Trim();
                string formSend = textBox_send.Text.ToString().Trim();

                IntPtr k = FindWindow(null, formName);
                if (k.ToString() != "0")
                {
                    SetForegroundWindow(k);//把找到的的对话框在最前面显示如果使用了这个方法
                    //for (int i = 0; i < 3;i++)
                    //{
                        InputStr(k, formSend);
                        SendMessage(k, WM_KEYDOWN, 0X0D, 0);//发
                        SendMessage(k, WM_KEYUP, 0X0D, 0);  //送
                        SendMessage(k, WM_CHAR, 0X0D, 0);   //回车
                    //}
                    timer1.Stop();
                    label7.Text = "未设置";
                    label7.ForeColor = Color.Blue;
                }
                else
                {
                    MessageBox.Show(this, "没有找到该程序");
                    timer1.Stop();
                    label7.Text = "未设置";
                    label7.ForeColor = Color.Blue;
                }
            }
            
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            string nowHour = DateTime.Now.Hour.ToString();
            string nowMinute = DateTime.Now.Minute.ToString();
            string nowSecond = DateTime.Now.Second.ToString();

            label6.Text = nowHour + ":" + nowMinute + ":" + nowSecond;
        }
    }
}
