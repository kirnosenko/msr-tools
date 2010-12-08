/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using MSR.Data;

namespace MSR.Tools.Visualizer
{
	public class Program
	{
		private static string configFile;
		
		[STAThread]
		static void Main()
		{
			//configFile = @"E:\repo\gnome-terminal\gnome-terminal.config"; // 3436 revisions
			configFile = @"E:\repo\dia\dia.config"; // 4384 revisions
			//configFile = @"E:\repo\gnome-vfs\gnome-vfs.config"; // 5550 revisions
			
			VisualizationTool tool = new VisualizationTool(configFile);
			tool.Show();
		}
	}
}
