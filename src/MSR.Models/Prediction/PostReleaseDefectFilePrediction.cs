/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Linq;

using MSR.Data;
using MSR.Data.Entities;
using MSR.Data.Entities.DSL.Selection;
using MSR.Data.Entities.DSL.Selection.Metrics;
using MSR.Models.Regressions;

namespace MSR.Models.Prediction
{
	public class PostReleaseDefectFilePrediction : Prediction
	{
		public PostReleaseDefectFilePrediction(IRepositoryResolver repositories)
			: base(repositories)
		{
			FilePortionLimit = 0.2;
		}
		public IEnumerable<string> Predict(string[] previousReleaseRevisions, string releaseRevision)
		{
			LogisticRegression lr = new LogisticRegression();
			
			string previousRevision = null;
			foreach (var revision in previousReleaseRevisions)
			{
				foreach (var file in FilesInRevision(revision))
				{
					lr.AddTrainingData(
						GetPredictorValues(file.ID, revision, previousRevision),
						FileHasDefects(file.ID, revision, previousRevision)
					);
				}
				previousRevision = revision;
			}
			
			lr.Train();

			var files = FilesInRevision(releaseRevision);
			int filesInRelease = files.Count();
			var faultProneFiles =
				(
					from f in files
					select new
					{
						Path = f.Path,
						FaultProneProbability = lr.Predict(
							GetPredictorValues(f.ID, releaseRevision, previousReleaseRevisions.Last())
						)
					}
				).Where(x => x.FaultProneProbability > 0.5)
				.OrderByDescending(x => x.FaultProneProbability);

			return faultProneFiles
				.Select(x => x.Path)
				.TakeNoMoreThan((int)(filesInRelease * FilePortionLimit));
		}
		public Func<ProjectFileSelectionExpression,ProjectFileSelectionExpression> FileSelector
		{
			get; set;
		}
		public double FilePortionLimit
		{
			get; set;
		}
		private double[] GetPredictorValues(int fileID, string revision, string previousRevision)
		{
			List<double> predictorValues = new List<double>();

			predictorValues.AddRange(
				GetPredictorValuesFor(
					repositories.SelectionDSL()
						.Commits()
							.Reselect(e => previousRevision == null ? e : e.AfterRevision(previousRevision))
							.TillRevision(revision)
						.Files().IdIs(fileID)
				)
			);
			var code = repositories.SelectionDSL()
				.Files().IdIs(fileID)
				.Commits()
					.Reselect(e => previousRevision == null ? e : e.AfterRevision(previousRevision))
					.TillRevision(revision)
				.Modifications().InCommits().InFiles()
				.CodeBlocks().InModifications();
			predictorValues.AddRange(GetPredictorValuesFor(code));

			return predictorValues.ToArray();
		}
		private IEnumerable<ProjectFile> FilesInRevision(string revision)
		{
			return repositories.SelectionDSL()
				.Files()
					.Reselect(FileSelector)
					.ExistInRevision(revision)
					.ToList();
		}
		private double FileHasDefects(int fileID, string revision, string previousRevision)
		{
			return repositories.SelectionDSL()
				.Files().IdIs(fileID)
				.Commits()
					.Reselect(e => previousRevision == null ? e : e.AfterRevision(previousRevision))
					.TillRevision(revision)
				.Modifications().InCommits().InFiles()
				.CodeBlocks().InModifications().CalculateNumberOfDefects() > 0 ? 1 : 0;
		}
	}
}
