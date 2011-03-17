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
		public override IEnumerable<string> Predict()
		{
			var files = FilesInRevision(LastReleaseRevision);
			int filesInRelease = files.Count();
			
			return files
				.Select(x => x.Path)
				.TakeRandomly((int)(filesInRelease * FilePortionLimit));
		}
	}
}
