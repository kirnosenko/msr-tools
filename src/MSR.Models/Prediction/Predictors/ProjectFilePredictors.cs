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
		public static Prediction AddFileTouchCountInRevisionsPredictor(this Prediction p)
		{
			p.AddPredictor((Func<PredictorContext,double>)(c =>
			{
				return c.SelectionDSL()
					.Files().IdIs(c.GetValue<int>("file_id"))
					.Commits()
						.Reselect(e =>
						{
							string afterRevision = c.GetValue<string>("after_revision");
							return afterRevision == null ? e : e.AfterRevision(afterRevision);
						})
						.TillRevision(c.GetValue<string>("till_revision"))
						.TouchFiles().Count();
			}));
			return p;
		}
	}
}
