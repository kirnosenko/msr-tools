/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;

namespace MSR.Tools.Visualizer
{
	public interface IVisualizerPresenterFactory
	{
		IFileDialogPresenter FileDialog();
		IVisualizationConfigPresenter ConfigDialog();
	}
	public class VisualizerPresenterFactory : IVisualizerPresenterFactory
	{
		private IVisualizerViewFactory views;
		
		public VisualizerPresenterFactory(IVisualizerViewFactory views)
		{
			this.views = views;
		}
		public IFileDialogPresenter FileDialog()
		{
			return new FileDialogPresenter(views.FileDialog());
		}
		public IVisualizationConfigPresenter ConfigDialog()
		{
			return new VisualizationConfigPresenter(views.ConfigDialog());
		}
	}
}
