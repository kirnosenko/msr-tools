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
	public class FileSizeDistribution : Distribution
	{
		public FileSizeDistribution()
		{
			Title = "File size distribution";
		}
		public override void Draw(IGraphView graph)
		{
			graph.XAxisTitle = "File size in LOC";
			base.Draw(graph);
		}
		protected override double[] DistributionData(IRepositoryResolver repositories)
		{			
			var fileIDs = repositories.SelectionDSL()
				.Files()
					.InDirectory(TargetDir)
					.Exist()
				.Select(x => x.ID).ToArray();

			List<double> data = new List<double>(fileIDs.Count());
			
			foreach (var fileID in fileIDs)
			{
				data.Add(repositories.SelectionDSL()
					.Files().IdIs(fileID)
					.Modifications().InFiles()
					.CodeBlocks().InModifications().CalculateLOC()
				);
			}
			
			return data.ToArray();
		}
	}
}
