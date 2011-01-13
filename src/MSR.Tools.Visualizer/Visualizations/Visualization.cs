/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;

using MSR.Data;

namespace MSR.Tools.Visualizer.Visualizations
{
	public abstract class Visualization : IVisualization
	{
		public abstract void Visualize(IRepositoryResolver repositories, IGraphView graph);
		public string Title
		{
			get; protected set;
		}
	}
}
