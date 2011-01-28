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
		public static Prediction AddTotalLocInFilesTillRevisionPredictor(this Prediction p)
		{
			p.AddPredictor((Func<PredictorContext,double>)(c =>
			{
				return c.CodeInFilesTillRevision().CalculateLOC();
			}));
			return p;
		}
		public static Prediction AddAddedLocInFilesInCommitsPredictor(this Prediction p)
		{
			p.AddPredictor((Func<PredictorContext,double>)(c =>
			{
				return c.CodeInCommitsInFiles()
					.Added().CalculateLOC();
			}));
			return p;
		}
		public static Prediction AddNumberOfBugsTouchFilesInCommitsFixedTillRevisionPredictor(this Prediction p)
		{
			p.AddPredictor((Func<PredictorContext,double>)(c =>
			{
				return c.CodeInCommitsInFiles()
					.CalculateNumberOfDefectsFixedTillRevision(c.GetValue<string>("till_revision"));
			}));
			return p;
		}
		public static Prediction AddDefectDensityForCodeInFilesInCommitsPredictor(this Prediction p)
		{
			p.AddPredictor((Func<PredictorContext, double>)(c =>
			{
				return c.CodeInCommitsInFiles()
					.CalculateTraditionalDefectDensity();
			}));
			return p;
		}
		public static Prediction AddDefectCodeDensityForCodeInFileInRevisionsPredictor(this Prediction p)
		{
			p.AddPredictor((Func<PredictorContext, double>)(c =>
			{
				return c.CodeInCommitsInFiles()
					.CalculateDefectCodeDensity();
			}));
			return p;
		}
		private static CodeBlockSelectionExpression CodeInCommitsInFiles(this PredictorContext c)
		{
			return c.SelectionDSL()
				.Commits().Reselect(
					c.GetValue<Func<CommitSelectionExpression,CommitSelectionExpression>>("commits")
				)
				.Files().Reselect(
					c.GetValue<Func<ProjectFileSelectionExpression,ProjectFileSelectionExpression>>("files")
				)
				.Modifications().InCommits().InFiles()
				.CodeBlocks().InModifications();
		}
		private static CodeBlockSelectionExpression CodeInFilesTillRevision(this PredictorContext c)
		{
			c.SetValue("commits",(Func<CommitSelectionExpression,CommitSelectionExpression>)(e =>
				e.TillRevision(c.GetValue<string>("till_revision"))
			));
			
			return CodeInCommitsInFiles(c);
		}
	}
}
