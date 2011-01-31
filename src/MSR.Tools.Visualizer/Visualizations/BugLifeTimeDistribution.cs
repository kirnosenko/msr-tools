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
			var distribution = repositories.SelectionDSL()
				.BugFixes()
				.CalculateBugLifetimeDistribution(b => b.CalculateAvarageBugLifetime());
			
			x = new double[distribution.Count];
			y = new double[distribution.Count];
			int i = 0;
			
			foreach (var point in distribution)
			{
				x[i] = point.Key;
				y[i] = point.Value;
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
