/*
 * 由SharpDevelop创建。
 * 用户： Administrator
 * 日期: 2010-12-22
 * 时间: 12:43
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace eflayMH_WPF
{
	/// <summary>
	/// Description of Module.
	/// </summary>
	public class Module
	{
		public Module()
		{

		}

		string fullName;

		public string FullName
		{
			get { return fullName; }
			set { fullName = value; }
		}
		int baseAddress;

		public int BaseAddress
		{
			get { return baseAddress; }
			set { baseAddress = value; }
		}

		public unsafe static Module[] GetModules(IntPtr ProcessHandle)
		{
			
			int tmpbaseaddr = 0;
			List<Module> ModuleList = new List<Module>();

			win32.MemoryBasicInformation mbi = new win32.MemoryBasicInformation();

			//win32.UNICODE_STRING usSectionName = new win32.UNICODE_STRING("");
			win32.MEMORY_SECTION_NAME usSectionName = new win32.MEMORY_SECTION_NAME();
			int dwStartAddr = 0x00000000;
			

			do
			{
				int rt1 = 0;
				if (win32.ZwQueryVirtualMemory(ProcessHandle, dwStartAddr, win32.MemoryInformationClass.MemoryBasicInformation,
				                               &mbi, Marshal.SizeOf(mbi), out rt1) >= 0)
				{
					//if ((int)mbi.AllocationBase == 0)
					//    goto JMPNEXT;



					if (mbi.lType == (int)win32.MbiType.MEM_IMAGE)
					{
						byte[] bt = new byte[260 * 2];
						int rt = 0;
						int result = win32.ZwQueryVirtualMemory(ProcessHandle, dwStartAddr,
						                                        win32.MemoryInformationClass.MemorySectionName,
						                                        out usSectionName, bt.Length,
						                                        //out usSectionName,
						                                        //Marshal.SizeOf(usSectionName),
						                                        out rt);
						if (result >= 0 
							&&    tmpbaseaddr!=(int)mbi.AllocationBase )
						{
							UnicodeEncoding une = new UnicodeEncoding();
							string path = une.GetString(usSectionName.bt).TrimEnd('\0');
							Module md = new Module();
							md.baseAddress = (int)mbi.AllocationBase;
							tmpbaseaddr = (int)mbi.AllocationBase;
							md.fullName = win32.DeviceName2Path(path);


							ModuleList.Add(md);


						}

						dwStartAddr += (int)mbi.RegionSize;
						dwStartAddr -= ((int)mbi.RegionSize % 0x10000);

					}//end of if Type == MEM_IMAGE

					//JMPNEXT:
					//   dwStartAddr += (int)mbi.RegionSize;
					//   dwStartAddr -= ((int)mbi.RegionSize % 0x10000);

				} // end of getmbi


				dwStartAddr += 0x10000;
			} while (dwStartAddr < 0x7ffeffff);

			//去除重复
//			int tmpbase = -1;
//			Module mod;
//			for (int i = 0; i < ModuleList.Count; i++)
//			{
//				mod = ModuleList[i];
//				try
//				{
//					while (tmpbase == mod.baseAddress)
//					{
//						ModuleList.RemoveAt(i);
//
//						mod = ModuleList[i];
//
//					}
//				}
//				catch (ArgumentOutOfRangeException)
//				{ break; }
//				tmpbase = mod.baseAddress;
//
//			}
			return ModuleList.ToArray();
		}


	}
}
