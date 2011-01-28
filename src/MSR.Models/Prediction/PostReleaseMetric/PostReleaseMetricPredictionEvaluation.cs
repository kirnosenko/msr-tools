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
			
			for (int i = 1; i <= NumberOfPredictions; i++)
			{
				PrepareSets();
				
				
			}
			
			return 0;
		}
		public string ReleaseRevision
		{
			get; set;
		}
		public string[] PreviousReleaseRevisions
		{
			get; set;
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
			trainSet = fileIDs.TakeRandomly((int)(fileIDs.Count() * 2d/3)).ToArray();
			predictSet = fileIDs.Except(trainSet).ToArray();
		}
	}
}
