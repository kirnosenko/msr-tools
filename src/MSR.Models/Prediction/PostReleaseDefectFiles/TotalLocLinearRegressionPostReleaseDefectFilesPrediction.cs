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
	public class TotalLocLinearRegressionPostReleaseDefectFilesPrediction : PostReleaseDefectFilesPrediction
	{
		private LinearRegression regression;

		public TotalLocLinearRegressionPostReleaseDefectFilesPrediction()
		{
			Title = "Total LOC linear regression model";
			
			this.AddTotalLocInFilesTillRevisionPredictor();
		}
		public override void Init(IRepository repository, IEnumerable<string> releases)
		{
			base.Init(repository, releases);
			
			double dd = repository.SelectionDSL()
				.Commits().TillRevision(PredictionRelease)
				.Modifications().InCommits()
				.CodeBlocks().InModifications().CalculateDefectDensity(PredictionRelease);
				
			context.SetCommits(null, PredictionRelease);
			
			regression = new LinearRegression();
			foreach (var file in GetFilesInRevision(PredictionRelease))
			{
				double ddForFile = repository.SelectionDSL()
					.Commits().TillRevision(PredictionRelease)
					.Files().IdIs(file.ID)
					.Modifications().InCommits().InFiles()
					.CodeBlocks().InModifications().CalculateDefectDensity(PredictionRelease);
				
				if (ddForFile >= dd)
				{
					context.SetFiles(e => e.IdIs(file.ID));

					regression.AddTrainingData(
						GetPredictorValuesFor(context)[0],
						NumberOfFixedDefectsForFile(file.ID)
					);
				}
			}
			
			regression.Train();
		}
		public override ROCEvaluationResult EvaluateUsingROC()
		{
			double maxFileEstimation = FileEstimations.Max();
			rocEvaluationDelta = (maxFileEstimation + maxFileEstimation / 100) / 100;
			return base.EvaluateUsingROC();
		}
		protected override double DefaultCutOffValue
		{
			get { return 1; }
		}
		protected override double GetFileEstimation(ProjectFile file)
		{
			double numberOfBugsReal = NumberOfFixedDefectsForFile(file.ID);
			double numberOfBugsEstimation = regression.Predict(
				GetPredictorValuesFor(context.SetFiles(e => e.IdIs(file.ID)))[0]
			);
			double numberOfBugs = numberOfBugsEstimation - numberOfBugsReal;
			return numberOfBugs > 0 ? numberOfBugs : 0;
		}
		protected double NumberOfFixedDefectsForFile(int fileID)
		{
			return repository.SelectionDSL()
				.Commits().TillRevision(PredictionRelease)
				.Files().IdIs(fileID)
				.Modifications().InCommits().InFiles()
				.CodeBlocks().InModifications().CalculateNumberOfDefects(PredictionRelease);
		}
	}
}
