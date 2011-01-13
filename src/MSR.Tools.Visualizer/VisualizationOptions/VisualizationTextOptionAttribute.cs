/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;

namespace MSR.Tools.Visualizer.VisualizationOptions
{
	public class VisualizationTextOptionAttribute : VisualizationOptionAttribute
	{
		public VisualizationTextOptionAttribute(string name)
			: base(name)
		{
		}
		public override void MapOption(IVisualizationConfigView view, object value)
		{
			view.AddTextOption(Name, (string)value);
		}
	}
}
