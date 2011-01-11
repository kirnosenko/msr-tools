/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Linq;

using MSR.Tools.Visualizer.Visualizations;

namespace MSR.Tools.Visualizer
{
	public class VisualizerModel
	{
		private VisualizationTool visualizer;
		private List<IVisualization> visualizations = new List<IVisualization>();
		
		public VisualizerModel()
		{
			visualizations.Add(new BugLifeTimeDistribution());
			visualizations.Add(new DefectDensityToFileSize());
			visualizations.Add(new CodeSizeToDate());
			
			AutomaticallyCleanUp = true;
		}
		public void OpenConfig(string fileName)
		{
			visualizer = new VisualizationTool(fileName);
		}
		public void Visualize(string visualizationName, IGraphView graph)
		{
			if (AutomaticallyCleanUp)
			{
				graph.CleanUp();
			}
			visualizer.Visualize(
				visualizations.Single(x => x.Title == visualizationName),
				graph
			);
		}
		public string LastVisualizationProfiling
		{
			get { return visualizer.LastVisualizationProfiling; }
		}
		public IEnumerable<string> Visualizations
		{
			get { return visualizations.Select(x => x.Title); }
		}
		public bool AutomaticallyCleanUp
		{
			get; set;
		}
	}
}
