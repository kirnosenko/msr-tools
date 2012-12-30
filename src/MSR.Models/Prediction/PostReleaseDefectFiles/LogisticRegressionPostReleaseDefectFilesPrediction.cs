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

namespace MSR.Models.Prediction.PostReleaseDefectFiles
{
	public class LogisticRegressionPostReleaseDefectFilesPrediction : PostReleaseDefectFilesPrediction
	{
		private LogisticRegression regression;

		public override void Init(IRepository repository, IEnumerable<string> releases)
		{
			base.Init(repository, releases);

			regression = new LogisticRegression();

			string previousRevision = null;
			foreach (var revision in TrainReleases)
			{
				foreach (var file in GetFilesInRevision(revision))
				{
					context
						.SetCommits(previousRevision, revision)
						.SetFiles(e => e.IdIs(file.ID));

					regression.AddTrainingData(
						GetPredictorValuesFor(context),
						FileHasDefects(file.ID, revision, previousRevision)
					);
				}
				previousRevision = revision;
			}

			regression.Train();

			context.SetCommits(TrainReleases.Last(), PredictionRelease);
		}
		protected override double GetFileEstimation(ProjectFile file)
		{
			return regression.Predict(
				GetPredictorValuesFor(context.SetFiles(e => e.IdIs(file.ID)))
			);
		}
		protected double FileHasDefects(int fileID, string revision, string previousRevision)
		{
			return repository.SelectionDSL()
				.Files().IdIs(fileID)
				.Commits()
					.AfterRevision(previousRevision)
					.TillRevision(revision)
				.Modifications().InCommits().InFiles()
				.CodeBlocks().InModifications().CalculateNumberOfDefects(revision) > 0 ? 1 : 0;
		}
	}
}
