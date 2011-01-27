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
	public static class CodeBlockPredictors
	{
		public static Prediction AddTotalLocInFileInRevisionPredictor(this Prediction p)
		{
			p.AddPredictor((Func<PredictorContext,double>)(c =>
			{
				return c.SelectionDSL()
					.Commits().TillRevision(c.GetValue<string>("till_revision"))
					.Files().IdIs(c.GetValue<int>("file_id"))
					.Modifications().InCommits().InFiles()
					.CodeBlocks().InModifications().CalculateLOC();
			}));
			return p;
		}
		public static Prediction AddAddedLocInFileInRevisionsPredictor(this Prediction p)
		{
			p.AddPredictor((Func<PredictorContext,double>)(c =>
			{
				return c.CodeInFileInRevisions()
					.Added().CalculateLOC();
			}));
			return p;
		}
		public static Prediction AddNumberOfBugsTouchFileInRevisionsFixedTillRevisionPredictor(this Prediction p)
		{
			p.AddPredictor((Func<PredictorContext,double>)(c =>
			{
				return c.CodeInFileInRevisions()
					.CalculateNumberOfDefectsFixedTillRevision(c.GetValue<string>("till_revision"));
			}));
			return p;
		}
		public static Prediction AddDefectDensityForCodeInFileInRevisionsPredictor(this Prediction p)
		{
			p.AddPredictor((Func<PredictorContext, double>)(c =>
			{
				return c.CodeInFileInRevisions()
					.CalculateTraditionalDefectDensity();
			}));
			return p;
		}
		public static Prediction AddDefectCodeDensityForCodeInFileInRevisionsPredictor(this Prediction p)
		{
			p.AddPredictor((Func<PredictorContext, double>)(c =>
			{
				return c.CodeInFileInRevisions()
					.CalculateDefectCodeDensity();
			}));
			return p;
		}
		private static CodeBlockSelectionExpression CodeInFileInRevisions(this PredictorContext c)
		{
			return c.SelectionDSL()
				.Commits()
					.Reselect(e =>
					{
						string afterRevision = c.GetValue<string>("after_revision");
						return afterRevision == null ? e : e.AfterRevision(afterRevision);
					})
					.TillRevision(c.GetValue<string>("till_revision"))
				.Files().IdIs(c.GetValue<int>("file_id"))
				.Modifications().InCommits().InFiles()
				.CodeBlocks().InModifications();
		}
	}
}
