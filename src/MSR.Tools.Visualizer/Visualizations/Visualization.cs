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
	public abstract class Visualization : IVisualization
	{
		private static string targetPath = "/";
		
		protected double[] x,y;
		
		public Visualization()
		{
		}
		public abstract void Calc(IRepositoryResolver repositories);
		public abstract void Draw(IGraphView graph);
		[Browsable(false)]
		public string Title
		{
			get; protected set;
		}
		[Browsable(false)]
		public virtual bool Configurable
		{
			get { return true; }
		}
		[DescriptionAttribute("Target path")]
		public string TargetPath
		{
			get { return targetPath; }
			set { targetPath = value; }
		}
	}
}
