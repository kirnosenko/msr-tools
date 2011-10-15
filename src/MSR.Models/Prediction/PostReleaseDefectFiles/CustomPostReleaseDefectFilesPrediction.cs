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
	public class CustomPostReleaseDefectFilesPrediction : LogisticRegressionPostReleaseDefectFilesPrediction
	{
		public CustomPostReleaseDefectFilesPrediction()
		{
			Title = "Custom logistic regression model";
		}
		public bool AddedLoc
		{
			set { if (value) this.AddAddedLocInFilesInCommitsPredictor(); }
		}
		public bool DefectCodeDensity
		{
			set { if (value) this.AddDefectCodeDensityForCodeInCommitsInFilesPredictor(); }
		}
		public bool DefectDensity
		{
			set { if (value) this.AddDefectDensityForCodeInCommitsInFilesPredictor(); }
		}
		public bool DeletedLoc
		{
			set { if (value) this.AddDeletedLocInFilesInCommitsPredictor(); }
		}
		public bool FilesTouchCount
		{
			set { if (value) this.AddFilesTouchCountInCommitsPredictor(); }
		}
		public bool NumberOfBugs
		{
			set { if (value) this.AddNumberOfBugsTouchFilesInCommitsFixedTillRevisionPredictor(); }
		}
		public bool TotalLoc
		{
			set { if (value) this.AddTotalLocInFilesTillRevisionPredictor(); }
		}
		public bool TraditionalDefectDensity
		{
			set { if (value) this.AddTraditionalDefectDensityForCodeInCommitsInFilesPredictor(); }
		}
	}
}
