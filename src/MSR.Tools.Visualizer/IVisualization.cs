/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;

using MSR.Data;

namespace MSR.Tools.Visualizer
{
	public interface IVisualization
	{
		void Visualize(IDataStore data, IGraphView graph);
	}
}
