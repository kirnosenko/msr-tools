/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;
using System.ComponentModel;
using System.Linq;

using MathNet.Numerics.Statistics;

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
		public override void Calc(IRepositoryResolver repositories)
		{
			var data = DistributionData(repositories);
			
			if (ShowProbabilityDistribution)
			{
				x = new double[data.Length];
				y = new double[data.Length];
				
				for (int i = 0; i < data.Length; i++)
				{
					x[i] = data[i];
					y[i] = (double)data.Where(d => d <= data[i]).Count() / data.Count();
				}
			}
			
			if (ShowProbabilityDensity)
			{
				densityX = new double[Intervals];
				densityY = new double[Intervals];
				
				double delta = data.Max() / Intervals;
				
				for (int i = 0; i < Intervals; i++)
				{
					densityX[i] = i * delta + delta / 2;
					densityY[i] = (double)data.Where(d => d >= i*delta && d < (i+1)*delta).Count() / data.Count();
				}
			}
			
			Legend = string.Format("Mean = {0:0.00} Standard deviation = {1:0.00}", x.Mean(), x.PopulationStandardDeviation());
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
		protected abstract double[] DistributionData(IRepositoryResolver repositories);
	}
}
