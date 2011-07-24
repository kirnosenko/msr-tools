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
	public class CodeSizeToDate : Visualization
	{
		public CodeSizeToDate()
		{
			Title = "Code size to date";
			DaysPerStep = 30;
		}
		public override void Calc(IRepositoryResolver repositories)
		{
			int daysPerStep = Convert.ToInt32(DaysPerStep);

			DateTime min = repositories.Repository<Commit>().Min(c => c.Date);
			DateTime max = repositories.Repository<Commit>().Max(c => c.Date);
			x = new double[(max - min).Days / daysPerStep + 1];
			y = new double[x.Length];

			DateTime from = min;
			DateTime to = from.AddDays(daysPerStep);
			int i = 0;
			double totalCodeSize = 0;
			while (from <= max)
			{
				x[i] = (from - min).Days;
				totalCodeSize += repositories.SelectionDSL()
					.Commits().DateIsGreaterOrEquelThan(from).DateIsLesserThan(to)
					.Files().InDirectory(TargetDir)
					.Modifications().InCommits().InFiles()
					.CodeBlocks().InModifications().CalculateLOC();
				y[i] = totalCodeSize;

				from = from.AddDays(daysPerStep);
				to = to.AddDays(daysPerStep);
				i++;
			}
		}
		public override void Draw(IGraphView graph)
		{
			graph.XAxisTitle = "Days";
			graph.YAxisTitle = "LOC";
			base.Draw(graph);
		}
		[DescriptionAttribute("Days per step")]
		public int DaysPerStep
		{
			get; set;
		}
	}
}
