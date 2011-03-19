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

namespace MSR.Models.Prediction.PostReleaseDefectFiles
{/*
	public class PostReleaseDefectFilesPredictionEvaluation
	{
		private IRepositoryResolver repositories;

		private IEnumerable<string> allFiles;
		private IEnumerable<string> defectFiles;
		
		public EvaluationResult Evaluate(IRepositoryResolver repositories, PostReleaseDefectFilesPrediction prediction)
		{
			this.repositories = repositories;
			FileSelector = prediction.FileSelector;

			if (allFiles == null)
			{
				Calc(prediction.LastReleaseRevision);
			}
			IEnumerable<string> predictedNonDefectFiles = allFiles.Except(prediction.PredictedDefectFiles);

			IEnumerable<string> P = defectFiles;
			IEnumerable<string> N = allFiles.Except(defectFiles);
			int TP = prediction.PredictedDefectFiles.Intersect(P).Count();
			int TN = predictedNonDefectFiles.Intersect(N).Count();
			int FP = prediction.PredictedDefectFiles.Count() - TP;
			int FN = predictedNonDefectFiles.Count() - TN;

			return new EvaluationResult(TP, TN, FP, FN);
		}
		public int PostReleasePeriod
		{
			get; set;
		}
		public Func<ProjectFileSelectionExpression,ProjectFileSelectionExpression> FileSelector
		{
			get; set;
		}
		public IEnumerable<string> DefectFiles
		{
			get { return defectFiles; }
		}

		private void Calc(string releaseRevision)
		{
			defectFiles = repositories.SelectionDSL()
				.Commits()
					.AfterRevision(releaseRevision)
					.DateIsLesserOrEquelThan(PostReleasePeriodEnd(releaseRevision))
					.AreBugFixes()
				.Files()
					.Reselect(FileSelector)
					.ExistInRevision(releaseRevision)
						.Do(e => allFiles = e.Select(f => f.Path))
					.TouchedInCommits()
				.Select(x => x.Path)
				.ToList();
		}
		private DateTime PostReleasePeriodEnd(string releaseRevision)
		{
			return repositories.Repository<Commit>()
				.Single(c => c.Revision == releaseRevision)
				.Date.AddDays(PostReleasePeriod);
		}
	}*/
}
