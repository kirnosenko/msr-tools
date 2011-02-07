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
		public RandomPostReleaseDefectFilesPrediction(IRepositoryResolver repositories)
			: base(repositories)
		{
		}
		public override IEnumerable<string> Predict(string[] revisions)
		{
			string releaseRevision = revisions.Last();
			
			var files = FilesInRevision(releaseRevision);
			int filesInRelease = files.Count();
			
			return files
				.Select(x => x.Path)
				.TakeRandomly((int)(filesInRelease * FilePortionLimit));	
		}
	}
}
