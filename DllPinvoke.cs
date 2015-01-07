/*
 * 由SharpDevelop创建。
 * 用户： Administrator
 * 日期: 2010-12-21
 * 时间: 15:32
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Runtime.InteropServices;

namespace eflayMH_WPF
{
	/// <summary>
	/// Description of DllPinvoke.
	/// </summary>
	public class DllPinvoke
	{

        #region Win API
        [DllImport("kernel32.dll")]
        public extern static IntPtr LoadLibrary(string path);

        [DllImport("kernel32.dll")]
        public extern static IntPtr GetProcAddress(IntPtr lib, string funcName);

        [DllImport("kernel32.dll")]
        public extern static bool FreeLibrary(IntPtr lib);
        #endregion

        private IntPtr hLib;        
        public DllPinvoke(String DLLPath)
        {
            hLib = LoadLibrary(DLLPath);
        }

        ~DllPinvoke()
        {
            FreeLibrary(hLib);            
        }

        public int GetProcAddress(string APIName)
        {
        	return (int)GetProcAddress(hLib, APIName);;
        }
        
        
        //将要执行的函数转换为委托
        public Delegate Invoke (string APIName,Type t)  
        {
            IntPtr api = GetProcAddress(hLib, APIName);
            return (Delegate)Marshal.GetDelegateForFunctionPointer(api, t);
        }

	}
}
