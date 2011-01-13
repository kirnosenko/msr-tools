/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
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
	public class BugLifeTimeDistribution : Visualization
	{
		public BugLifeTimeDistribution()
		{
			Title = "Bug lifetime distribution";
		}
		public override void Calc(IRepositoryResolver repositories)
		{
			var bugLiveTimes = repositories.SelectionDSL()
				.BugFixes().CalculateMaxBugLifetime();

			x = new double[bugLiveTimes.Count()];
			y = new double[bugLiveTimes.Count()];
			int i = 0;
			foreach (var bugLiveTime in bugLiveTimes)
			{
				x[i] = bugLiveTime;
				y[i] = bugLiveTimes.Where(t => t <= bugLiveTime).Count();
				i++;
			}
		}
		public override void Draw(IGraphView graph)
		{
			graph.Title = "Bug lifetime distribution";
			graph.XAxisTitle = "Days";
			graph.YAxisTitle = "Total number of fixed bugs";
			graph.ShowPoints("", x, y);
		}
	}
}
