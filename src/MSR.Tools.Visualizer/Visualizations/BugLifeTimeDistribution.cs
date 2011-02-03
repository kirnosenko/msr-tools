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
			var bugLifetimes = repositories.SelectionDSL()
				.BugFixes()
				.CalculateAvarageBugLifetime();
			
			x = new double[bugLifetimes.Count()];
			y = new double[bugLifetimes.Count()];
			int i = 0;
			
			foreach (var bugLifetime in bugLifetimes)
			{
				x[i] = bugLifetime;
				y[i] = (double)bugLifetimes.Where(t => t <= bugLifetime).Count() / bugLifetimes.Count();
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
		public override bool Configurable
		{
			get { return false; }
		}
	}
}
