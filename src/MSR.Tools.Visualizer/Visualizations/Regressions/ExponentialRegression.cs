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
using MSR.Models.Regressions;

namespace MSR.Tools.Visualizer.Visualizations.Regressions
{
	public class ExponentialRegression : RegressionVisualization
	{
		public ExponentialRegression()
		{
			Title = "Exponential regression";
		}
		public override bool Configurable
		{
			get { return false; }
		}
		protected override Regression<double> GetRegression()
		{
			return new MSR.Models.Regressions.ExponentialRegression();
		}
	}
}
