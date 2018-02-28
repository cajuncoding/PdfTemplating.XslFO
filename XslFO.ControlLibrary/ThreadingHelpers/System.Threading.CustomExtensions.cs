/*
Copyright 2012 Brandon Bernard

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

     http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.Threading.CustomExtensions
{
	public static class SystemThreadingCustomExtensinos
	{
		/// <summary>
		/// Will cause the specified Thread to Sleep with a delay specified, but as opposed to the standard Sleep() method, 
		/// this thread will allows COM and other Windows message pumps to continue processing.  This is done by launching the Sleep
		/// call on a background thread and then waiting for it to complete by a Thread.Join call.
		/// </summary>
		/// <param name="thread"></param>
		public static void SleepJoin(this Thread thread, int millisecondsTimeout)
		{
			thread.SleepJoin(TimeSpan.FromMilliseconds(millisecondsTimeout));
		}

		/// <summary>
		/// Will cause the specified Thread to Sleep with a delay specified, but as opposed to the standard Sleep() method, 
		/// this thread will allows COM and other Windows message pumps to continue processing.  This is done by launching the Sleep
		/// call on a background thread and then waiting for it to complete by a Thread.Join call.
		/// </summary>
		/// <param name="thread"></param>
		public static void SleepJoin(this Thread thread, TimeSpan timespan)
		{
			Thread sleepThread = new Thread(new ThreadStart(() =>
			{
                Thread.Sleep((int)timespan.TotalMilliseconds);
			}));

			sleepThread.Start();
            sleepThread.Join((int)timespan.TotalMilliseconds);
		}

	}

	public static class SystemThreadingTasksCustomExtensions_dotNet4
	{
		public static TaskFactory ToTaskFactory(this TaskScheduler taskScheduler)
		{
			return taskScheduler.GetTaskFactory();
		}

		public static TaskFactory GetTaskFactory(this TaskScheduler taskScheduler)
		{
			return new TaskFactory(taskScheduler);
		}

		public static void DelayExecution(this TaskScheduler taskScheduler, int milliseconds, Action action)
		{
			taskScheduler.DelayExecution(TimeSpan.FromMilliseconds(milliseconds), action);
		}

		public static void DelayExecution(this TaskScheduler taskScheduler, TimeSpan delay, Action action)
		{
			Task.Factory.StartNew(() => 
			{
				Thread.Sleep((int)delay.TotalMilliseconds);
				//System.Threading.Timer timer = new System.Threading.Timer((ignore) =>
				//{
				//    timer.Dispose();
				//    thread.(ignore2 => action(), null);
				//}, null, delay, TimeSpan.FromMilliseconds(-1));
			})
			.ContinueWith((task) => action(), taskScheduler);
			
		}
	}

	public static class SynchronizationContextExtensions
	{
	}
}
