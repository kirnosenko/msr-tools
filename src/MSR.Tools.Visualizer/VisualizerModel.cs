/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;

using MSR.Tools.Visualizer.Visualizations;

namespace MSR.Tools.Visualizer
{
	public class VisualizerModel
	{
		private VisualizationTool visualizer;
		private Dictionary<string,IVisualization> visualizations = new Dictionary<string,IVisualization>();
		
		public VisualizerModel()
		{
			visualizations.Add("bugs", new BugLifeTimeDistribution());
			visualizations.Add("ddToFileSize", new DefectDensityToFileSize());
			visualizations.Add("LOC", new CodeSizeToDate());
			
			AutomaticallyCleanUp = true;
		}
		public void OpenConfig(string fileName)
		{
			visualizer = new VisualizationTool(fileName);
		}
		public void Visualize(string visualizationName, IGraphView graph)
		{
			if (AutomaticallyCleanUp)
			{
				graph.CleanUp();
			}
			visualizer.Visualize(visualizations[visualizationName], graph);
		}
		public string LastVisualizationProfiling
		{
			get { return visualizer.LastVisualizationProfiling; }
		}
		public IEnumerable<string> Visualizations
		{
			get { return visualizations.Keys; }
		}
		public bool AutomaticallyCleanUp
		{
			get; set;
		}
		/*
		public IEnumerable<PointPairList> BugLifeTimes()
		{
			List<PointPairList> pointsList = new List<PointPairList>();

			using (var s = Data.OpenSession())
			{
				RepositorySelectionExpression selectionDSL = new RepositorySelectionExpression(s);
				Dictionary<string,string> revPairs = new Dictionary<string,string>()
				{
					{ "1", "999" },
					{ "1000", "1999" },
					{ "2000", "2999" },
				};
				
				foreach (var revPair in revPairs)
				{
					var code = selectionDSL
						.Commits()
							.FromRevision(revPair.Key)
							.TillRevision(revPair.Value)
						.Files()
							.InDirectory("/trunk")
							.Exist()
						.Modifications()
							.InFiles().InCommits()
						.CodeBlocks()
							.InModifications();
					
					pointsList.Add(BugLifeTimeForCode(code));
				}
			}

			return pointsList;
		}*/
	}
}
