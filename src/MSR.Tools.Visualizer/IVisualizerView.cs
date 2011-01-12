/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;

namespace MSR.Tools.Visualizer
{
	public interface IVisualizerView
	{
		void ShowView();

		string Title { get; set; }
		IMainMenuView MainMenu { get; }
		IGraphView Graph { get; }
		IStatusBarView StatusBar { get; }
	}
}
