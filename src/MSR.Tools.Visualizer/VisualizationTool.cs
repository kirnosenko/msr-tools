/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
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
		public PersistentDataStore Data
		{
			get { return data; }
		}
	}
}
