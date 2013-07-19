/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;
using System.ComponentModel;
using System.Linq;

using MSR.Data;
using MSR.Data.Entities;
using MSR.Data.Entities.DSL.Selection;
using MSR.Data.Entities.DSL.Selection.Metrics;

namespace MSR.Tools.Visualizer.Visualizations.Distributions
{
	public abstract class Distribution : Visualization
	{
		private double[] densityX, densityY;
		
		public Distribution()
		{
			ShowProbabilityDistribution = true;
			ShowProbabilityDensity = false;
			Intervals = 10;
		}
		public override void Calc(IRepository repository)
		{
			var ddata = DistributionData(repository);
			
			if (ShowProbabilityDistribution)
			{
				x = new double[ddata.Length];
				y = new double[ddata.Length];
				
				for (int i = 0; i < ddata.Length; i++)
				{
					x[i] = ddata[i];
					y[i] = (double)ddata.Where(d => d <= ddata[i]).Count() / ddata.Count();
				}
			}
			
			if (ShowProbabilityDensity)
			{
				densityX = new double[Intervals];
				densityY = new double[Intervals];
				
				double delta = ddata.Max() / Intervals;
				
				for (int i = 0; i < Intervals; i++)
				{
					densityX[i] = i * delta + delta / 2;
					densityY[i] = (double)ddata.Where(d => d >= i*delta && d < (i+1)*delta).Count() / ddata.Count();
				}
			}
			
			Legend = string.Format(
				"Mean = {0:0.00} Standard deviation = {1:0.00}",
				Accord.Statistics.Tools.Mean(x), Accord.Statistics.Tools.StandardDeviation(x)
			);
		}
		public override void Draw(IGraphView graph)
		{
			graph.YAxisTitle = "Probability";
			if (ShowProbabilityDistribution)
			{
				base.Draw(graph);
			}
			if (ShowProbabilityDensity)
			{
				graph.ShowHistogram("", densityX, densityY);
			}
		}
		[DescriptionAttribute("Show probability distribution")]
		public bool ShowProbabilityDistribution
		{
			get; set;
		}
		[DescriptionAttribute("Show probability density function")]
		public bool ShowProbabilityDensity
		{
			get; set;
		}
		[DescriptionAttribute("Number of intervals for probability density function")]
		public int Intervals
		{
			get; set;
		}
		protected abstract double[] DistributionData(IRepository repository);
	}
}
