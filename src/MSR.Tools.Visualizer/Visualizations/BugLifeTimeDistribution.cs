/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
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
	public class BugLifeTimeDistribution : IVisualization
	{
		public void Visualize(IDataStore data, IGraphView graph)
		{
			using (var s = data.OpenSession())
			{
				var bugLiveTimes = s.SelectionDSL()
					.BugFixes().CalculateMaxBugLifetime();

				double[] x = new double[bugLiveTimes.Count()];
				double[] y = new double[bugLiveTimes.Count()];
				int i = 0;
				foreach (var bugLiveTime in bugLiveTimes)
				{
					x[i] = bugLiveTime;
					y[i] = bugLiveTimes.Where(t => t <= bugLiveTime).Count();
					i++;
				}
				graph.ShowPoints(x, y);
			}
		}
	}
}
