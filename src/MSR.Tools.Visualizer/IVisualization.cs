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
		void Init(IRepository repository);
		void Calc(IRepository repository);
		void Draw(IGraphView graph);
		
		bool Initialized { get; }
		string Title { get; }
		bool Configurable { get; }
		bool AllowCleanUp { get; }
		string TargetDir { get; set; }
	}
}
