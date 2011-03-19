/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Linq;

using MSR.Data;
using MSR.Models.Prediction.Predictors;

namespace MSR.Models.Prediction.PostReleaseDefectFiles
{
	public class RandomPostReleaseDefectFilesPrediction : PostReleaseDefectFilesPrediction
	{
		public RandomPostReleaseDefectFilesPrediction()
		{
			Title = "Random model";
		}
		public override void Predict()
		{
			var files = GetFilesInRevision(LastReleaseRevision);
			int filesInRelease = files.Count();
			
			PredictedDefectFiles = files.Select(x => x.Path)
				.TakeRandomly((int)(filesInRelease * FilePortionLimit))
				.ToList();
		}
	}
}
