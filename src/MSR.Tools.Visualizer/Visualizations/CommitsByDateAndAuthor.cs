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
	public class CommitsByDateAndAuthor : Visualization
	{
		Dictionary<string,double[]> yByAuthor;
		
		public CommitsByDateAndAuthor()
		{
			Type = VisualizationType.LINE;
			Title = "Commits by date and author";
			DaysPerStep = 30;
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
			
			x = new double[dates.Count()-1];
			yByAuthor = new Dictionary<string,double[]>();
			for (int i = 0; i < Authors.Length; i++)
			{
				yByAuthor.Add(Authors[i], new double[dates.Count()-1]);
			}
			
			for (int i = 0; i < Authors.Length; i++)
			{
				y = yByAuthor[Authors[i]];
				DateTime prev = dates.First();
				int counter = 0;
				
				foreach (var date in dates)
				{
					if (counter >= 30)
					{
						Console.WriteLine();
					}
					if (date == prev)
					{
						continue;
					}
					if (i == 0)
					{
						x[counter] = (date - dates.First()).TotalDays;
					}
					y[counter] = repositories.SelectionDSL().Commits()
						.DateIsGreaterOrEquelThan(prev)
						.DateIsLesserThan(date)
						.AuthorIs(Authors[i])
						.Count();
					
					prev = date;
					counter++;
				}
			}
		}
		public override void Draw(IGraphView graph)
		{
			graph.XAxisTitle = "Days";
			graph.YAxisTitle = "Commits";
			foreach (var y in yByAuthor)
			{
				Legend = y.Key;
				this.y = y.Value;
				base.Draw(graph);
			}
		}
		[DescriptionAttribute("Days per step")]
		public int DaysPerStep
		{
			get; set;
		}
		[DescriptionAttribute("Author list")]
		public string[] Authors
		{
			get; set;
		}
		protected IEnumerable<DateTime> GetDates(IRepositoryResolver repositories)
		{
			int daysPerStep = Convert.ToInt32(DaysPerStep);

			DateTime min = repositories.Repository<Commit>().Min(c => c.Date);
			DateTime max = repositories.Repository<Commit>().Max(c => c.Date);
			
			DateTime date = min;
			while (date <= max)
			{
				yield return date;
				date = date.AddDays(daysPerStep);
			}
		}
	}
}
