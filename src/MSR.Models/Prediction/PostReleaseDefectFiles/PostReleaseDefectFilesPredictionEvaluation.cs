/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Linq;

using MSR.Data;
using MSR.Data.Entities;
using MSR.Data.Entities.DSL.Selection;
using MSR.Data.Entities.DSL.Selection.Metrics;

namespace MSR.Models.Prediction.PostReleaseDefectFiles
{
	public class PostReleaseDefectFilesPredictionEvaluation
	{
		private IRepositoryResolver repositories;

		private IEnumerable<string> allFiles;
		private IEnumerable<string> defectFiles;
		
		public PostReleaseDefectFilesPredictionEvaluation(IRepositoryResolver repositories)
		{
			this.repositories = repositories;
		}
		public EvaluationResult Evaluate(PostReleaseDefectFilesPrediction prediction)
		{
			prediction.FileSelector = FileSelector;
			return Evaluate(prediction.Predict(Revisions));
		}
		public EvaluationResult Evaluate(IEnumerable<string> predictedDefectFiles)
		{
			if (allFiles == null)
			{
				Calc();
			}
			IEnumerable<string> predictedNonDefectFiles = allFiles.Except(predictedDefectFiles);

			IEnumerable<string> P = defectFiles;
			IEnumerable<string> N = allFiles.Except(defectFiles);
			int TP = predictedDefectFiles.Intersect(P).Count();
			int TN = predictedNonDefectFiles.Intersect(N).Count();
			int FP = predictedDefectFiles.Count() - TP;
			int FN = predictedNonDefectFiles.Count() - TN;

			return new EvaluationResult(TP, TN, FP, FN);
		}
		public string[] Revisions
		{
			get; set;
		}
		public string ReleaseRevision
		{
			get { return Revisions.Last(); }
		}
		public int PostReleasePeriod
		{
			get; set;
		}
		public Func<ProjectFileSelectionExpression,ProjectFileSelectionExpression> FileSelector
		{
			get; set;
		}

		private void Calc()
		{
			defectFiles = repositories.SelectionDSL()
				.Commits()
					.AfterRevision(ReleaseRevision)
					.DateIsLesserOrEquelThan(PostReleasePeriodEnd())
					.AreBugFixes()
				.Files()
					.Reselect(FileSelector)
					.ExistInRevision(ReleaseRevision)
						.Do(e => allFiles = e.Select(f => f.Path))
					.TouchedInCommits()
				.Select(x => x.Path).ToList();
		}
		private DateTime PostReleasePeriodEnd()
		{
			return repositories.Repository<Commit>()
				.Single(c => c.Revision == ReleaseRevision)
				.Date.AddDays(PostReleasePeriod);
		}
	}
}
