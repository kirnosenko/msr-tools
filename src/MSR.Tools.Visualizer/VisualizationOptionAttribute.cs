/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;

namespace MSR.Tools.Visualizer
{
	[AttributeUsage(AttributeTargets.Property)]
	public abstract class VisualizationOptionAttribute : Attribute
	{
		public VisualizationOptionAttribute(string name)
		{
			Name = name;
		}
		public string Name
		{
			get; private set;
		}
		public abstract void MapOption(IVisualizationConfigView view, object value);
	}
}
