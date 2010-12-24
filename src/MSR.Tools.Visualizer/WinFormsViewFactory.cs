/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using System.Windows.Forms;

namespace MSR.Tools.Visualizer
{
	public interface IWinFormsViewFactory
	{
		IVisualizerView GraphView();
	}
	
	public class WinFormsViewFactory
	{
		public WinFormsViewFactory()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
		}
		public IVisualizerView VisualizerView()
		{
			return new VisualizerView();
		}
	}
}
