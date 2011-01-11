/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;
using MSR.Data.Persistent;

namespace MSR.Tools.Visualizer
{
	public class VisualizationTool : Tool
	{
		public VisualizationTool(string configFile)
			: base(configFile)
		{
		}
		public void Visualize(IVisualization visualization, IGraphView graph)
		{
			PersistentDataStoreProfiler prof = new PersistentDataStoreProfiler(Data);
			prof.Start();
			using (var s = data.OpenSession())
			{
				visualization.Visualize(s, graph);
			}
			prof.Stop();
			LastVisualizationProfiling = string.Format(
				"Last visualization: queries = {0} time = {1}",
				prof.NumberOfQueries, prof.ElapsedTime.ToFormatedString()
			);
		}
		public string LastVisualizationProfiling
		{
			get; private set;
		}
		public PersistentDataStore Data
		{
			get { return data; }
		}
	}
}
