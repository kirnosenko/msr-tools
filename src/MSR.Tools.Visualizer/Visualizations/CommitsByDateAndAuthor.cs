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
	public class CommitsByDateAndAuthor : DatePeriodVisualization
	{
		private Dictionary<string,double[]> yByAuthor;
		
		public CommitsByDateAndAuthor()
		{
			Type = VisualizationType.LINEWITHPOINTS;
			Title = "Commits by date and author";
		}
		public override void Init(IRepository repository)
		{
			Authors = repository.Queryable<Commit>()
				.Select(x => x.Author).Distinct().OrderBy(x => x)
				.ToArray();
			base.Init(repository);
		}
		public override void Calc(IRepository repository)
		{
			base.Calc(repository);
			
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
					y[counter] = repository.SelectionDSL().Commits()
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
			graph.PrepairPointsForDateScale(x, dates[0]);
			graph.YAxisTitle = "Commits";
			foreach (var y in yByAuthor)
			{
				Legend = y.Key;
				this.y = y.Value;
				base.Draw(graph);
			}
		}
		[DescriptionAttribute("Author list")]
		public string[] Authors
		{
			get; set;
		}
	}
}
