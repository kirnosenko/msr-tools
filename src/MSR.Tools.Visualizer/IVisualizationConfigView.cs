/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;

namespace MSR.Tools.Visualizer
{
	public interface IConfigView
	{
		void Add(object obj);
		
		bool ShowDialog();
	}
}
