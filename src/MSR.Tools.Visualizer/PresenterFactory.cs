/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;

namespace MSR.Tools.Visualizer
{
	public interface IPresenterFactory
	{
		IFileDialogPresenter FileDialog();
		IMessageDialogPresenter MessageDialog();
	}
	public class PresenterFactory : IPresenterFactory
	{
		private IViewFactory views;
		
		public PresenterFactory(IViewFactory views)
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
	}

}
