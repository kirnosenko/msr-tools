/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;

namespace MSR.Tools.Visualizer
{
	public interface IStatusBarView
	{
		string Status { get; set; }
	}
}
