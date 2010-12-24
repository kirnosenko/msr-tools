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
	public class VisualizerPresenter
	{
		private VisualizerModel model;
		private IVisualizerView view;
		
		public VisualizerPresenter(VisualizerModel model, IVisualizerView view)
		{
			this.model = model;
			this.view = view;
		}
		public void Show()
		{
			view.ShowView();
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
