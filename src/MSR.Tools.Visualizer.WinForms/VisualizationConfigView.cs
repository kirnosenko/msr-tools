/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MSR.Tools.Visualizer.WinForms
{
	public class VisualizationConfigView : Form, IVisualizationConfigView
	{
		public new IDictionary<string,object> Show()
		{
			Dictionary<string,object> options = new Dictionary<string,object>();
			
			base.ShowDialog();
			
			return options;
		}
	}
}
