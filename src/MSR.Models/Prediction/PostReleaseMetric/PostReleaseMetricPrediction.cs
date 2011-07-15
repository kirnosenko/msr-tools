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
		
		public override void Init(IRepositoryResolver repositories, IEnumerable<string> releaseRevisions)
		{
			base.Init(repositories, releaseRevisions);
			context.SetFiles(e => e);
		}
		public virtual void Train()
		{
			regression = new MultipleLinearRegression();

			string previousRevision = null;
			foreach (var revision in TrainReleases)
			{
				context.SetCommits(previousRevision, revision);
				
				regression.AddTrainingData(
					GetPredictorValuesFor(context),
					PostReleaseMetric(previousRevision, revision)
				);
				previousRevision = revision;
			}

			regression.Train();
		}
		public virtual double Predict()
		{
			context.SetCommits(TrainReleases.Last(), PredictionRelease);
			
			return regression.Predict(
				GetPredictorValuesFor(context)
			);
		}
		public Func<ProjectFileSelectionExpression,ProjectFileSelectionExpression> FileSelector
		{
			get; set;
		}
		public abstract double PostReleaseMetric(string previousReleaseRevision, string releaseRevision);
	}
}
