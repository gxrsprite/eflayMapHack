using System;
using System.Windows;
using System.Windows.Documents;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Interop;
using System.IO;
//using System.Collections.Generic;




namespace eflayMH_WPF
{
	/// <summary>
	/// Window1.xaml 的交互逻辑
	/// </summary>
	public partial class Window1 : Window
	{

		public Window1()
		{
			InitializeComponent();


			this.notifyIcon = new NotifyIcon();
			this.notifyIcon.BalloonTipText = "最小化到托盘";
			this.notifyIcon.Text = "eflayMH";
			this.notifyIcon.Icon = Resource.lvdou;//new System.Drawing.Icon("NotifyIcon.ico");
			//this.notifyIcon.Visible = true;
			notifyIcon.MouseDoubleClick += OnNotifyIconDoubleClick;
			ContextMenuStrip cmc = new ContextMenuStrip();
			System.Windows.Forms.ToolStripMenuItem ExitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			System.Windows.Forms.ToolStripMenuItem 显示主界面ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			cmc.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			                   	显示主界面ToolStripMenuItem,ExitToolStripMenuItem
			                   });
			ExitToolStripMenuItem.Name = "ExitToolStripMenuItem";
			ExitToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			ExitToolStripMenuItem.Text = "退出";
			ExitToolStripMenuItem.Click += new EventHandler(ExitToolStripMenuItem_Click);
			// 
			// 显示主界面ToolStripMenuItem
			// 
			显示主界面ToolStripMenuItem.Name = "显示主界面ToolStripMenuItem";
			显示主界面ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			显示主界面ToolStripMenuItem.Text = "显示主界面";
			显示主界面ToolStripMenuItem.Click += OnNotifyIconDoubleClick;


			notifyIcon.ContextMenuStrip = cmc;
			this.notifyIcon.ShowBalloonTip(1000);

			richTextBox1.Document.AddHandler(Hyperlink.ClickEvent, new RoutedEventHandler(HandleHyperlinkClick));
			
			string skinDictPath = @".\Resources\Skins\Skin1.xaml";
			Uri skinDictUri = new Uri(skinDictPath, UriKind.Relative);

			// Tell the Application to load the skin resources.
			App app = System.Windows.Application.Current as App;
			app.ApplySkin(skinDictUri);
			
			
			this.checkBoxxsdw.Checked += delegate(object sender,RoutedEventArgs e)
			{
				checkBoxbxdw.IsChecked = false;
			};
			this.checkBoxbxdw.Checked += delegate(object sender,RoutedEventArgs e)
			{
				checkBoxxsdw.IsChecked = false;
				checkBoxswdx.IsChecked = false;
				checkBoxxssf.IsChecked = false;
			};
			
			//this.Title = "MH from Breeze356.5d6d.com";
			//new Thread((ThreadStart)delegate(){Thread.Sleep(60000*5);this.Dispatcher.Invoke((MethodInvoker)delegate(){this.Title = "";});}).Start();
		}


		void ExitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.notifyIcon.Visible = false;
			this.Close();
		}

		NotifyIcon notifyIcon;

		private void OnNotifyIconDoubleClick(object sender, EventArgs e)
		{
			this.Show();
			this.notifyIcon.Visible = false;
			this.Activate();
			this.WindowState = WindowState.Normal;
			this.Topmost = true;
			this.Topmost = false;

		}
		private void Window_StateChanged(object sender, EventArgs e)
		{

			if (this.WindowState == WindowState.Minimized)
			{
				Hide();
				this.notifyIcon.Visible = true;

			}
		}

		[DllImport("user32.dll")]
		public static extern bool RegisterHotKey(IntPtr hWnd, int id, KeyModifiers fsModifiers, System.Windows.Forms.Keys vk);
		[DllImport("user32.dll")]
		public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

		/// <summary>
		/// RichTextBox中的链接被点击时触发
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void HandleHyperlinkClick(object sender, RoutedEventArgs args)
		{
			Hyperlink link = args.Source as Hyperlink;
			//...
			Process.Start(link.NavigateUri.ToString());
		}

		private static ProcessC war3;


		//public delegate void findwar3delegate();
		//findwar3delegate find = null;

		string ProcessVersion;

		/// <summary>
		/// 表示是否手动关闭，同时表示：MH自动开启时为false，手动开启时为true
		/// </summary>
		bool forceOff = true;

		public enum KeyModifiers
		{ None = 0, Alt = 1, Ctrl = 2, Shift = 4, Windows = 8 }

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{

			Process.EnterDebugMode();


			Thread war3mhtd = new Thread(new ThreadStart(war3mh));
			war3mhtd.IsBackground = true;
			war3mhtd.Start();
			//Dispatcher.BeginInvoke(find, null);
			LoadConfig();
			
		}




		/// <summary>
		/// 处理消息
		/// </summary>
		/// <param name="hwnd"></param>
		/// <param name="msg"></param>
		/// <param name="wParam"></param>
		/// <param name="lParam"></param>
		/// <param name="handle"></param>
		/// <returns></returns>
		IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handle)
		{
			switch (msg)
			{
				case 0x312:
					switch (wParam.ToInt32())
					{
						case 101:
							buttonON_Click(buttonON, new RoutedEventArgs());
							//此处填写快捷键响应代码
							break;
						case 102:
							buttonOFF_Click(buttonOFF, new RoutedEventArgs());
							break;
						default:
							break;
					}
					break;


			}
			return IntPtr.Zero;
		}

		bool isMHON = false;

		//private void foundwar3proc(IAsyncResult e)
		//{
		//    find.EndInvoke(e);
		//}


		/// <summary>
		/// 找到魔兽进程进行初始化
		/// </summary>
		/// <returns></returns>
		private bool findwar3process()
		{
			Process[] war3p = Process.GetProcessesByName("war3");



			if (war3p != null && war3p.Length >= 1)
			{

				if (war3!=null&&war3.Pid == war3p[0].Id)
				{
					return true;
				}



				war3 = new ProcessC(war3p[0]);
				war3.pname.Exited += new EventHandler(war3_pname_Exited);
				//只在第一次见到魔兽时触发
				if (war3start_event != null)
					war3start_event(war3);

				//没找到game.dll的话默认值
				war3.DllBaseAddress = 0x6F000000;

				foreach (Module md in war3.Modules)
				{


					string filename = Path.GetFileName(md.FullName).ToLower();
					if (filename == "game.dll" || filename == "game124.dll")//||filename.Contains("game")
					{

						FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(md.FullName);
						try
						{
							//if (fvi.OriginalFilename.ToLower() == "game.dll")
							//{
							war3.DllBaseAddress = md.BaseAddress;
							war3.Version = fvi.FileVersion.Replace(", ", ".");
							
							//}
						}
						catch (NullReferenceException)
						{ }
						
						Dispatcher.Invoke((ThreadStart)(delegate() { textBlock1.Text = "魔兽已启动,PID:" + war3.Pid + ",版本:" + war3.Version; }), null);
						return true;
					}

					
					
				}
				Dispatcher.Invoke(new ThreadStart(delegate() { textBlock1.Text = "魔兽已启动,PID:" + war3.Pid + ",游戏还未加载完毕" ;}), null);
				return false;
			}
			return false;
		}

		void war3_pname_Exited(object sender, EventArgs e)
		{
			//用此事件判断进程退出并不及时，不用也罢
			Dispatcher.BeginInvoke(new ThreadStart(delegate() { textBlock1.Text = "魔兽已关闭"; }), null);
			//
		}



		public delegate void war3runningdelegate(ProcessC war3);
		public event war3runningdelegate war3start_event;
		public event war3runningdelegate war3isrunning_event;


		/// <summary>
		/// 主功能线程循环
		/// </summary>
		private void war3mh()
		{


			while (true)
			{
				#if RELEASE
				try
				{
					#endif
					Thread.Sleep(2000);
					//Thread.SpinWait(3000);
					Process[] processes = Process.GetProcessesByName("war3");


					if (processes != null && processes.Length > 0)//发现魔兽进程
					{

						if (war3 == null || war3.Pid != processes[0].Id)
						{
							Thread.Sleep(2000);
						}

						if (!findwar3process())
						{
							continue;
						}

						//每次见到魔兽都触发
						if (war3isrunning_event != null)
							war3isrunning_event(war3);


						if (isMHON == false && forceOff == false)
						{


							lock (war3)
							{



								War3MHon(true);
							}
						}


						SetWar3Windows();


					}
					else //没发现魔兽进程
					{

						Dispatcher.BeginInvoke(new ThreadStart(delegate() { textBlock1.Text = "魔兽未启动"; }), null);

						isMHON = false;
						//forceOff = false;
					}

					bool isdesjoyAA = false;

					Dispatcher.Invoke((MethodInvoker)delegate
					                  { isdesjoyAA = CheckBoxDesjoyAA.IsChecked.Value; }
					                 );

					if (isdesjoyAA == true)
					{
						new Thread((ThreadStart)DesjoyAA).Start();
					}

					#if RELEASE
				}
				//catch (Exception)
				//{
				//    //this.Dispatcher.Invoke((MethodInvoker)delegate
				//    //{

				//    //    throw ex;

				//    //});

				//}
				catch
				{ }
				#endif

			}

		}



		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			SaveConfig();
			this.notifyIcon.Visible = false;
			//Environment.Exit(0);
		}



		private void gotobreeze_Click(object sender, RoutedEventArgs e)
		{
            Process.Start("http://breeze356.5d6d.com/?fromuid=424");
		}




		private void MenuItem_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void checkBoxzdkq_Checked(object sender, RoutedEventArgs e)
		{
			forceOff = false;
		}

		private void checkBoxzdkq_Unchecked(object sender, RoutedEventArgs e)
		{
			forceOff = true;
		}



		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);

			//快捷键相关
			IntPtr whandle = new System.Windows.Interop.WindowInteropHelper(this).Handle;
			RegisterHotKey(whandle, 101, KeyModifiers.Ctrl | KeyModifiers.Alt, Keys.O);
			RegisterHotKey(whandle, 102, KeyModifiers.Ctrl | KeyModifiers.Alt, Keys.P);
			HwndSource source = PresentationSource.FromVisual(this) as HwndSource;
			source.AddHook(WndProc);
			//SetWindowLong(whandle, -16, 0);
		}

		private void checkBoxqzbb_Checked(object sender, RoutedEventArgs e)
		{
			ComboBoxVersion.IsEnabled = true;
		}

		private void checkBoxqzbb_Unchecked(object sender, RoutedEventArgs e)
		{
			ComboBoxVersion.IsEnabled = false;
		}

		private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
				this.DragMove();
		}


		

	} //end of public partial class Window1 : Window



}
