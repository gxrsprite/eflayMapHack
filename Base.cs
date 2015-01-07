/*
 * Created by SharpDevelop.
 * User: Administrator
 * Date: 2011-1-19
 * Time: 16:38
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
//using System;
//using System.Collections;
//using System.Collections.Generic;

namespace eflayMH_WPF
{
	/// <summary>
	/// 扩展方法
	/// </summary>
	public static class Extension
	{
		/// <summary>
		/// 比较两个数组内容是否相等
		/// </summary>
		/// <param name="b1">数组1</param>
		/// <param name="b2">数组2</param>
		/// <returns>是否相等</returns>
        //public static bool Equals <T> (T[] b1, T[] b2)
        //{
        //    if (b1.Length != b2.Length) return false;
        //    if (b1 == null || b2 == null) return false;
			
        //    for (int i = 0; i < b1.Length; i++)
        //        if (!b1[i].Equals( b2[i]))
        //            return false;
			
        //    return true;
        //}

        public static bool ArrayEquals( byte[] b1, byte[] b2)
        {
            if (b1.Length != b2.Length) return false;
            if (b1 == null || b2 == null) return false;

            for (int i = 0; i < b1.Length; i++)
                if (!b1[i].Equals(b2[i]))
                    return false;

            return true;
        }
		
		// 在 s 中查找 pattern 。
		// 如果找到，返回 pattern 在 s 中第一次出现的位置(0起始)。
		// 如果没找到，返回 -1。
		//static int IndexOf<T>(T[] s, T[] pattern)
        static int IndexOf(byte[] s, byte[] pattern)
		{
			int slen = s.Length;
			int plen = pattern.Length;
			for (int i = 0; i <= slen - plen; i++)
			{
				for (int j = 0; j < plen; j++)
				{
					if (!s[i + j].Equals(pattern[j])) goto next;
				}
				return i;
				next:;
			}
			return -1;
		}
		
		
	}
}
