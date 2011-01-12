/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;

namespace MSR.Tools.Visualizer
{
	public interface IMenuItemView
	{
		event Action<IMenuItemView> OnClick;
		string Name { get; }
		string Text { get; set; }
		bool Enabled { get; set; }
		bool Checked { get; set; }
	}
}
