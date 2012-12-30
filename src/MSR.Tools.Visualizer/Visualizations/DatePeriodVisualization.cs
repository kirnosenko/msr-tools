/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using MSR.Data;
using MSR.Data.Entities;
using MSR.Data.Entities.DSL.Selection;
using MSR.Data.Entities.DSL.Selection.Metrics;

namespace MSR.Tools.Visualizer.Visualizations
{
	public enum DatePeriod
	{
		DAY,
		WEEK,
		MONTH,
		QUARTER,
		YEAR
	}

	public class DatePeriodConvertor : TypeConverter
	{
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			return new StandardValuesCollection(new DatePeriod[]
			{
				DatePeriod.DAY, DatePeriod.WEEK, DatePeriod.MONTH, DatePeriod.QUARTER, DatePeriod.YEAR
			});
		}
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			return true;
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			switch ((string)value)
			{
				case "DAY": return DatePeriod.DAY;
				case "WEEK": return DatePeriod.WEEK;
				case "MONTH": return DatePeriod.MONTH;
				case "QUARTER": return DatePeriod.QUARTER;
				case "YEAR": return DatePeriod.YEAR;
				default: return null;
			}
		}
	}
	
	public abstract class DatePeriodVisualization : Visualization
	{
		protected DateTime[] dates;
		
		public DatePeriodVisualization()
		{
			DatePeriod = DatePeriod.MONTH;
		}
		public override void Calc(IRepository repository)
		{
			dates = GetDates(repository);
		}
		public override void Draw(IGraphView graph)
		{
			switch (DatePeriod)
			{
				case DatePeriod.DAY:
					graph.XAxisTitle = "Days";
					break;
				case DatePeriod.WEEK:
					graph.XAxisTitle = "Weeks";
					break;
				case DatePeriod.MONTH:
					graph.XAxisTitle = "Months";
					break;
				case DatePeriod.QUARTER:
					graph.XAxisTitle = "Quarters";
					break;
				case DatePeriod.YEAR:
					graph.XAxisTitle = "Years";
					break;
			}
			graph.XAxisDayScale = true;
			graph.XAxisFontAngle = 90;
			
			base.Draw(graph);
		}
		[TypeConverter(typeof(DatePeriodConvertor)), DescriptionAttribute("Date period"), DefaultValue(DatePeriod.MONTH)]
		public DatePeriod DatePeriod
		{
			get; set;
		}
		protected DateTime[] GetDates(IRepository repository)
		{
			List<DateTime> dates = new List<DateTime>();

			DateTime min = repository.Queryable<Commit>().Min(c => c.Date);
			DateTime max = repository.Queryable<Commit>().Max(c => c.Date);

			DateTime date = min;
			switch (DatePeriod)
			{
				case DatePeriod.DAY: date = date.StartOfDay(); break;
				case DatePeriod.WEEK: date = date.StartOfWeek(); break;
				case DatePeriod.MONTH: date = date.StartOfMonth(); break;
				case DatePeriod.QUARTER: date = date.StartOfQuarter(); break;
				case DatePeriod.YEAR: date = date.StartOfYear(); break;
				default: break;
			}

			DateTime prevDate = date;
			while (prevDate < max)
			{
				dates.Add(date);
				prevDate = date;
				switch (DatePeriod)
				{
					case DatePeriod.DAY: date = date.AddDays(1); break;
					case DatePeriod.WEEK: date = date.AddWeeks(1); break;
					case DatePeriod.MONTH: date = date.AddMonths(1); break;
					case DatePeriod.QUARTER: date = date.AddQuarters(1); break;
					case DatePeriod.YEAR: date = date.AddYears(1); break;
					default: break;
				}
			}
			
			if (dates.Count > 0 && dates.First() < min)
			{
				dates[0] = min;
			}
			if (dates.Count > 1 && dates.Last() > max)
			{
				dates[dates.Count-1] = max;
			}

			return dates.ToArray();
		}
	}
}
