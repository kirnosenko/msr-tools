/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MSR.Tools.Visualizer
{
	public class VisualizationPool
	{
		private List<IVisualization> visualizations = new List<IVisualization>();
		
		public VisualizationPool()
		{
			TargetDir = "/";
			FindVisualizationsInAssembly(Assembly.GetExecutingAssembly());
		}
		public IVisualization[] Visualizations
		{
			get { return visualizations.ToArray(); }
		}
		public string[] AssembliesToLookForVisualizations
		{
			set
			{
				foreach (var assembly in value)
				{
					FindVisualizationsInAssembly(Assembly.Load(assembly));
				}
			}
		}
		public string TargetDir
		{
			get; set;
		}
		private void FindVisualizationsInAssembly(Assembly assembly)
		{
			var visualizationTypes = assembly.GetTypes().Where(x =>
				x.IsAbstract == false
				&&
				typeof(IVisualization).IsAssignableFrom(x)
			);
			
			foreach (var type in visualizationTypes)
			{
				var ci = type.GetConstructors()[0];
				IVisualization v = (IVisualization)ci.Invoke(null);
				v.TargetDir = TargetDir;
				visualizations.Add(v);
			}
			
			visualizations.Sort((Comparison<IVisualization>)((a,b) =>
			{
				return a.Title.CompareTo(b.Title);
			}));
		}
	}
}
