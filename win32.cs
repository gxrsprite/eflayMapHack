using System;
using System.Windows;
using System.Windows.Documents;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace eflayMH_WPF
{
	unsafe class win32
	{
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern int QueryDosDevice(string lpDeviceName, StringBuilder lpTargetPath, int ucchMax);

		static public Dictionary<string, string> DeviceNameAndPath = new Dictionary<string, string>();
		
		public static string DeviceName2Path(string sbProcImagePath)
		{
			int iRet;
			string strImageFilePath = "";
			if (sbProcImagePath.Length > 0)
			{
				int iDeviceIndex = sbProcImagePath.ToString().IndexOf("\\", "\\Device\\HarddiskVolume".Length);
				string strDevicePath = sbProcImagePath.ToString().Substring(0, iDeviceIndex);
				
				string DiskName = "";
				if(DeviceNameAndPath.TryGetValue(strDevicePath,out DiskName))
				{
					return DiskName+ ":"+sbProcImagePath.ToString().Replace(strDevicePath, "");
				}
				
				int iStartDisk = (int)'A';
				while (iStartDisk <= (int)'Z')
				{
					StringBuilder sbWindowImagePath = new StringBuilder(256);
					iRet = QueryDosDevice(((char)iStartDisk).ToString() + ":", sbWindowImagePath, sbWindowImagePath.Capacity);
					if (iRet != 0)
					{
						
						if (sbWindowImagePath.ToString() == strDevicePath)
						{
							DeviceNameAndPath.Add(strDevicePath,((char)iStartDisk).ToString());
							strImageFilePath = ((char)iStartDisk).ToString() + ":" + sbProcImagePath.ToString().Replace(strDevicePath, "");
							break;
						}
					}
					iStartDisk++;
				}
				
			}
			return strImageFilePath;
		}
		
		[DllImport("ntdll.dll", SetLastError = true)]
		public extern static int ZwQueryVirtualMemory(IntPtr ProcessHandle,int BaseAddress,MemoryInformationClass _MemoryInformationClass,IntPtr MemoryInformation,Int32 MemoryInformationLength,out int ReturnLenth);
		
		[DllImport("ntdll.dll", SetLastError = true)]
		public extern static int ZwQueryVirtualMemory(IntPtr ProcessHandle,int BaseAddress,MemoryInformationClass _MemoryInformationClass,[Out] void* mbi,Int32 MemoryInformationLength,out int Zero);
		[DllImport("ntdll.dll", SetLastError = true)]
		public extern static int ZwQueryVirtualMemory(IntPtr ProcessHandle,int BaseAddress,MemoryInformationClass _MemoryInformationClass,[Out] out MEMORY_SECTION_NAME mbi,Int32 MemoryInformationLength,out int Zero);
		[DllImport("ntdll.dll", SetLastError = true)]
		public extern static int ZwQueryVirtualMemory(IntPtr ProcessHandle,int BaseAddress,MemoryInformationClass _MemoryInformationClass,[Out] byte[] MSN,Int32 MemoryInformationLength,out int Zero);

		
		
		public enum MbiType
		{
			MEM_IMAGE = 0x1000000,MEM_MAPPED = 0x40000,MEM_PRIVATE = 0x20000
		}
		
		public struct   MemoryBasicInformation
		{
			public IntPtr BaseAddress;
			public IntPtr AllocationBase;
			public UInt32 AllocationProtect;
			public IntPtr RegionSize;
			public UInt32 State;
			public UInt32 Protect;
			public UInt32 lType;
		}
		
		
		public  enum MemoryInformationClass
		{
			MemoryBasicInformation,
			MemoryWorkingSetList,
			MemorySectionName,
			MemoryBasicVlmInformation
		}
		
		[StructLayout(LayoutKind.Sequential)]
		public struct MEMORY_SECTION_NAME
		{
			public UNICODE_STRING usstring;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 520)]
            public byte[] bt;

			
		}
		
		[StructLayout(LayoutKind.Sequential)]
		public struct UNICODE_STRING2
		{
			public ushort Length;
			public ushort MaximumLength;
			public int buffer;

		}
		
		
		[StructLayout(LayoutKind.Sequential)]
		public struct UNICODE_STRING : IDisposable
		{
			public ushort Length;
			public ushort MaximumLength;
			public IntPtr buffer;

			
			public UNICODE_STRING(string s)
			{
				Length = (ushort)(s.Length * 2);
				MaximumLength = (ushort)(Length + 2);
				buffer = Marshal.StringToHGlobalUni(s);
			}

			public void Dispose()
			{
				Marshal.FreeHGlobal(buffer);
				buffer = IntPtr.Zero;
			}

			public override string ToString()
			{
				return Marshal.PtrToStringUni(buffer);
			}
		}

		
		

	}//end of class win32
}//end of namespace eflayMH_WPF
