/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Linq;

using MSR.Data.Persistent;
using MSR.Tools.Visualizer.Visualizations;

namespace MSR.Tools.Visualizer
{
	public class VisualizationTool : Tool
	{
		private List<IVisualization> visualizations = new List<IVisualization>();
		
		public VisualizationTool(string configFile)
			: base(configFile, "visualizationtool")
		{
			visualizations.Add(new BugLifeTimeDistribution());
			visualizations.Add(new DefectDensityToFileSize());
			visualizations.Add(new CodeSizeToDate());
		}
		public void Visualize(string visualizationName, IGraphView graph)
		{
			PersistentDataStoreProfiler prof = new PersistentDataStoreProfiler(data);
			prof.Start();
			using (var s = data.OpenSession())
			{
				visualizations.Single(x => x.Title == visualizationName)
					.Visualize(s, graph);
			}
			prof.Stop();
			LastVisualizationProfiling = string.Format(
				"Last visualization: queries = {0} time = {1}",
				prof.NumberOfQueries, prof.ElapsedTime.ToFormatedString()
			);
		}
		public IEnumerable<string> Visualizations
		{
			get { return visualizations.Select(x => x.Title); }
		}
		public string LastVisualizationProfiling
		{
			get; private set;
		}
	}
}
