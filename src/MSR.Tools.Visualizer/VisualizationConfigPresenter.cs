/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;

namespace MSR.Tools.Visualizer
{
	public interface IVisualizationConfigPresenter
	{
		IDictionary<string,object> GetConfig();
	}

	public class VisualizationConfigPresenter : IVisualizationConfigPresenter
	{
		private IVisualizationConfigView view;
		
		public VisualizationConfigPresenter(IVisualizationConfigView view)
		{
			this.view = view;
		}
		public IDictionary<string,object> GetConfig()
		{
			return view.Show();
		}
	}
}
