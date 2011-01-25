/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Linq;
using Accord.Statistics.Analysis;

using MSR.Data;
using MSR.Data.Entities;
using MSR.Data.Entities.DSL.Selection;
using MSR.Data.Entities.DSL.Selection.Metrics;

namespace MSR.Models
{
	public class LogisticRegressionPostReleaseDefectFilePrediction : IPostReleaseDefectFilePrediction
	{
		private IRepositoryResolver repositories;

		public LogisticRegressionPostReleaseDefectFilePrediction(IRepositoryResolver repositories)
		{
			this.repositories = repositories;
		}
		public IEnumerable<string> Predict(string previousReleaseRevision, string releaseRevision)
		{
			RepositorySelectionExpression selectionDSL = new RepositorySelectionExpression(repositories);
			
			var previousReleaseCode = selectionDSL
				.Commits()
					.TillRevision(previousReleaseRevision)
				.Files()
					.Reselect(FileSelector)
					.ExistInRevision(previousReleaseRevision)
				.Modifications()
					.InCommits()
					.InFiles()
				.CodeBlocks()
					.InModifications()
					.Added();

			var files =
				from cb in previousReleaseCode
				join m in previousReleaseCode.Selection<Modification>() on cb.ModificationID equals m.ID
				join f in previousReleaseCode.Selection<ProjectFile>() on m.FileID equals f.ID
				group cb.Size by f.ID into g
				select new
				{
					id = g.Key,
					addedLoc = g.Sum()
				};
			
			double[][] predictors = new double[files.Count()][];
			double[] results = new double[files.Count()];
			int counter = 0;
			foreach (var file in files)
			{
				predictors[counter] = new double[] { file.addedLoc };
				results[counter] = selectionDSL
					.Files()
						.IdIs(file.id)
					.Modifications()
						.InFiles()
					.CodeBlocks()
						.InModifications().CalculateTraditionalDefectDensity();
						
				counter++;
			}
			
			LogisticRegressionAnalysis regression = new LogisticRegressionAnalysis(
				predictors, results
			);
			regression.Compute();
			
			var releaseCode = selectionDSL
				.Commits()
					.AfterRevision(previousReleaseRevision)
					.TillRevision(releaseRevision)
				.Files()
					.Reselect(FileSelector)
					.ExistInRevision(releaseRevision)
				.Modifications()
					.InCommits()
					.InFiles()
				.CodeBlocks()
					.InModifications()
					.Added();
			
			var releaseFiles =
				from cb in releaseCode
				join m in releaseCode.Selection<Modification>() on cb.ModificationID equals m.ID
				join f in releaseCode.Selection<ProjectFile>() on m.FileID equals f.ID
				group cb.Size by f.Path into g
				select new
				{
					path = g.Key,
					addedLoc = g.Sum()
				};
			//return releaseFiles.OrderByDescending(x => x.addedLoc);
			
			return null;
		}
		public Func<ProjectFileSelectionExpression, ProjectFileSelectionExpression> FileSelector
		{
			get; set;
		}
		private void CalculatePredictorsAndResultsForRelease()
		{
			
		}
	}
}
