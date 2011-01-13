/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace MSR.Tools.Visualizer
{
	public class VisualizerModel
	{
		private VisualizationTool visualizer;
		
		public VisualizerModel()
		{
			AutomaticallyCleanUp = true;
		}
		public void OpenConfig(string fileName)
		{
			visualizer = new VisualizationTool(fileName);
		}
		public IVisualization Visualization(string visualizationName)
		{
			return visualizer.Visualization(visualizationName);
		}
		public void CalcVisualization(IVisualization visualization)
		{	
			visualizer.CalcVisualization(visualization);
		}
		public string LastVisualizationProfiling
		{
			get { return visualizer.LastVisualizationProfiling; }
		}
		public IEnumerable<string> Visualizations
		{
			get
			{
				if (visualizer != null)
				{
					return visualizer.Visualizations;
				}
				return Enumerable.Empty<string>();
			}
		}
		public bool AutomaticallyCleanUp
		{
			get; set;
		}
	}
}
