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
		IGraphView GraphView();
	}
	
	public class WinFormsViewFactory
	{
		public WinFormsViewFactory()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
		}
		public IGraphView GraphView()
		{
			return new GraphView();
		}
	}
}
