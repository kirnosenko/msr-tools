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
	public class DefectDensityToFileSize : Visualization
	{
		public DefectDensityToFileSize()
		{
			Title = "Defect density to file size";
		}
		public override void Visualize(IRepositoryResolver repositories, IGraphView graph)
		{
			var fileIDs = repositories.SelectionDSL()
				.Files().Exist()
				.Select(f => f.ID).ToList();
			
			List<double> x = new List<double>(fileIDs.Count);
			List<double> y = new List<double>(fileIDs.Count);
			
			foreach (var fileID in fileIDs)
			{
				var code = repositories.SelectionDSL()
					.Files().IdIs(fileID)
					.Modifications().InFiles()
					.CodeBlocks().InModifications()
					.Fixed();
				
				var dd = code.CalculateTraditionalDefectDensity();
				if (dd > 0)
				{
					x.Add(code.CalculateLOC());
					y.Add(dd);
				}
			}
			
			graph.Title = "Defect density to file size";
			graph.XAxisTitle = "File size (LOC)";
			graph.YAxisTitle = "Defect density (defects per KLOC)";
			graph.ShowPoints("", x.ToArray(), y.ToArray());
		}
	}
}
