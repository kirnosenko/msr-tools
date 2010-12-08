/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using ZedGraph;

namespace MSR.Tools.Visualizer
{
	public class GraphPresenter
	{
		private GraphModel model;
		private IGraphView view;
		
		public GraphPresenter(GraphModel model, IGraphView view)
		{
			this.model = model;
			this.view = view;
		}
		public void ShowPoints()
		{
			PointPairList points;
			
			using (new TimeLogger("", x => view.Title = x.FormatedTime))
			{
				points = model.DefectDensityToFileSize();
				//points = model.BugLifeTime();
			}
			
			view.ShowPoints(points);
		}
		public void ShowLines()
		{
			List<PointPairList> lines = new List<PointPairList>();

			using (new TimeLogger("", x => view.Title = x.FormatedTime))
			{
				lines.AddRange(model.BugLifeTimes());
			}

			view.ShowPoints(lines);
		}
	}
}
