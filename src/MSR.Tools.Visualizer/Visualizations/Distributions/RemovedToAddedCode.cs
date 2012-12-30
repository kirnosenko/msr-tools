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
	public class RemovedToAddedCode : Distribution
	{
		public RemovedToAddedCode()
		{
			Title = "Removed to added code in commit distribution";
		}
		public override void Draw(IGraphView graph)
		{
			graph.XAxisTitle = "Removed to added code";
			base.Draw(graph);
		}
		protected override double[] DistributionData(IRepository repository)
		{
			var commits = repository.Queryable<Commit>()
				.Select(c => new
				{
					Revision = c.Revision,
					Message = c.Message
				}).ToArray();

			List<double> deletedToAdded = new List<double>();
			double added, deleted;
			
			foreach (var c in commits)
			{
				var code = repository.SelectionDSL()
					.Commits()
						.RevisionIs(c.Revision)
					.Modifications()
						.InCommits()
					.CodeBlocks()
						.InModifications().Fixed();

				added = code.Added().CalculateLOC();
				if (added > 0)
				{
					deleted = -code.Deleted().CalculateLOC();
					deletedToAdded.Add(deleted / added);
				}
			}
			
			return deletedToAdded.ToArray();
		}
	}
}
