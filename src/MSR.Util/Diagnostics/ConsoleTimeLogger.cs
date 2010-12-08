/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

namespace System.Diagnostics
{
	public class ConsoleTimeLogger : TimeLogger
	{
		public static ConsoleTimeLogger Start(string taskTitle)
		{
			return new ConsoleTimeLogger(taskTitle);
		}
		
		public ConsoleTimeLogger(string taskTitle)
			: base(
				taskTitle,
				x => Console.WriteLine(x.FormatedTime)
			)
		{
		}
	}
}
