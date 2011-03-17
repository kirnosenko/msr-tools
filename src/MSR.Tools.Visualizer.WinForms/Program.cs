/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;

namespace MSR.Tools.Visualizer.WinForms
{
	public class Program
	{
		[STAThread]
		static void Main()
		{
			VisualizerViewFactory views = new VisualizerViewFactory();
			VisualizerPresenterFactory presenters = new VisualizerPresenterFactory(views);
			
			VisualizerPresenter graph = new VisualizerPresenter(
				new VisualizerModel(), views.Visualizer(), presenters
			);

			graph.Show();
		}
	}
}
