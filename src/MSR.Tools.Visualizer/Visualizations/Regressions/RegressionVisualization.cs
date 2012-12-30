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
	public abstract class RegressionVisualization : Visualization
	{
		public RegressionVisualization()
		{
			Intervals = 100;
		}
		public override void Calc(IRepository repository)
		{
		}
		public override void Draw(IGraphView graph)
		{
			var points = graph.Points.Last();
			
			var r = GetRegression();
			r.Train(points.Key, points.Value);
			
			double x1 = points.Key.Min();
			double x2 = points.Key.Max();
			double delta = (x2 - x1) / Intervals;
			x = new double[Intervals];
			y = new double[Intervals];
			for (int i = 0; i < Intervals; i++)
			{
				x[i] = x1;
				y[i] = r.Predict(x1);
				x1 += delta;
			}
			
			graph.ShowLine("", x, y);
		}
		public override bool AllowCleanUp
		{
			get { return false; }
		}
		[Browsable(false)]
		public int Intervals
		{
			get; set;
		}
		protected abstract Regression<double> GetRegression();
	}
}
