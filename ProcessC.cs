
using System;
//using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace eflayMH_WPF
{
	/// <summary>
	/// Description of dllinject.
	/// </summary>
	public unsafe class ProcessC
	{
		public ProcessC(int _pid)
		{
			Pid = _pid;


			//bool ttladj;
			//RtlAdjustPrivilege(20, true, 0, out ttladj);

			try
			{
				pname = Process.GetProcessById(Pid);
				hwnd = pname.Handle;
			}
			catch (Exception error)
			{   //当标示pid的进程不存在时发生异常；
				throw new ProcessCException("找不到进程，" + error);
			}
		}

		public ProcessC(Process proc)
		{
			Pid = proc.Id;
			pname = proc;
			hwnd = pname.Handle;

			//bool ttladj;
			//RtlAdjustPrivilege(20, true, 0, out ttladj);
		}

		//public static void enterDebugPrivilege()
		//{
		//    bool ttladj;
		//    RtlAdjustPrivilege(20, true, 0, out ttladj);
		//}

		[DllImport("kernel32.dll")]
		static extern int VirtualAllocEx(IntPtr hwnd, int lpaddress, int size, int type, Int32 tect);
		[DllImport("kernel32.dll")]
		static extern Boolean WriteProcessMemory(IntPtr hwnd, int baseaddress, string buffer, int nsize,out int filewriten);
		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool WriteProcessMemory(IntPtr hProcess, int lpBaseAddress, Byte[] lpBuffer, int nSize, out int lpNumberOfBytesWritten);
		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool WriteProcessMemory(IntPtr hProcess, int lpBaseAddress, char[] lpBuffer, int nSize, out int lpNumberOfBytesWritten);
		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool WriteProcessMemory(IntPtr hProcess, int lpBaseAddress, byte* lpBuffer, int nSize, out int lpNumberOfBytesWritten);
		[DllImport("kernel32.dll")]
		static extern bool WriteProcessMemory(IntPtr hProcess, int lpBaseAddress, int lpBuffer,
		                                      int nSize, out int lpNumberOfBytesWritten);
		[DllImport("kernel32.dll", SetLastError = true, PreserveSig = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress,void* lpBuffer, int nSize, out int lpNumberOfBytesRead);
		[DllImport("kernel32.dll", SetLastError = true, PreserveSig = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] Buffer, int nSize, out int lpNumberOfBytesRead);

		[DllImport("kernel32.dll")]
		static extern int GetProcAddress(int hwnd, string lpname);
		[DllImport("kernel32.dll")]
		static extern int GetModuleHandleA(string name);
		[DllImport("kernel32.dll")]
		static extern IntPtr CreateRemoteThread(IntPtr hwnd, int attrib, int size, int address, int par, int flags, int threadid);
		[DllImport("kernel32.dll")]
		static extern Int32 WaitForSingleObject(IntPtr hHandle, UInt32 dwMilliseconds);
		//[DllImport("kernel32.dll")]
		//static extern Boolean VirtualFree(IntPtr lpAddress, Int32 dwSize, Int32 dwFreeType);
		[DllImport("kernel32", SetLastError = true)]
		static extern IntPtr LoadLibrary(string lpFileName);

		[Flags]
		public enum FreeType
		{
			Decommit = 0x4000,
			Release = 0x8000,
		}
		[DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
		static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress,
		                                 int dwSize, FreeType dwFreeType);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="v1">enum imformation ,Process Token & Privilege</param>
		/// <param name="v2">是否打开权限</param>
		/// <param name="v3">仅当前线程？否则当前进程,adjustPrivilegeType</param>
		/// <param name="v4">此权限原来的状态</param>
		/// <returns></returns>
		//[DllImport("ntdll.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		//public static extern int RtlAdjustPrivilege(int v1, bool v2, int v3, out bool v4);


		[DllImport("kernel32.dll")]
		static extern bool VirtualProtectEx(IntPtr hProcess, int lpAddress,
		                                    uint dwSize, uint flNewProtect, ref uint lpflOldProtect);


		public enum Protection
		{
			PAGE_NOACCESS = 0x01,
			PAGE_READONLY = 0x02,
			PAGE_READWRITE = 0x04,
			PAGE_WRITECOPY = 0x08,
			PAGE_EXECUTE = 0x10,
			PAGE_EXECUTE_READ = 0x20,
			PAGE_EXECUTE_READWRITE = 0x40,
			PAGE_EXECUTE_WRITECOPY = 0x80,
			PAGE_GUARD = 0x100,
			PAGE_NOCACHE = 0x200,
			PAGE_WRITECOMBINE = 0x400
		}


        //[StructLayout(LayoutKind.Sequential)]
        //public struct UNICODE_STRING : IDisposable
        //{
        //    public ushort Length;
        //    public ushort MaximumLength;
        //    private IntPtr buffer;

        //    public UNICODE_STRING(string s)
        //    {
        //        Length = (ushort)(s.Length * 2);
        //        MaximumLength = (ushort)(Length + 2);
        //        buffer = Marshal.StringToHGlobalUni(s);
        //    }

        //    public void Dispose()
        //    {
        //        Marshal.FreeHGlobal(buffer);
        //        buffer = IntPtr.Zero;
        //    }

        //    public override string ToString()
        //    {
        //        return Marshal.PtrToStringUni(buffer);
        //    }
        //}

        //[StructLayout(LayoutKind.Sequential)]
        //struct PEB_LDR_DATA
        //{
        //    public int Length;
        //    public uint Initialized;
        //    public IntPtr SsHandle;
        //    public LIST_ENTRY InLoadOrderModuleList;
        //    public LIST_ENTRY InMemoryOrderModuleList;
        //    public LIST_ENTRY InInitializationOrderModuleList;
        //}

		[StructLayout(LayoutKind.Sequential)]
		unsafe struct LIST_ENTRY
		{
			public LIST_ENTRY* Flink;
			public LIST_ENTRY* Blink;
		}

        //[StructLayout(LayoutKind.Sequential)]
        //unsafe struct LDR_MODULE
        //{
        //    public LIST_ENTRY InLoadOrderModuleList;   //+0x00
        //    public LIST_ENTRY InMemoryOrderModuleList; //+0x08
        //    public LIST_ENTRY InInitializationOrderModuleList; //+0x10
        //    public void* BaseAddress;  //+0x18
        //    public void* EntryPoint;   //+0x1c
        //    public ULONG SizeOfImage;
        //    public UNICODE_STRING FullDllName;
        //    public UNICODE_STRING BaseDllName;
        //    public ULONG Flags;
        //    public SHORT LoadCount;
        //    public SHORT TlsIndex;
        //    public IntPtr SectionHandle;
        //    public uint CheckSum;
        //    public uint TimeDateStamp;
        //}

        //[StructLayout(LayoutKind.Sequential)]
        //private struct ProcessBasicInformation
        //{
        //    public int ExitStatus;
        //    public int PebBaseAddress;
        //    public int AffinityMask;
        //    public int BasePriority;
        //    public uint UniqueProcessId;
        //    public uint InheritedFromUniqueProcessId;
        //}

        //[DllImport("ntdll.dll")]
        //private static extern int NtQueryInformationProcess(
        //    IntPtr hProcess,
        //    int processInformationClass /* 0 */,
        //    ref ProcessBasicInformation processBasicInformation,
        //    uint processInformationLength,
        //    out uint returnLength
        //);

        //public static int VarPtr(object e)
        //{
        //    System.Runtime.InteropServices.GCHandle gh = System.Runtime.InteropServices.GCHandle.Alloc(e, System.Runtime.InteropServices.GCHandleType.Pinned);
        //    int gc = gh.AddrOfPinnedObject().ToInt32();
        //    gh.Free();
        //    return gc;
        //}

		public Process pname;
		const UInt32 INFINITE = 0xFFFFFFFF;
		const Int32 PAGE_EXECUTE_READWRITE = 0x40;
		const Int32 MEM_COMMIT = 0x1000;
		const Int32 MEM_RESERVE = 0x2000;
		const Int32 MEM_RELEASE = 0x8000;
		Int32 AllocBaseAddress;
		public IntPtr hwnd;
		string dllname;
		public Int32 Pid;
		Boolean ok;
		Int32 loadaddr;
		IntPtr ThreadHwnd;
		public int DllBaseAddress;
		public string Version;
		
		Module[] modules;
		
		public Module[] Modules
		{
			get
			{
				if (modules == null)
					modules = GetModules();
				

				return modules;
			}
			set
			{
                modules = value;
			}
		}

		public bool WriteMemory(int baseAddress, byte[] lpbuffer, int Size, out int RealSize)
		{
			return WriteProcessMemory(hwnd, baseAddress, lpbuffer, Size, out RealSize);
		}

		public bool WriteMemory(int baseAddress, byte[] lpbuffer, int Size)
		{
			int RealSize;


			bool result = WriteProcessMemory(hwnd, baseAddress, lpbuffer, Size, out RealSize);

			if (result == false)
			{
				throw new ProcessCException("内存写错误");
			}

			return result;
		}

		public bool WriteMemory(int baseAddress, int lpbuffer, int Size)
		{
			int RealSize;


			bool result = WriteProcessMemory(hwnd, baseAddress, lpbuffer, Size, out RealSize);

			if (result == false)
			{
				throw new ProcessCException("内存写错误");
			}

			return result;
		}

		/// <summary>
		/// 包含去除内存保护
		/// </summary>
		/// <param name="baseAddress"></param>
		/// <param name="lpbuffer"></param>
		/// <param name="Size"></param>
		/// <returns></returns>
		public bool WriteMemoryVP(int baseAddress, byte[] lpbuffer)
		{
			int RealSize;
			uint Size = (uint)lpbuffer.Length;
			uint oldProtect = 0;
			VirtualProtectEx(this.hwnd, baseAddress, Size, (uint)Protection.PAGE_READWRITE, ref oldProtect);
			bool result = WriteProcessMemory(hwnd, baseAddress, lpbuffer, (int)Size, out RealSize);
			VirtualProtectEx(hwnd, baseAddress, Size, oldProtect, ref oldProtect);

			if (result == false)
			{
				throw new ProcessCException("内存写错误");
			}

			return result;
		}


        public void WriteBytes(int BaseAddress, byte[] Buffer)
        {
            int oi;
            WriteBytes(BaseAddress, Buffer, Buffer.Length,out oi);
        }


		public void WriteBytes(int BaseAddress, byte[] Buffer, int Size, out int BytesWriten)
		{
			if (hwnd == IntPtr.Zero)
				BytesWriten = 0;
			else
				WriteProcessMemory(hwnd, BaseAddress, Buffer, Size, out BytesWriten);
		}

		/// <summary>
		/// 写内存地址加上指定Dll基址
		/// </summary>
		/// <param name="BaseAddress"></param>
		/// <param name="Buffer"></param>
		/// <param name="Size"></param>
		public void patch(int BaseAddress,params byte[] Buffer)
		{
			int RealSize;


			bool result = WriteProcessMemory(hwnd, BaseAddress + DllBaseAddress, Buffer, Buffer.Length, out RealSize);

			if (result == false)
			{
				throw new ProcessCException("内存写错误");
			}

			//return result;
		}





		public bool VirtualProtectEx(int Address, uint Size, Protection Prot, ref uint oldProtect)
		{
			return VirtualProtectEx(this.hwnd, Address, Size, (uint)Prot, ref oldProtect);
		}

		public bool ReadMemory(IntPtr baseAddress, void* lpbuffer, int Size, out int RealSize)
		{
			bool result = ReadProcessMemory(hwnd, baseAddress, lpbuffer, Size, out RealSize);
			return result;
		}

		public bool ReadMemory(IntPtr baseAddress, void* lpbuffer, int Size)
		{
			int RealSize;
			return ReadProcessMemory(hwnd, baseAddress, lpbuffer, Size, out RealSize);
		}

		public bool ReadMemory(IntPtr baseAddress, byte[] buffer,int  Size)
		{
			int RealSize;
			return ReadProcessMemory(hwnd, baseAddress, buffer, Size, out RealSize);
		}

		public int VirtualAllocEx(int lpaddress, int size)
		{
			return VirtualAllocEx(this.hwnd, lpaddress, size, MEM_COMMIT | MEM_RESERVE, PAGE_EXECUTE_READWRITE);
		}

		public bool VirtualFreeEx(int pladdress)
		{
			return VirtualFreeEx(hwnd, new IntPtr(pladdress), 0, FreeType.Release);
		}

		public bool VirtualFreeEx(int pladdress, int Size)
		{
			return VirtualFreeEx(hwnd, new IntPtr(pladdress), Size, FreeType.Decommit);
		}

		public Module[] GetModules()
		{
			return Module.GetModules(this.hwnd);
		}

        /// <summary>
        /// 更新模块信息
        /// </summary>
        public void reflesh()
        {
            this.modules = this.GetModules();
        }

		public bool inject(string _dllname, bool hidedll)
		{
			dllname = _dllname;

			int umstrcnt = System.Text.Encoding.Default.GetByteCount(dllname);
	
			if (Pid < 0)
			{
				throw new NullReferenceException("pid错误");
			}
			if (string.IsNullOrEmpty(dllname))
			{
				throw new ProcessCException("模块名为空");
			}


			AllocBaseAddress = VirtualAllocEx(hwnd, 0, umstrcnt, MEM_COMMIT | MEM_RESERVE, PAGE_EXECUTE_READWRITE);
			if (AllocBaseAddress == 0)
			{
				throw new ProcessCException("virtualallocex  fail");
			}
			//ok=WriteProcessMemory(hwnd, AllocBaseAddress, dllname, dllname.Length + 2,0);
			IntPtr AddrWM = Marshal.StringToHGlobalAnsi(dllname);
			ok = WriteMemory(AllocBaseAddress, (int)AddrWM, umstrcnt);
			Marshal.FreeHGlobal(AddrWM);
			if (!ok)
			{
				throw new ProcessCException("writeprocessmemory fail");
			}
			loadaddr = GetProcAddress(GetModuleHandleA("kernel32.dll"), "LoadLibraryA");
			if (loadaddr == 0)
			{   //取得LoadLibraryA的地址失败时返回
				throw new ProcessCException("get loadlibraryA fail");
			}
			ThreadHwnd = CreateRemoteThread(hwnd, 0, 0, loadaddr, AllocBaseAddress, 0, 0);
			if (ThreadHwnd == IntPtr.Zero)
			{
				throw new ProcessCException("createremotethread fail");
			}
			//MessageBox.Show("ok ,you can check now!!!");
			WaitForSingleObject(ThreadHwnd, INFINITE);


			if (hidedll == true)
			{

			}
			return true;
		}

		
		
				
		private bool disposed = false;
        public void Dispose()
        {
            Dispose(true);            
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
           
            if (!this.disposed)          
            {               
                if (disposing)
                {
                    // 释放托管资源
                    this.pname.Dispose();
                }
                // 释放非托管资源，如果disposing为false, 
                // 只有非托管资源被释放
                hwnd = IntPtr.Zero;
				AllocBaseAddress = 0;
				Pid = 0;loadaddr=0;ThreadHwnd = IntPtr.Zero;DllBaseAddress = 0;
                // 注意这里不是线程安全的
            }
            disposed = true;
        }



	}










	[Serializable]
	public class ProcessCException : Exception
	{
		private readonly int errorCode;

		/// <summary>
		/// Initializes a new instance of the <see cref="ProcessCException"/> class.
		/// </summary>
		public ProcessCException()
		{
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="ProcessCException"/> class.
		/// </summary>
		/// <param name="errorCode">The error code.</param>
		public ProcessCException(int errorCode)
		{
			this.errorCode = errorCode;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ProcessCException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		public ProcessCException(string message)
			: base(message)
		{
			errorCode = Marshal.GetLastWin32Error();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ProcessCException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="errorCode">The error code.</param>
		public ProcessCException(string message, int errorCode)
			: base(message)
		{
			this.errorCode = errorCode;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ProcessCException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="innerException">The inner exception.</param>
		public ProcessCException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ProcessCException"/> class.
		/// </summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
		/// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
		/// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
		public ProcessCException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		/// <summary>
		/// Gets the error code.
		/// </summary>
		public int ErrorCode
		{
			get { return errorCode; }
		}

		/// <summary>
		/// Gets a message that describes the current exception.
		/// </summary>
		/// <returns>The error message that explains the reason for the exception, or an empty string("").</returns>
		public override string Message
		{
			get
			{
				return base.Message + " Error Code: " + errorCode;
			}

		}
	}//end of public class ProcessCException : Exception
}
