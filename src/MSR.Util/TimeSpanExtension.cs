/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;

namespace System
{
	public static class TimeSpanExtension
	{
		public static string ToFormatedString(this TimeSpan span)
		{
			return
				span.Hours.ToString("D2") + ":" +
				span.Minutes.ToString("D2") + ":" +
				span.Seconds.ToString("D2") + ":" +
				span.Milliseconds.ToString("D3");
		}
	}
}
