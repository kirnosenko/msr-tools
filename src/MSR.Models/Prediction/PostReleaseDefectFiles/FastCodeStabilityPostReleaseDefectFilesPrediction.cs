/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;

namespace MSR.Models.Prediction.PostReleaseDefectFiles
{
	public class FastCodeStabilityPostReleaseDefectFilesPrediction : CodeStabilityPostReleaseDefectFilesPrediction
	{
		public FastCodeStabilityPostReleaseDefectFilesPrediction()
		{
			Title = "Code stability model (fast)";
			DefectLineProbabilityEstimation = new DefectLineProbabilityForTheCodeOfAuthorInFileAverage(this);
			BugLifetimeDistributionEstimation = new BugLifetimeDistributionExperimentalMax(this);
			FileEstimation = new G5M2(this);
		}
	}
}
