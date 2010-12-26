/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
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
	public class DefectDensityToFileSize : IVisualization
	{
		public void Visualize(IDataStore data, IGraphView graph)
		{
			using (var s = data.OpenSession())
			{
				var fileIDs = s.SelectionDSL().Files()
					.Exist()
					.Select(f => f.ID)
					.ToList();

				double[] x = new double[fileIDs.Count()];
				double[] y = new double[fileIDs.Count()];
				int i = 0;
				foreach (var fileID in fileIDs)
				{
					var code = s.SelectionDSL().Files()
						.IdIs(fileID)
						.Modifications().InFiles()
						.CodeBlocks().InModifications();
					
					x[i] = code.CalculateLOC();
					y[i] = code.CalculateTraditionalDefectDensity();
					i++;
				}
				
				graph.Title = "Defect density to file size";
				graph.XAxisTitle = "File size (LOC)";
				graph.YAxisTitle = "Defect density (defects per KLOC)";
				graph.ShowPoints("", x, y);
			}
		}
	}
}
