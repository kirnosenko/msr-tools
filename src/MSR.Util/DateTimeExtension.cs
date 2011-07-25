/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;

namespace System
{
	public static class DateTimeExtension
	{
		public static DateTime StartOfDay(this DateTime date)
		{
			return new DateTime(date.Year, date.Month, date.Day);
		}
		public static DateTime StartOfWeek(this DateTime date)
		{
			return date.AddDays((int)date.DayOfWeek);
		}
		public static DateTime StartOfMonth(this DateTime date)
		{
			return new DateTime(date.Year, date.Month, 1);
		}
		public static DateTime StartOfYear(this DateTime date)
		{
			return new DateTime(date.Year, 1, 1);
		}
		public static DateTime AddWeeks(this DateTime date, int weeks)
		{
			return date.AddDays(weeks * 7);
		}
	}
}
