/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using MSR.Data;
using MSR.Data.Entities;
using MSR.Data.Entities.DSL.Selection;
using MSR.Data.Entities.DSL.Selection.Metrics;

namespace MSR.Tools.Visualizer.Visualizations
{
	public class DefectDensityToCommitSize : Visualization
	{
		public DefectDensityToCommitSize()
		{
			Title = "Defect density to commit size";
		}
		public override void Calc(IRepository repository)
		{
			var revisions = repository.SelectionDSL()
				.Commits()
				.Select(c => c.Revision)
				.ToList();
				
			List<double> xlist = new List<double>(revisions.Count);
			List<double> ylist = new List<double>(revisions.Count);

			foreach (var revision in revisions)
			{
				var code = repository.SelectionDSL()
					.Commits().RevisionIs(revision)
					.Files().InDirectory(TargetDir)
					.Modifications().InCommits().InFiles()
					.CodeBlocks().InModifications()
					.Fixed();
				
				var addedLoc = code.Added().CalculateLOC();
				if (addedLoc == 0)
				{
					continue;
				}
				var dd = code.CalculateTraditionalDefectDensity();
				if (dd == 0)
				{
					continue;
				}
				xlist.Add(addedLoc);
				ylist.Add(dd);
			}
			
			x = xlist.ToArray();
			y = ylist.ToArray();
		}
		public override void Draw(IGraphView graph)
		{
			graph.XAxisTitle = "Commit size (LOC)";
			graph.YAxisTitle = "Defect density (defects per KLOC)";
			base.Draw(graph);
		}
	}
}
