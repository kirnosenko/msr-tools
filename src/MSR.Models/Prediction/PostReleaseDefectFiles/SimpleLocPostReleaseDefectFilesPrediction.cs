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
using MSR.Models.Prediction.Predictors;
using MSR.Models.Regressions;

namespace MSR.Models.Prediction.PostReleaseDefectFiles
{
	public class SimpleLocPostReleaseDefectFilesPrediction : PostReleaseDefectFilesPrediction
	{
		private LinearRegression regression;
		
		public SimpleLocPostReleaseDefectFilesPrediction()
		{
			this.AddTotalLocInFilesTillRevisionPredictor();
			defaultCutOffValue = 1;
			
			Title = "Simple total LOC model";
		}
		public override void Init(IRepositoryResolver repositories, IEnumerable<string> releases)
		{
			base.Init(repositories, releases);

			regression = new LinearRegression();
			foreach (var revision in TrainReleases)
			{
				foreach (var file in GetFilesInRevision(revision))
				{
					context
						.SetCommits(null, revision)
						.SetFiles(e => e.IdIs(file.ID));

					regression.AddTrainingData(
						GetPredictorValuesFor(context)[0],
						NumberOfFixedDefectsForFile(file.ID, revision)
					);
				}
			}

			regression.Train();
			context.SetCommits(null, PredictionRelease);
		}
		public override ROCEvaluationResult EvaluateUsingROC()
		{
			double maxFileEstimation = FileEstimations.Max();
			rocEvaluationDelta = (maxFileEstimation + maxFileEstimation / 100) / 100;
			return base.EvaluateUsingROC();
		}
		protected override double GetFileEstimation(ProjectFile file)
		{
			double numberOfBugs = regression.Predict(
				GetPredictorValuesFor(context.SetFiles(e => e.IdIs(file.ID)))[0]
			);
			return numberOfBugs > 0 ? numberOfBugs : 0;
		}
		protected double NumberOfFixedDefectsForFile(int fileID, string revision)
		{
			return repositories.SelectionDSL()
				.Commits().TillRevision(revision)
				.Files().IdIs(fileID)
				.Modifications().InCommits().InFiles()
				.CodeBlocks().InModifications().CalculateNumberOfDefects(revision);
		}
	}
}
