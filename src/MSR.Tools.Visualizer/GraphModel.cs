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
using MSR.Data.VersionControl;

namespace MSR.Tools.Visualizer
{
	public class GraphModel
	{
		private IDataStore data;
		
		public GraphModel(IDataStore data)
		{
			this.data = data;
		}
		public PointPairList DefectDensityToFileSize()
		{
			PointPairList points = new PointPairList();
			
			using (var s = data.OpenSession())
			{
				RepositorySelectionExpression selectionDSL = new RepositorySelectionExpression(s);
				var fileIDs = selectionDSL.Files()
					.InDirectory("/trunk")
					.Exist()
					.Select(f => f.ID);

				foreach (var fileID in fileIDs)
				{
					RepositorySelectionExpression fileDSL = new RepositorySelectionExpression(s);

					var code = fileDSL.Files()
						.IdIs(fileID)
						.Modifications().InFiles()
						.CodeBlocks().InModifications();

					var loc = code.CalculateLOC();
					var dd = code.CalculateTraditionalDefectDensity();
					points.Add(loc, dd);
				}
			}

			return points;
		}
		public PointPairList BugLifeTime()
		{
			PointPairList points = new PointPairList();
			
			using (var s = data.OpenSession())
			{
				RepositorySelectionExpression selectionDSL = new RepositorySelectionExpression(s);
				
				var bugLiveTimes = selectionDSL
					.BugFixes().CalculateMaxBugLifetime();
				
				foreach (var bugLiveTime in bugLiveTimes)
				{
					points.Add(
						bugLiveTime,
						bugLiveTimes.Where(x => x <= bugLiveTime).Count()
					);
				}
				
			}
			
			return points;
		}
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

			using (var s = data.OpenSession())
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
		}
	}
}
