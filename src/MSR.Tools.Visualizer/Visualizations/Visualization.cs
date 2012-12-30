/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;

using MSR.Data;

namespace MSR.Tools.Visualizer.Visualizations
{
	public enum VisualizationType
	{
		POINTS,
		LINE,
		LINEWITHPOINTS,
		HISTOGRAM
	}
	
	public abstract class Visualization : IVisualization
	{
		protected double[] x,y;
		
		public Visualization()
		{
			Initialized = false;
			Type = VisualizationType.POINTS;
			Legend = "";
			TargetDir = "/";
		}
		public virtual void Init(IRepository repository)
		{
			Initialized = true;
		}
		public abstract void Calc(IRepository repository);
		public virtual void Draw(IGraphView graph)
		{
			graph.Title = Title;
			switch (Type)
			{
				case VisualizationType.POINTS:
					graph.ShowPoints(Legend, x, y);
					break;
				case VisualizationType.LINE:
					graph.ShowLine(Legend, x, y);
					break;
				case VisualizationType.LINEWITHPOINTS:
					graph.ShowLineWithPoints(Legend, x, y);
					break;
				case VisualizationType.HISTOGRAM:
					graph.ShowHistogram(Legend, x, y);
					break;
				default:
					break;
			}
		}
		[Browsable(false)]
		public bool Initialized
		{
			get; protected set;
		}
		[Browsable(false)]
		public VisualizationType Type
		{
			get; protected set;
		}
		[Browsable(false)]
		public string Title
		{
			get; protected set;
		}
		[Browsable(false)]
		public string Legend
		{
			get; protected set;
		}
		[Browsable(false)]
		public virtual bool Configurable
		{
			get { return true; }
		}
		[Browsable(false)]
		public virtual bool AllowCleanUp
		{
			get { return true; }
		}
		[DescriptionAttribute("Target directory")]
		public string TargetDir
		{
			get; set;
		}
	}
}
