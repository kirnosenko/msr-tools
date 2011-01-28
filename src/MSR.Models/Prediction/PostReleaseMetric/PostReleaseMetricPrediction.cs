/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Linq;

using MSR.Data;
using MSR.Data.Entities;
using MSR.Data.Entities.DSL.Selection;
using MSR.Data.Entities.DSL.Selection.Metrics;
using MSR.Models.Regressions;

namespace MSR.Models.Prediction.PostReleaseMetric
{
	public abstract class PostReleaseMetricPrediction : Prediction
	{
		private MultipleLinearRegression regression;
		
		public PostReleaseMetricPrediction(IRepositoryResolver repositories)
			: base(repositories)
		{
		}
		public virtual void Train(string[] revisions)
		{
			regression = new MultipleLinearRegression();

			string previousRevision = null;
			foreach (var revision in revisions)
			{
				context
					.SetValue("after_revision", previousRevision)
					.SetValue("till_revision", revision);
				regression.AddTrainingData(
					GetPredictorValuesFor(context),
					PostReleaseMetric(revision, previousRevision)
				);
				previousRevision = revision;
			}

			regression.Train();
		}
		public virtual double Predict(string previousReleaseRevision, string releaseRevision)
		{
			context
				.SetValue("after_revision", previousReleaseRevision)
				.SetValue("till_revision", releaseRevision);
			
			return regression.Predict(
				GetPredictorValuesFor(context)
			);
		}
		public Func<ProjectFileSelectionExpression,ProjectFileSelectionExpression> FileSelector
		{
			get; set;
		}
		protected abstract double PostReleaseMetric(string releaseRevision, string previousReleaseRevision);
	}
}
