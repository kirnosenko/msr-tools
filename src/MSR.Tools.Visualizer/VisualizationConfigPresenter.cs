/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MSR.Tools.Visualizer
{
	public interface IVisualizationConfigPresenter
	{
		bool Config(IVisualization visualization);
	}

	public class VisualizationConfigPresenter : IVisualizationConfigPresenter
	{
		private IConfigView view;
		
		public VisualizationConfigPresenter(IConfigView view)
		{
			this.view = view;
		}
		public bool Config(IVisualization visualization)
		{
			view.Add(visualization);
			
			if (view.ShowDialog())
			{
				
				return true;
			}
			return false;
		}
	}
}
