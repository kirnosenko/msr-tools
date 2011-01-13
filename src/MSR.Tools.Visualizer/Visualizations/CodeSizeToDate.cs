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
using MSR.Tools.Visualizer.VisualizationOptions;

namespace MSR.Tools.Visualizer.Visualizations
{
	public class CodeSizeToDate : Visualization
	{
		public CodeSizeToDate()
		{
			Title = "Code size to date";
			DaysPerStep = "30";
		}
		public override void Visualize(IRepositoryResolver repositories, IGraphView graph)
		{
			int daysPerStep = Convert.ToInt32(DaysPerStep);
			
			DateTime min = repositories.Repository<Commit>().Min(c => c.Date);
			DateTime max = repositories.Repository<Commit>().Max(c => c.Date);
			double[] x = new double[(max - min).Days / daysPerStep + 1];
			double[] y = new double[x.Length];
			
			DateTime from = min;
			DateTime to = from.AddDays(daysPerStep);
			int i = 0;
			double totalCodeSize = 0;
			while (from <= max)
			{
				x[i] = (from - min).Days;
				totalCodeSize += repositories.SelectionDSL()
					.Commits().DateIsGreaterOrEquelThan(from).DateIsLesserThan(to)
					.Modifications().InCommits()
					.CodeBlocks().InModifications().CalculateLOC();
				y[i] = totalCodeSize;
					
				from = from.AddDays(daysPerStep);
				to = to.AddDays(daysPerStep);
				i++;
			}

			graph.Title = "Code size to date";
			graph.XAxisTitle = "Days";
			graph.YAxisTitle = "LOC";
			graph.ShowPoints("", x, y);
		}
		[VisualizationSelectionOption("Days per step", Values = new string[] { "1", "7", "30" })]
		public string DaysPerStep
		{
			get; set;
		}
	}
}
