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
		IMessageDialogPresenter MessageDialog();
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
		public IMessageDialogPresenter MessageDialog()
		{
			return new MessageDialogPresenter(views.MessageDialog());
		}
		public IVisualizationConfigPresenter ConfigDialog()
		{
			return new VisualizationConfigPresenter(views.ConfigDialog());
		}
	}
}
