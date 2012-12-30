/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
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
	public class DefectDensityToDate : DatePeriodVisualization
	{
		private double[] tdd, dd;
		
		public DefectDensityToDate()
		{
			Type = VisualizationType.LINEWITHPOINTS;
			Title = "Defect density to date";
		}
		public override void Calc(IRepository repository)
		{
			base.Calc(repository);

			x = new double[dates.Count()];
			tdd = new double[dates.Count()];
			dd = new double[dates.Count()];
			string revision = null;
			
			for (int i = 0; i < dates.Length; i++)
			{
				x[i] = (dates[i] - dates[0]).TotalDays;
				var code = repository.SelectionDSL()
					.Commits()
						.DateIsLesserOrEquelThan(dates[i])
						.Do(c =>
							revision = repository.Queryable<Commit>().Single(cc =>
								cc.OrderedNumber == c.Max(ccc => ccc.OrderedNumber)
							).Revision
						)
					.Files()
						.InDirectory(TargetDir)
					.Modifications()
						.InCommits()
						.InFiles()
					.CodeBlocks()
						.InModifications();
				
				tdd[i] = code.CalculateTraditionalDefectDensity(revision);
				dd[i] = code.CalculateDefectDensity(revision);
			}
		}
		public override void Draw(IGraphView graph)
		{
			graph.PrepairPointsForDateScale(x, dates[0]);
			graph.YAxisTitle = "Defect density (defects per KLOC)";

			Legend = "Traditional defect density";
			this.y = tdd;
			base.Draw(graph);

			Legend = "Alternative defect density";
			this.y = dd;
			base.Draw(graph);
		}
	}
}
