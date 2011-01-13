/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;

namespace MSR.Tools.Visualizer.VisualizationOptions
{
	public class VisualizationSelectionOptionAttribute : VisualizationOptionAttribute
	{
		public VisualizationSelectionOptionAttribute(string name)
			: base(name)
		{
		}
		public override void MapOption(IVisualizationConfigView view, object value)
		{
			view.AddSelectionOption(Name, Values, value);
		}
		public string[] Values
		{
			get; set;
		}
	}
}
