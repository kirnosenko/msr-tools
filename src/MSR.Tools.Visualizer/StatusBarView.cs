/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using System.Windows.Forms;

namespace MSR.Tools.Visualizer
{
	public class StatusBarView : StatusStrip, IStatusBarView
	{
		private ToolStripStatusLabel label;
		
		public StatusBarView(Control parent)
		{
			Parent = parent;
			Dock = DockStyle.Bottom;
			label = new ToolStripStatusLabel();
			Items.Add(label);
		}
		public string Status
		{	
			get { return label.Text; }
			set { label.Text = value; }
		}
	}
}
