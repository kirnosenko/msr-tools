/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;

using MSR.Data;
using MSR.Models.Prediction.Predictors;

namespace MSR.Models.Prediction.PostReleaseDefectFiles
{
	public class SimpleLocPostReleaseDefectFilesPrediction : PostReleaseDefectFilesPrediction
	{
		public SimpleLocPostReleaseDefectFilesPrediction(IRepositoryResolver repositories)
			: base(repositories)
		{
			this.AddTotalLocInFilesTillRevisionPredictor();
		}
	}
}
