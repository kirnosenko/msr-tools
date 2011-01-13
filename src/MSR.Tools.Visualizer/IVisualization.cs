/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;

using MSR.Data;

namespace MSR.Tools.Visualizer
{
	public interface IVisualization
	{
		void Visualize(IRepositoryResolver repositories, IGraphView graph);
		string Title { get; }
		bool Configurable { get; }
	}
}
