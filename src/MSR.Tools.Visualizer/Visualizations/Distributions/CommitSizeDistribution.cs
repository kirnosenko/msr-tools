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

namespace MSR.Tools.Visualizer.Visualizations.Distributions
{
	public class CommitSizeDistribution : Distribution
	{
		public CommitSizeDistribution()
		{
			Title = "Commit size distribution";
		}
		public override void Draw(IGraphView graph)
		{
			graph.XAxisTitle = "Commit size in LOC";
			base.Draw(graph);
		}
		protected override double[] DistributionData(IRepository repository)
		{
			var commits = repository.SelectionDSL()
				.Commits()
				.Select(x => x.Revision).ToArray();

			List<double> locData = new List<double>(commits.Count());
			
			foreach (var commit in commits)
			{
				locData.Add(repository.SelectionDSL()
					.Commits().RevisionIs(commit)
					.Modifications().InCommits()
					.CodeBlocks().InModifications().Added().CalculateLOC()
				);
			}
			
			return locData.ToArray();
		}
	}
}
