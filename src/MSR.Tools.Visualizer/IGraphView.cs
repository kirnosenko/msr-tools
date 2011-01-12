/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;

namespace MSR.Tools.Visualizer
{
	public interface IGraphView
	{
		void ShowPoints(string legend, double[] x, double[] y);
		void CleanUp();
		
		string Title { get; set; }
		string XAxisTitle { get; set; }
		string YAxisTitle { get; set; }
		bool XAxisLogScale { get; set; }
		bool YAxisLogScale { get; set; }
	}
}
