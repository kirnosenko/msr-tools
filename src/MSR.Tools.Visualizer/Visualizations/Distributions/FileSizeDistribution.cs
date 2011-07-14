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
			graph.Title = Title;
			graph.XAxisTitle = "File size in LOC";
			graph.YAxisTitle = "Probability";
			graph.ShowPoints("", x, y);
		}
		public override bool Configurable
		{
			get { return false; }
		}
		protected override IEnumerable<double> DistributionData(IRepositoryResolver repositories)
		{			
			var fileIDs = repositories.SelectionDSL()
				.Files().Exist().Select(x => x.ID).ToArray();

			List<double> data = new List<double>(fileIDs.Count());
			
			foreach (var fileID in fileIDs)
			{
				data.Add(repositories.SelectionDSL()
					.Files().IdIs(fileID)
					.Modifications().InFiles()
					.CodeBlocks().InModifications().CalculateLOC()
				);
			}
			
			return data;
		}
	}
}
