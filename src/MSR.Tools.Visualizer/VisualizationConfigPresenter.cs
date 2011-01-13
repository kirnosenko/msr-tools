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
		private IVisualizationConfigView view;
		
		public VisualizationConfigPresenter(IVisualizationConfigView view)
		{
			this.view = view;
		}
		public bool Config(IVisualization visualization)
		{
			Dictionary<string,PropertyInfo> optionProperties = new Dictionary<string,PropertyInfo>();
			
			var type = visualization.GetType();
			var properties = type.GetProperties();
			foreach (var property in properties)
			{
				var options = property.GetCustomAttributes(true)
					.Where(x => typeof(VisualizationOptionAttribute).IsAssignableFrom(x.GetType()));
				foreach (VisualizationOptionAttribute option in options)
				{
					option.MapOption(view, property.GetValue(visualization, null));
					optionProperties.Add(option.Name, property);
				}
			}
			
			if (view.ShowDialog())
			{
				foreach (var option in optionProperties)
				{
					option.Value.SetValue(visualization, view.GetOption(option.Key), null);
				}
				return true;
			}
			return false;
		}
	}
}
