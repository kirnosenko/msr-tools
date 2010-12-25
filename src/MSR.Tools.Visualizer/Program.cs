/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;

namespace MSR.Tools.Visualizer
{
	public class Program
	{
		[STAThread]
		static void Main()
		{
			WinFormsViewFactory views = new WinFormsViewFactory();
			PresenterFactory presenters = new PresenterFactory(views);
			
			VisualizerPresenter graph = new VisualizerPresenter(
				new VisualizerModel(), views.Visualizer(), presenters
			);

			graph.Show();
		}
	}
}
