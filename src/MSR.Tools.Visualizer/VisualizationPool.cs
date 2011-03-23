/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace MSR.Tools.Visualizer
{
	public class VisualizationPool
	{
		private IVisualization[] visualizations;

		public VisualizationPool(IVisualization[] visualizations)
		{
			this.visualizations = visualizations;
		}
		public IVisualization[] Visualizations
		{
			get { return visualizations; }
		}
		public string TargetDir
		{
			set
			{
				foreach (var v in visualizations)
				{
					
					v.TargetDir = value;
				}
			}
		}
	}
}
