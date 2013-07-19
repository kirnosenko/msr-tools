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
	public class FileDefectDensityDistribution : Distribution
	{
		public FileDefectDensityDistribution()
		{
			Title = "File defect density distribution";
		}
		public override void Draw(IGraphView graph)
		{
			graph.XAxisTitle = "File defect density";
			base.Draw(graph);
		}
		protected override double[] DistributionData(IRepository repository)
		{			
			var fileIDs = repository.SelectionDSL()
				.Files()
					.InDirectory(TargetDir)
					.Exist()
				.Select(x => x.ID).ToArray();

			List<double> ddData = new List<double>(fileIDs.Count());
			
			foreach (var fileID in fileIDs)
			{
				ddData.Add(repository.SelectionDSL()
					.Files().IdIs(fileID)
					.Modifications().InFiles()
					.CodeBlocks().InModifications().CalculateDefectDensity()
				);
			}
			
			return ddData.ToArray();
		}
	}
}
