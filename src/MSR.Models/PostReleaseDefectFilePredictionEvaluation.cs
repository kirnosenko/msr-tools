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

namespace MSR.Models
{
	public class PostReleaseDefectFilePredictionEvaluation
	{
		private IRepositoryResolver repositories;

		private IEnumerable<string> allFiles;
		private IEnumerable<string> defectFiles;
		
		public PostReleaseDefectFilePredictionEvaluation(IRepositoryResolver repositories)
		{
			this.repositories = repositories;
		}
		public EvaluationResult Evaluate(IPostReleaseDefectFilePrediction prediction)
		{
			if (allFiles == null)
			{
				Calc();
			}
			IEnumerable<string> predictedDefectFiles = prediction.Predict(
				PreviousReleaseRevision, ReleaseRevision, FileSelector
			);
			IEnumerable<string> predictedNonDefectFiles = allFiles.Except(predictedDefectFiles);

			IEnumerable<string> P = defectFiles;
			IEnumerable<string> N = allFiles.Except(defectFiles);
			int TP = predictedDefectFiles.Intersect(P).Count();
			int TN = predictedNonDefectFiles.Intersect(N).Count();
			int FP = predictedDefectFiles.Count() - TP;
			int FN = predictedNonDefectFiles.Count() - TN;
			
			return new EvaluationResult(TP, TN, FP, FN);
		}
		public string ReleaseRevision
		{
			get; set;
		}
		public string PreviousReleaseRevision
		{
			get; set;
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
			RepositorySelectionExpression selectionDSL = new RepositorySelectionExpression(repositories);
			
			defectFiles = selectionDSL
				.Commits()
					.AfterRevision(PreviousReleaseRevision)
					.TillRevision(ReleaseRevision)
				.Files()
					.Reselect(FileSelector)
					.ExistInRevision(ReleaseRevision)
					.Do(e => allFiles = e.Select(f => f.Path))
				.Modifications().InCommits().InFiles()
				.CodeBlocks().InModifications().Added().ModifiedBy()
				.Modifications().ContainCodeBlocks()
				.Commits()
					.AfterRevision(ReleaseRevision)
					.DateIsLesserThan(PostReleasePeriodEnd())
					.ContainModifications()
					.AreBugFixes()
				.Files().Again().TouchedInCommits()
				.Select(x => x.Path)
				.ToList();
		}
		private DateTime PostReleasePeriodEnd()
		{
			return repositories.Repository<Commit>()
				.Single(c => c.Revision == ReleaseRevision)
				.Date.AddDays(PostReleasePeriod);
		}
	}
}
