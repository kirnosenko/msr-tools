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

namespace MSR.Models.Prediction.PostReleaseDefectFiles
{
	public class SimplestTotalLocPostReleaseDefectFilesPrediction : PostReleaseDefectFilesPrediction
	{
		public SimplestTotalLocPostReleaseDefectFilesPrediction()
		{
			Title = "Simplest total LOC model";

			FilePortionLimit = 0.2f;
		}
		public override ROCEvaluationResult EvaluateUsingROC()
		{
			double maxFileEstimation = FileEstimations.Max();
			rocEvaluationDelta = (maxFileEstimation + maxFileEstimation / 100) / 100;
			return base.EvaluateUsingROC();
		}
		protected override double DefaultCutOffValue
		{
			get { return 0; }
		}
		protected override double GetFileEstimation(ProjectFile file)
		{
			return repository.SelectionDSL()
				.Commits().TillRevision(PredictionRelease)
				.Files().IdIs(file.ID)
				.Modifications().InCommits().InFiles()
				.CodeBlocks().InModifications().CalculateLOC();
		}
	}
}
