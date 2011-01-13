/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;

namespace MSR.Tools.Visualizer
{
	public interface IVisualizationConfigView
	{
		void AddTextOption(string name, string value);
		void AddSelectionOption(string name, object[] values, object selected);
		object GetOption(string name);
		
		bool ShowDialog();
	}
}
