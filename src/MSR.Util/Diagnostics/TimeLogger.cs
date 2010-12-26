/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

namespace System.Diagnostics
{
	public class TimeLogger : IDisposable
	{
		private Stopwatch timer;
		private string taskTitle;
		private Action<TimeLogger> finalAction;

		public TimeLogger(string taskTitle, Action<TimeLogger> finalAction)
		{
			timer = Stopwatch.StartNew();
			this.taskTitle = taskTitle;
			this.finalAction = finalAction;
		}
		public void Dispose()
		{
			if (finalAction != null)
			{
				finalAction(this);
			}
		}
		public string FormatedTime
		{
			get
			{
				return string.Format(
					"{0}: {1}",
					taskTitle,
					timer.Elapsed.ToFormatedString()
				);
			}
		}
	}
}
