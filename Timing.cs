/*
 * Created by SharpDevelop.
 * User: Administrator
 * Date: 2011-1-17
 * Time: 14:26
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Diagnostics;
using System.Threading;


namespace eflayMH_WPF
{
	/// <summary>
	/// 时间测试
	/// </summary>
	public class Timing
	{
		TimeSpan statingTime;
		TimeSpan duration;
		
		public Timing()
		{
			statingTime = new TimeSpan(0);
			duration = new TimeSpan(0);	
		}
		
		public void StopTime()
		{
			duration = Process.GetCurrentProcess().Threads[0].UserProcessorTime.Subtract(statingTime);
		}
		public void startTime()
		{
			GC.Collect();
			GC.WaitForPendingFinalizers();
			statingTime = Process.GetCurrentProcess().Threads[0].UserProcessorTime;
		}
		public TimeSpan Result()
		{
			return duration;
		}
	}
}
