using System;
using System.Windows;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.IO;
using System.Text;
//using System.Collections.Generic;
using Microsoft.Win32;






namespace eflayMH_WPF
{
	/// <summary>
	/// Window1.xaml 的交互逻辑
	/// </summary>
	public partial class Window1 : Window
	{

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="nIndex"></param>
        /// <param name="dwNewLong">WinStyle</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hWnd">GWL</param>
        /// <param name="nIndex"></param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        const int WS_CAPTION = 0xC00000;//有标题栏
        const int WS_THICKFRAME = 0x40000; //调整大小用的边框
        const int WS_MAXIMIZEBOX = 0x10000; //最大化


        enum GWL : int
        {
            GWL_ID = (-12),
            GWL_STYLE = (-16),
            GWL_EXSTYLE = (-20)
        }

        /// <summary>
        /// Sets the show state and the restored, minimized, and maximized positions of the specified window.
        /// </summary>
        /// <param name="hWnd">
        /// A handle to the window.
        /// </param>
        /// <param name="lpwndpl">
        /// A pointer to a WINDOWPLACEMENT structure that specifies the new show state and window positions.
        /// <para>
        /// Before calling SetWindowPlacement, set the length member of the WINDOWPLACEMENT structure to sizeof(WINDOWPLACEMENT). SetWindowPlacement fails if the length member is not set correctly.
        /// </para>
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is nonzero.
        /// <para>
        /// If the function fails, the return value is zero. To get extended error information, call GetLastError.
        /// </para>
        /// </returns>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetWindowPlacement(IntPtr hWnd,
                                              [In] ref WINDOWPLACEMENT lpwndpl);


        /// <summary>
        /// Contains information about the placement of a window on the screen.
        /// </summary>
        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        internal struct WINDOWPLACEMENT
        {
            /// <summary>
            /// The length of the structure, in bytes. Before calling the GetWindowPlacement or SetWindowPlacement functions, set this member to sizeof(WINDOWPLACEMENT).
            /// <para>
            /// GetWindowPlacement and SetWindowPlacement fail if this member is not set correctly.
            /// </para>
            /// </summary>
            public int Length;

            /// <summary>
            /// Specifies flags that control the position of the minimized window and the method by which the window is restored.
            /// </summary>
            public int Flags;

            /// <summary>
            /// The current show state of the window.
            /// </summary>
            public int ShowCmd;

            /// <summary>
            /// The coordinates of the window's upper-left corner when the window is minimized.
            /// </summary>
            public System.Windows.Point MinPosition;

            /// <summary>
            /// The coordinates of the window's upper-left corner when the window is maximized.
            /// </summary>
            public System.Windows.Point MaxPosition;

            /// <summary>
            /// The window's coordinates when the window is in the restored position.
            /// </summary>
            public Rect NormalPosition;

            /// <summary>
            /// Gets the default (empty) value.
            /// </summary>
            public static WINDOWPLACEMENT Default
            {
                get
                {
                    WINDOWPLACEMENT result = new WINDOWPLACEMENT();
                    result.Length = Marshal.SizeOf(result);
                    return result;
                }
            }
        }


        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);
        static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        static readonly IntPtr HWND_TOP = new IntPtr(0);
        static readonly IntPtr HWND_BOTTOM = new IntPtr(1);

        [Flags()]
        private enum SetWindowPosFlags : uint
        {
            /// <summary>If the calling thread and the thread that owns the window are attached to different input queues,
            /// the system posts the request to the thread that owns the window. This prevents the calling thread from
            /// blocking its execution while other threads process the request.</summary>
            /// <remarks>SWP_ASYNCWINDOWPOS</remarks>
            SynchronousWindowPosition = 0x4000,
            /// <summary>Prevents generation of the WM_SYNCPAINT message.</summary>
            /// <remarks>SWP_DEFERERASE</remarks>
            DeferErase = 0x2000,
            /// <summary>Draws a frame (defined in the window's class description) around the window.</summary>
            /// <remarks>SWP_DRAWFRAME</remarks>
            DrawFrame = 0x0020,
            /// <summary>Applies new frame styles set using the SetWindowLong function. Sends a WM_NCCALCSIZE message to
            /// the window, even if the window's size is not being changed. If this flag is not specified, WM_NCCALCSIZE
            /// is sent only when the window's size is being changed.</summary>
            /// <remarks>SWP_FRAMECHANGED</remarks>
            FrameChanged = 0x0020,
            /// <summary>Hides the window.</summary>
            /// <remarks>SWP_HIDEWINDOW</remarks>
            HideWindow = 0x0080,
            /// <summary>Does not activate the window. If this flag is not set, the window is activated and moved to the
            /// top of either the topmost or non-topmost group (depending on the setting of the hWndInsertAfter
            /// parameter).</summary>
            /// <remarks>SWP_NOACTIVATE</remarks>
            DoNotActivate = 0x0010,
            /// <summary>Discards the entire contents of the client area. If this flag is not specified, the valid
            /// contents of the client area are saved and copied back into the client area after the window is sized or
            /// repositioned.</summary>
            /// <remarks>SWP_NOCOPYBITS</remarks>
            DoNotCopyBits = 0x0100,
            /// <summary>Retains the current position (ignores X and Y parameters).</summary>
            /// <remarks>SWP_NOMOVE</remarks>
            IgnoreMove = 0x0002,
            /// <summary>Does not change the owner window's position in the Z order.</summary>
            /// <remarks>SWP_NOOWNERZORDER</remarks>
            DoNotChangeOwnerZOrder = 0x0200,
            /// <summary>Does not redraw changes. If this flag is set, no repainting of any kind occurs. This applies to
            /// the client area, the nonclient area (including the title bar and scroll bars), and any part of the parent
            /// window uncovered as a result of the window being moved. When this flag is set, the application must
            /// explicitly invalidate or redraw any parts of the window and parent window that need redrawing.</summary>
            /// <remarks>SWP_NOREDRAW</remarks>
            DoNotRedraw = 0x0008,
            /// <summary>Same as the SWP_NOOWNERZORDER flag.</summary>
            /// <remarks>SWP_NOREPOSITION</remarks>
            DoNotReposition = 0x0200,
            /// <summary>Prevents the window from receiving the WM_WINDOWPOSCHANGING message.</summary>
            /// <remarks>SWP_NOSENDCHANGING</remarks>
            DoNotSendChangingEvent = 0x0400,
            /// <summary>Retains the current size (ignores the cx and cy parameters).</summary>
            /// <remarks>SWP_NOSIZE</remarks>
            IgnoreResize = 0x0001,
            /// <summary>Retains the current Z order (ignores the hWndInsertAfter parameter).</summary>
            /// <remarks>SWP_NOZORDER</remarks>
            IgnoreZOrder = 0x0004,
            /// <summary>Displays the window.</summary>
            /// <remarks>SWP_SHOWWINDOW</remarks>
            ShowWindow = 0x0040,
        }



        private static bool DisjoyZMR(ProcessC war3,Module md)
        {
            //Process warp = Process.GetProcessesByName("war3")[0];
            //ProcessC war3p = new ProcessC(warp);
            Module[] Moudles = //war3p.Modules;
            war3.Modules;

                    FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(md.FullName);
                    string zmrversion = fvi.FileVersion.ToString().Replace(", ", ".");
            string filename = Path.GetFileName(md.FullName).ToLower();
            int zmroffset1 = 0;
            int outb;

            byte[] hash = FileHash.GetFileHash(md.FullName);

            if (filename == "vlanproxy.dll")
            {
                
                switch (zmrversion)
                {
                    case "1.0.0.1":

                        //zmroffset1 = 0x16570;
                        if (Extension.ArrayEquals(hash, new byte[] { 0x48, 0x06, 0x3B, 0x94, 0xD2, 0x27, 0x79, 0x95, 0x24, 0x82, 0x44, 0x82, 0x6E, 0x46, 0x30, 0x22 }))
                        {
                            PatchSleepExit(war3, md, 0xD6B0);
                            //war3.WriteBytes(md.BaseAddress + 0xD6B0, new byte[] { 0xe9, 0x08, 0xe7, 0x02, 0x00 });
                        }
                        else
                        {
                            return false;
                        }
                        break;
                    default:
                        return false;
                }
            }
            else if (filename == "tvlanproxy.dll")
            {
                switch (zmrversion)
                {
                    case "1.0.0.1":
                        if (Extension.ArrayEquals(hash,new byte[] { 0xE9, 0x4E, 0x72, 0x26, 0xCF, 0x52, 0xFE, 0x6E, 0x4B, 0x56, 0xD4, 0xC2, 0xD0, 0x1B, 0x02, 0xDF }))
                        {
                            PatchSleepExit(war3, md, 0x35CDD);
                            war3.WriteBytes(md.BaseAddress + 0x75D0, new byte[] { 0xe9, 0x08, 0xe7, 0x02, 0x00 });
                        }
                        if (Extension.ArrayEquals(hash, new byte[] { 0xF5, 0x96, 0x35, 0x87, 0x9F, 0x77, 0xCD, 0x27, 0xA0, 0x4D, 0x4A, 0xFF, 0x54, 0xF6, 0xF7, 0xF7 }))
                        {
                            PatchSleepExit(war3, md, 0x365db);// 165BA57B
                            war3.WriteBytes(md.BaseAddress + 0x7a50, new byte[] { 0xe9, 0x86, 0xeb, 0x02, 0x00 });
                        }

                        else if (Extension.ArrayEquals(hash, new byte[] { 0x62, 0x99, 0xBC, 0x91, 0xF3, 0xB3, 0x3D, 0x92, 0x46, 0xEC, 0xFE, 0xE5, 0xDF, 0xB5, 0xF0, 0xFC })) // 110331
                        {
                            PatchSleepExit(war3, md, 0x39037);
                            war3.WriteBytes(md.BaseAddress + 0x82e0, new byte[] { 0xE9, 0x52, 0x0D, 0x03, 0x00 });
                        }
                        else if (Extension.ArrayEquals(hash, new byte[] { 0x1B,0xAC,0x9C,0xDC,0x2C,0x15,0xF5,0xA6,0xDF,0x76,0x7C,0x7C,0xA1,0x34,0xAC,0xC8 })) // 110422
                        {
                            PatchSleepExit(war3, md, 0x54971);
                            war3.WriteBytes(md.BaseAddress + 0x7DB0, new byte[] { 0xE9, 0xBC,0xCB,0x04,0x00 });
                        }
                        else
                        {
                            return false;
                        }

                        //zmroffset1 = 0x90168;
                        
                        break;
                    default:
                        {
                            return false;
                        }
                }
            }

            if (zmroffset1 != 0)
            {
                PatchSleepLoop(war3, md, zmroffset1);

            }

            return true;

        }// end of private void DisjoyZMR()

        private static void PatchSleepExit(ProcessC war3, Module md, int zmroffset1)
        {
            int tmp = 0;
            uint oldprot = 0;
            war3.VirtualProtectEx(md.BaseAddress + zmroffset1, 15, ProcessC.Protection.PAGE_EXECUTE_READWRITE, ref oldprot);
            DllPinvoke kernel32 = new DllPinvoke("kernel32.dll");
            int Saddr = kernel32.GetProcAddress("Sleep");
            int ExitThreadAddr = kernel32.GetProcAddress("ExitThread");
            byte[] AddrBt = BitConverter.GetBytes(Saddr - (md.BaseAddress + zmroffset1 + 0x5 + 0x5));//0xaf657a);
            byte[] AddrBt2 = BitConverter.GetBytes(ExitThreadAddr - (md.BaseAddress + zmroffset1 + 0x5 + 0xc));//0x0c5ba57b);
            //0xd96580
            war3.WriteBytes(md.BaseAddress + zmroffset1,
                             new byte[]{0x68,0x00,0x00,0x01,0x00,
					                 	0xe8,AddrBt[0],AddrBt[1],AddrBt[2],AddrBt[3],
                                        0x6a,0x0,
                                        0xe8,AddrBt2[0],AddrBt2[1],AddrBt2[2],AddrBt2[3]
                                        //0xeb,0xf4
                             },
                             17, out tmp);
        }


        private static void PatchSleepLoop(ProcessC war3, Module md, int zmroffset1)
        {
            int tmp = 0;
            uint oldprot = 0;
            war3.VirtualProtectEx(md.BaseAddress + zmroffset1, 12, ProcessC.Protection.PAGE_EXECUTE_READWRITE, ref oldprot);
            DllPinvoke kernel32 = new DllPinvoke("kernel32.dll");
            int Saddr = kernel32.GetProcAddress("Sleep");
            //int ExitThreadAddr = kernel32.GetProcAddress("ExitThread");
            byte[] AddrBt = BitConverter.GetBytes(Saddr - (md.BaseAddress + zmroffset1 + 0x5 + 0x5));//0xaf657a);
            //0xd96580
            war3.WriteBytes(md.BaseAddress + zmroffset1,
                             new byte[]{0x68,0x00,0x00,0x01,0x00,
					                 	0xe8,AddrBt[0],AddrBt[1],AddrBt[2],AddrBt[3],
                                        0xeb,0xf4
                             },
                             12, out tmp);
        }



        #region 辅助



        void SetWar3Windows()
        {

            if (war3 == null)
                return;

            IntPtr war3hwnd = war3.pname.MainWindowHandle;
            bool? chuangtiischeck = null;
            this.Dispatcher.Invoke((ThreadStart)delegate() { chuangtiischeck = this.chuangti.IsChecked; }, null);
            if (chuangtiischeck.HasValue && chuangtiischeck.Value == true)
            {
                int wins = GetWindowLong(war3hwnd, (int)GWL.GWL_STYLE);

                wins &= ~WS_CAPTION;
                wins &= ~WS_THICKFRAME;
                SetWindowLong(war3hwnd, (int)GWL.GWL_STYLE, wins);

                WINDOWPLACEMENT tmpplac = WINDOWPLACEMENT.Default;
                tmpplac.ShowCmd = 3;
                SetWindowPlacement(war3hwnd, ref tmpplac);

                Screen sc = Screen.PrimaryScreen;

                SetWindowPos(war3hwnd, HWND_TOP, 0, 0, sc.Bounds.Width, sc.Bounds.Height, SetWindowPosFlags.ShowWindow);

            }
        }



        /// <summary>
        /// 魔兽窗体还原
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void button1_Click(object sender, RoutedEventArgs e)
        {

            if (war3 == null)
                return;


            IntPtr war3hwnd = war3.pname.MainWindowHandle;
            bool? chuangtiischeck = null;
            this.Dispatcher.Invoke((MethodInvoker)delegate() { chuangtiischeck = this.chuangti.IsChecked; }, null);

            int wins = GetWindowLong(war3hwnd, (int)GWL.GWL_STYLE);

            wins |= WS_CAPTION;
            wins |= WS_THICKFRAME;
            SetWindowLong(war3hwnd, (int)GWL.GWL_STYLE, wins);

            WINDOWPLACEMENT tmpplac = WINDOWPLACEMENT.Default;
            tmpplac.ShowCmd = 3;
            SetWindowPlacement(war3hwnd, ref tmpplac);

            Screen sc = Screen.PrimaryScreen;

            SetWindowPos(war3hwnd, HWND_TOP, 100, 100, 800, 600, SetWindowPosFlags.ShowWindow);

        }


        #endregion



        void buttonInject_Click(object sender, RoutedEventArgs e)
        {
            if (war3 != null)
            {
                string currentpath = Environment.CurrentDirectory;
                System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
                ofd.InitialDirectory = Path.Combine(currentpath, "dll");
                ofd.Filter = "DLL|*.dll|All|*.*";


                if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    foreach (string file in ofd.FileNames)
                    {
                        war3.inject(file, false);
                    }
                }
                Environment.CurrentDirectory = currentpath;
            }
        }


        void button2_Click(object sender, RoutedEventArgs e)
        {
            new Thread(
                new ThreadStart(
                    DesjoyAA)).Start();
            //			DesjoyAA();
        }





        //int AApid = 0;

        public void DesjoyAA()
        {
            try
            {
                int AAoffset1 = 0;
                Process AAProc = Process.GetProcessesByName("AAClientOriginal")[0];

                byte[] AAhash = FileHash.GetFileHash(AAProc.MainModule.FileName);
                byte[] bt2 = new byte[] { 0x08, 0xBF, 0x98, 0x54, 0x13, 0x2C, 0x8C, 0x47, 0xC9, 0x56, 0x80, 0x93, 0x8F, 0xB2, 0x3A, 0xDC };
                if (Extension.Equals(AAhash, bt2))
                {
                    AAoffset1 = 0x40711E;
                }

                if (AAoffset1 == 0)
                {
                    Dispatcher.BeginInvoke(new ThreadStart(delegate() { ButtonDesjoyAA.Content = "不支持的版本"; }), null);
                    Thread.Sleep(2000);
                    Dispatcher.BeginInvoke(new ThreadStart(delegate() { ButtonDesjoyAA.Content = "终结AA"; }), null);

                    return;

                }

                ProcessC AAC = new ProcessC(AAProc);
                AAC.WriteMemoryVP(AAoffset1, new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90 });
                Dispatcher.BeginInvoke(new ThreadStart(delegate() { ButtonDesjoyAA.Content = "已终结"; }), null);
                Thread.Sleep(2000);
                Dispatcher.BeginInvoke(new ThreadStart(delegate() { ButtonDesjoyAA.Content = "终结AA"; }), null);


            }
            catch (Exception)
            { }
        }



        void ButtonGaimin_Click(object sender, RoutedEventArgs e)
        {
            RegistryKey War3string = Registry.CurrentUser.CreateSubKey("Software").CreateSubKey("Blizzard Entertainment").CreateSubKey("Warcraft III")
                .CreateSubKey("String");
            War3string.SetValue("userlocal", UTF8Encoding.UTF8.GetBytes(TextBoxWarName.Text), RegistryValueKind.Binary);
            
        }

        private void ButtonFuzhi_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                byte[] tmp = Encoding.UTF8.GetBytes(TextBoxWarName.Text);
                System.Windows.Clipboard.SetText(Encoding.Default.GetString(tmp), System.Windows.TextDataFormat.Text);
            }
            catch
            {}
        }

    } //end of public partial class Window1 : Window



}
