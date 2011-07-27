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

namespace MSR.Tools.Visualizer.Visualizations
{
	public class PolynomialRegression : RegressionVisualization
	{
		public PolynomialRegression()
		{
			Title = "Polynomial regression";
			Degree = 3;
		}
		public int Degree
		{
			get; set;
		}
		protected override Regression<double> GetRegression()
		{
			return new MSR.Models.Regressions.PolynomialRegression()
			{
				Degree = Degree
			};
		}
	}
}
