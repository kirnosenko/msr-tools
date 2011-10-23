/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;

namespace MSR.Models.Prediction.PostReleaseDefectFiles
{
	public class FullCodeStabilityPostReleaseDefectFilesPrediction : CodeStabilityPostReleaseDefectFilesPrediction
	{
		public FullCodeStabilityPostReleaseDefectFilesPrediction()
		{
			Title = "Code stability model";
			DefectLineProbabilityEstimation = new DefectLineProbabilityForTheCodeOfAuthorInFileAverage(this);
			BugLifetimeDistributionEstimation = new BugLifetimeDistributionExperimentalMax(this);
			FileEstimation = new G5M3(this);
		}
	}
}
