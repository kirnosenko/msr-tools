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

using MSR.Data.Persistent;

namespace MSR.Tools.Visualizer
{
	public class VisualizationTool : Tool
	{
		public VisualizationTool(string configFile)
			: base(configFile)
		{
		}
		public void Show()
		{
			PersistentDataStoreProfiler prof = new PersistentDataStoreProfiler(data);

			WinFormsViewFactory views = new WinFormsViewFactory();

			GraphPresenter graph = new GraphPresenter(
				new GraphModel(data), views.GraphView()
			);

			//graph.ShowPoints();
			graph.ShowLines();

			MessageBox.Show(string.Format("SQL queries: {0}", prof.NumberOfQueries));
		}
	}
}
