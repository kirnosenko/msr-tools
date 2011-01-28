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
		public static T AddFilesTouchCountInCommitsPredictor<T>(this T p) where T : Prediction
		{
			p.AddPredictor((Func<PredictorContext,double>)(c =>
			{
				return c.SelectionDSL()
					.Files().Reselect(
						c.GetValue<Func<ProjectFileSelectionExpression,ProjectFileSelectionExpression>>("files")
					)
					.Commits()
						.Reselect(
							c.GetValue<Func<CommitSelectionExpression,CommitSelectionExpression>>("commits")
						)
						.TouchFiles().Count();
			}));
			return p;
		}
	}
}
