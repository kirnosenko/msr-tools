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

namespace MSR.Tools.Visualizer.Visualizations.Distributions
{
	public class BugLifeTimeDistribution : Distribution
	{
		public BugLifeTimeDistribution()
		{
			Title = "Bug lifetime distribution";
		}
		public override void Draw(IGraphView graph)
		{
			graph.XAxisTitle = "Days";
			base.Draw(graph);
		}
		protected override double[] DistributionData(IRepository repository)
		{
			return repository.SelectionDSL()
				.BugFixes().CalculateAvarageBugLifetime().ToArray();
		}
	}
}
