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
		public event Action OnVisualizationListUpdated;
		
		private VisualizationTool visualizer;
		
		public VisualizerModel()
		{
			AutomaticallyCleanUp = true;
		}
		public void OpenConfig(string fileName)
		{
			visualizer = new VisualizationTool(fileName);
			OnVisualizationListUpdated();
		}
		public void Visualize(string visualizationName, IGraphView graph, IVisualizationConfigPresenter config)
		{
			if (AutomaticallyCleanUp)
			{
				graph.CleanUp();
			}
			
			if (config.Config(visualizer.Visualization(visualizationName)))
			{
				visualizer.Visualize(visualizationName, graph);
			}
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
