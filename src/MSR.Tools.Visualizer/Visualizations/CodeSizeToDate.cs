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
	public class CodeSizeToDate : DatePeriodVisualization
	{
		public CodeSizeToDate()
		{
			Type = VisualizationType.LINEWITHPOINTS;
			Title = "Code size to date";
		}
		public override void Calc(IRepository repository)
		{
			base.Calc(repository);

			x = new double[dates.Count()];
			y = new double[dates.Count()];
			
			double codeSize = 0;
			for (int i = 0; i < dates.Length; i++)
			{
				x[i] = (dates[i] - dates[0]).TotalDays;
				codeSize += repository.SelectionDSL()
					.Commits()
						.Reselect(e => i > 0 ? e.DateIsGreaterThan(dates[i-1]) : e)
						.DateIsLesserOrEquelThan(dates[i])
					.Files()
						.InDirectory(TargetDir)
					.Modifications()
						.InCommits()
						.InFiles()
					.CodeBlocks()
						.InModifications()
						.CalculateLOC();
				y[i] = codeSize;
			}
		}
		public override void Draw(IGraphView graph)
		{
			graph.PrepairPointsForDateScale(x, dates[0]);
			graph.YAxisTitle = "LOC";
			base.Draw(graph);
		}
	}
}
