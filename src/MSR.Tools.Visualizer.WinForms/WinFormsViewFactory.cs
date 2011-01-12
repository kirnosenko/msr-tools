/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using System.Windows.Forms;

namespace MSR.Tools.Visualizer.WinForms
{
	public class WinFormsViewFactory : IViewFactory
	{
		public WinFormsViewFactory()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
		}
		public IMessageDialogView MessageDialog()
		{
			return new MessageDialogView();
		}
		public IFileDialogView FileDialog()
		{
			return new FileDialogView();
		}
		
		public IVisualizerView Visualizer()
		{
			return new VisualizerView();
		}
	}
}
