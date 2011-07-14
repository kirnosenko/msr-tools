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
	public abstract class Distribution : Visualization
	{
		public override void Calc(IRepositoryResolver repositories)
		{
			var data = DistributionData(repositories);

			x = new double[data.Count()];
			y = new double[data.Count()];
			int i = 0;

			foreach (var value in data)
			{
				x[i] = value;
				y[i] = (double)data.Where(d => d <= value).Count() / data.Count();
				i++;
			}
		}
		protected abstract IEnumerable<double> DistributionData(IRepositoryResolver repositories);
	}
}
