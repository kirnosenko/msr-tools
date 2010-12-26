/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Linq;
using ZedGraph;

using MSR.Data;
using MSR.Data.Entities;
using MSR.Data.Entities.DSL.Selection;
using MSR.Data.Entities.DSL.Selection.Metrics;
using MSR.Data.Persistent;
using MSR.Data.VersionControl;
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
		}
		public void OpenConfig(string fileName)
		{
			visualizer = new VisualizationTool(fileName);
		}
		public void Visualize(string visualizationName, IGraphView graph)
		{
			PersistentDataStoreProfiler prof = new PersistentDataStoreProfiler(Data);
			prof.Start();
			visualizations[visualizationName].Visualize(visualizer.Data, graph);
			prof.Stop();
			LastVisualizationProfiling = string.Format(
				"Last visualization: queries = {0} time = {1}",
				prof.NumberOfQueries, prof.ElapsedTime.ToFormatedString()
			);
		}
		public string LastVisualizationProfiling
		{
			get; private set;
		}
		public IEnumerable<string> Visualizations
		{
			get { return visualizations.Keys; }
		}
		/*
		public PointPairList BugLifeTimeForCode(CodeBlockSelectionExpression code)
		{
			PointPairList points = new PointPairList();
			
			var bugLiveTimes = code
				.Modifications().ContainCodeBlocks()
				.Commits().ContainModifications()
				.BugFixes().InCommits().CalculateAvarageBugLifetime();

			foreach (var bugLiveTime in bugLiveTimes)
			{
				points.Add(
					bugLiveTime,
					(double)bugLiveTimes.Where(x => x <= bugLiveTime).Count() / bugLiveTimes.Count()
				);
			}
			
			return points;
		}
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
		private PersistentDataStore Data
		{
			get { return visualizer.Data; }
		}
	}
}
