/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;

namespace MSR.Tools.Visualizer
{
	public interface IMessageDialogView
	{
		bool ShowYesNo(string title, string text);
		void ShowError(string title, string text);
	}
}
