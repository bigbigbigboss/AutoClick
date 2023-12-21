using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoClick
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            RegHotKey();
            HandleGridView.RowHeadersVisible = false;
        }

        private void ButtonStop_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;
            timer.Stop();
            timer.Dispose();
            StatusLable.Text = "闲置";
        }
        private void m_gridControl_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                List<IntPtr> removedIntPtr = new List<IntPtr>();
                foreach (IntPtr intPtr in HandleList)
                {
                    if (intPtr.ToString("X") == HandleGridView.Rows[e.RowIndex].Cells[0].Value.ToString())
                    {
                        removedIntPtr.Add(intPtr);
                    }
                }
                foreach (IntPtr intPtr in removedIntPtr)
                {
                    HandleList.Remove(intPtr);
                }
                List<DataGridViewRow> removedRows = new List<DataGridViewRow>();
                foreach (DataGridViewRow dgr in HandleGridView.Rows)
                {
                    if (dgr.Cells[0].Value.ToString() == HandleGridView.Rows[e.RowIndex].Cells[0].Value.ToString())
                    {
                        removedRows.Add(dgr);
                    }
                }
                foreach (DataGridViewRow dgr in removedRows)
                {
                    HandleGridView.Rows.Remove(dgr);
                }
            }
        }
        private void StartButton_Click(object sender, EventArgs e)
        {
            StartTimer();
            StatusLable.Text = "运行";
        }
        #region 热键注册
        private void RegHotKey()
        {
            uint ctrlHotKey = (uint)(KeyModifiers.Alt | KeyModifiers.Ctrl);
            // 注册热键为Alt+Ctrl+C, "100"为唯一标识热键
            HotKey.RegisterHotKey(Handle, 99, ctrlHotKey, Keys.A);
            HotKey.RegisterHotKey(Handle, 100, ctrlHotKey, Keys.B);
            HotKey.RegisterHotKey(Handle, 101, ctrlHotKey, Keys.C);
        }
        /// <summary>
        /// 窗体关闭时处理程序
        /// 窗体关闭时取消热键注册
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 卸载热键
            HotKey.UnregisterHotKey(Handle, 100);
            HotKey.UnregisterHotKey(Handle, 99);
            HotKey.UnregisterHotKey(Handle, 101);
        }
        /// <summary>
        ///  有些人会问,为什么把Alt定义为1， Ctrl为2了
        ///  因为 http://msdn.microsoft.com/en-us/library/windows/desktop/ms646309(v=vs.85).aspx 列出了值的对应关系
        /// 定义辅助键的名称,也就是定义RegisterHotKey第三个参数的取值
        /// （将数字转化为字符，可以使大家更容易理解代码）
        /// </summary>
        [Flags]
        public enum KeyModifiers
        {
            None = 0,
            Alt = 1,
            Ctrl = 2,
            Shift = 4,
            WindowsKey = 8
        }
        // 热键按下执行的方法
        private void GlobalKeyProcess()
        {
            //this.WindowState = FormWindowState.Minimized;
            // 窗口最小化也需要一定时间
            //Thread.Sleep(200);
            StartTimer();
        }

        /// <summary>
        /// 重写WndProc()方法，通过监视系统消息，来调用过程
        /// 监视Windows消息
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            //如果m.Msg的值为0x0312那么表示用户按下了热键
            const int WM_HOTKEY = 0x0312;
            switch (m.Msg)
            {
                case WM_HOTKEY:
                    if (m.WParam.ToString() == "100")
                    {
                        GlobalKeyProcess();
                    }
                    if (m.WParam.ToString() == "101")
                    {
                        GlobalKeyProcess();
                    }
                    if (m.WParam.ToString() == "99")
                    {
                        Point p;
                        GetCursorPos(out p);//获取鼠标坐标
                        HandleList.Add(WindowFromPoint(p));
                        HandleGridView.Rows.Add(WindowFromPoint(p).ToString("X"));
                    }
                    break;
            }

            // 将系统消息传递自父类的WndProc
            base.WndProc(ref m);
        }
        #endregion

        #region 发送消息
        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, Keys wParam, uint lParam);

        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, uint wParam, uint lParam);

        [DllImport("user32.dll", EntryPoint = "PostMessage")]
        public static extern int PostMessage(IntPtr hwnd, int wMsg, Keys wParam, uint lParam);
        /// <summary>
        /// 返回包含了指定点的窗口的句柄。忽略屏蔽、隐藏以及透明窗口
        /// </summary>
        /// <param name="Point">指定的鼠标坐标</param>
        /// <returns>鼠标坐标处的窗口句柄，如果没有，返回</returns>
        [DllImport("user32.dll")]
        internal static extern IntPtr WindowFromPoint(Point Point);

        List<IntPtr> HandleList = new List<IntPtr>();

        /// <summary>
        /// 获取鼠标处的坐标
        /// </summary>
        /// <param name="lpPoint">随同指针在屏幕像素坐标中的位置载入的一个结构</param>
        /// <returns>非零表示成功，零表示失败</returns>
        [DllImport("user32.dll")]
        internal static extern bool GetCursorPos(out Point lpPoint);
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            foreach (IntPtr intptr in HandleList)
            {
                //Task.Factory.StartNew(() => SendMessage(intptr, 0x0102, keyData, 0x20210001));
                Task.Factory.StartNew(() => PostMessage(intptr, WM_KEYDOWN, keyData, 0x20210001));
            }
            return true;
        }
        #endregion

        #region  定时器方法
        System.Timers.Timer timer;

        private void StartTimer()
        {
            int interval = 0;
            try
            {
                interval = Convert.ToInt32(TimeInterval.Text);
            }
            catch
            {
                MessageBox.Show("请输入纯数字！");
                return;
            }
            timer = new System.Timers.Timer();
            timer.Enabled = true;
            timer.Interval = interval;//执行间隔时间,单位为毫秒  
            timer.Start();
            timer.SynchronizingObject = this; //this就是窗体本身
            timer.Elapsed += new System.Timers.ElapsedEventHandler(Timer1_Elapsed);
        }


        private void Timer1_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Console.WriteLine(DateTime.Now.ToString());
            foreach (IntPtr intptr in HandleList)
            {
                foreach (String checkedString in checkedListBox.CheckedItems)
                {
                    switch (checkedString)
                    {
                        case "按F1":
                            //Task.Factory.StartNew(() => SendMessage(intptr, 0x0102, Keys.D1, 0x20210001));
                            Task.Factory.StartNew(() => PostMessage(intptr, WM_KEYDOWN, Keys.F1, 0x20210001));
                            Task.Factory.StartNew(() => PostMessage(intptr, WM_KEYUP, Keys.F1, 0x20210001));
                            break;
                        case "按F2":
                            Task.Factory.StartNew(() => PostMessage(intptr, WM_KEYDOWN, Keys.F2, 0x20210001));
                            Task.Factory.StartNew(() => PostMessage(intptr, WM_KEYUP, Keys.F2, 0x20210001));
                            break;
                        case "按F3":
                            Task.Factory.StartNew(() => PostMessage(intptr, WM_KEYDOWN, Keys.F3, 0x20210001));
                            Task.Factory.StartNew(() => PostMessage(intptr, WM_KEYUP, Keys.F3, 0x20210001));
                            break;
                        case "按F4":
                            Task.Factory.StartNew(() => PostMessage(intptr, WM_KEYDOWN, Keys.F4, 0x20210001));
                            Task.Factory.StartNew(() => PostMessage(intptr, WM_KEYUP, Keys.F4, 0x20210001));
                            break;
                        case "按F5":
                            Task.Factory.StartNew(() => PostMessage(intptr, WM_KEYDOWN, Keys.F5, 0x20210001));
                            Task.Factory.StartNew(() => PostMessage(intptr, WM_KEYUP, Keys.F5, 0x20210001));
                            break;
                        case "按F6":
                            Task.Factory.StartNew(() => PostMessage(intptr, WM_KEYDOWN, Keys.F6, 0x20210001));
                            Task.Factory.StartNew(() => PostMessage(intptr, WM_KEYUP, Keys.F6, 0x20210001));
                            break;
                        case "按F7":
                            Task.Factory.StartNew(() => PostMessage(intptr, WM_KEYDOWN, Keys.F7, 0x20210001));
                            Task.Factory.StartNew(() => PostMessage(intptr, WM_KEYUP, Keys.F7, 0x20210001));
                            break;
                        case "按F8":
                            Task.Factory.StartNew(() => PostMessage(intptr, WM_KEYDOWN, Keys.F8, 0x20210001));
                            Task.Factory.StartNew(() => PostMessage(intptr, WM_KEYUP, Keys.F8, 0x20210001));
                            break;
                        case "按9键":
                            Task.Factory.StartNew(() => SendMessage(intptr, 0x0102, Keys.D9, 0x20210001));
                            break;
                        case "按0键":
                            Task.Factory.StartNew(() => SendMessage(intptr, 0x0102, Keys.D0, 0x20210001));
                            break;
                        case "按空格键":
                            //Task.Factory.StartNew(() => SendMessage(intptr, 0x0102, Keys.Space, 0x20210001));
                            Task.Factory.StartNew(() => PostMessage(intptr, WM_KEYDOWN, Keys.Space, 0x20210001));
                            Task.Factory.StartNew(() => PostMessage(intptr, WM_KEYUP, Keys.Space, 0x20210001));
                            break;

                    }
                }
            }

        }
        #endregion

        #region wMsg常量
        //创建一个窗口   
        const int WM_CREATE = 0x01;
        //当一个窗口被破坏时发送   
        const int WM_DESTROY = 0x02;
        //移动一个窗口   
        const int WM_MOVE = 0x03;
        //改变一个窗口的大小   
        const int WM_SIZE = 0x05;
        //一个窗口被激活或失去激活状态   
        const int WM_ACTIVATE = 0x06;
        //一个窗口获得焦点   
        const int WM_SETFOCUS = 0x07;
        //一个窗口失去焦点   
        const int WM_KILLFOCUS = 0x08;
        //一个窗口改变成Enable状态   
        const int WM_ENABLE = 0x0A;
        //设置窗口是否能重画   
        const int WM_SETREDRAW = 0x0B;
        //应用程序发送此消息来设置一个窗口的文本   
        const int WM_SETTEXT = 0x0C;
        //应用程序发送此消息来复制对应窗口的文本到缓冲区   
        const int WM_GETTEXT = 0x0D;
        //得到与一个窗口有关的文本的长度（不包含空字符）   
        const int WM_GETTEXTLENGTH = 0x0E;
        //要求一个窗口重画自己   
        const int WM_PAINT = 0x0F;
        //当一个窗口或应用程序要关闭时发送一个信号   
        const int WM_CLOSE = 0x10;
        //当用户选择结束对话框或程序自己调用ExitWindows函数   
        const int WM_QUERYENDSESSION = 0x11;
        //用来结束程序运行   
        const int WM_QUIT = 0x12;
        //当用户窗口恢复以前的大小位置时，把此消息发送给某个图标   
        const int WM_QUERYOPEN = 0x13;
        //当窗口背景必须被擦除时（例在窗口改变大小时）   
        const int WM_ERASEBKGND = 0x14;
        //当系统颜色改变时，发送此消息给所有顶级窗口   
        const int WM_SYSCOLORCHANGE = 0x15;
        //当系统进程发出WM_QUERYENDSESSION消息后，此消息发送给应用程序，通知它对话是否结束   
        const int WM_ENDSESSION = 0x16;
        //当隐藏或显示窗口是发送此消息给这个窗口   
        const int WM_SHOWWINDOW = 0x18;
        //发此消息给应用程序哪个窗口是激活的，哪个是非激活的   
        const int WM_ACTIVATEAPP = 0x1C;
        //当系统的字体资源库变化时发送此消息给所有顶级窗口   
        const int WM_FONTCHANGE = 0x1D;
        //当系统的时间变化时发送此消息给所有顶级窗口   
        const int WM_TIMECHANGE = 0x1E;
        //发送此消息来取消某种正在进行的摸态（操作）   
        const int WM_CANCELMODE = 0x1F;
        //如果鼠标引起光标在某个窗口中移动且鼠标输入没有被捕获时，就发消息给某个窗口   
        const int WM_SETCURSOR = 0x20;
        //当光标在某个非激活的窗口中而用户正按着鼠标的某个键发送此消息给//当前窗口   
        const int WM_MOUSEACTIVATE = 0x21;
        //发送此消息给MDI子窗口//当用户点击此窗口的标题栏，或//当窗口被激活，移动，改变大小   
        const int WM_CHILDACTIVATE = 0x22;
        //此消息由基于计算机的训练程序发送，通过WH_JOURNALPALYBACK的hook程序分离出用户输入消息   
        const int WM_QUEUESYNC = 0x23;
        //此消息发送给窗口当它将要改变大小或位置   
        const int WM_GETMINMAXINFO = 0x24;
        //发送给最小化窗口当它图标将要被重画   
        const int WM_PAINTICON = 0x26;
        //此消息发送给某个最小化窗口，仅//当它在画图标前它的背景必须被重画   
        const int WM_ICONERASEBKGND = 0x27;
        //发送此消息给一个对话框程序去更改焦点位置   
        const int WM_NEXTDLGCTL = 0x28;
        //每当打印管理列队增加或减少一条作业时发出此消息    
        const int WM_SPOOLERSTATUS = 0x2A;
        //当button，combobox，listbox，menu的可视外观改变时发送   
        const int WM_DRAWITEM = 0x2B;
        //当button, combo box, list box, list view control, or menu item 被创建时   
        const int WM_MEASUREITEM = 0x2C;
        //此消息有一个LBS_WANTKEYBOARDINPUT风格的发出给它的所有者来响应WM_KEYDOWN消息    
        const int WM_VKEYTOITEM = 0x2E;
        //此消息由一个LBS_WANTKEYBOARDINPUT风格的列表框发送给他的所有者来响应WM_CHAR消息    
        const int WM_CHARTOITEM = 0x2F;
        //当绘制文本时程序发送此消息得到控件要用的颜色   
        const int WM_SETFONT = 0x30;
        //应用程序发送此消息得到当前控件绘制文本的字体   
        const int WM_GETFONT = 0x31;
        //应用程序发送此消息让一个窗口与一个热键相关连    
        const int WM_SETHOTKEY = 0x32;
        //应用程序发送此消息来判断热键与某个窗口是否有关联   
        const int WM_GETHOTKEY = 0x33;
        //此消息发送给最小化窗口，当此窗口将要被拖放而它的类中没有定义图标，应用程序能返回一个图标或光标的句柄，当用户拖放图标时系统显示这个图标或光标   
        const int WM_QUERYDRAGICON = 0x37;
        //发送此消息来判定combobox或listbox新增加的项的相对位置   
        const int WM_COMPAREITEM = 0x39;
        //显示内存已经很少了   
        const int WM_COMPACTING = 0x41;
        //发送此消息给那个窗口的大小和位置将要被改变时，来调用setwindowpos函数或其它窗口管理函数   
        const int WM_WINDOWPOSCHANGING = 0x46;
        //发送此消息给那个窗口的大小和位置已经被改变时，来调用setwindowpos函数或其它窗口管理函数   
        const int WM_WINDOWPOSCHANGED = 0x47;
        //当系统将要进入暂停状态时发送此消息   
        const int WM_POWER = 0x48;
        //当一个应用程序传递数据给另一个应用程序时发送此消息   
        const int WM_COPYDATA = 0x4A;
        //当某个用户取消程序日志激活状态，提交此消息给程序   
        const int WM_CANCELJOURNA = 0x4B;
        //当某个控件的某个事件已经发生或这个控件需要得到一些信息时，发送此消息给它的父窗口    
        const int WM_NOTIFY = 0x4E;
        //当用户选择某种输入语言，或输入语言的热键改变   
        const int WM_INPUTLANGCHANGEREQUEST = 0x50;
        //当平台现场已经被改变后发送此消息给受影响的最顶级窗口   
        const int WM_INPUTLANGCHANGE = 0x51;
        //当程序已经初始化windows帮助例程时发送此消息给应用程序   
        const int WM_TCARD = 0x52;
        //此消息显示用户按下了F1，如果某个菜单是激活的，就发送此消息个此窗口关联的菜单，否则就发送给有焦点的窗口，如果//当前都没有焦点，就把此消息发送给//当前激活的窗口   
        const int WM_HELP = 0x53;
        //当用户已经登入或退出后发送此消息给所有的窗口，//当用户登入或退出时系统更新用户的具体设置信息，在用户更新设置时系统马上发送此消息   
        const int WM_USERCHANGED = 0x54;
        //公用控件，自定义控件和他们的父窗口通过此消息来判断控件是使用ANSI还是UNICODE结构   
        const int WM_NOTIFYFORMAT = 0x55;
        //当用户某个窗口中点击了一下右键就发送此消息给这个窗口   
        //const int WM_CONTEXTMENU = ??;   
        //当调用SETWINDOWLONG函数将要改变一个或多个 窗口的风格时发送此消息给那个窗口   
        const int WM_STYLECHANGING = 0x7C;
        //当调用SETWINDOWLONG函数一个或多个 窗口的风格后发送此消息给那个窗口   
        const int WM_STYLECHANGED = 0x7D;
        //当显示器的分辨率改变后发送此消息给所有的窗口   
        const int WM_DISPLAYCHANGE = 0x7E;
        //此消息发送给某个窗口来返回与某个窗口有关连的大图标或小图标的句柄   
        const int WM_GETICON = 0x7F;
        //程序发送此消息让一个新的大图标或小图标与某个窗口关联   
        const int WM_SETICON = 0x80;
        //当某个窗口第一次被创建时，此消息在WM_CREATE消息发送前发送   
        const int WM_NCCREATE = 0x81;
        //此消息通知某个窗口，非客户区正在销毁    
        const int WM_NCDESTROY = 0x82;
        //当某个窗口的客户区域必须被核算时发送此消息   
        const int WM_NCCALCSIZE = 0x83;
        //移动鼠标，按住或释放鼠标时发生   
        const int WM_NCHITTEST = 0x84;
        //程序发送此消息给某个窗口当它（窗口）的框架必须被绘制时   
        const int WM_NCPAINT = 0x85;
        //此消息发送给某个窗口仅当它的非客户区需要被改变来显示是激活还是非激活状态   
        const int WM_NCACTIVATE = 0x86;
        //发送此消息给某个与对话框程序关联的控件，widdows控制方位键和TAB键使输入进入此控件通过应   
        const int WM_GETDLGCODE = 0x87;
        //当光标在一个窗口的非客户区内移动时发送此消息给这个窗口 非客户区为：窗体的标题栏及窗 的边框体   
        const int WM_NCMOUSEMOVE = 0xA0;
        //当光标在一个窗口的非客户区同时按下鼠标左键时提交此消息   
        const int WM_NCLBUTTONDOWN = 0xA1;
        //当用户释放鼠标左键同时光标某个窗口在非客户区十发送此消息    
        const int WM_NCLBUTTONUP = 0xA2;
        //当用户双击鼠标左键同时光标某个窗口在非客户区十发送此消息   
        const int WM_NCLBUTTONDBLCLK = 0xA3;
        //当用户按下鼠标右键同时光标又在窗口的非客户区时发送此消息   
        const int WM_NCRBUTTONDOWN = 0xA4;
        //当用户释放鼠标右键同时光标又在窗口的非客户区时发送此消息   
        const int WM_NCRBUTTONUP = 0xA5;
        //当用户双击鼠标右键同时光标某个窗口在非客户区十发送此消息   
        const int WM_NCRBUTTONDBLCLK = 0xA6;
        //当用户按下鼠标中键同时光标又在窗口的非客户区时发送此消息   
        const int WM_NCMBUTTONDOWN = 0xA7;
        //当用户释放鼠标中键同时光标又在窗口的非客户区时发送此消息   
        const int WM_NCMBUTTONUP = 0xA8;
        //当用户双击鼠标中键同时光标又在窗口的非客户区时发送此消息   
        const int WM_NCMBUTTONDBLCLK = 0xA9;
        //WM_KEYDOWN 按下一个键   
        const int WM_KEYDOWN = 0x0100;
        //释放一个键   
        const int WM_KEYUP = 0x0101;
        //按下某键，并已发出WM_KEYDOWN， WM_KEYUP消息   
        const int WM_CHAR = 0x102;
        //当用translatemessage函数翻译WM_KEYUP消息时发送此消息给拥有焦点的窗口   
        const int WM_DEADCHAR = 0x103;
        //当用户按住ALT键同时按下其它键时提交此消息给拥有焦点的窗口   
        const int WM_SYSKEYDOWN = 0x104;
        //当用户释放一个键同时ALT 键还按着时提交此消息给拥有焦点的窗口   
        const int WM_SYSKEYUP = 0x105;
        //当WM_SYSKEYDOWN消息被TRANSLATEMESSAGE函数翻译后提交此消息给拥有焦点的窗口   
        const int WM_SYSCHAR = 0x106;
        //当WM_SYSKEYDOWN消息被TRANSLATEMESSAGE函数翻译后发送此消息给拥有焦点的窗口   
        const int WM_SYSDEADCHAR = 0x107;
        //在一个对话框程序被显示前发送此消息给它，通常用此消息初始化控件和执行其它任务   
        const int WM_INITDIALOG = 0x110;
        //当用户选择一条菜单命令项或当某个控件发送一条消息给它的父窗口，一个快捷键被翻译   
        const int WM_COMMAND = 0x111;
        //当用户选择窗口菜单的一条命令或//当用户选择最大化或最小化时那个窗口会收到此消息   
        const int WM_SYSCOMMAND = 0x112;
        //发生了定时器事件   
        const int WM_TIMER = 0x113;
        //当一个窗口标准水平滚动条产生一个滚动事件时发送此消息给那个窗口，也发送给拥有它的控件   
        const int WM_HSCROLL = 0x114;
        //当一个窗口标准垂直滚动条产生一个滚动事件时发送此消息给那个窗口也，发送给拥有它的控件   
        const int WM_VSCROLL = 0x115;
        //当一个菜单将要被激活时发送此消息，它发生在用户菜单条中的某项或按下某个菜单键，它允许程序在显示前更改菜单   
        const int WM_INITMENU = 0x116;
        //当一个下拉菜单或子菜单将要被激活时发送此消息，它允许程序在它显示前更改菜单，而不要改变全部   
        const int WM_INITMENUPOPUP = 0x117;
        //当用户选择一条菜单项时发送此消息给菜单的所有者（一般是窗口）   
        const int WM_MENUSELECT = 0x11F;
        //当菜单已被激活用户按下了某个键（不同于加速键），发送此消息给菜单的所有者   
        const int WM_MENUCHAR = 0x120;
        //当一个模态对话框或菜单进入空载状态时发送此消息给它的所有者，一个模态对话框或菜单进入空载状态就是在处理完一条或几条先前的消息后没有消息它的列队中等待   
        const int WM_ENTERIDLE = 0x121;
        //在windows绘制消息框前发送此消息给消息框的所有者窗口，通过响应这条消息，所有者窗口可以通过使用给定的相关显示设备的句柄来设置消息框的文本和背景颜色   
        const int WM_CTLCOLORMSGBOX = 0x132;
        //当一个编辑型控件将要被绘制时发送此消息给它的父窗口通过响应这条消息，所有者窗口可以通过使用给定的相关显示设备的句柄来设置编辑框的文本和背景颜色   
        const int WM_CTLCOLOREDIT = 0x133;

        //当一个列表框控件将要被绘制前发送此消息给它的父窗口通过响应这条消息，所有者窗口可以通过使用给定的相关显示设备的句柄来设置列表框的文本和背景颜色   
        const int WM_CTLCOLORLISTBOX = 0x134;
        //当一个按钮控件将要被绘制时发送此消息给它的父窗口通过响应这条消息，所有者窗口可以通过使用给定的相关显示设备的句柄来设置按纽的文本和背景颜色   
        const int WM_CTLCOLORBTN = 0x135;
        //当一个对话框控件将要被绘制前发送此消息给它的父窗口通过响应这条消息，所有者窗口可以通过使用给定的相关显示设备的句柄来设置对话框的文本背景颜色   
        const int WM_CTLCOLORDLG = 0x136;
        //当一个滚动条控件将要被绘制时发送此消息给它的父窗口通过响应这条消息，所有者窗口可以通过使用给定的相关显示设备的句柄来设置滚动条的背景颜色   
        const int WM_CTLCOLORSCROLLBAR = 0x137;
        //当一个静态控件将要被绘制时发送此消息给它的父窗口通过响应这条消息，所有者窗口可以 通过使用给定的相关显示设备的句柄来设置静态控件的文本和背景颜色   
        const int WM_CTLCOLORSTATIC = 0x138;
        //当鼠标轮子转动时发送此消息个当前有焦点的控件   
        const int WM_MOUSEWHEEL = 0x20A;
        //双击鼠标中键   
        const int WM_MBUTTONDBLCLK = 0x209;
        //释放鼠标中键   
        const int WM_MBUTTONUP = 0x208;
        //移动鼠标时发生，同WM_MOUSEFIRST   
        const int WM_MOUSEMOVE = 0x200;
        //按下鼠标左键   
        const int WM_LBUTTONDOWN = 0x201;
        //释放鼠标左键   
        const int WM_LBUTTONUP = 0x202;
        //双击鼠标左键   
        const int WM_LBUTTONDBLCLK = 0x203;
        //按下鼠标右键   
        const int WM_RBUTTONDOWN = 0x204;
        //释放鼠标右键   
        const int WM_RBUTTONUP = 0x205;
        //双击鼠标右键   
        const int WM_RBUTTONDBLCLK = 0x206;
        //按下鼠标中键   
        const int WM_MBUTTONDOWN = 0x207;

        const int WM_USER = 0x0400;
        const int MK_LBUTTON = 0x0001;
        const int MK_RBUTTON = 0x0002;
        const int MK_SHIFT = 0x0004;
        const int MK_CONTROL = 0x0008;
        const int MK_MBUTTON = 0x0010;
        const int MK_XBUTTON1 = 0x0020;
        const int MK_XBUTTON2 = 0x0040;
        //Windows 使用的256个虚拟键码
        public const int VK_LBUTTON = 0x1;
        public const int VK_RBUTTON = 0x2;
        public const int VK_CANCEL = 0x3;
        public const int VK_MBUTTON = 0x4;
        public const int VK_BACK = 0x8;
        public const int VK_TAB = 0x9;
        public const int VK_CLEAR = 0xC;
        public const int VK_RETURN = 0xD;
        public const int VK_SHIFT = 0x10;
        public const int VK_CONTROL = 0x11;
        public const int VK_MENU = 0x12;
        public const int VK_PAUSE = 0x13;
        public const int VK_CAPITAL = 0x14;
        public const int VK_ESCAPE = 0x1B;
        public const int VK_SPACE = 0x20;
        public const int VK_PRIOR = 0x21;
        public const int VK_NEXT = 0x22;
        public const int VK_END = 0x23;
        public const int VK_HOME = 0x24;
        public const int VK_LEFT = 0x25;
        public const int VK_UP = 0x26;
        public const int VK_RIGHT = 0x27;
        public const int VK_DOWN = 0x28;
        public const int VK_Select = 0x29;
        public const int VK_PRINT = 0x2A;
        public const int VK_EXECUTE = 0x2B;
        public const int VK_SNAPSHOT = 0x2C;
        public const int VK_Insert = 0x2D;
        public const int VK_Delete = 0x2E;
        public const int VK_HELP = 0x2F;
        public const int VK_0 = 0x30;
        public const int VK_1 = 0x31;
        public const int VK_2 = 0x32;
        public const int VK_3 = 0x33;
        public const int VK_4 = 0x34;
        public const int VK_5 = 0x35;
        public const int VK_6 = 0x36;
        public const int VK_7 = 0x37;
        public const int VK_8 = 0x38;
        public const int VK_9 = 0x39;
        public const int VK_A = 0x41;
        public const int VK_B = 0x42;
        public const int VK_C = 0x43;
        public const int VK_D = 0x44;
        public const int VK_E = 0x45;
        public const int VK_F = 0x46;
        public const int VK_G = 0x47;
        public const int VK_H = 0x48;
        public const int VK_I = 0x49;
        public const int VK_J = 0x4A;
        public const int VK_K = 0x4B;
        public const int VK_L = 0x4C;
        public const int VK_M = 0x4D;
        public const int VK_N = 0x4E;
        public const int VK_O = 0x4F;
        public const int VK_P = 0x50;
        public const int VK_Q = 0x51;
        public const int VK_R = 0x52;
        public const int VK_S = 0x53;
        public const int VK_T = 0x54;
        public const int VK_U = 0x55;
        public const int VK_V = 0x56;
        public const int VK_W = 0x57;
        public const int VK_X = 0x58;
        public const int VK_Y = 0x59;
        public const int VK_Z = 0x5A;
        public const int VK_STARTKEY = 0x5B;
        public const int VK_CONTEXTKEY = 0x5D;
        public const int VK_NUMPAD0 = 0x60;
        public const int VK_NUMPAD1 = 0x61;
        public const int VK_NUMPAD2 = 0x62;
        public const int VK_NUMPAD3 = 0x63;
        public const int VK_NUMPAD4 = 0x64;
        public const int VK_NUMPAD5 = 0x65;
        public const int VK_NUMPAD6 = 0x66;
        public const int VK_NUMPAD7 = 0x67;
        public const int VK_NUMPAD8 = 0x68;
        public const int VK_NUMPAD9 = 0x69;
        public const int VK_MULTIPLY = 0x6A;
        public const int VK_ADD = 0x6B;
        public const int VK_SEPARATOR = 0x6C;
        public const int VK_SUBTRACT = 0x6D;
        public const int VK_DECIMAL = 0x6E;
        public const int VK_DIVIDE = 0x6F;
        public const int VK_F1 = 0x70;
        public const int VK_F2 = 0x71;
        public const int VK_F3 = 0x72;
        public const int VK_F4 = 0x73;
        public const int VK_F5 = 0x74;
        public const int VK_F6 = 0x75;
        public const int VK_F7 = 0x76;
        public const int VK_F8 = 0x77;
        public const int VK_F9 = 0x78;
        public const int VK_F10 = 0x79;
        public const int VK_F11 = 0x7A;
        public const int VK_F12 = 0x7B;
        public const int VK_F13 = 0x7C;
        public const int VK_F14 = 0x7D;
        public const int VK_F15 = 0x7E;
        public const int VK_F16 = 0x7F;
        public const int VK_F17 = 0x80;
        public const int VK_F18 = 0x81;
        public const int VK_F19 = 0x82;
        public const int VK_F20 = 0x83;
        public const int VK_F21 = 0x84;
        public const int VK_F22 = 0x85;
        public const int VK_F23 = 0x86;
        public const int VK_F24 = 0x87;
        public const int VK_NUMLOCK = 0x90;
        public const int VK_OEM_SCROLL = 0x91;
        public const int VK_OEM_1 = 0xBA;
        public const int VK_OEM_PLUS = 0xBB;
        public const int VK_OEM_COMMA = 0xBC;
        public const int VK_OEM_MINUS = 0xBD;
        public const int VK_OEM_PERIOD = 0xBE;
        public const int VK_OEM_2 = 0xBF;
        public const int VK_OEM_3 = 0xC0;
        public const int VK_OEM_4 = 0xDB;
        public const int VK_OEM_5 = 0xDC;
        public const int VK_OEM_6 = 0xDD;
        public const int VK_OEM_7 = 0xDE;
        public const int VK_OEM_8 = 0xDF;
        public const int VK_ICO_F17 = 0xE0;
        public const int VK_ICO_F18 = 0xE1;
        public const int VK_OEM102 = 0xE2;
        public const int VK_ICO_HELP = 0xE3;
        public const int VK_ICO_00 = 0xE4;
        public const int VK_ICO_CLEAR = 0xE6;
        public const int VK_OEM_RESET = 0xE9;
        public const int VK_OEM_JUMP = 0xEA;
        public const int VK_OEM_PA1 = 0xEB;
        public const int VK_OEM_PA2 = 0xEC;
        public const int VK_OEM_PA3 = 0xED;
        public const int VK_OEM_WSCTRL = 0xEE;
        public const int VK_OEM_CUSEL = 0xEF;
        public const int VK_OEM_ATTN = 0xF0;
        public const int VK_OEM_FINNISH = 0xF1;
        public const int VK_OEM_COPY = 0xF2;
        public const int VK_OEM_AUTO = 0xF3;
        public const int VK_OEM_ENLW = 0xF4;
        public const int VK_OEM_BACKTAB = 0xF5;
        public const int VK_ATTN = 0xF6;
        public const int VK_CRSEL = 0xF7;
        public const int VK_EXSEL = 0xF8;
        public const int VK_EREOF = 0xF9;
        public const int VK_PLAY = 0xFA;
        public const int VK_ZOOM = 0xFB;
        public const int VK_NONAME = 0xFC;
        public const int VK_PA1 = 0xFD;
        public const int VK_OEM_CLEAR = 0xFE;
        #endregion


    }
}
