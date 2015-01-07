﻿
using System;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections.Generic;
namespace eflayMH_WPF
{

    /// <summary> 
    /// 这个类可以让你得到一个在运行中程序的所有键盘事件 
    /// 并且引发一个带KeyEventArgs和MouseEventArgs参数的.NET事件以便你很容易使用这些信息 
    /// </summary> 
    /// <remarks> 
    /// 修改:lihx 
    /// 修改时间:04.11.8 
    /// </remarks> 
    public class KeyBordHook
    {
        private const int WM_KEYDOWN = 0x100;
        private const int WM_KEYUP = 0x101;
        private const int WM_SYSKEYDOWN = 0x104;
        private const int WM_SYSKEYUP = 0x105;

        //全局的事件 
        public event KeyEventHandler OnKeyDownEvent;
        public event KeyEventHandler OnKeyUpEvent;
        public event KeyPressEventHandler OnKeyPressEvent;

        static int hKeyboardHook = 0; //键盘钩子句柄

        //鼠标常量 
        public const int WH_KEYBOARD_LL = 13; //keyboard hook constant

        HookProc KeyboardHookProcedure; //声明键盘钩子事件类型.

        //声明键盘钩子的封送结构类型 
        [StructLayout(LayoutKind.Sequential)]
        public class KeyboardHookStruct
        {
            public int vkCode; //表示一个在1到254间的虚似键盘码 
            public int scanCode; //表示硬件扫描码 
            public int flags;
            public int time;
            public int dwExtraInfo;
        }
        //装置钩子的函数 
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);

        //卸下钩子的函数 
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnhookWindowsHookEx(int idHook);

        //下一个钩挂的函数 
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int CallNextHookEx(int idHook, int nCode, Int32 wParam, IntPtr lParam);

        [DllImport("user32")]
        public static extern int ToAscii(int uVirtKey, int uScanCode, byte[] lpbKeyState, byte[] lpwTransKey, int fuState);

        [DllImport("user32")]
        public static extern int GetKeyboardState(byte[] pbKeyState);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto,
CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        public delegate int HookProc(int nCode, Int32 wParam, IntPtr lParam);

        //先前按下的键
        public List<Keys> preKeys = new List<Keys>();

        /// <summary> 
        /// 墨认的构造函数构造当前类的实例并自动的运行起来. 
        /// </summary> 
        public KeyBordHook()
        {
            Start();
        }

        //析构函数. 
        ~KeyBordHook()
        {
            Stop();
        }

        public void Start()
        {
            //安装键盘钩子 
            if (hKeyboardHook == 0)
            {
                KeyboardHookProcedure = new HookProc(KeyboardHookProc);
                //hKeyboardHook = SetWindowsHookEx(WH_KEYBOARD_LL, KeyboardHookProcedure, Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]), 0);
                Process curProcess = Process.GetCurrentProcess();
                ProcessModule curModule = curProcess.MainModule;

                hKeyboardHook = SetWindowsHookEx(WH_KEYBOARD_LL, KeyboardHookProcedure, GetModuleHandle(curModule.ModuleName), 0);

                if (hKeyboardHook == 0)
                {
                    Stop();
                    throw new Exception("SetWindowsHookEx ist failed.");
                }
            }
        }

        public void Stop()
        {
            bool retKeyboard = true;

            if (hKeyboardHook != 0)
            {
                retKeyboard = UnhookWindowsHookEx(hKeyboardHook);
                hKeyboardHook = 0;
            }
            //如果卸下钩子失败 
            if (!(retKeyboard)) throw new Exception("UnhookWindowsHookEx failed.");
        }

        private int KeyboardHookProc(int nCode, Int32 wParam, IntPtr lParam)
        {

            if ((nCode >= 0) && (OnKeyDownEvent != null || OnKeyUpEvent != null || OnKeyPressEvent != null))
            {
                KeyboardHookStruct MyKeyboardHookStruct = (KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyboardHookStruct));
                //当有OnKeyDownEvent 或 OnKeyPressEvent不为null时,ctrl alt shift keyup时 preKeys
                //中的对应的键增加                   
                if ((OnKeyDownEvent != null || OnKeyPressEvent != null) && (wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN))
                {
                    Keys keyData = (Keys)MyKeyboardHookStruct.vkCode;
                    if (IsCtrlAltShiftKeys(keyData) && preKeys.IndexOf(keyData) == -1)
                    {
                        preKeys.Add(keyData);
                    }
                }
                //引发OnKeyDownEvent 
                if (OnKeyDownEvent != null && (wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN))
                {
                    Keys keyData = (Keys)MyKeyboardHookStruct.vkCode;
                    KeyEventArgs e = new KeyEventArgs(GetDownKeys(keyData));

                    OnKeyDownEvent(this, e);
                }

                //引发OnKeyPressEvent 
                if (OnKeyPressEvent != null && wParam == WM_KEYDOWN)
                {
                    byte[] keyState = new byte[256];
                    GetKeyboardState(keyState);

                    byte[] inBuffer = new byte[2];
                    if (ToAscii(MyKeyboardHookStruct.vkCode,
                    MyKeyboardHookStruct.scanCode,
                    keyState,
                    inBuffer,
                    MyKeyboardHookStruct.flags) == 1)
                    {
                        KeyPressEventArgs e = new KeyPressEventArgs((char)inBuffer[0]);
                        OnKeyPressEvent(this, e);
                    }
                }

                //当有OnKeyDownEvent 或 OnKeyPressEvent不为null时,ctrl alt shift keyup时 preKeys
                //中的对应的键删除
                if ((OnKeyDownEvent != null || OnKeyPressEvent != null) && (wParam == WM_KEYUP || wParam == WM_SYSKEYUP))
                {
                    Keys keyData = (Keys)MyKeyboardHookStruct.vkCode;
                    if (IsCtrlAltShiftKeys(keyData))
                    {

                        for (int i = preKeys.Count - 1; i >= 0; i--)
                        {
                            if (preKeys[i] == keyData)
                            {
                                preKeys.RemoveAt(i);
                            }
                        }

                    }
                }
                //引发OnKeyUpEvent 
                if (OnKeyUpEvent != null && (wParam == WM_KEYUP || wParam == WM_SYSKEYUP))
                {
                    Keys keyData = (Keys)MyKeyboardHookStruct.vkCode;
                    KeyEventArgs e = new KeyEventArgs(GetDownKeys(keyData));
                    OnKeyUpEvent(this, e);
                }
            }
            return CallNextHookEx(hKeyboardHook, nCode, wParam, lParam);
        }




        private Keys GetDownKeys(Keys key)
        {
            Keys rtnKey = Keys.None;
            foreach (Keys keyTemp in preKeys)
            {
                switch (keyTemp)
                {
                    case Keys.LControlKey:
                    case Keys.RControlKey:
                        rtnKey = rtnKey | Keys.Control;
                        break;
                    case Keys.LMenu:
                    case Keys.RMenu:
                        rtnKey = rtnKey | Keys.Alt;
                        break;
                    case Keys.LShiftKey:
                    case Keys.RShiftKey:
                        rtnKey = rtnKey | Keys.Shift;
                        break;
                    default:
                        break;
                }
            }
            rtnKey = rtnKey | key;

            return rtnKey;
        }

        private Boolean IsCtrlAltShiftKeys(Keys key)
        {

            switch (key)
            {
                case Keys.LControlKey:
                case Keys.RControlKey:
                case Keys.LMenu:
                case Keys.RMenu:
                case Keys.LShiftKey:
                case Keys.RShiftKey:
                    return true;
                default:
                    return false;
            }
        }
    }
}



