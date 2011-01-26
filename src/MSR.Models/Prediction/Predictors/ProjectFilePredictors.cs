/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;
using System.Linq;

using MSR.Data.Entities.DSL.Selection;
using MSR.Data.Entities.DSL.Selection.Metrics;

namespace MSR.Models.Prediction.Predictors
{
	public static class ProjectFilePredictors
	{
		public static Prediction AddFileTouchCountPredictor(this Prediction p)
		{
			p.AddPredictor((Func<ProjectFileSelectionExpression, double>)(files =>
			{
				return files
					.Commits().Again().TouchFiles().Count();
			}));
			return p;
		}
	}
}
