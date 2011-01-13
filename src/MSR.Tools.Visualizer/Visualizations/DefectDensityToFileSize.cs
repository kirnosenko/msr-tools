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
		public override void Calc(IRepositoryResolver repositories)
		{
			var fileIDs = repositories.SelectionDSL()
				.Files().Exist()
				.Select(f => f.ID).ToList();

			List<double> xlist = new List<double>(fileIDs.Count);
			List<double> ylist = new List<double>(fileIDs.Count);

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
					xlist.Add(code.CalculateLOC());
					ylist.Add(dd);
				}
			}
			
			x = xlist.ToArray();
			y = ylist.ToArray();
		}
		public override void Draw(IGraphView graph)
		{
			graph.Title = "Defect density to file size";
			graph.XAxisTitle = "File size (LOC)";
			graph.YAxisTitle = "Defect density (defects per KLOC)";
			graph.ShowPoints("", x, y);
		}
	}
}
