/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Linq;

using MSR.Data;
using MSR.Data.Entities;
using MSR.Data.Entities.DSL.Selection;
using MSR.Data.Entities.DSL.Selection.Metrics;

namespace MSR.Tools.Visualizer.Visualizations
{
	public class RemovedToAddedCode : Visualization
	{
		public RemovedToAddedCode()
		{
			Title = "Removed to added code probability density";
			Intervals = 10;
			OnSpecifiedInterval = false;
			IntervalFrom = 0;
			IntervalTo = 0;
		}
		public override void Calc(IRepositoryResolver repositories)
		{
			var commits = repositories.Repository<Commit>()
				.Select(c => new
				{
					Revision = c.Revision,
					Message = c.Message
				}).ToList();

			List<double> deletedToAdded = new List<double>();

			double added,deleted;
			foreach (var c in commits)
			{
				var code = repositories.SelectionDSL()
					.Commits().RevisionIs(c.Revision)
					.Modifications().InCommits()
					.CodeBlocks().InModifications().Fixed();
				
				added = code.Added().CalculateLOC();
				if (added > 0)
				{
					deleted = - code.Deleted().CalculateLOC();
					deletedToAdded.Add(deleted / added);
				}
			}
			if (! OnSpecifiedInterval)
			{
				IntervalFrom = deletedToAdded.Min();
				IntervalTo = deletedToAdded.Max();
			}
			else
			{
				List<double> deletedToAddedNew = new List<double>();
				foreach (var dta in deletedToAdded)
				{
					if ((dta >= IntervalFrom) && (dta <= IntervalTo))
					{
						deletedToAddedNew.Add(dta);
					}
				}
				deletedToAdded = deletedToAddedNew;
			}

			x = new double[Intervals];
			y = new double[Intervals];
			
			double delta = (IntervalTo - IntervalFrom) / Intervals;
			double nextX = IntervalFrom;
			for (int i = 0; i < Intervals; i++)
			{
				x[i] = nextX;
				y[i] = (double)deletedToAdded.Where(dta => dta >= nextX && dta < nextX+delta).Count() / deletedToAdded.Count;
				nextX += delta;
			}
		}
		public override void Draw(IGraphView graph)
		{
			graph.Title = Title;
			graph.XAxisTitle = "Removed to added code";
			graph.YAxisTitle = "Probability";
			graph.ShowHistogram("", x, y);
		}
		public int Intervals
		{
			get; set;
		}
		public bool OnSpecifiedInterval
		{
			get; set;
		}
		public double IntervalFrom
		{
			get; set;
		}
		public double IntervalTo
		{
			get; set;
		}
	}
}
