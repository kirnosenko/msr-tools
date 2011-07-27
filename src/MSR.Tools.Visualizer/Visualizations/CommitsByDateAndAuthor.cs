/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
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
	
	public class CommitsByDateAndAuthor : Visualization
	{
		private Dictionary<string,double[]> yByAuthor;
		private DateTime startDate;
		
		public CommitsByDateAndAuthor()
		{
			Type = VisualizationType.LINEWITHPOINTS;
			Title = "Commits by date and author";
			DatePeriod = DatePeriod.MONTH;
		}
		public override void Init(IRepositoryResolver repositories)
		{
			Authors = repositories.Repository<Commit>()
				.Select(x => x.Author).Distinct().OrderBy(x => x)
				.ToArray();
			base.Init(repositories);
		}
		public override void Calc(IRepositoryResolver repositories)
		{
			var dates = GetDates(repositories);
			startDate = dates[0];
			
			x = new double[dates.Count()-1];
			yByAuthor = new Dictionary<string,double[]>();
			for (int i = 0; i < Authors.Length; i++)
			{
				yByAuthor.Add(Authors[i], new double[dates.Count()-1]);
			}
			
			for (int i = 0; i < Authors.Length; i++)
			{
				y = yByAuthor[Authors[i]];
				DateTime prev = dates[0];
				int counter = 0;

				for (int j = 1; j < dates.Length; j++)
				{
					if (i == 0)
					{
						x[counter] = (dates[j] - dates[0]).TotalDays;
					}
					y[counter] = repositories.SelectionDSL().Commits()
						.DateIsGreaterOrEquelThan(prev)
						.DateIsLesserThan(dates[j])
						.AuthorIs(Authors[i])
						.Count();
					
					prev = dates[j];
					counter++;
				}
			}
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
			graph.PrepairPointsForDateScale(x, startDate);
			graph.YAxisTitle = "Commits";
			foreach (var y in yByAuthor)
			{
				Legend = y.Key;
				this.y = y.Value;
				base.Draw(graph);
			}
		}
		[TypeConverter(typeof(DatePeriodConvertor)), DescriptionAttribute("Date period"), DefaultValue(DatePeriod.MONTH)]
		public DatePeriod DatePeriod
		{
			get; set;
		}
		[DescriptionAttribute("Author list")]
		public string[] Authors
		{
			get; set;
		}
		protected DateTime[] GetDates(IRepositoryResolver repositories)
		{
			List<DateTime> dates = new List<DateTime>();
			
			DateTime min = repositories.Repository<Commit>().Min(c => c.Date);
			DateTime max = repositories.Repository<Commit>().Max(c => c.Date);
			
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
				if (date > max)
				{
					date = max;
				}
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
			
			return dates.ToArray();
		}
	}
}
