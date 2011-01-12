/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;

namespace MSR.Tools.Visualizer
{
	public interface IMainMenuView
	{
		IMenuItemView AddCommand(string name);
		IMenuItemView AddCommand(string name, string parentName);
		void DeleteCommand(string name);
	}
}
