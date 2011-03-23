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
	public interface IVisualizerModel
	{
		void OpenConfig(string fileName);
		void CalcVisualization(IVisualization visualization);
		
		IVisualization[] Visualizations { get; }
		bool AutomaticallyCleanUp { get; set; }
		string LastVisualizationProfiling { get; }
	}
	
	public class VisualizerModel : IVisualizerModel
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
		public void CalcVisualization(IVisualization visualization)
		{	
			visualizer.CalcVisualization(visualization);
		}
		public string LastVisualizationProfiling
		{
			get { return visualizer.LastVisualizationProfiling; }
		}
		public IVisualization[] Visualizations
		{
			get
			{
				if (visualizer != null)
				{
					return visualizer.Visualizations.Visualizations;
				}
				return new IVisualization[] {};
			}
		}
		public bool AutomaticallyCleanUp
		{
			get; set;
		}
	}
}
