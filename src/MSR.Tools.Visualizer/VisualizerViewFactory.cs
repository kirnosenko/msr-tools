/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using System.Windows.Forms;

namespace MSR.Tools.Visualizer
{
	public class VisualizerViewFactory : IVisualizerViewFactory
	{
		public VisualizerViewFactory()
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
		public IConfigView ConfigDialog()
		{
			return new ConfigView();
		}			
		public IVisualizerView Visualizer()
		{
			return new VisualizerView();
		}
	}
}
