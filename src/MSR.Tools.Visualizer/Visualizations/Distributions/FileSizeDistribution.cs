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
		protected override double[] DistributionData(IRepository repository)
		{			
			var fileIDs = repository.SelectionDSL()
				.Files()
					.InDirectory(TargetDir)
					.Exist()
				.Select(x => x.ID).ToArray();

			List<double> locData = new List<double>(fileIDs.Count());
			
			foreach (var fileID in fileIDs)
			{
				locData.Add(repository.SelectionDSL()
					.Files().IdIs(fileID)
					.Modifications().InFiles()
					.CodeBlocks().InModifications().CalculateLOC()
				);
			}
			
			return locData.ToArray();
		}
	}
}
