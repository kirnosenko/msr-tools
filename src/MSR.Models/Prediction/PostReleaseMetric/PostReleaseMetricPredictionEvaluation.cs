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
	public class PostReleaseMetricPredictionEvaluation
	{
		private IRepositoryResolver repositories;
		private IEnumerable<int> fileIDs;
		private int[] trainSet;
		private int[] predictSet;

		public PostReleaseMetricPredictionEvaluation(IRepositoryResolver repositories)
		{
			this.repositories = repositories;
			NumberOfPredictions = 10;
		}
		public double Evaluate(PostReleaseMetricPrediction prediction)
		{
			fileIDs = repositories.SelectionDSL()
				.Files()
					.Reselect(FileSelector)
					.ExistInRevision(ReleaseRevision)
				.Select(f => f.ID).ToArray();
			
			LinearRegression regression = new LinearRegression();
			for (int i = 1; i <= NumberOfPredictions; i++)
			{
				PrepareSets();
				
				prediction.FileSelector = e => e.IdIn(trainSet);
				prediction.Train(PreviousReleaseRevisions);
				
				prediction.FileSelector = e => e.IdIn(predictSet);
				
				regression.AddTrainingData(
					prediction.Predict(PreviousReleaseRevisions.Last(), ReleaseRevision),
					prediction.PostReleaseMetric(PreviousReleaseRevisions.Last(), ReleaseRevision)
				);
			}
			
			return regression.R2;
		}
		public string[] Revisions
		{
			get; set;
		}
		public string[] PreviousReleaseRevisions
		{
			get { return Revisions.Take(Revisions.Count() - 1).ToArray(); }
		}
		public string ReleaseRevision
		{
			get { return Revisions.Last(); }
		}
		public Func<ProjectFileSelectionExpression,ProjectFileSelectionExpression> FileSelector
		{
			get; set;
		}
		public int NumberOfPredictions
		{
			get; set;
		}
		private void PrepareSets()
		{
			predictSet = fileIDs.TakeRandomly((int)(fileIDs.Count() * 1d/3)).ToArray();
			trainSet = fileIDs.Except(predictSet).ToArray();
		}
	}
}
